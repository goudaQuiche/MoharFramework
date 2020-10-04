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


        public bool HasPendingTreatment => currentHT != MyDefs.HealingTask.None;
        public bool HasNoPendingTreatment => !HasPendingTreatment;
        public bool DoesNotKnowIfTreatment => CheckingTickCounter != 0;

        int CheckingTickCounter = 0;
        int HealingTickCounter = 0;
        public float BodyPartsHealthSum = 0;


        public Hediff currentHediff;
        public MyDefs.HealingTask currentHT;
        //public RegenerationPriority regenerationPriority;

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
                    bool DidIt = false;
                    bool DoneWithIt = false;
                    bool Impossible = false;
                    bool NextHediffIfDidIt = false;
                    bool NextHediffIfDoneWithIt = false;
                    ThingDef MyMoteDef = null;

                    // 00 Tending - Blood loss
                    if ( currentHT.IsBloodLossTending() ) 
                    {
                        NextHediffIfDidIt = true;
                        MyMoteDef = Props.BloodLossTendingParams.MoteDef;
                        DidIt = this.TryTendBleeding(out Impossible);
                    }
                    // 01 Tending - Chronic disease
                    else if (currentHT.IsChronicDiseaseTending() )
                    {
                        NextHediffIfDidIt = true;
                        MyMoteDef = Props.ChronicHediffTendingParams.MoteDef;
                        DidIt = this.TryTendChronic(out Impossible);
                    }
                    // 02 Tending - Regular disease
                    else if (currentHT.IsRegularDiseaseTending())
                    {
                        NextHediffIfDidIt = true;
                        MyMoteDef = Props.RegularDiseaseTendingParams.MoteDef;
                        DidIt = this.TryTendRegularDisease(out Impossible);
                    }
                    // 03 Regeneration - Injury 
                    else if (currentHT.IsDiseaseHealing())
                    {
                        NextHediffIfDoneWithIt = true;
                        MyMoteDef = Props.DiseaseHediffRegenParams.MoteDef;
                        DidIt = this.TryCureDisease(out DoneWithIt, out Impossible);
                    }
                    // 04 Regeneration - Injury 
                    else if (currentHT.IsInjuryRegeneration())
                    {
                        NextHediffIfDoneWithIt = true;
                        MyMoteDef = Props.PhysicalInjuryRegenParams.MoteDef;
                        DidIt = this.TryRegenInjury(out DoneWithIt, out Impossible);
                    }
                    // 05 Regeneration - Chemical 
                    else if (currentHT.IsChemicalRemoval())
                    {
                        NextHediffIfDoneWithIt = true;
                        DidIt = this.TryChemicalRemoval(out DoneWithIt, out Impossible);
                    }
                    // 06 Regeneration - Permanent injury
                    else if (currentHT.IsPermanentInjuryRegeneration())
                    {
                        NextHediffIfDoneWithIt = true;
                        MyMoteDef = Props.PermanentInjuryRegenParams.MoteDef;
                        DidIt = this.TryRemovePermanentInjury(out DoneWithIt, out Impossible);
                    }
                    // 07 Regeneration -Bodypart
                    else if (currentHT.IsBodyPartRegeneration())
                    {
                        NextHediffIfDidIt = true;
                        bool AppliedProsthetic = false;

                        if (Effect_GrowProsthetic)
                        {
                            HediffDef ProstheticHediff = this.TryFindBodyPartProsthetic();
                            if(ProstheticHediff!=null)
                                DidIt = AppliedProsthetic = this.TryRegrowProsthetic(ProstheticHediff);
                        }

                        if (!AppliedProsthetic)
                        {
                            MyMoteDef = Props.BodyPartRegenParams.MoteDef;
                            if (Props.BodyPartRegenParams.FullyRegenBodyPart)
                                DidIt = this.TryBodyPartFullRegeneration(out Impossible);
                            else
                                DidIt = this.TryBodyPartRegeneration(out Impossible);
                        }
                    }
                    
                    if(DidIt)
                        Tools.Warn(Pawn.LabelShort + " had " + currentHT.DescriptionAttr() + " performed", MyDebug);
                    if (DoneWithIt)
                        Tools.Warn(Pawn.LabelShort + " had " + currentHT.DescriptionAttr() + " fully cured/healed/regen", MyDebug);

                    //if (!DidIt)
                    HealingTickCounter = this.ResetHealingTicks();

                    if (NextHediffIfDidIt && DidIt || NextHediffIfDoneWithIt && DoneWithIt)
                    {
                        if(MyMoteDef != null)
                            MoteMaker.ThrowMetaIcon(Pawn.Position, Pawn.Map, MyMoteDef);

                        NextHediff();
                        Tools.Warn(Pawn.LabelShort + " new HT: " + currentHT.DescriptionAttr(), MyDebug);
                    }
                    else if (Impossible)
                    {
                        NextHediff();
                        Tools.Warn(Pawn.LabelShort + " Impossible to heal hediff found - new HT: " + currentHT.DescriptionAttr(), MyDebug);
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
            currentHT = this.InitHealingTask(out currentHediff, out HealingTickCounter);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref CheckingTickCounter, "MoHarRegen.CheckingTickCounter");
            Scribe_Values.Look(ref HealingTickCounter, "MoHarRegen.HealingTickCounter");

            Scribe_References.Look(ref currentHediff, "MoHarRegen.currentHediff");
            Scribe_Values.Look(ref currentHT, "MoHarRegen.currentHT");

            Scribe_Values.Look(ref BodyPartsHealthSum, "MoHarRegen.BodyPartsHealthSum");

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
                    /*else if(DoesNotKnowIfTreatment)
                        result += SecondsBeforePillTakesEffect + "s. before medicament effect";
                        */

                return result;
            }
        }
    }
}
