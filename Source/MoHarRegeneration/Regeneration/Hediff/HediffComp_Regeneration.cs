using Verse;
using System;
using RimWorld;

namespace MoHarRegeneration
{
    public class HediffComp_Regeneration : HediffComp
    {
        public HediffCompProperties_Regeneration Props => (HediffCompProperties_Regeneration)this.props;
        public bool MyDebug => Props.debug;

        public bool Effect_TendBleeding => Props.BloodLossTendingParams != null;
        public bool Effect_TendRegularDisease=> Props.RegularDiseaseTendingParams != null;
        public bool Effect_TendChronicDisease => Props.ChronicHediffTendingParams != null;

        public bool Effect_RegeneratePhysicalInjuries => Props.PhysicalInjuryRegenParams != null && Props.PhysicalInjuryRegenParams.TargetedHediffDefs.Count != 0;
        public bool Effect_HealDiseases => Props.DiseaseHediffRegenParams != null && Props.DiseaseHediffRegenParams.TargetedHediffDefs.Count != 0;
        public bool Effect_RemoveChemicals => Props.ChemicalHediffRegenParams != null && Props.ChemicalHediffRegenParams.TargetedHediffDefs.Count != 0;

        public bool Effect_RemoveScares => Props.PermanentInjuryRegenParams != null;
        public bool Effect_RegenerateBodyParts => Props.BodyPartRegenParams != null;

        public bool Effect_GrowProsthetic => !Props.BodyPartRegenParams.techHediffTag.NullOrEmpty();
        public bool Effect_PartialHealthUponRegrow => Props.BodyPartRegenParams.BPMaxHealth < 1;
        public bool Effect_RegenBodyPartChildrenAtOnce => Props.BodyPartRegenParams.RegenBodyPartChildrenAtOnce;

        public bool HasPendingTreatment => currentHT != MyDefs.HealingTask.None;
        public bool HasNoPendingTreatment => !HasPendingTreatment;
        public bool DoesNotKnowIfTreatment => CheckingTickCounter != 0;

        public bool HasLimits => Props.Limit != null;
        public bool ExceedsQuantity => Props.Limit.IsQuantityLimited ? TreatmentPerformedNum >= Props.Limit.LimitedTreatmentQuantity : false;
        public bool ExceedsQuality => Props.Limit.IsQualityLimited ? TreatmentPerformedQuality >= Props.Limit.LimitedTreatmentQuality : false;

        public int CheckingTickCounter = 0;
        public int HealingTickCounter = 0;
        public float BodyPartsHealthSum = 0;

        public int TreatmentPerformedNum = 0;
        public float TreatmentPerformedQuality = 0;

        public Hediff currentHediff;
        public MyDefs.HealingTask currentHT;
        //public RegenerationPriority regenerationPriority;

        public override bool CompShouldRemove {
            get
            {
                return base.CompShouldRemove || (HasLimits ? (ExceedsQuantity || ExceedsQuality) : false);
            }
        }

        public override void CompPostMake()
        {
            Tools.Warn("HediffComp_Regeneration - CompPostMake", MyDebug);
            InitCheckCounter();
            InitBodyPartsHP();
            //regenerationPriority = new RegenerationPriority(this);
            if (MyDebug)
                Log.Warning(MyDefs.DumpDefaultPriority());
        }

        public string SecondsBeforeNextTreatment
        {
            get
            {
                return HealingTickCounter.TicksToSeconds().ToString("0.0");
            }
        }

        public string SecondsBeforePillTakesEffect
        {
            get
            {
                return CheckingTickCounter.TicksToSeconds().ToString("0.0");
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.NegligeablePawn())
                return;

            if (HasPendingTreatment)
            {
                if(currentHediff == null)
                {
                    currentHT = this.InitHealingTask(out currentHediff, out HealingTickCounter);
                    return;
                }

                if(HealingTickCounter-- <= 0)
                {
                    this.Dispatcher();
                }
            }
            else
            {
                if (CheckingTickCounter-- <= 0)
                {
                    NextHediff();
                    InitCheckCounter();
                }
            }
        }

        public void InitBodyPartsHP()
        {
            BodyPartsHealthSum = Pawn.GetAllBodyPartsHealthSum();
        }

        public void InitCheckCounter()
        {
            CheckingTickCounter = Props.CheckingTicksPeriod;
        }

        public void NextHediff()
        {
            Hediff oldHediff = currentHediff;

            currentHT = this.InitHealingTask(out currentHediff, out HealingTickCounter);

            this.ApplyProgressHediff(oldHediff);
        }
        public void NextHediffWithoutTickReset()
        {
            currentHT = this.InitHealingTask(out currentHediff, out _);
            HealingTickCounter = 5;
        }


        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref CheckingTickCounter, "MoHarRegen.CheckingTickCounter");
            Scribe_Values.Look(ref HealingTickCounter, "MoHarRegen.HealingTickCounter");

            Scribe_References.Look(ref currentHediff, "MoHarRegen.currentHediff");
            Scribe_Values.Look(ref currentHT, "MoHarRegen.currentHT");

            Scribe_Values.Look(ref BodyPartsHealthSum, "MoHarRegen.BodyPartsHealthSum");

            Scribe_Values.Look(ref TreatmentPerformedNum, "MoHarRegen.TreatmentPerformedNum");
            Scribe_Values.Look(ref TreatmentPerformedQuality, "MoHarRegen.TreatmentPerformedQuality");
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                //result += "Puff in " + this.sprayTicksLeft.ToStringTicksToPeriod();
                if (MyDebug)
                    if (HasPendingTreatment)
                        //+ "s. before " +  + " next progress";
                        result += "MoHarRegeneration.CompTipStringExtra".Translate(SecondsBeforeNextTreatment, this.GetTreatmentLabel());
                             
                    /*else if(DoesNotKnowIfTreatment)
                        result += SecondsBeforePillTakesEffect + "s. before medicament effect";
                        */

                return result;
            }
        }
    }
}
