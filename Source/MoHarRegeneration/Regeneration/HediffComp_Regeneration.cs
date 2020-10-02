using Verse;
using System;
using RimWorld;

namespace MoHarRegeneration
{
    public class HediffComp_Regeneration : HediffComp
    {
        public HediffCompProperties_Regeneration Props => (HediffCompProperties_Regeneration)this.props;
        bool MyDebug => Props.debug;

        bool Effect_TendBleeding => Props.BleedingHediff != null;
        bool Effect_RegeneratesPhysicalInjuries => Props.PhysicalHediff != null && Props.PhysicalHediff.HediffDefs.Count != 0;
        bool Effect_HealsDiseases => Props.DiseaseHediff != null && Props.DiseaseHediff.HediffDefs.Count != 0;
        bool Effect_RemovesChemicals => Props.ChemicalHediff != null && Props.ChemicalHediff.HediffDefs.Count != 0;

        bool Effect_RemovesScares => Props.PermanentInjury != null;
        bool Effect_RegeneratesBodyParts => Props.BodyPartRegeneration != null;

        int CheckingTickCounter = 0;
        int HealingTickCounter = 0;

        Hediff CurrentHediff;

        public override void CompPostMake()
        {
            CheckingTickCounter = Props.CheckingTicksPeriod;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.NegligeablePawn())
                return;

            if(CheckingTickCounter-- <= 0)
            {

            }


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
