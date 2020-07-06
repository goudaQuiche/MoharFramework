using System;
using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace CustomLifeSpan
{
	public class CompLifespan : ThingComp
	{
		public int age = -1;

        public CompProperties_Lifespan Props => (CompProperties_Lifespan)props;
        bool myDebug => Props.debug;
        Pawn pawn;
        Building building;

        bool IsPawn;
        bool IsBuilding;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            /*
            if (!parent.IsPawn())
            {
                Tools.Warn("this is a comp is meant for a pawn, this will fail", myDebug);
            }
            */
            if (parent is Pawn nPawn)
            {
                pawn = nPawn;
                IsPawn = true;
                IsBuilding = false;
            }
            else if (parent is Building nBuilding)
            {
                building = nBuilding;
                IsPawn = false;
                IsBuilding = true;
            }
        }

        public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref age, "age", 0);
		}

        private bool ShouldBeDestroyed
        {
            get
            {
                return age >= Props.lifeSpanTicks;
            }
        }

        private bool SpawnMeat()
        {
            if (!Props.spawnMeat)
                return false;

            if (!IsPawn && Props.meatToSpawn == null)
            {
                Tools.Warn("parent is not pawn but tried to spawn meat while there is no Props.meatToSpawn, use spawnThing instead, maybe", myDebug);
                return false;
            }

            if (Props.meatToSpawn == null && pawn.RaceProps.meatDef == null)
            {
                Tools.Warn("Found no meat def for pawn or stat of meatamount is <= 0", myDebug);
                return false;
            }
                
            if ( IsPawn && pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount) <= 0)
            {
                Tools.Warn("pawn but no stat meatamount (is <= 0)", myDebug);
                return false;
            }

             if( IsBuilding && building.MaxHitPoints <= 0)
            {
                Tools.Warn("building but no stat maxhitpoints (is <= 0)", myDebug);
                return false;
            }

            int meatAmount = 0;
            if (Props.relativeMeatAmount)
            {
                //meatAmount = GenMath.RoundRandom(pawn.GetStatValue(StatDefOf.MeatAmount));
                if(IsPawn)
                    meatAmount = (int)pawn.GetStatValue(StatDefOf.MeatAmount);
                else if(IsBuilding)
                    meatAmount = (int)((float)building.HitPoints/ building.MaxHitPoints);
            }
            else
            {
                meatAmount = Props.fixedMeatAmount;
            }

            if (meatAmount <= 0)
            {
                Tools.Warn("Calculated meat amount is <= 0, nothing to do", myDebug);
                return false;
            }

            ThingDef thingToSpawn = null;
            if (IsPawn)
                thingToSpawn = (Props.meatToSpawn == null) ? pawn.RaceProps.meatDef : Props.meatToSpawn;
            else if (IsBuilding)
                thingToSpawn = Props.meatToSpawn;

            if (thingToSpawn == null)
            {
                Tools.Warn("ThingToSpawn is null, cant work", myDebug);
                return false;
            }

            int pileNum = Props.meatPilesIntRange.RandomInRange;
            Tools.Warn("Trying to spawn " + pileNum + " piles of " + pawn.RaceProps.meatDef + "(x" + meatAmount + ")", myDebug);
            int loopBreaker = 20;
            for (int i = 0; i < pileNum; i++)
            {
                Tools.Warn(i + "/" + pileNum + " meat left: "+meatAmount, myDebug);
                if (loopBreaker-- < 0)
                {
                    Tools.Warn("Had to break the meat loop, bc it would not end", myDebug);
                    return false;
                }
                    
                int currentPileAmount = Rand.Range(1, meatAmount);
                meatAmount -= currentPileAmount;

                Tools.Warn(i + "/" + pileNum + " spawning: " + currentPileAmount, myDebug);
                bool Didit = false;

                Didit = Lifespan_Utility.TryDoSpawn(parent, thingToSpawn, currentPileAmount, Props.spawnMaxAdjacent, Props.tryToUnstack, Props.inheritFaction, Props.spawnForbidden, Props.showMessageIfOwned);

                if (!Didit)
                {
                    Tools.Warn("Had to break the meat loop, bc it cant spawn things", myDebug);
                    return false;
                }

                if (meatAmount <= 0)
                {
                    Tools.Warn("Had to break the meat loop, bc it did its job", myDebug);
                    break;
                }
                    
            }

            return true;
        }

        private bool SpawnThing()
        {
            if (!Props.spawnThing)
                return false;

            int thingAmount = Props.thingNumIfFullBody;

            if (Props.relativeThingAmount)
            {
                if (IsPawn && pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount) <= 0)
                {
                    Tools.Warn("parent is pawn but no meat amount in def while relativeThingAmount is set, cant spawn things", myDebug);
                    return false;
                }
                if (IsBuilding && building.MaxHitPoints <= 0)
                {
                    Tools.Warn("parent is building but no HP set in def while relativeThingAmount is set, cant spawn things", myDebug);
                    return false;
                }

                if (IsPawn)
                    thingAmount *=
                    GenMath.RoundRandom(pawn.GetStatValue(StatDefOf.MeatAmount)) / Mathf.RoundToInt(pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount));
                else if (IsBuilding)
                    thingAmount = (int)(thingAmount * (float)building.HitPoints / building.MaxHitPoints);
            }


            if (thingAmount <= 0)
            {
                Tools.Warn("calculated thingAmount was <= 0, cant spawn things", myDebug);
                return false;
            }


            int pileNum = Props.thingPilesIntRange.RandomInRange;
            Tools.Warn("Trying to spawn " + pileNum + " piles of " + Props.thingToSpawn + "(x"+thingAmount+")", myDebug);
            int loopBreaker = 20;
            for (int i = 0; i < pileNum; i++)
            {
                Tools.Warn(i + "/" + pileNum + " meat left: " + thingAmount, myDebug);
                if (loopBreaker-- < 0)
                {
                    Tools.Warn("Had to break the thing loop, bc it would not end", myDebug);
                    return false;
                }
                int currentPileAmount = Rand.Range(1, thingAmount);
                thingAmount -= currentPileAmount;
                bool DidIt = false;
                DidIt = Lifespan_Utility.TryDoSpawn(parent, Props.thingToSpawn, currentPileAmount, Props.spawnMaxAdjacent, Props.tryToUnstack, Props.inheritFaction, Props.spawnForbidden, Props.showMessageIfOwned);

                if (!DidIt)
                {
                    Tools.Warn("Had to break the thing loop, bc it cant spawn things", myDebug);
                    return false;
                }

                if (thingAmount <= 0)
                {
                    Tools.Warn("Had to break the thing loop, bc it did its job", myDebug);
                    break;
                }
                    
            }

            return true;
        }

        private bool SpawnFilth()
        {
            if (Props.filthDef == null)
            {
                Tools.Warn("no Props.filthDef, cant SpawnFilth", myDebug);
                return false;
            }

            Thing refThing = Lifespan_Utility.ThingInCaseOfDeath(parent);
            if (refThing == null)
            {
                Tools.Warn("no refThing in caase of death, cant SpawnFilth", myDebug);
                return false;
            }

            int filthNum = Props.filthIntRange.RandomInRange;
            if(filthNum <= 0)
            {
                Tools.Warn("filthNum <= 0, cant SpawnFilth", myDebug);
                return false;
            }
            for (int i = 0; i < filthNum; i++)
            {
                Lifespan_Utility.TrySpawnFilth(refThing, Props.filthRadius, Props.filthDef);
            }
            return true;
        }

        

        public bool SpawnMote()
        {
            if (!Props.spawnMote)
            {
                Tools.Warn("no Props.spawnMote, wont SpawnMote", myDebug);
                return false;
            }

            Thing refThing = Lifespan_Utility.ThingInCaseOfDeath(parent);
            if (refThing == null)
            {
                Tools.Warn("no refThing in caase of death, cant SpawnMote", myDebug);
                return false;
            }

            int moteNum = Props.moteNumRange.RandomInRange;
            if (moteNum <= 0)
            {
                Tools.Warn("moteNum <= 0, cant SpawnMote", myDebug);
                return false;
            }

            for (int i = 0; i < moteNum; i++)
            {
                Vector3 origin = refThing.DrawPos;
                origin.x += Rand.Range(0, Props.moteRadius) * (Rand.Chance(.5f) ? -1 : 1);
                origin.y += Rand.Range(0, Props.moteRadius) * (Rand.Chance(.5f) ? -1 : 1);
                if (Props.moteDef == null)
                {
                    Tools.Warn("no Props.moteDef, will spawn smoke", myDebug);
                    Lifespan_Utility.ThrowCustomSmoke(ThingDefOf.Mote_Smoke, origin, refThing.Map, Props.moteScale.RandomInRange);
                }
                    
                else
                    Lifespan_Utility.ThrowCustomSmoke(Props.moteDef, origin, refThing.Map, Props.moteScale.RandomInRange);
            }

            return true;
        }

        private bool BuildingSaysGoodBye()
        {
            if (!IsBuilding)
            {
                Tools.Warn("Non building tried to say good bye as building, wont destroy", myDebug);
                return false;
            }

            building.Destroy();
            return true;
        }

        private bool PawnSaysGoodBye()
        {
            if (!IsPawn)
            {
                Tools.Warn("Non pawn tried to say good bye as pawn, wont destroy", myDebug);
                return false;
            }
                
            Lifespan_Utility.RemoveBadMemoriesOfDeadPawn(pawn, myDebug);

            if (pawn.Dead)
            {
                if (pawn.Corpse == null)
                {
                    Tools.Warn("found no corpse to work with, wont do anything", myDebug);
                    return false;
                }
                    

                Corpse corpse = pawn.Corpse;

                if (corpse.AnythingToStrip())
                    corpse.Strip();

                corpse.DeSpawn();
            }
            else
                parent.Destroy();

            return true;
        }

        private void DestroyProcedure()
        {
            bool didMeat, didThing, didFilth, didMote, didDestroy;
            didMeat= didThing= didFilth= didMote = didDestroy=false;

            didMeat = SpawnMeat();
            didThing = SpawnThing();
            didFilth = SpawnFilth();
            didMote = SpawnMote();
            //SpawnCorpse();
            Tools.Warn(parent.Label + " will try to autodestroy", myDebug);

            if (IsPawn)
                didDestroy=PawnSaysGoodBye();
            else if (IsBuilding)
                didDestroy=BuildingSaysGoodBye();

            Tools.Warn("didMeat="+didMeat+"; didThing="+didThing+"; didFilth="+didFilth+"; didMote="+didMote+";didDestroy="+didDestroy, myDebug);
            
        }
        public void MyTick(int tickNum)
        {
            if(parent != null && IsPawn && pawn.Dead)
            {
                Tools.Warn("found dead pawn, starting DestroyProcedure()", myDebug);
                DestroyProcedure();
                return;
            }

            if (IsPawn && pawn.Negligeable())
            {
                Tools.Warn("found negligeable pawn, wont tick", myDebug);
                return;
            }else if (IsBuilding && building.Negligeable())
            {
                Tools.Warn("found negligeable building, wont tick", myDebug);
                return;
            }
                
            age += tickNum;
            if (ShouldBeDestroyed)
                DestroyProcedure();
        }

        public void ForceDeath()
        {
            age = Props.lifeSpanTicks;
        }

        public override void CompTick()
		{
            MyTick(1);
        }

		public override void CompTickRare()
		{
            MyTick(250);
        }

        public override string CompInspectStringExtra()
        {
            string text = base.CompInspectStringExtra();
            string result = "";
            int num = Props.lifeSpanTicks - age;
            if (num > 0)
            {
                result = "LifespanCompInspectStringExtra".Translate() + " " + num.ToStringTicksToPeriod();
                if (Prefs.DevMode)
                {
                    result += " - " + age + "/" + Props.lifeSpanTicks;
                }
                if (!text.NullOrEmpty())
                {
                    result = "\n" + text;
                }
            }
            return result;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            //foreach (Gizmo item in base.CompGetGizmosExtra())yield return item;
            
            if (Prefs.DevMode)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "DEBUG: force end of life";
                command_Action.action = delegate
                {
                    ForceDeath();
                };
                yield return command_Action;
            }
        }
    }
}