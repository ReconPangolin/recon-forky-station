using Robust.Shared.GameStates;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Botany;

[RegisterComponent, NetworkedComponent]
public sealed partial class SeedSplicerComponent : Component
{


    public const string SeedSlotLeftId = "SeedSplicer-left";
    public const string SeedSlotRightId = "SeedSplicer-right";
    public const string ResourceSlotId = "SeedSplicer-resource";

    [DataField]
    public ItemSlot SeedSlotLeft = new();

    [DataField]
    public ItemSlot SeedSlotRight = new();

    [DataField]
    public ItemSlot ResourceSlot = new();
}

// Used for SeedSplicerEjectMessage to tell the server what slot to eject
[Serializable, NetSerializable]
public enum SeedSplicerSlot : byte
{
    LeftSeed,
    RightSeed,
    Resource,
    Gene,
}

// UI call to eject an item so the user can avoid the context menu
[Serializable, NetSerializable]
public sealed class SeedSplicerEjectMessage(SeedSplicerSlot slotToEject) : BoundUserInterfaceMessage
{
    public SeedSplicerSlot SlotToEject = slotToEject;
}

// UI call to combine two seeds together
[Serializable, NetSerializable]
public sealed class SeedSplicerActivateMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public enum SeedSplicerUiKey : byte
{
    Key
}
