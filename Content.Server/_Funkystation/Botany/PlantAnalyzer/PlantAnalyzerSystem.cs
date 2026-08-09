using System.Linq;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.PowerCell;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Timing;

using Content.Shared._Funkystation.Botany.PlantAnalyzer;
using Content.Server.Botany;
using Content.Server.Botany.Components;


namespace Content.Server._Funkystation.Botany.PlantAnalyzer;

public sealed partial class PlantAnalyzerSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private PowerCellSystem _cell = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private ItemToggleSystem _toggle = default!;
    [Dependency] private UserInterfaceSystem _uiSystem = default!;
    [Dependency] private TransformSystem _transformSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PlantAnalyzerComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<PlantAnalyzerComponent, PlantAnalyzerDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<PlantAnalyzerComponent, EntGotInsertedIntoContainerMessage>(OnInsertedIntoContainer);
        SubscribeLocalEvent<PlantAnalyzerComponent, ItemToggledEvent>(OnToggled);
        SubscribeLocalEvent<PlantAnalyzerComponent, DroppedEvent>(OnDropped);
    }


    public override void Update(float frameTime)
    {
        var analyzerQuery = EntityQueryEnumerator<PlantAnalyzerComponent, TransformComponent>();
        while (analyzerQuery.MoveNext(out var uid, out var component, out var transform))
        {
            //Update rate limited to 1 second
            if (component.NextUpdate > _timing.CurTime)
                continue;

            if (component.ScannedEntity is not {} plant)
                continue;

            if (Deleted(plant))
            {
                StopAnalyzingEntity((uid, component), plant);
                continue;
            }

            component.NextUpdate = _timing.CurTime + component.UpdateInterval;

            //Get distance between health analyzer and the scanned entity
            //null is infinite range
            var plantCoords = Transform(plant).Coordinates;
            if (component.MaxScanRange != null && !_transformSystem.InRange(plantCoords, transform.Coordinates, component.MaxScanRange.Value))
            {
                //Range too far, disable updates until they are back in range
                PauseAnalyzingEntity((uid, component), plant);
                continue;
            }

            component.IsAnalyzerActive = true;
            UpdateScannedUser((uid, component), plant, true);
        }
    }



    /// <summary>
    /// Trigger the doafter for scanning
    /// </summary>
    private void OnAfterInteract(Entity<PlantAnalyzerComponent> uid, ref AfterInteractEvent args)
    {
        if (args.Target == null || !args.CanReach || !HasComp<PlantHolderComponent>(args.Target) || !_cell.HasDrawCharge(uid.Owner, user: args.User))
            return;

        _audio.PlayPvs(uid.Comp.ScanningBeginSound, uid);

        var doAfterCancelled = !_doAfterSystem.TryStartDoAfter(new DoAfterArgs(EntityManager, args.User, uid.Comp.ScanDelay, new PlantAnalyzerDoAfterEvent(), uid, target: args.Target, used: uid)
        {
            NeedHand = true,
            BreakOnMove = true,
        });
    }


    private void OnDoAfter(Entity<PlantAnalyzerComponent> uid, ref PlantAnalyzerDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Target == null || !_cell.HasDrawCharge(uid.Owner, user: args.User))
            return;

        if (!uid.Comp.Silent)
            _audio.PlayPvs(uid.Comp.ScanningEndSound, uid);

        OpenUserInterface(args.User, uid);
        BeginAnalyzingEntity(uid, args.Target.Value);
        args.Handled = true;
    }



    /// <summary>
    /// Turn off when placed into a storage item or moved between slots/hands
    /// </summary>
    private void OnInsertedIntoContainer(Entity<PlantAnalyzerComponent> uid, ref EntGotInsertedIntoContainerMessage args)
    {
        if (uid.Comp.ScannedEntity is { } plant)
            _toggle.TryDeactivate(uid.Owner);
    }

    /// <summary>
    /// Disable continuous updates once turned off
    /// </summary>
    private void OnToggled(Entity<PlantAnalyzerComponent> ent, ref ItemToggledEvent args)
    {
        if (!args.Activated && ent.Comp.ScannedEntity is { } plant)
            StopAnalyzingEntity(ent, plant);
    }

    /// <summary>
    /// Turn off the analyser when dropped
    /// </summary>
    private void OnDropped(Entity<PlantAnalyzerComponent> uid, ref DroppedEvent args)
    {
        if (uid.Comp.ScannedEntity is { } plant)
            _toggle.TryDeactivate(uid.Owner);
    }


    private void OpenUserInterface(EntityUid user, EntityUid analyzer)
    {
        if (!_uiSystem.HasUi(analyzer, PlantAnalyzerUiKey.Key))
            return;

        _uiSystem.OpenUi(analyzer, PlantAnalyzerUiKey.Key, user);
    }


    /// <summary>
    /// Mark the entity as having its health analyzed, and link the analyzer to it
    /// </summary>
    /// <param name="plantAnalyzer">The health analyzer that should receive the updates</param>
    /// <param name="target">The entity to start analyzing</param>
    private void BeginAnalyzingEntity(Entity<PlantAnalyzerComponent> plantAnalyzer, EntityUid target)
    {
        //Link the health analyzer to the scanned entity
        plantAnalyzer.Comp.ScannedEntity = target;

        _toggle.TryActivate(plantAnalyzer.Owner);

        UpdateScannedUser(plantAnalyzer, target, true);
    }

    /// <summary>
    /// Remove the analyzer from the active list, and remove the component if it has no active analyzers
    /// </summary>
    /// <param name="plantAnalyzer">The health analyzer that's receiving the updates</param>
    /// <param name="target">The entity to analyze</param>
    private void StopAnalyzingEntity(Entity<PlantAnalyzerComponent> plantAnalyzer, EntityUid target)
    {
        //Unlink the analyzer
        plantAnalyzer.Comp.ScannedEntity = null;
        _toggle.TryDeactivate(plantAnalyzer.Owner);

        UpdateScannedUser(plantAnalyzer, target, false);
    }


    /// <summary>
    /// If the scanner is active, sends one last update and sets it to inactive.
    /// </summary>
    /// <param name="plantAnalyzer">The health analyzer that's receiving the updates</param>
    /// <param name="target">The entity to analyze</param>
    private void PauseAnalyzingEntity(Entity<PlantAnalyzerComponent> plantAnalyzer, EntityUid target)
    {
        if (!plantAnalyzer.Comp.IsAnalyzerActive)
            return;

        UpdateScannedUser(plantAnalyzer, target, false);
        plantAnalyzer.Comp.IsAnalyzerActive = false;
    }


    /// <summary>
    /// Send an update for the target to the healthAnalyzer
    /// </summary>
    /// <param name="plantAnalyzer">The health analyzer</param>
    /// <param name="target">The entity being scanned</param>
    /// <param name="scanMode">True makes the UI show ACTIVE, False makes the UI show INACTIVE</param>
    public void UpdateScannedUser(Entity<PlantAnalyzerComponent> plantAnalyzer, EntityUid target, bool scanMode)
    {
        if (!_uiSystem.HasUi(plantAnalyzer, PlantAnalyzerUiKey.Key))
            return;

        var analyzerMessage = GetPlantAnalyzerUiState(plantAnalyzer, target);

        _uiSystem.ServerSendUiMessage(
            plantAnalyzer.Owner,
            PlantAnalyzerUiKey.Key,
            analyzerMessage
        );
    }

    /// <summary>
    /// Creates a HealthAnalyzerState based on the current state of an entity.
    /// </summary>
    /// <param name="target">The entity being scanned</param>
    /// <returns></returns>
    public PlantAnalyzerUserMessage GetPlantAnalyzerUiState(Entity<PlantAnalyzerComponent> plantAnalyzer, EntityUid? target)
    {
        if (TryComp<PlantHolderComponent>(target, out var plantHolderComp))
        {
            SeedData? seed = plantHolderComp.Seed;
            if (seed != null)
            {
                return new PlantAnalyzerUserMessage(
                    GetNetEntity(target),
                    plantAnalyzer.Comp.Version,
                    seed.Production,
                    seed.Maturation,
                    seed.Yield,
                    seed.Potency,
                    seed.Chemicals.Keys.ToList(),
                    seed.DisplayName,
                    seed.Lifespan,
                    seed.NutrientConsumption,
                    seed.WaterConsumption,
                    seed.IdealHeat);
            }
        }

        return new PlantAnalyzerUserMessage(
            GetNetEntity(target),
            1,
            1,
            1,
            1,
            1,
            null,
            "No plant",
            1,
            1,
            1,
            1);

    }
}
