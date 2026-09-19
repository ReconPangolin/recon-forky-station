using Content.Shared.EntityTable.EntitySelectors;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany.Selector;

public sealed partial class GroupMutationSelector : BaseMutationSelector
{
    [DataField(required: true)]
    public List<BaseMutationSelector> Children = new();

}
