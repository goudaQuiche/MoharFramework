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
        bool MyDebug => Props.debug;
        Pawn pawn = null;
        Building building = null;
        Thing thing = null;

        bool IsPawn => pawn != null;
        bool IsBuilding => building != null;
        bool IsThing => thing != null;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            /*
            if (!parent.IsPawn())
            {
                if(MyDebug) Log.Warning("this is a comp is meant for a pawn, this will fail");
            }
            */
            if (parent is Pawn nPawn)
            {
                pawn = nPawn;
            }
            else if (parent is Building nBuilding)
            {
                building = nBuilding;
            }
            else
            {
                thing = parent;
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

        private int GetMeatAmount()
        {
            if (IsPawn && pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount) <= 0)
            {
                if(MyDebug) Log.Warning("pawn but no stat meatamount (is <= 0)");
                return 0;
            }

            //if( IsBuilding && building.MaxHitPoints <= 0)
            if (Props.relativeMeatAmount)
            {
                if ((IsBuilding || IsThing) && !parent.def.useHitPoints)
                {
                    if (MyDebug) Log.Warning("building/thing but relativeMeatAmount && no useHitpoints");
                    return 0;
                }

                //meatAmount = GenMath.RoundRandom(pawn.GetStatValue(StatDefOf.MeatAmount));
                if (IsPawn)
                    return (int)pawn.GetStatValue(StatDefOf.MeatAmount);
                else if (IsBuilding || IsThing)
                    return (int)((float)parent.HitPoints / parent.MaxHitPoints);
            }

            return Props.fixedMeatAmount;
        }

        private bool SpawnMeat()
        {
            if (!Props.spawnMeat)
                return false;

            if (!IsPawn && Props.meatToSpawn == null)
            {
                if (MyDebug) Log.Warning("parent is not pawn but tried to spawn meat while there is no Props.meatToSpawn, use spawnThing instead, maybe");
                return false;
            }

            if (Props.meatToSpawn == null && pawn.RaceProps.meatDef == null)
            {
                if (MyDebug) Log.Warning("Found no meat def for pawn or stat of meatamount is <= 0");
                return false;
            }

            int meatAmount = GetMeatAmount();

            if (meatAmount <= 0)
            {
                if (MyDebug) Log.Warning("Calculated meat amount is <= 0, nothing to do");
                return false;
            }

            ThingDef thingToSpawn = null;
            if (IsPawn)
                thingToSpawn = (Props.meatToSpawn == null) ? pawn.RaceProps.meatDef : Props.meatToSpawn;
            else if (IsBuilding || IsThing)
                thingToSpawn = Props.meatToSpawn;

            if (thingToSpawn == null)
            {
                if (MyDebug) Log.Warning("ThingToSpawn is null, cant work");
                return false;
            }

            int pileNum = Props.meatPilesIntRange.RandomInRange;
            if (MyDebug) Log.Warning("Trying to spawn " + pileNum + " piles of " + thingToSpawn + "(x" + meatAmount + ")");
            int loopBreaker = 20;
            for (int i = 0; i < pileNum; i++)
            {
                if (MyDebug) Log.Warning(i + "/" + pileNum + " meat left: " + meatAmount);
                if (loopBreaker-- < 0)
                {
                    if (MyDebug) Log.Warning("Had to break the meat loop, bc it would not end");
                    return false;
                }
                    
                int currentPileAmount = Rand.Range(1, meatAmount);
                meatAmount -= currentPileAmount;

                if(MyDebug) Log.Warning(i + "/" + pileNum + " spawning: " + currentPileAmount);
                bool Didit = false;

                Didit = Lifespan_Utility.TryDoSpawn(parent, thingToSpawn, currentPileAmount, Props.spawnMaxAdjacent, Props.tryToUnstack, Props.inheritFaction, Props.spawnForbidden, Props.showMessageIfOwned);

                if (!Didit)
                {
                    if (MyDebug) Log.Warning("Had to break the meat loop, bc it cant spawn things");
                    return false;
                }

                if (meatAmount <= 0)
                {
                    if (MyDebug) Log.Warning("Had to break the meat loop, bc it did its job");
                    break;
                }
                    
            }

            return true;
        }
        private int GeThingAmount()
        {
            int tmpValue = 0;
            if (Props.thingNumIfFullBody >= 0)
                tmpValue = Props.thingNumIfFullBody;

            if (Props.relativeThingAmount)
            {
                if (IsPawn && pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount) <= 0)
                {
                    if (MyDebug) Log.Warning("parent is pawn but no meat amount in def while relativeThingAmount is set, cant spawn things");
                    return 0;
                }
                if ((IsBuilding || IsThing) && !parent.def.useHitPoints)
                {
                    if (MyDebug) Log.Warning("parent is building but no HP set in def while relativeThingAmount is set, cant spawn things");
                    return 0;
                }

                if (IsPawn)
                    return tmpValue * GenMath.RoundRandom(pawn.GetStatValue(StatDefOf.MeatAmount)) / Mathf.RoundToInt(pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount));
                else if (IsBuilding || IsThing)
                    return (int)(tmpValue * (float)parent.HitPoints / parent.MaxHitPoints);
            }
            return 0;
        }
        private bool SpawnThing()
        {
            if (!Props.spawnThing)
                return false;

            int thingAmount = GeThingAmount();

            if (thingAmount <= 0)
            {
                if (MyDebug) Log.Warning("calculated thingAmount was <= 0, cant spawn things");
                return false;
            }


            int pileNum = Props.thingPilesIntRange.RandomInRange;
            if (MyDebug) Log.Warning("Trying to spawn " + pileNum + " piles of " + Props.thingToSpawn + "(x" + thingAmount + ")");
            int loopBreaker = 20;
            for (int i = 0; i < pileNum; i++)
            {
                if(MyDebug) Log.Warning(i + "/" + pileNum + " meat left: " + thingAmount);
                if (loopBreaker-- < 0)
                {
                    if(MyDebug) Log.Warning("Had to break the thing loop, bc it would not end");
                    return false;
                }
                int currentPileAmount = Rand.Range(1, thingAmount);
                thingAmount -= currentPileAmount;
                bool DidIt = false;
                DidIt = Lifespan_Utility.TryDoSpawn(parent, Props.thingToSpawn, currentPileAmount, Props.spawnMaxAdjacent, Props.tryToUnstack, Props.inheritFaction, Props.spawnForbidden, Props.showMessageIfOwned);

                if (!DidIt)
                {
                    if(MyDebug) Log.Warning("Had to break the thing loop, bc it cant spawn things");
                    return false;
                }

                if (thingAmount <= 0)
                {
                    if(MyDebug) Log.Warning("Had to break the thing loop, bc it did its job");
                    break;
                }
                    
            }

            return true;
        }

        private bool SpawnFilth()
        {
            if (Props.filthDef == null)
            {
                if(MyDebug) Log.Warning("no Props.filthDef, cant SpawnFilth");
                return false;
            }

            Thing refThing = Lifespan_Utility.ThingInCaseOfDeath(parent);
            if (refThing == null)
            {
                if(MyDebug) Log.Warning("no refThing in caase of death, cant SpawnFilth");
                return false;
            }

            int filthNum = Props.filthIntRange.RandomInRange;
            if(filthNum <= 0)
            {
                if(MyDebug) Log.Warning("filthNum <= 0, cant SpawnFilth");
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
                if(MyDebug) Log.Warning("no Props.spawnMote, wont SpawnMote");
                return false;
            }

            Thing refThing = Lifespan_Utility.ThingInCaseOfDeath(parent);
            if (refThing == null)
            {
                if(MyDebug) Log.Warning("no refThing in case of death, cant SpawnMote");
                return false;
            }

            int moteNum = Props.moteNumRange.RandomInRange;
            if (moteNum <= 0)
            {
                if(MyDebug) Log.Warning("moteNum <= 0, cant SpawnMote");
                return false;
            }

            for (int i = 0; i < moteNum; i++)
            {
                Vector3 origin = refThing.DrawPos;
                origin.x += Rand.Range(0, Props.moteRadius) * (Rand.Chance(.5f) ? -1 : 1);
                origin.y += Rand.Range(0, Props.moteRadius) * (Rand.Chance(.5f) ? -1 : 1);
                if (Props.moteDef == null)
                {
                    if(MyDebug) Log.Warning("no Props.moteDef, will spawn smoke");
                    FleckMaker.ThrowSmoke(origin, refThing.Map, Props.moteScale.RandomInRange);
                    //Lifespan_Utility.ThrowCustomSmoke(ThingDefOf.Mote_Smoke, origin, refThing.Map, Props.moteScale.RandomInRange);
                }
                    
                else
                    Lifespan_Utility.ThrowCustomSmoke(Props.moteDef, origin, refThing.Map, Props.moteScale.RandomInRange);
            }

            return true;
        }

        private bool ThingSaysGoodBye()
        {
            if (!IsBuilding && !IsThing)
            {
                if(MyDebug) Log.Warning("Tried to say good bye as building or thing, wont destroy");
                return false;
            }

            parent.Destroy();
            return true;
        }

        private bool PawnSaysGoodBye()
        {
            if (!IsPawn)
            {
                if(MyDebug) Log.Warning("Non pawn tried to say good bye as pawn, wont destroy");
                return false;
            }
                
            Lifespan_Utility.RemoveBadMemoriesOfDeadPawn(pawn);

            if (pawn.Dead)
            {
                if (pawn.Corpse == null)
                {
                    if(MyDebug) Log.Warning("found no corpse to work with, wont do anything");
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
            didMeat = didThing = didFilth = didMote = didDestroy = false;

            didMeat = SpawnMeat();
            didThing = SpawnThing();
            didFilth = SpawnFilth();
            didMote = SpawnMote();
            //SpawnCorpse();
            if(MyDebug) Log.Warning(parent.Label + " will try to autodestroy");

            if (IsPawn)
                didDestroy = PawnSaysGoodBye();
            else if (IsBuilding || IsThing)
                didDestroy = ThingSaysGoodBye();

            if(MyDebug) Log.Warning("didMeat="+didMeat+"; didThing="+didThing+"; didFilth="+didFilth+"; didMote="+didMote+";didDestroy="+didDestroy);
            
        }
        public void MyTick(int tickNum)
        {
            if(parent != null && IsPawn && pawn.Dead)
            {
                if(MyDebug) Log.Warning("found dead pawn, starting DestroyProcedure()");
                DestroyProcedure();
                return;
            }

            if (IsPawn && pawn.Negligeable())
            {
                if(MyDebug) Log.Warning("found negligeable pawn, wont tick");
                return;
            }else if ( (IsBuilding && building.Negligeable()) || (IsThing && thing.Negligeable()) )
            {
                if(MyDebug) Log.Warning("found negligeable thing/building, wont tick");
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