using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany;

/// <summary>
/// Used to combine seeds together to create new hybrid plant species
/// </summary>
public abstract partial class SharedSeedSplicerSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem _itemSlotsSystem = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;

    //Could switch to hashset for faster lookup or enumerate each query, leaving as a list for now
    private List<SeedSplicerRecipePrototype> _splicerRecipes = default!;


    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        _splicerRecipes = new List<SeedSplicerRecipePrototype>();
        foreach (var item in _prototypeManager.EnumeratePrototypes<SeedSplicerRecipePrototype>())
        {
            //Only add valid recipes
            if (item.Seeds.Count == 2)
                _splicerRecipes.Add(item);
        }


        //Add and remove the item slots
        SubscribeLocalEvent<SeedSplicerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SeedSplicerComponent, ComponentRemove>(OnComponentRemove);

        //Handle UI messages
        SubscribeLocalEvent<SeedSplicerComponent, SeedSplicerActivateMessage>(OnActivate);
        SubscribeLocalEvent<SeedSplicerComponent, SeedSplicerEjectMessage>(OnEjectPressed);

        //Update the UI if it's opened or item slots are changed
        SubscribeLocalEvent((Entity<SeedSplicerComponent> ent, ref BoundUIOpenedEvent _)  => UpdateBui(ent));
        SubscribeLocalEvent((Entity<SeedSplicerComponent> ent, ref EntRemovedFromContainerMessage _)  => UpdateBui(ent));
        SubscribeLocalEvent((Entity<SeedSplicerComponent> ent, ref EntInsertedIntoContainerMessage _)  => UpdateBui(ent));
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

        var recipe = FindSplicerRecipe(ent, ent.Comp.SeedSlotLeft.Item.Value, ent.Comp.SeedSlotRight.Item.Value);

        if (recipe == null)
            return;

        ProcessRecipe(ent, recipe, ent.Comp.SeedSlotLeft.Item.Value, ent.Comp.SeedSlotRight.Item.Value);
    }

    protected virtual void UpdateBui(Entity<SeedSplicerComponent> ent)
    {

    }

    //Returns a recipe if all prototypes in that recipe are within an item slot, otherwise returns null
    private SeedSplicerRecipePrototype? FindSplicerRecipe(Entity<SeedSplicerComponent> ent, EntityUid seedLeft, EntityUid seedRight)
    {
        EntProtoId? idLeft = MetaData(seedLeft).EntityPrototype?.ID;
        EntProtoId? idRight = MetaData(seedRight).EntityPrototype?.ID;

        foreach (var recipe in _splicerRecipes)
        {
            if (recipe.Seeds[0] != idLeft && recipe.Seeds[0] != idRight
                || recipe.Seeds[1] != idLeft && recipe.Seeds[1] != idRight)
                continue;
            return recipe;
        }

        return null;
    }

    //Deletes all ingredients that were used in a recipe then spawns the product
    private void ProcessRecipe(Entity<SeedSplicerComponent> ent, SeedSplicerRecipePrototype recipe, EntityUid seedLeft, EntityUid seedRight)
    {
        //Recipes require at least two seeds so they are always deleted
        PredictedDel(seedLeft);
        PredictedDel(seedRight);
        PredictedSpawnAtPosition(recipe.Result, Transform(ent).Coordinates);
        Dirty(ent, ent.Comp);
        UpdateBui(ent);
    }

}
