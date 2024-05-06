using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace MoharHediffs
{
	public class HediffComp_RandySpawnUponDeath : HediffComp
	{
        private bool blockSpawn = false;

        private int RandomIndex = -1;
        public Faction RandomFaction = null;
        public MentalStateDef RandomMS = null;
        private int RandomQuantity = 0;

        //public bool newBorn = false;

        public readonly float minDaysB4NextErrorLimit = .001f;
        public readonly int spawnCountErrorLimit = 750;

        public HediffCompProperties_RandySpawnUponDeath Props => (HediffCompProperties_RandySpawnUponDeath)this.props;

        public bool MyDebug => Props.debug;

        public bool ValidIndex => RandomIndex != -1 && Props.settings.HasSomethingToSpawn && RandomIndex < NumberOfItems;
        public bool ValidQuantity => RandomQuantity > 0;

        public ThingSettings ChosenItem => ValidIndex ? Props.settings.things[RandomIndex] : null;

        public bool HasRequirement => Props.HasRequirements && Props.requirements.HasAtLeastOneRequirementSetting;

        public bool HasHediffRequirement => Props.HasRequirements && Props.requirements.HasHediffRequirement;
        public bool HasThingRequirement => Props.HasRequirements && Props.requirements.HasThingRequirement;

        public bool HasCustomSpawn => HasThingRequirement && Props.requirements.thing.Any(t => t.HasCustomSpawn);
        public bool HasContainerSpawn => HasThingRequirement && Props.requirements.thing.Any(t => t.HasContainerSpawn);

        public bool HasChosenThing => ChosenItem != null && ChosenItem.thingToSpawn != null;
        public bool HasChosenPawn => ChosenItem != null && (ChosenItem.pawnKindToSpawn != null || IsParentPawnKindCopier);

        public bool IsParentPawnKindCopier => ChosenItem.IsCopier && ChosenItem.copyParent.pawnKind;
        public bool PrecedentIterationsExclusion => Props.excludeAlreadyPickedOptions;

        public bool HasColorCondition => Props.settings.things.Any(t => t.HasColorCondition);

        public ThingDef ThingOfChoice => HasChosenThing ? ChosenItem.thingToSpawn : null;

        public List<ThingSettings> FullOptionList => Props.settings.things;

        public Pawn_SkillTracker rememberSkillTracker = null;
        public int lastSkillUpdateTick = -1;

        public PawnKindDef PawnOfChoice
        {
            get
            {
                if (!HasChosenPawn)
                    return null;

                if (IsParentPawnKindCopier)
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
                if(HasChosenThing && ChosenItem.HasStackSettings)
                    return ChosenItem.specificSettings.stack.spawnCount.RandomInRange;
                else if (Props.settings.HasDefaultSettings)
                    return Props.settings.defaultSettings.stack.spawnCount.RandomInRange;

                return 1;
            }
        }

        public bool WeightedSpawn
        {
            get
            {
                 if (HasChosenThing && ChosenItem.HasStackSettings)
                    return ChosenItem.specificSettings.stack.weightedSpawnCount;
                else if (Props.settings.HasDefaultSettings)
                    return Props.settings.defaultSettings.stack.weightedSpawnCount;

                return false;
            }
        }

        public ThingDef FilthToSpawn
        {
            get
            {
                if (HasChosenThing && ChosenItem.HasFilthSettings)
                    return ChosenItem.specificSettings.filth.filthDef;
                else if (Props.settings.HasDefaultSettings)
                    return Props.settings.defaultSettings.filth.filthDef;

                return null;
            }
        }
        public FloatRange FilthRadius
        {
            get
            {
                if (HasChosenThing && ChosenItem.HasFilthSettings)
                    return ChosenItem.specificSettings.filth.filthRadius;
                else if (Props.settings.HasDefaultSettings)
                    return Props.settings.defaultSettings.filth.filthRadius;

                return new FloatRange(0,1);
            }
        }
        public IntRange FilthNum
        {
            get
            {
                if (HasChosenThing && ChosenItem.HasFilthSettings)
                    return ChosenItem.specificSettings.filth.filthNum;
                else if (Props.settings.HasDefaultSettings)
                    return Props.settings.defaultSettings.filth.filthNum;

                return new IntRange(0, 0);
            }
        }

        public override void CompPostMake()
        {
            if(MyDebug)Log.Warning(">>> " + Pawn?.Label + " - " + parent.def.defName + " - CompPostMake start");

           // CalculateValues();
        }

        //public override void Notify_PawnDied()
        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            string debugStr = MyDebug ? Pawn.LabelShort + " HediffComp_RandySpawnUponDeath Notify_PawnDied" : "";

            if (MyDebug) Log.Warning(debugStr + " Entering");

            bool failure = false;

            if (Pawn.Corpse.Negligible())
            {
                if (MyDebug) Log.Warning(debugStr + " Corpse is no more, cant find its position - giving up");
                failure = true;
            }

            if (blockSpawn)
            {
                if (MyDebug) Log.Warning(debugStr + " blockSpawn for some reason- giving up");
                failure = true;
            }


            if (!this.FulfilsRequirement(out Thing closestContainerThing))
            {
                if (MyDebug) Log.Warning(debugStr + "not Fulfiling requirements- giving up");
                failure = true;
            }

            if (failure)
            {
                base.Notify_PawnDied(dinfo, culprit); 
                return;
            }

            int RandomIteration = Props.iterationRange.RandomInRange;
            List<int> AlreadyPickedOptions = new List<int>();

            if (MyDebug) Log.Warning(debugStr + "iterationNum: "+ RandomIteration);

            for (int i = 0; i < RandomIteration; i++){

                if (MyDebug) Log.Warning(debugStr + " Trying to spawn " + i + "/" + (RandomIteration-1));

                if (!DiceThrow(AlreadyPickedOptions))
                {
                    if (MyDebug) Log.Warning(debugStr + " DiceThrow wrong results");
                    base.Notify_PawnDied(dinfo, culprit);
                    return;
                }
                else
                    if (MyDebug) Log.Warning( debugStr +" index: " + RandomIndex + " quantity: " + RandomQuantity +" nature: " + ChosenItem.ItemDump);

                if(PrecedentIterationsExclusion)
                    AlreadyPickedOptions.Add(RandomIndex);

                if (CheckShouldSpawn(closestContainerThing))
                {
                    if (MyDebug) Log.Warning(
                        debugStr +
                        " Spawn " + i + "/" + (RandomIteration - 1) + " occured " +
                        " nature: t:" + ChosenItem.ItemDump
                    );
                }

                if (MyDebug) Log.Warning("#################");
            }
            if (CheckShouldHandleCorpse())
            {
                if (MyDebug) Log.Warning(debugStr + " Corpse handled");
            }

            base.Notify_PawnDied(dinfo, culprit);
        }

        //
        public bool DiceThrow(List<int> AlreadyPickedOptions)
        {
            RandomIndex = this.GetWeightedRandomIndex(AlreadyPickedOptions);

            if (HasChosenPawn && ChosenItem.HasFactionParams)
                    this.ComputeRandomFaction();
            RandomQuantity = this.ComputeSpawnCount();

            if (!ValidIndex)
            {
                BlockAndDestroy(">ERROR< failed to find an index for IP, check and adjust your hediff props", MyDebug);
                return false;
            }

            if(!ValidQuantity)
            {
                if (MyDebug) Log.Warning("random quantity: " + RandomQuantity + " - impossible to spawn anything");
                return false;
            }

            return true;
        }

        public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
        {
            if (MyDebug && !ErrorLog.NullOrEmpty()) Log.Warning(ErrorLog);
            blockSpawn = true;
            Tools.DestroyParentHediff(parent, myDebug);
        }

        private bool CheckShouldSpawn(Thing closestThing)
        {
            if (MyDebug) Log.Warning(Pawn.LabelShort + " CheckShouldSpawn");

            if (MyDebug) Log.Warning(
                " Trying to spawn " + RandomQuantity + " " +
                ThingOfChoice + "/" +
                PawnOfChoice
            );

            Thing referenceThing = HasCustomSpawn ? closestThing : Pawn.Corpse;

            bool didSpawn = this.TryDoSpawn(referenceThing, RandomQuantity);
            if(MyDebug) Log.Warning("TryDoSpawn: " + didSpawn);

            return didSpawn;
        }

        private bool CheckShouldHandleCorpse()
        {
            Corpse corpse = Pawn.Corpse;

            bool didSomething = false;

            didSomething |= this.StripCorpse(corpse);
            didSomething |= this.DestroyCorpse(corpse);

            return didSomething;
        }
    }
}
