﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace MoharHediffs
{
	public class HediffComp_RandySpawnUponDeath : HediffComp
	{
        private bool blockSpawn = false;

        private int randomlyChosenIndex = -1;
        public Faction randomlyChosenItemfaction = null;

        //public bool newBorn = false;

        public readonly float minDaysB4NextErrorLimit = .001f;
        public readonly int spawnCountErrorLimit = 750;

        public HediffCompProperties_RandySpawnUponDeath Props => (HediffCompProperties_RandySpawnUponDeath)this.props;

        public bool MyDebug => Props.debug;

        public bool ValidIndex => randomlyChosenIndex != -1 && Props.settings.HasSomethingToSpawn && randomlyChosenIndex < NumberOfItems;

        public ThingSettings ChosenItem => ValidIndex ? Props.settings.things[randomlyChosenIndex] : null;

        public bool HasSeverityRequirement => Props.requiredMinSeverity > 0;
        public bool FulfilsSeverityRequirement => parent.Severity > Props.requiredMinSeverity;

        public bool HasChosenThing => ChosenItem != null && ChosenItem.thingToSpawn != null;
        public bool HasChosenPawn => ChosenItem != null && (ChosenItem.pawnKindToSpawn != null || ChosenItem.copyParentPawnKind);


        public ThingDef ThingOfChoice => HasChosenThing ? ChosenItem.thingToSpawn : null;
        public PawnKindDef PawnOfChoice
        {
            get
            {
                if (!HasChosenPawn)
                    return null;

                if (ChosenItem.copyParentPawnKind)
                    return Pawn.kindDef;

                return ChosenItem.pawnKindToSpawn;
            }
        }

        public bool HasFilth => FilthToSpawn != null;
        public int NumberOfItems => Props.settings.things.Count;

        public int NumberToSpawn
        {
            get
            {
                if(HasChosenThing && ChosenItem.HasSpecificSettings)
                    return ChosenItem.specificSettings.spawnCount.RandomInRange;
                else if (Props.settings.HasDefaultSettings)
                    return Props.settings.defaultSettings.spawnCount.RandomInRange;

                return 1;
            }
        }

        public bool WeightedSpawn
        {
            get
            {
                 if (HasChosenThing && ChosenItem.HasSpecificSettings)
                    return ChosenItem.specificSettings.weightedSpawnCount;
                else if (Props.settings.HasDefaultSettings)
                    return Props.settings.defaultSettings.weightedSpawnCount;

                return false;
            }
        }

        public ThingDef FilthToSpawn
        {
            get
            {
                if (HasChosenThing && ChosenItem.HasSpecificSettings)
                    return ChosenItem.specificSettings.filthDef;
                else if (Props.settings.HasDefaultSettings)
                    return Props.settings.defaultSettings.filthDef;

                return null;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref randomlyChosenItemfaction, "randomlyChosenItemfaction");
            //Scribe_Values.Look(ref randomlyChosenQuantity, "randomlyChosenQuantity");
            Scribe_Values.Look(ref randomlyChosenIndex, "randomlyChosenIndex");
        }

        public override void CompPostMake()
        {
            Tools.Warn(">>> " + Pawn?.Label + " - " + parent.def.defName + " - CompPostMake start", MyDebug);

            CalculateValues();
        }

        public override void Notify_PawnDied()
        {
            Tools.Warn("Entering HediffComp_RandySpawnUponDeath Notify_PawnDied", MyDebug);
            if (Pawn.Corpse.Negligeable())
            {
                Tools.Warn("Corpse is no more, cant find its position - giving up", MyDebug);
                base.Notify_PawnDied();
                return;
            }
                

            if (blockSpawn)
            {
                Tools.Warn("blockSpawn for some reason- giving up", MyDebug);
                base.Notify_PawnDied();
                return;
            }

            if (HasSeverityRequirement && !FulfilsSeverityRequirement)
            {
                Tools.Warn("hediff severity not fulfiled", MyDebug);
                base.Notify_PawnDied();
                return;
            }

            if (Pawn.Corpse.Negligeable())
            {
                Tools.Warn("corpse negligeable", MyDebug);
                base.Notify_PawnDied();
                return;
            }

            if (CheckShouldSpawn())
            {
                Tools.Warn("Spawn occured", MyDebug);
            }

            if (CheckShouldHandleCorpse())
            {
                Tools.Warn("Corpse handled", MyDebug);
            }

            base.Notify_PawnDied();
        }

        private void CalculateValues()
        {
            randomlyChosenIndex = this.GetWeightedRandomIndex();
            if (HasChosenPawn && ChosenItem.HasFactionParams)
                    this.ComputeRandomFaction();

            if(!ValidIndex)
            {
                BlockAndDestroy(">ERROR< failed to find an index for IP, check and adjust your hediff props", MyDebug);
                return;
            }
        }

        /*
        private void CheckCalculatedValues()
        {
            if (randomlyChosenQuantity > spawnCountErrorLimit)
            {
                BlockAndDestroy(">ERROR< calculatedQuantity is too high: " + randomlyChosenQuantity + "(>" + spawnCountErrorLimit + "), check and adjust your hediff props", MyDebug);
                return;
            }
        }

        private void DumpCalculatedValues()
        {
            Tools.Warn(
                "<<< " +
                "; randomlyChosenQuantity: " + randomlyChosenQuantity + "; "
                , MyDebug
            );
        }
        */

        public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
        {
            Tools.Warn(ErrorLog, myDebug);
            blockSpawn = true;
            Tools.DestroyParentHediff(parent, myDebug);
        }

        private bool CheckShouldSpawn()
        {
            Tools.Warn(Pawn.LabelShort + " CheckShouldSpawn", MyDebug);

            int randomQuantity = this.ComputeSpawnCount();
            Tools.Warn(
                " Trying to spawn " + randomQuantity + " " +
                ThingOfChoice + "/" +
                PawnOfChoice
                , MyDebug
            );

            bool didSpawn = this.TryDoSpawn(Pawn.Corpse, randomQuantity, Pawn.Corpse.Map);
            Tools.Warn("TryDoSpawn: " + didSpawn, MyDebug);

            return didSpawn;
        }

        private bool CheckShouldHandleCorpse()
        {
            Corpse corpse = Pawn.Corpse;

            bool didSomething = false;

            if (Props.StripBeforeDeath && corpse.AnythingToStrip())
            {
                corpse.Strip();
                didSomething = true;
            }
                

            if (Props.destroyBodyUponDeath)
            {
                corpse.DeSpawn();
                didSomething = true;
            }

            return didSomething;
        }


    }
}