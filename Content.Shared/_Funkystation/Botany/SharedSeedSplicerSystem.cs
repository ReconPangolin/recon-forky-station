using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany;

/// <summary>
/// This handles...
/// </summary>
public abstract partial class SharedSeedSplicerSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem _itemSlotsSystem = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;

    private List<SeedSplicerRecipePrototype> _splicerRecipes = default!;


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

        SubscribeLocalEvent<SeedSplicerComponent, BoundUIOpenedEvent>(OnBuiOpened);

        SubscribeLocalEvent<SeedSplicerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SeedSplicerComponent, ComponentRemove>(OnComponentRemove);

        //Run the splicing process
        SubscribeLocalEvent<SeedSplicerComponent, SeedSplicerActivateMessage>(OnActivate);

        SubscribeLocalEvent<SeedSplicerComponent, SeedSplicerEjectMessage>(OnEjectPressed);

        //TODO: Event after the item was ejected manually
    }

    private void OnEjectPressed(Entity<SeedSplicerComponent> ent, ref SeedSplicerEjectMessage args)
    {
        switch (args.SlotToEject)
        {
            case SeedSplicerSlot.LeftSeed:
                _itemSlotsSystem.TryEjectToHands(ent.Owner, ent.Comp.SeedSlotLeft, args.Actor);
                break;
            case SeedSplicerSlot.RightSeed:
                _itemSlotsSystem.TryEjectToHands(ent.Owner, ent.Comp.SeedSlotRight, args.Actor);
                break;
        }
        Dirty(ent, ent.Comp);
        UpdateBui(ent);
    }

    private void OnComponentInit(Entity<SeedSplicerComponent> ent, ref ComponentInit args)
    {
        _itemSlotsSystem.AddItemSlot(ent.Owner, SeedSplicerComponent.ResourceSlotId, ent.Comp.ResourceSlot);
        _itemSlotsSystem.AddItemSlot(ent.Owner, SeedSplicerComponent.SeedSlotLeftId, ent.Comp.SeedSlotLeft);
        _itemSlotsSystem.AddItemSlot(ent.Owner, SeedSplicerComponent.SeedSlotRightId, ent.Comp.SeedSlotRight);

    }

    private void OnComponentRemove(Entity<SeedSplicerComponent> ent, ref ComponentRemove args)
    {
        _itemSlotsSystem.RemoveItemSlot(ent.Owner, ent.Comp.ResourceSlot);
        _itemSlotsSystem.RemoveItemSlot(ent.Owner, ent.Comp.SeedSlotLeft);
        _itemSlotsSystem.RemoveItemSlot(ent.Owner, ent.Comp.SeedSlotRight);
    }

    private void OnActivate(Entity<SeedSplicerComponent> ent, ref SeedSplicerActivateMessage args)
    {
        if (ent.Comp.SeedSlotLeft.Item == null || ent.Comp.SeedSlotRight.Item == null)
            return;

        FindSplicerRecipe(ent, ent.Comp.SeedSlotLeft.Item.Value, ent.Comp.SeedSlotRight.Item.Value);
    }

    private void OnBuiOpened(Entity<SeedSplicerComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateBui(ent);
    }

    protected virtual void UpdateBui(Entity<SeedSplicerComponent> ent)
    {

    }

    private void FindSplicerRecipe(Entity<SeedSplicerComponent> ent, EntityUid seedLeft, EntityUid seedRight)
    {

        EntProtoId? idLeft = MetaData(seedLeft).EntityPrototype?.ID;
        EntProtoId? idRight = MetaData(seedRight).EntityPrototype?.ID;

        foreach (var recipe in _splicerRecipes)
        {
            if (recipe.Seeds[0] != idLeft && recipe.Seeds[0] != idRight
                || recipe.Seeds[1] != idLeft && recipe.Seeds[1] != idRight)
                continue;

            ProcessRecipe(ent, recipe, seedLeft, seedRight);
            break;
        }
    }

    private void ProcessRecipe(Entity<SeedSplicerComponent> ent, SeedSplicerRecipePrototype recipe, EntityUid seedLeft, EntityUid seedRight)
    {
        PredictedDel(seedLeft);
        PredictedDel(seedRight);
        PredictedSpawnAtPosition(recipe.Result, Transform(ent).Coordinates);
        Dirty(ent, ent.Comp);
        UpdateBui(ent);
    }

}
