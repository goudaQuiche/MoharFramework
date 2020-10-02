using Verse;
using System;
using RimWorld;

namespace MoHarRegeneration
{
    public class HediffComp_Regeneration : HediffComp
    {
        public HediffCompProperties_Regeneration Props => (HediffCompProperties_Regeneration)this.props;
        bool MyDebug => Props.debug;

        public bool Effect_TendBleeding => Props.BleedingHediff != null;
        public bool Effect_RegeneratePhysicalInjuries => Props.PhysicalHediff != null && Props.PhysicalHediff.HediffDefs.Count != 0;
        public bool Effect_HealDiseases => Props.DiseaseHediff != null && Props.DiseaseHediff.HediffDefs.Count != 0;
        public bool Effect_RemoveChemicals => Props.ChemicalHediff != null && Props.ChemicalHediff.HediffDefs.Count != 0;

        public bool Effect_TendChronicDisease => Props.ChronicHediff != null && Props.ChronicHediff.HediffDefs.Count != 0;

        public bool Effect_RemoveScares => Props.PermanentInjury != null;
        public bool Effect_RegenerateBodyParts => Props.BodyPartRegeneration != null;

        public bool HasPendingTreatment => currentHT != RegenParamsUtility.HealingTask.None;
        public bool HasNoPendingTreatment => !HasPendingTreatment;

        int CheckingTickCounter = 0;
        int HealingTickCounter = 0;

        public Hediff currentHediff;
        public RegenParamsUtility.HealingTask currentHT;
        public RegenerationPriority regenerationPriority;

        public override void CompPostMake()
        {
            InitCheckCounter();
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
                    bool DidIt = false;
                    bool DoneWithIt = false;

                    if ( currentHT == RegenParamsUtility.HealingTask.BleedingTending) 
                    {
                        DidIt = this.TryTendBleeding();
                    }
                    else if (currentHT == RegenParamsUtility.HealingTask.ChronicDisease)
                    {
                        DidIt = this.TryTendChronic();
                    }
                    else if(currentHT == RegenParamsUtility.HealingTask.InjuryRegeneration)
                    {
                        DidIt = this.TryRegenInjury(out DoneWithIt);
                        if (DoneWithIt)
                            currentHediff = null;
                    }
                      
                    if(DidIt)
                        Tools.Warn(Pawn.LabelShort + " had " + currentHT.DescriptionAttr() + " performed", MyDebug);
                    if (DoneWithIt)
                        Tools.Warn(Pawn.LabelShort + " had " + currentHT.DescriptionAttr() + " fully cured/healed/regen", MyDebug);
                }
            }
            else
            {
                if (CheckingTickCounter-- <= 0)
                {
                    currentHT = this.InitHealingTask(out currentHediff, out HealingTickCounter);
                    InitCheckCounter();
                }
            }
        }

        public void InitCheckCounter()
        {
            CheckingTickCounter = Props.CheckingTicksPeriod;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                //result += "Puff in " + this.sprayTicksLeft.ToStringTicksToPeriod();
                return result;
            }
        }
    }
}
