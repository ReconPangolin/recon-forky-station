using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Botany;

[UsedImplicitly]
public sealed partial class SeedSplicerBoundUserInterface: BoundUserInterface
{
    [ViewVariables]
    private SeedSplicerMenu? _splicerWindow;

    public SeedSplicerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {

    }



    protected override void Open()
    {
        base.Open();
        _splicerWindow = this.CreateWindow<SeedSplicerMenu>();
    }
}

