using System.Collections.Generic;
using System.Linq;

using Verse;
using RimWorld;
using Ubet;

namespace YAHA
{
    /*
     * Todo: 
     * takeDamage trigger ; override postpostdamage
     */
    public class HediffComp_YetAnotherHediffApplier : HediffComp
    {
        Map myMap = null;
        public HediffCompProperties_YetAnotherHediffApplier Props => (HediffCompProperties_YetAnotherHediffApplier)props;

        public List<AssociatedHediffHistory> Registry = new List<AssociatedHediffHistory>();

        public bool ShouldSkip = false;
        public bool MyDebug => Props.debug;

        public bool TriggeredOnlyHediffs = false;
        public List<TriggerEvent> WoundTriggers = new List<TriggerEvent>();
        public bool HasWoundTrigger => !WoundTriggers.NullOrEmpty();

        public int UnspawnedGrace = 0;

        public int UpdateNumthisTick = 0;

        public bool HasEmptyRegistry => Registry.NullOrEmpty();
        public bool HasRegistry => !HasEmptyRegistry;

        public override void CompPostMake()
        {
            Init();
        }



        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);

            this.WoundTriggerManager(dinfo);
        }
        

        public override void CompPostTick(ref float severityAdjustment)
        {
            UpdateNumthisTick = 0;

            if (UnspawnedGrace > 0)
            {
                UnspawnedGrace--;
                if (UnspawnedGrace == 0)
                    ShouldSkip = false;
            }

            if (ShouldSkip)
                return;

            if (Pawn.Spawned)
            {
                    if (myMap == null)
                    {
                        Init();
                        return;
                    }
            }
            else
            {
                if (MyDebug) Log.Warning(Pawn.Name + " : pawn unspawned - Entering grace");
                UnspawnedGrace = Props.UnspawnedGrace;
                ShouldSkip = true;
                return;
            }

            this.DidNothing();

            // no need to periodic check, since associations are event triggered
            if (TriggeredOnlyHediffs)
                return;

            if (Props.PeriodicCheck && Pawn.IsHashIntervalTick(Props.checkFrequency))
                this.CheckAllHediffAssociations();
        }


        public void Init()
        {
            if (MyDebug) Log.Warning("Entering Init");

            myMap = Pawn.Map;

            // pawn is not spawned ? caravaning ? from another faction ?
            if (!Pawn.Spawned)
            {
                if (MyDebug) Log.Warning("Null map");
                //shouldSkip = true;
                return;
            }

            this.CreateRegistry();

            this.SetTriggerOnly();
            this.SetHasAnyWoundTrigger();

            if (MyDebug)
            {
                Log.Warning("HasWoundTrigger:" + HasWoundTrigger);
                Log.Warning("TriggeredOnlyHediffs:" + TriggeredOnlyHediffs);
            }

            /*
             * if (TriggeredOnlyHediffs)
                return;
                */

            this.CheckAllHediffAssociations();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();

            Scribe_Values.Look(ref TriggeredOnlyHediffs, "TriggeredOnlyHediffs");

            Scribe_Collections.Look(ref WoundTriggers, "WoundTriggers");
            if (Scribe.mode == LoadSaveMode.PostLoadInit && WoundTriggers == null)
            {
                WoundTriggers = new List<TriggerEvent>();
            }

            Scribe_Collections.Look(ref Registry, "Registry");
            if (Scribe.mode == LoadSaveMode.PostLoadInit && Registry == null)
            {
                Registry = new List<AssociatedHediffHistory>();
            }
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;

                if (Props.debug)
                {
                    result +=
                        Pawn.PawnResumeString() +
                        (Props.associations.NullOrEmpty() ? "empty association" : Props.associations.Count + " hediff associations") +
                        (Registry.NullOrEmpty()?"empty registry":Registry.Count + " registered hediff associations")
                        //+ "; hasAssociationHediffMaster: " + myPawn.Has_OHPLS()
                       ;
                }

                return result;
            }
        }

    }
}
