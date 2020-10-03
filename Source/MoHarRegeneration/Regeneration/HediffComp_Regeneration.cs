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

        public bool Effect_RegeneratePhysicalInjuries => Props.PhysicalInjuryRegenParams != null && Props.PhysicalInjuryRegenParams.HediffDefs.Count != 0;
        public bool Effect_HealDiseases => Props.DiseaseHediffRegenParams != null && Props.DiseaseHediffRegenParams.HediffDefs.Count != 0;
        public bool Effect_RemoveChemicals => Props.ChemicalHediffRegenParams != null && Props.ChemicalHediffRegenParams.HediffDefs.Count != 0;

        public bool Effect_RemoveScares => Props.PermanentInjuryRegenParams != null;
        public bool Effect_RegenerateBodyParts => Props.BodyPartRegenParams != null;

        public bool HasPendingTreatment => currentHT != MyDefs.HealingTask.None;
        public bool HasNoPendingTreatment => !HasPendingTreatment;

        int CheckingTickCounter = 0;
        int HealingTickCounter = 0;

        public Hediff currentHediff;
        public MyDefs.HealingTask currentHT;
        public RegenerationPriority regenerationPriority;

        public override void CompPostMake()
        {
            InitCheckCounter();
            regenerationPriority = new RegenerationPriority(this);
            if (MyDebug)
                Log.Warning(regenerationPriority.DumpDefaultPriority());
        }

        public string SecondsBeforeNextTreatment
        {
            get
            {
                return HealingTickCounter.TicksToSeconds().ToString("0.0");
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
                    bool DidIt = false;
                    bool DoneWithIt = false;
                    bool NextHediffIfDidIt = false;
                    bool NextHediffIfDoneWithIt = false;

                    // 00 Tending - Blood loss
                    if ( currentHT.IsBloodLossTending() ) 
                    {
                        NextHediffIfDidIt = true;
                        DidIt = this.TryTendBleeding();
                    }
                    // 01 Tending - Chronic disease
                    else if (currentHT.IsChronicDiseaseTending() )
                    {
                        NextHediffIfDidIt = true;
                        DidIt = this.TryTendChronic();
                    }
                    // 02 Tending - Regular disease
                    else if (currentHT.IsRegularDiseaseTending())
                    {
                        NextHediffIfDidIt = true;
                        DidIt = this.TryTendRegularDisease();
                    }
                    // 03 Regeneration - Injury 
                    else if (currentHT.IsDiseaseHealing())
                    {
                        NextHediffIfDoneWithIt = true;
                        DidIt = this.TryCureDisease(out DoneWithIt);
                    }
                    // 04 Regeneration - Injury 
                    else if (currentHT.IsInjuryRegeneration())
                    {
                        NextHediffIfDoneWithIt = true;
                        DidIt = this.TryRegenInjury(out DoneWithIt);
                    }
                    // 05 Regeneration - Chemical 
                    else if (currentHT.IsChemicalRemoval())
                    {
                        NextHediffIfDoneWithIt = true;
                        DidIt = this.TryChemicalRemoval(out DoneWithIt);
                    }
                    // 06 Regeneration - Permanent injury
                    else if (currentHT.IsPermanentInjuryRegeneration())
                    {
                        NextHediffIfDoneWithIt = true;
                        DidIt = this.TryRemovePermanentInjury(out DoneWithIt);
                    }
                    // 07 Regeneration -Bodypart
                    else if (currentHT.IsBodyPartRegeneration())
                    {
                        NextHediffIfDidIt = true;
                        DidIt = this.TryBodyPartRegeneration();
                    }
                    
                    if(DidIt)
                        Tools.Warn(Pawn.LabelShort + " had " + currentHT.DescriptionAttr() + " performed", MyDebug);
                    if (DoneWithIt)
                        Tools.Warn(Pawn.LabelShort + " had " + currentHT.DescriptionAttr() + " fully cured/healed/regen", MyDebug);

                    if (NextHediffIfDidIt && DidIt || NextHediffIfDoneWithIt && DoneWithIt)
                    {
                        NextHediff();
                        Tools.Warn(Pawn.LabelShort + " new HT: " + currentHT.DescriptionAttr(), MyDebug);
                    }
                        
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

        public void InitCheckCounter()
        {
            CheckingTickCounter = Props.CheckingTicksPeriod;
        }

        public void NextHediff()
        {
            currentHT = this.InitHealingTask(out currentHediff, out HealingTickCounter);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref CheckingTickCounter, "MoHarRegen.CheckingTickCounter");
            Scribe_References.Look(ref currentHediff, "MoHarRegen.currentHediff");
            Scribe_Values.Look(ref currentHT, "MoHarRegen.currentHT");

        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                //result += "Puff in " + this.sprayTicksLeft.ToStringTicksToPeriod();
                if (MyDebug)
                    if (HasPendingTreatment)
                        result += SecondsBeforeNextTreatment + "s. before " + currentHT.DescriptionAttr() + " next progress";

                return result;
            }
        }
    }
}
