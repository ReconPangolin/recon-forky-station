using Content.Shared._Funkystation.Botany.Components;
using Content.Shared.Botany.Events;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class FuPlantMutationSystem : EntitySystem
{

    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private SharedEntityEffectsSystem _entityEffects = default!;

    [SubscribeLocalEvent]
    private void OnInit(Entity<FuPlantMutationComponent> ent, ref ComponentStartup args)
    {
        foreach (var mutationId in ent.Comp.StartingMutations)
        {
            //TODO: Add proper method
            _prototypeManager.Index(mutationId);

            var mutation = Spawn(mutationId);

            if (!TryComp<FuPlantEffectComponent>(mutation, out var comp))
            {
                Del(mutation);
                continue;
            }

            ent.Comp.Mutations.Add(mutation);
        }
    }

    [SubscribeLocalEvent]
    private void OnAfterDoHarvest(Entity<FuPlantMutationComponent> ent, ref AfterDoHarvestEvent args)
    {
        //TODO: Optimise
        foreach (var mutationId in ent.Comp.Mutations)
        {

            if (!TryComp<FuPlantEffectComponent>(mutationId, out var comp))
                continue;


            foreach (var effect in comp.Effects)
            {

                if ((PlantEffectType) effect.Item1 == PlantEffectType.Harvester)
                {
                    _entityEffects.TryApplyEffect(args.User, effect.Item2);
                }
                else if ((PlantEffectType) effect.Item1 == PlantEffectType.OnHarvest)
                {
                    _entityEffects.TryApplyEffect(ent, effect.Item2);
                }

            }
        }
    }
}
