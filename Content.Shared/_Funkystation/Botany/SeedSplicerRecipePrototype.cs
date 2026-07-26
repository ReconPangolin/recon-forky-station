using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype()]
public sealed partial class SeedSplicerRecipePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public List<EntProtoId?> Seeds = new();

    [DataField]
    public EntProtoId? Result;

}
