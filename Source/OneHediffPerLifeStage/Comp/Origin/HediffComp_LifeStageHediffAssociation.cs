using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Verse;
using RimWorld;

namespace OHPLS
{
    public class HediffComp_LifeStageHediffAssociation : HediffComp
    {
        //Pawn myPawn = null;
        Map myMap = null;

        //60 = 1 sec; 1800 = 30s
        //private readonly int CheckEveryXTicks = 1800;

        bool SafeRemoval = LoadedModManager.GetMod<OHPLS_Mod>().GetSettings<OHPLS_Settings>().SafeRemoval;

        bool shouldSkip = false;

        public HediffCompProperties_LifeStageHediffAssociation Props => (HediffCompProperties_LifeStageHediffAssociation)this.props;
        bool HasBPLabel => !Props.bodyPartLabel.NullOrEmpty();
        bool HasBPDef => Props.bodyPartDef != null;
        bool HasBPSpecification => HasBPDef || HasBPLabel;

        bool MyDebug => Props.debug;

        public override void CompPostMake()
        {
            Init();
        }

        public void UpdateHediffDependingOnLifeStage()
        {
            LifeStageDef lifeStageDef = Pawn.ageTracker.CurLifeStage;
            foreach(Association association in Props.associations)
            {
                // unlegitimate hediff regarding lifestage
                if(Pawn.HasHediff(association.hediff) && association.lifeStageDef != lifeStageDef)
                {
                    Hediff destroyhediff = Pawn.health.hediffSet.GetFirstHediffOfDef(association.hediff);
                    destroyhediff.Severity = 0; destroyhediff.PostRemoved();
                }

                // missing hediff for cur lifestage
                if (!Pawn.HasHediff(association.hediff) && association.lifeStageDef == lifeStageDef)
                {
                    Hediff lifeStageHediff = null;
                    BodyPartRecord myBPR;

                    if (HasBPSpecification)
                    {
                        IEnumerable<BodyPartRecord> bodyPartRecords;
                        if (!Props.bodyPartLabel.NullOrEmpty())
                            bodyPartRecords = Pawn.RaceProps.body.GetPartsWithDef(Props.bodyPartDef).Where(bp => bp.customLabel == Props.bodyPartLabel);
                        else
                            bodyPartRecords = Pawn.RaceProps.body.GetPartsWithDef(Props.bodyPartDef);

                        if (bodyPartRecords.EnumerableNullOrEmpty())
                        {
                            if(MyDebug) Log.Warning("Cant find BPR with def: " + Props.bodyPartDef.defName + ", skipping");
                            continue;
                        }
                        myBPR = bodyPartRecords.FirstOrFallback();
                    }
                    else
                        myBPR = null;

                    if(HasBPSpecification && myBPR == null)
                    {
                        if (MyDebug) Log.Warning("Cant find BPR with def: " + Props.bodyPartDef.defName + ", skipping");
                        continue;
                    }

                    lifeStageHediff = HediffMaker.MakeHediff(association.hediff, Pawn, myBPR);
                    if (lifeStageHediff == null)
                    {
                        if (MyDebug) Log.Warning("hediff maker null");
                    }
                    Pawn.health.AddHediff(lifeStageHediff, myBPR, null);
                }
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (myMap == null)
                Init();
                
            if (shouldSkip)
                return;

            if (!Pawn.Spawned)
            {
                if (MyDebug) Log.Warning("pawn unspawned");
                return;
            }

            //if (Tools.TrueEvery30Sec)
            if (Pawn.IsHashIntervalTick(1800))
                UpdateHediffDependingOnLifeStage();
        }

        public void Init()
        {
            if (MyDebug) Log.Warning("Entering Init");
            //myPawn = parent.pawn;
            myMap = Pawn.Map;

            if (SafeRemoval)
            {
                Log.Warning("SafeModRemoval activated");
                parent.Severity = 0;
                shouldSkip = true;
                parent.PostRemoved();
                return;
            }
            if(Pawn == null || myMap == null)
            {
                if (MyDebug) Log.Warning("Null pawn or map");
                parent.Severity = 0;
                shouldSkip = true;
                return;
            }
            // Race checking
            if (Props.race.NullOrEmpty())
            {
                Log.Error("no race set in the OHPLS hediffCompProps, you need one, destroying hediff");
                parent.Severity = 0;
                shouldSkip = true;
                return;
            }
            // Props array checking
            if (Props.associations.NullOrEmpty())
            {
                if (MyDebug) Log.Warning("no Props.associations found, destroying hediff");
                parent.Severity = 0;
                shouldSkip = true;
                return;
            }

            if(HasBPSpecification)
            {
                if (Pawn.def.race.body.GetPartsWithDef(Props.bodyPartDef).EnumerableNullOrEmpty())
                {
                    Log.Error("no bodyPartDef (" + Props.bodyPartDef + ") found in the race body definition, destroying hediff");
                    parent.Severity = 0;
                    shouldSkip = true;
                    return;
                }
                
            }
            
            UpdateHediffDependingOnLifeStage();
        }


        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;

                if (Props.debug)
                {
                    result +=
                        Pawn.PawnResumeString() 
                        //+ "; hasAssociationHediffMaster: " + myPawn.Has_OHPLS()
                       ;
                }

                return result;
            }
        }

    }
}
