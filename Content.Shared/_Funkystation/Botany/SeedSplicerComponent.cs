using Robust.Shared.GameStates;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Botany;

/// <summary>
/// This is used for...
/// </summary>
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

[Serializable, NetSerializable]
public enum SeedSplicerUiKey : byte
{
    Key
}
