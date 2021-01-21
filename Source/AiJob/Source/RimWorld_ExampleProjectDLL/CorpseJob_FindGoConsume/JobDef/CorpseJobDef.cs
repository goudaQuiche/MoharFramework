using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharAiJob
{
    /*
    public class CorpseRecipeJob : IExposable, ILoadReferenceable
    {
        public int id;
        public string label;
        public CorpseRecipeSettingsDef def;

        public string GetUniqueLoadID()
        {
            return "CorpseRecipeJob_" + label + id;
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref id, "id", -1);
        }
    }
    */

    public class CorpseJobDef : Def
    {
        public List<PawnKindDef> worker;
        public List<CorpseRecipeSettings> corpseRecipeList;
        public bool debug = false;

        public override string ToString() => defName;
        public CorpseJobDef Named(string searchedDN) => DefDatabase<CorpseJobDef>.GetNamed(searchedDN);
        public bool IsEmpty => corpseRecipeList.NullOrEmpty();

        public override int GetHashCode() => defName.GetHashCode();
        /*
        static CorpseJobDef()
        {

        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();
        }
        */

    }
    
    public class CorpseRecipeSettings
    {
        public Type driverClass;
        public JobDef jobDef;

        public WorkerRequirement workerRequirement;
        public CorpseSpecification target;
        public CorpseProduct product;

        public bool HasWorkerRequirement => workerRequirement != null;

		public bool HasMinHpRequirement => workerRequirement.HasMinHpRequirement;
		public bool HasFactionRequirement => workerRequirement.HasFactionRequirement;
        public bool HasHediffRequirement => workerRequirement.HasHediffRequirement;

        public bool HasTargetSpec => target != null;
        public bool HasTargetCategory => HasTargetSpec && target.HasCorpseCategoryDef;
        public bool HasRottenSpec => HasTargetSpec && target.HasCorpseRotStages;
    }

    public class WorkerRequirement
    {
        public float minHealth = 0;
        public List<FactionRequirement> needsFaction;
        public List<HediffRequirement> needsHediff;

        public bool HasHediffRequirement => !needsHediff.NullOrEmpty();
        public bool HasFactionRequirement => !needsFaction.NullOrEmpty();
        public bool HasMinHpRequirement => minHealth > 0;
    }

    public class HediffRequirement
    {
        public HediffDef hediff;
        public float Severity;
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

        public FactionDef forcedFaction = null;
        public float manhunterChance = .25f;

        public bool HasThingProduct => !thing.NullOrEmpty();
        public bool HasPawnKindProduct => !pawnKind.NullOrEmpty();
    }

    public class CorpseSpecification
    {
        public List<ThingCategoryDef> categoryDef;
        public List<RotStage> rotStages;

        public float minHealthPerc = .25f;
        public float maxDistance = 10;

        public bool HasCorpseCategoryDef => !categoryDef.NullOrEmpty();
        public bool HasCorpseRotStages => !rotStages.NullOrEmpty();
    }
}
