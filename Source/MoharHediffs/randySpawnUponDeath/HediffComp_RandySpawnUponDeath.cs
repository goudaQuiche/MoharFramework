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
            Tools.Warn(">>> " + Pawn?.Label + " - " + parent.def.defName + " - CompPostMake start", MyDebug);

            if(ModCompatibilityCheck.MoharCheckAndDisplay() == false)
                BlockAndDestroy();

           // CalculateValues();
        }

        public override void Notify_PawnDied()
        {
            string debugStr = MyDebug ? Pawn.LabelShort + " HediffComp_RandySpawnUponDeath Notify_PawnDied" : "";

            Tools.Warn(debugStr + " Entering", MyDebug);

            bool failure = false;

            if (Pawn.Corpse.Negligible())
            {
                Tools.Warn(debugStr + " Corpse is no more, cant find its position - giving up", MyDebug);
                failure = true;
            }

            if (blockSpawn)
            {
                Tools.Warn(debugStr + " blockSpawn for some reason- giving up", MyDebug);
                failure = true;
            }

            Thing closestContainerThing = null;

            if(!this.FulfilsRequirement(out closestContainerThing))
            {
                Tools.Warn(debugStr + "not Fulfiling requirements- giving up", MyDebug);
                failure = true;
            }

            if (failure)
            {
                base.Notify_PawnDied();
                return;
            }

            int RandomIteration = Props.iterationRange.RandomInRange;
            List<int> AlreadyPickedOptions = new List<int>();

            Tools.Warn(debugStr + "iterationNum: "+ RandomIteration, MyDebug);

            for (int i = 0; i < RandomIteration; i++){

                Tools.Warn(debugStr + " Trying to spawn " + i + "/" + (RandomIteration-1), MyDebug);

                if (!DiceThrow(AlreadyPickedOptions))
                {
                    Tools.Warn(debugStr + " DiceThrow wrong results", MyDebug);
                    base.Notify_PawnDied();
                    return;
                }
                else
                    Tools.Warn(
                        debugStr +
                        " index: " + RandomIndex + " quantity: " + RandomQuantity +
                        " nature: " + ChosenItem.ItemDump
                        , MyDebug
                    );

                if(PrecedentIterationsExclusion)
                    AlreadyPickedOptions.Add(RandomIndex);

                if (CheckShouldSpawn(closestContainerThing))
                {
                    Tools.Warn(
                        debugStr +
                        " Spawn " + i + "/" + (RandomIteration - 1) + " occured " +
                        " nature: t:" + ChosenItem.ItemDump
                        , MyDebug
                    );
                }

                Tools.Warn("#################", MyDebug);
            }
            if (CheckShouldHandleCorpse())
            {
                Tools.Warn(debugStr + " Corpse handled", MyDebug);
            }

            base.Notify_PawnDied();
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
                Tools.Warn("random quantity: " + RandomQuantity + " - impossible to spawn anything", MyDebug);
                return false;
            }

            return true;
        }

        public void BlockAndDestroy(string ErrorLog = "", bool myDebug = false)
        {
            Tools.Warn(ErrorLog, myDebug && !ErrorLog.NullOrEmpty());
            blockSpawn = true;
            Tools.DestroyParentHediff(parent, myDebug);
        }

        private bool CheckShouldSpawn(Thing closestThing)
        {
            Tools.Warn(Pawn.LabelShort + " CheckShouldSpawn", MyDebug);

            Tools.Warn(
                " Trying to spawn " + RandomQuantity + " " +
                ThingOfChoice + "/" +
                PawnOfChoice
                , MyDebug
            );

            Thing referenceThing = HasCustomSpawn ? closestThing : Pawn.Corpse;

            bool didSpawn = this.TryDoSpawn(referenceThing, RandomQuantity);
            Tools.Warn("TryDoSpawn: " + didSpawn, MyDebug);

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

        /*
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            //updateSkills();
        }
        */

            /*
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);

            Tools.Warn("Notify_PawnPostApplyDamage", MyDebug);

            //updateSkills();
        }
        */

            /*
        public void updateSkills()
        {
            if (rememberSkillTracker == null)
                rememberSkillTracker = new Pawn_SkillTracker(Pawn);

            if (Find.TickManager.TicksGame - lastSkillUpdateTick - 120 > 0)
            {
                rememberSkillTracker.skills = Pawn.skills.skills.ListFullCopy();
                lastSkillUpdateTick = Find.TickManager.TicksGame;
            }
        }
*/
    }
}
