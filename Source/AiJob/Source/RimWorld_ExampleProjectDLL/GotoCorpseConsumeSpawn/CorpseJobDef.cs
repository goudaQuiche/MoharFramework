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
        public WorkerRequirement worker;
        public CorpseSpecification target;
        public CorpseProduct product;
        public WorkFlow workFlow;

        public bool HasWorkerSpec => worker != null;

        private bool HasTarget => target != null;
        public bool HasTargetSpec => HasTarget && target.HasCorpseCategoryDef;
        public bool HasRottenSpec => HasTargetSpec && target.HasCorpseRotStages;

        private bool HasProduct => product != null;
        public bool HasProductSpec => HasProduct && product.HasPawnKindProduct;

        public bool HasWorkFlow => workFlow != null;
    }

    public class WorkerRequirement
    {
        public float minHealthPerc = 0;
        public int chancesToWorkDivider = -1;

        public List<FactionRequirement> factionRequirement;
        public List<HediffRequirement> hediffRequirement;
        public List<LifeStageDef> lifeStageRequirement;

        public bool HasHediffRequirement => !hediffRequirement.NullOrEmpty();
        public bool HasFactionRequirement => !factionRequirement.NullOrEmpty();
        public bool HasLifeStageRequirement => !lifeStageRequirement.NullOrEmpty();

        public bool HasRelevantMinHp => minHealthPerc > 0;
        public bool HasRelevantChancesToWorkDivider => chancesToWorkDivider > 0;
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
        //public List<ThingDefCountClass> thing;
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

        //public bool HasThingProduct => !thing.NullOrEmpty();
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
        public FloatRange mass = new FloatRange(0, 9999);
        public float maxDistance = 10;

        public ReservationProcess reservation;

        public bool HasCorpseCategoryDef => !categoryDef.NullOrEmpty();
        public bool HasCorpseRotStages => !rotStages.NullOrEmpty();

        public bool HasRelevantHealthPerc => healthPerc.min != 0 || healthPerc.max != 1;
        public bool HasRelevantMassPerc => mass.min != 0 || mass.max != 9999;

        public bool HasReservationProcess => reservation != null;
    }

    public class ReservationProcess
    {
        public bool reserves = true;
        public bool respectsThingReservation = true;
        public bool respectsFaction = true;
        public bool respectsPawnKind = true;
    }

    public class WorkFlow
    {
        public int workAmount = 300;
        public int workAmountPerHealthScale = -1;
        public int nibblingPeriod = 120;
        public int nibblingPeriodPerHealthScale = -1;

        public float nibblingAmount = -1;

        public SoundDef sustainSound = null;
        public EffecterDef effecterDef = null;

        public bool bloodFilth = true;
        public ThingDef filthDef = null;
        public FloatRange filthPerHealthScale = new FloatRange(0,0);
        public float filthRadius = 1.5f;

        public StripAndDamage strip;

        public bool HasWorkAmountPerHS => workAmountPerHealthScale > 0;
        public bool HasNibblingAmount => nibblingAmount > 0;
        public bool HasNibblingPeriodPerHS => nibblingPeriodPerHealthScale > 0 && HasNibblingAmount;

        public bool HasCustomSustainSound => sustainSound != null;
        public bool HasCustomEffecterDef => effecterDef != null;

        public bool SpawnsFilth => (filthPerHealthScale.min != 0 || filthPerHealthScale.min != 0) && (bloodFilth == true || filthDef != null);

        public bool MustStrip => strip != null && strip.mustStrip;
    }

    public class StripAndDamage
    {
        public bool mustStrip = true;
        public bool mustDamage = true;
        public FloatRange apparelsDamagingRatio = new FloatRange(.35f, .85f);
        public FloatRange primaryDamagingRatio = new FloatRange(.65f, .85f);
        public FloatRange inventoryDamagingRatio = new FloatRange(.15f, .75f);
    }
}
