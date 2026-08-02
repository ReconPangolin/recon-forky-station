using Content.Shared._Funkystation.Botany;
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

        _splicerWindow.EjectButtonLeft.OnPressed += _ =>
        {
            SendPredictedMessage(new SeedSplicerEjectMessage(SeedSplicerSlot.LeftSeed));
        };

        _splicerWindow.EjectButtonRight.OnPressed += _ =>
        {
            SendPredictedMessage(new SeedSplicerEjectMessage(SeedSplicerSlot.RightSeed));
        };

        _splicerWindow.CombineButton.OnPressed += _ =>
        {
            SendPredictedMessage(new SeedSplicerActivateMessage());
        };
    }

    public void Update(Entity<SeedSplicerComponent> ent)
    {
        if (_splicerWindow == null)
            return;

        _splicerWindow.Update(ent);
    }
}

