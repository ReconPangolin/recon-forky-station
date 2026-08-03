using Content.Shared._Funkystation.Botany;
using Robust.Client.GameObjects;

namespace Content.Client._Funkystation.Botany;

/// <summary>
/// Client implementation of the seed splicer updates the UI
/// </summary>
public sealed partial class SeedSplicerSystem : SharedSeedSplicerSystem
{
    [Dependency] private UserInterfaceSystem _ui = default!;

    protected override void UpdateBui(Entity<SeedSplicerComponent> seedSplicer)
    {
        if (_ui.TryGetOpenUi<SeedSplicerBoundUserInterface>(seedSplicer.Owner, SeedSplicerUiKey.Key, out var bui))
        {
            bui.UpdateUi();
        }

    }
}
