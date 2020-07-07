using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Verse;
using RimWorld;

namespace OHPG
{
    public class HediffComp_GenderHediffAssociation : HediffComp
    {
        Pawn myPawn = null;
        Map myMap = null;

        //60 = 1 sec; 1800 = 30s
        //private readonly int CheckEveryXTicks = 1800;

        bool SafeRemoval = LoadedModManager.GetMod<OHPG_Mod>().GetSettings<OHPG_Settings>().SafeRemoval;
        bool shouldSkip = false;

        public HediffCompProperties_GenderHediffAssociation Props
        {
            get
            {
                return (HediffCompProperties_GenderHediffAssociation)this.props;
            }
        }

        public override void CompPostMake()
        {
            Init();
        }

        public void UpdateHediffDependingOnGender()
        {
            Gender pGender = myPawn.gender;
            foreach(Association association in Props.associations)
            {
                // unlegitimate hediff regarding lifestage
                if(Pawn.HasHediff(association.hediff) && association.gender != pGender)
                {
                    Hediff destroyhediff = myPawn.health.hediffSet.GetFirstHediffOfDef(association.hediff);
                    destroyhediff.Severity = 0; destroyhediff.PostRemoved();
                }

                // missing hediff for cur lifestage
                if (!Pawn.HasHediff(association.hediff) && association.gender == pGender)
                {
                    Hediff lifeStageHediff = null;
                    IEnumerable<BodyPartRecord> bodyPartRecords;

                    if (!Props.bodyPartLabel.NullOrEmpty())
                        bodyPartRecords = myPawn.RaceProps.body.GetPartsWithDef(Props.bodyPartDef).Where(bp => bp.customLabel == Props.bodyPartLabel);
                    else
                        bodyPartRecords = myPawn.RaceProps.body.GetPartsWithDef(Props.bodyPartDef);

                    if (bodyPartRecords.EnumerableNullOrEmpty())
                    {
                        Log.Error("Cant find BPR with def: " + Props.bodyPartDef.defName);
                        continue;
                    }
                    BodyPartRecord myBPR = bodyPartRecords.First();


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
                UpdateHediffDependingOnGender();
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
                Log.Error("no race set in the OHPG hediffCompProps, you need one, destroying hediff");
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

            if(Props.bodyPartDef == null)
            {
                Log.Error("no bodyPartDef set in the OHPG  hediffCompProps, you need one, destroying hediff");
                parent.Severity = 0;
                shouldSkip = true;
                return;
            }
            UpdateHediffDependingOnGender();
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                if (Props.debug)
                {
                    if (Props.debug)
                    {
                        result +=
                            myPawn.PawnResumeString() + 
                            "; hasAssociationHediffMaster: " + myPawn.Has_OHPG()
                           ;
                    }
                }
                return result;
            }
        }

    }
}
