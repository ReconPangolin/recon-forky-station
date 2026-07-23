using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;

namespace Content.Shared._Funkystation.Botany;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class SharedSeedSplicerSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem ItemSlotsSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SeedSplicerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SeedSplicerComponent, ComponentRemove>(OnComponentRemove);

        //SubscribeLocalEvent<SeedSplicerComponent, EntInsertedIntoContainerMessage>(OnItemInserted);
        //SubscribeLocalEvent<SeedSplicerComponent, EntRemovedFromContainerMessage>(OnItemRemoved);

    }

    private void OnComponentInit(EntityUid uid, SeedSplicerComponent splicer, ComponentInit args)
    {
        ItemSlotsSystem.AddItemSlot(uid, SeedSplicerComponent.ResourceSlotId, splicer.ResourceSlot);
        ItemSlotsSystem.AddItemSlot(uid, SeedSplicerComponent.SeedSlotLeftId, splicer.SeedSlotLeft);
        ItemSlotsSystem.AddItemSlot(uid, SeedSplicerComponent.SeedSlotRightId, splicer.SeedSlotRight);

    }

    private void OnComponentRemove(EntityUid uid, SeedSplicerComponent splicer, ComponentRemove args)
    {
        ItemSlotsSystem.RemoveItemSlot(uid, splicer.ResourceSlot);
        ItemSlotsSystem.RemoveItemSlot(uid, splicer.SeedSlotLeft);
        ItemSlotsSystem.RemoveItemSlot(uid, splicer.SeedSlotRight);
    }




}
