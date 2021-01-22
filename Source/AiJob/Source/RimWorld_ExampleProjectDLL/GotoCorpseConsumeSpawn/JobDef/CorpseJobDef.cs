using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharAiJob
{
    public class CorpseJobDef : Def
    {
        public List<PawnKindDef> workerPawnKind;
        public List<CorpseRecipeSettings> corpseRecipeList;
        public JobDef jobDef;
        public bool debug = false;

        public override string ToString() => defName;
        public CorpseJobDef Named(string searchedDN) => DefDatabase<CorpseJobDef>.GetNamed(searchedDN);
        public override int GetHashCode() => defName.GetHashCode();

        public bool IsEmpty => corpseRecipeList.NullOrEmpty();
    }
    
    public class CorpseRecipeSettings
    {
        public WorkerRequirement workerRequirement;
        public CorpseSpecification target;
        public CorpseProduct product;

        public bool HasWorkerRequirement => workerRequirement != null;

        /*
		public bool HasMinHpRequirement => workerRequirement.HasMinHpRequirement;
		public bool HasFactionRequirement => workerRequirement.HasFactionRequirement;
        public bool HasHediffRequirement => workerRequirement.HasHediffRequirement;
        public bool HasLifeStageRequirement => workerRequirement.HasLifeStageRequirement;
        */

        public bool HasTargetSpec => target != null;
        public bool HasTargetCategory => HasTargetSpec && target.HasCorpseCategoryDef;
        public bool HasRottenSpec => HasTargetSpec && target.HasCorpseRotStages;
    }

    public class WorkerRequirement
    {
        public float minHealthPerc = 0;
        public List<FactionRequirement> factionRequirement;
        public List<HediffRequirement> hediffRequirement;
        public List<LifeStageDef> lifeStageRequirement;

        public bool HasHediffRequirement => !hediffRequirement.NullOrEmpty();
        public bool HasFactionRequirement => !factionRequirement.NullOrEmpty();
        public bool HasLifeStageRequirement => !lifeStageRequirement.NullOrEmpty();

        public bool HasMinHpRequirement => minHealthPerc > 0;
    }

    public class HediffRequirement
    {
        public HediffDef hediff;
        public float severity;
    }
	
	public class FactionRequirement
	{
        public bool noFaction = false;
        public FactionDef belongsToFaction = null;
	}

    public class CorpseProduct
    {
        public List<ThingDefCountClass> thing;
        public List<PawnGenOption> pawnKind = new List<PawnGenOption>();

        public List<WeightedFaction> forcedFaction = null;

        public float manhunterChance = 0;
        public float newBornChance = 0;
        public IntRange pawnNum = new IntRange(1, 1);
        public int combatPowerLimit = -1;
        public float combatPowerPerMass = -1;
        public bool inheritSettingsFromParent = true;
        public bool setRelationsWithParent = true;
        public float newBornCombatPowerRatio = .3f;

        public bool HasThingProduct => !thing.NullOrEmpty();
        public bool HasPawnKindProduct => !pawnKind.NullOrEmpty();

        public bool HasWeightedFaction => !forcedFaction.NullOrEmpty();

        public bool HasRelevantManhunterChance => manhunterChance != 0;
        public bool HasRelevantNewBornChance => newBornChance != 0;
        public bool HasRelevantCombatPowerLimit => combatPowerLimit > 0;
        public bool HasRelevantCombatPowerPerMass => combatPowerPerMass > 0;
    }

    public class WeightedFaction
    {
        public FactionDef factionDef;
        public float weight;
        public bool inheritFromParent = false;
    }

    public class CorpseSpecification
    {
        public List<ThingCategoryDef> categoryDef;
        public List<RotStage> rotStages;

        public FloatRange healthPerc = new FloatRange(0, 1);
        public FloatRange massPerc = new FloatRange(0, 9999);
        public float maxDistance = 10;

        public bool HasCorpseCategoryDef => !categoryDef.NullOrEmpty();
        public bool HasCorpseRotStages => !rotStages.NullOrEmpty();
        public bool HasRelevantHealthPerc => healthPerc.min != 0 || healthPerc.max != 1;
        public bool HasRelevantMassPerc => massPerc.min != 0 || massPerc.max != 9999;
    }
}
