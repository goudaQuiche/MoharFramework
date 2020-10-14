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
        Pawn myPawn = null;
        Map myMap = null;

        //60 = 1 sec; 1800 = 30s
        //private readonly int CheckEveryXTicks = 1800;

        bool SafeRemoval = LoadedModManager.GetMod<OHPLS_Mod>().GetSettings<OHPLS_Settings>().SafeRemoval;

        bool shouldSkip = false;

        public HediffCompProperties_LifeStageHediffAssociation Props => (HediffCompProperties_LifeStageHediffAssociation)this.props;
        bool HasBPLabel => !Props.bodyPartLabel.NullOrEmpty();
        bool HasBPDef => Props.bodyPartDef != null;
        bool HasBPSpecification => HasBPDef || HasBPLabel;

        bool myDebug => Props.debug;

        public override void CompPostMake()
        {
            Init();
        }

        public void UpdateHediffDependingOnLifeStage()
        {
            LifeStageDef lifeStageDef = myPawn.ageTracker.CurLifeStage;
            foreach(Association association in Props.associations)
            {
                // unlegitimate hediff regarding lifestage
                if(Pawn.HasHediff(association.hediff) && association.lifeStageDef != lifeStageDef)
                {
                    Hediff destroyhediff = myPawn.health.hediffSet.GetFirstHediffOfDef(association.hediff);
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
                            bodyPartRecords = myPawn.RaceProps.body.GetPartsWithDef(Props.bodyPartDef).Where(bp => bp.customLabel == Props.bodyPartLabel);
                        else
                            bodyPartRecords = myPawn.RaceProps.body.GetPartsWithDef(Props.bodyPartDef);

                        if (bodyPartRecords.EnumerableNullOrEmpty())
                        {
                            Tools.Warn("Cant find BPR with def: " + Props.bodyPartDef.defName + ", skipping", myDebug);
                            continue;
                        }
                        myBPR = bodyPartRecords.FirstOrFallback();
                    }
                    else
                        myBPR = null;

                    if(HasBPSpecification && myBPR == null)
                    {
                        Tools.Warn("Cant find BPR with def: " + Props.bodyPartDef.defName + ", skipping", myDebug);
                        continue;
                    }

                    lifeStageHediff = HediffMaker.MakeHediff(association.hediff, myPawn, myBPR);
                    if (lifeStageHediff == null)
                    {
                        Tools.Warn("hediff maker null", Props.debug);
                    }
                    myPawn.health.AddHediff(lifeStageHediff, myBPR, null);
                }
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (myPawn == null)
                Init();

            if (shouldSkip)
                return;

            if (!myPawn.Spawned)
            {
                Tools.Warn("pawn unspawned", Props.debug);
                return;
            }

            if (Tools.TrueEvery30Sec)
                UpdateHediffDependingOnLifeStage();
        }

        public void Init()
        {
            Tools.Warn("Entering Init", Props.debug);
            myPawn = parent.pawn;
            myMap = myPawn.Map;

            if (SafeRemoval)
            {
                Log.Warning("SafeModRemoval activated");
                parent.Severity = 0;
                shouldSkip = true;
                parent.PostRemoved();
                return;
            }
            if(myPawn == null || myMap == null)
            {
                Tools.Warn("Null pawn or map", Props.debug);
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
                Tools.Warn("no Props.associations found, destroying hediff", Props.debug);
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
                        myPawn.PawnResumeString() 
                        //+ "; hasAssociationHediffMaster: " + myPawn.Has_OHPLS()
                       ;
                }

                return result;
            }
        }

    }
}
