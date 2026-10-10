using Content.Shared.EntityEffects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Botany;

/// <summary>
/// Data that specifies the odds and effects of possible random plant mutations.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FuPlantEffectComponent : Component
{

    /// <summary>
    /// List of RandomFills that can be picked from.
    /// </summary>
    [DataField]
    public List<(Enum, EntityEffect)> Effects = [];

}

[Serializable, NetSerializable]
public enum PlantEffectType
{
    Produce,
    Harvester,
    OnHarvest,
    Plant,
}
