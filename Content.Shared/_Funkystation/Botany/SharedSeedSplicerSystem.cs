using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class SharedSeedSplicerSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem _itemSlotsSystem = null!;
    [Dependency] private IPrototypeManager _prototypeManager = null!;

    private List<SeedSplicerRecipePrototype> _splicerRecipes = null!;


    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        //Could switch to hashset for faster lookup, not enough recipes to justify right now though
        _splicerRecipes = new List<SeedSplicerRecipePrototype>();
        foreach (var item in _prototypeManager.EnumeratePrototypes<SeedSplicerRecipePrototype>())
        {
            _splicerRecipes.Add(item);
        }

        SubscribeLocalEvent<SeedSplicerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SeedSplicerComponent, ComponentRemove>(OnComponentRemove);

        //Run the splicing process
        SubscribeLocalEvent<SeedSplicerComponent, ActivateInWorldEvent>(OnActivate);

    }

    private void OnComponentInit(EntityUid uid, SeedSplicerComponent splicer, ComponentInit args)
    {
        _itemSlotsSystem.AddItemSlot(uid, SeedSplicerComponent.ResourceSlotId, splicer.ResourceSlot);
        _itemSlotsSystem.AddItemSlot(uid, SeedSplicerComponent.SeedSlotLeftId, splicer.SeedSlotLeft);
        _itemSlotsSystem.AddItemSlot(uid, SeedSplicerComponent.SeedSlotRightId, splicer.SeedSlotRight);

    }

    private void OnComponentRemove(EntityUid uid, SeedSplicerComponent splicer, ComponentRemove args)
    {
        _itemSlotsSystem.RemoveItemSlot(uid, splicer.ResourceSlot);
        _itemSlotsSystem.RemoveItemSlot(uid, splicer.SeedSlotLeft);
        _itemSlotsSystem.RemoveItemSlot(uid, splicer.SeedSlotRight);
    }


    private void OnActivate(EntityUid uid, SeedSplicerComponent component, ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        if ((component.SeedSlotLeft.Item == null) || (component.SeedSlotRight.Item == null))
            return;


        //TODO: Terrible code until the botany refactor
        var seedSlotLeft = component.SeedSlotLeft.Item ?? uid;
        var seedSlotRight = component.SeedSlotRight.Item ?? uid;

        FindSplicerRecipe(uid, seedSlotLeft, seedSlotRight);

    }

    private void FindSplicerRecipe(EntityUid splicerUid, EntityUid seedLeft, EntityUid seedRight)
    {

        EntProtoId? idLeft = MetaData(seedLeft).EntityPrototype?.ID;
        EntProtoId? idRight = MetaData(seedRight).EntityPrototype?.ID;

        foreach (var recipe in _splicerRecipes)
        {
            if (recipe.Seeds[0] != idLeft && recipe.Seeds[0] != idRight
                || recipe.Seeds[1] != idLeft && recipe.Seeds[1] != idRight)
                continue;

            ProcessRecipe(splicerUid, recipe, seedLeft, seedRight);
            break;
        }
    }

    private void ProcessRecipe(EntityUid splicerUid, SeedSplicerRecipePrototype recipe, EntityUid seedLeft, EntityUid seedRight)
    {
        PredictedDel(seedLeft);
        PredictedDel(seedRight);
        PredictedSpawnAtPosition(recipe.Result, Transform(splicerUid).Coordinates);
    }

}
