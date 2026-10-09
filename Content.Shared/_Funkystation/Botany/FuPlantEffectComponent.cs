using Content.Shared.EntityEffects;
using Robust.Shared.GameStates;

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
    public List<EntityEffect> ProduceEffects = [];

    /// <summary>
    /// List of RandomFills that can be picked from.
    /// </summary>
    [DataField]
    public List<EntityEffect> PlantEffects = [];

    /// <summary>
    /// List of RandomFills that can be picked from.
    /// </summary>
    [DataField]
    public List<EntityEffect> HarvesterEffects = [];

    /// <summary>
    /// List of RandomFills that can be picked from.
    /// </summary>
    [DataField]
    public List<EntityEffect> OnHarvestEffects = [];
}
