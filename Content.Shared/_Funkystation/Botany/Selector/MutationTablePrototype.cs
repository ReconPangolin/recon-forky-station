using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany.Selector;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype]
public sealed partial class MutationTablePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The Entity Table associated with this prototype.
    /// </summary>
    [DataField(required: true)]
    public BaseMutationSelector Table = default!;
}
