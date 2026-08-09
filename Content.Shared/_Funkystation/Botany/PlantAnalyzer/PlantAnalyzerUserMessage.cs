using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Botany.PlantAnalyzer;

[Serializable, NetSerializable]
public sealed class PlantAnalyzerUserMessage : BoundUserInterfaceMessage
{
    public readonly NetEntity? TargetEntity;
    public int AnalyzerTier;
    public float Production;
    public float Maturation;
    public int Yield;
    public float Potency;
    public List<string>? ChemsBasic;
    public string PlantName;


    public float Lifespan;

    public float NutrientCons;

    public float WaterCons;

    public float IdealHeat;


    public PlantAnalyzerUserMessage(NetEntity? targetEntity, int analyzerTier, float production, float maturation,
        int yield, float potency, List<string>? chems, string plantName,
        float lifespan, float nutrientCons, float waterCons, float idealHeat)
    {
        TargetEntity = targetEntity;
        AnalyzerTier = analyzerTier;

        //Tier 1 and above stats
        Production = production;
        Maturation = maturation;
        Yield = yield;
        PlantName = plantName;
        Potency = potency;

        Lifespan = lifespan;
        NutrientCons = nutrientCons;
        WaterCons = waterCons;
        IdealHeat = idealHeat;

        if (analyzerTier > 1)
        {
            ChemsBasic = chems;
        }

        if (analyzerTier > 2)
        {

        }
    }
}
