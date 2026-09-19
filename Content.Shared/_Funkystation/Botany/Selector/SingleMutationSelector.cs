using Content.Shared.EntityTable.EntitySelectors;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany.Selector;

public sealed partial class SingleMutationSelector : BaseMutationSelector
{
    public const string IdDataFieldTag = "id";

    /// <summary>
    /// The prototype this entry yields.
    /// </summary>
    [DataField(IdDataFieldTag, required: true)]
    public ProtoId<FuPlantMutationPrototype> Id;

}
