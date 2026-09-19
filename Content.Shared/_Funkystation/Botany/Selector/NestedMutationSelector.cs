using Content.Shared.EntityTable.EntitySelectors;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany.Selector;

public sealed partial class NestedMutationSelector : BaseMutationSelector
{
    /// <summary>
    /// The prototype from which to draw random items.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<MutationTablePrototype> TableId;

}
