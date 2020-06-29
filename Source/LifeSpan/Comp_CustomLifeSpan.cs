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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!parent.IsPawn())
            {
                Tools.Warn("this is a comp is meant for a pawn, this will fail", myDebug);
            }
            pawn = (Pawn)parent;
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
        

        private void SpawnMeat()
        {
            if (!Props.spawnMeat)
                return;

            if (pawn.RaceProps.meatDef == null || pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount) <= 0)
                return;

            int meatAmount;
            if (Props.relativeMeatAmount)
            {
                //meatAmount = GenMath.RoundRandom(pawn.GetStatValue(StatDefOf.MeatAmount));
                meatAmount = (int)pawn.GetStatValue(StatDefOf.MeatAmount);
            }
            else
            {
                meatAmount = Props.fixedMeatAmount;
            }

            if (meatAmount <= 0)
                return;
            
            int pileNum = Props.meatPilesIntRange.RandomInRange;

            for(int i = 0; i < pileNum; i++)
            {
                //Tools.Warn(i + "/" + pileNum + " meat left: "+meatAmount, myDebug);

                int currentPileAmount = Rand.Range(1, meatAmount);
                meatAmount -= currentPileAmount;
                if (currentPileAmount <= 0)
                    break;

                //Tools.Warn(i + "/" + pileNum + " spawning: " + currentPileAmount, myDebug);
                Lifespan_Utility.TryDoSpawn(pawn, pawn.RaceProps.meatDef, currentPileAmount, Props.spawnMaxAdjacent, Props.tryToUnstack, Props.inheritFaction, Props.spawnForbidden, Props.showMessageIfOwned);
            }
        }

        private void SpawnThing()
        {
            if (!Props.spawnThing)
                return;

            if (pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount) <= 0)
                return;

            int thingAmount = Props.thingNumIfFullBody;
            if(Props.relativeThingAmount)
                thingAmount *= 
                    GenMath.RoundRandom(pawn.GetStatValue(StatDefOf.MeatAmount)) / Mathf.RoundToInt( pawn.def.GetStatValueAbstract(StatDefOf.MeatAmount));

            if (thingAmount <= 0)
                return;

            int pileNum = Props.thingPilesIntRange.RandomInRange;

            for (int i = 0; i < pileNum; i++)
            {
                int currentPileAmount = Rand.Range(1, thingAmount);
                thingAmount -= currentPileAmount;
                if (currentPileAmount <= 0)
                    break;
                Lifespan_Utility.TryDoSpawn(pawn, Props.thingToSpawn, currentPileAmount, Props.spawnMaxAdjacent, Props.tryToUnstack, Props.inheritFaction, Props.spawnForbidden, Props.showMessageIfOwned);
            }
        }

        private void SpawnFilth()
        {
            if (!Props.spawnThing)
                return;

            Thing refThing = Lifespan_Utility.ThingInCaseOfDeath(pawn);
            if (refThing == null)
                return;

            int filthNum = Props.filthIntRange.RandomInRange;

            for (int i = 0; i < filthNum; i++)
            {
                Lifespan_Utility.TrySpawnFilth(refThing, Props.filthRadius, Props.filthDef);
            }
        }

        

        public void SpawnMote()
        {
            if (!Props.spawnMote)
                return;

            Thing refThing = Lifespan_Utility.ThingInCaseOfDeath(pawn);
            if (refThing == null)
                return;

            int moteNum = Props.moteNumRange.RandomInRange;
            for (int i = 0; i < moteNum; i++)
            {
                Vector3 origin = refThing.DrawPos;
                origin.x += Rand.Range(0, Props.moteRadius) * (Rand.Chance(.5f) ? -1 : 1);
                origin.y += Rand.Range(0, Props.moteRadius) * (Rand.Chance(.5f) ? -1 : 1);
                if (Props.moteDef == null)
                    Lifespan_Utility.ThrowCustomSmoke(ThingDefOf.Mote_Smoke, origin, refThing.Map, Props.moteScale.RandomInRange);
                else
                    Lifespan_Utility.ThrowCustomSmoke(Props.moteDef, origin, refThing.Map, Props.moteScale.RandomInRange);
            }
        }


        private void SayGoodBye()
        {
            //Lifespan_Utility.removingRelationAndThoughts(pawn, myDebug);
            Lifespan_Utility.RemoveBadMemoriesOfDeadPawn(pawn, myDebug);

            if (pawn.Dead)
            {
                if (pawn.Corpse == null)
                    return;

                Corpse corpse = pawn.Corpse;

                if (corpse.AnythingToStrip())
                    corpse.Strip();

                corpse.DeSpawn();
            }
            else
                parent.Destroy();
        }

        private void DestroyProcedure()
        {
            SpawnMeat();
            SpawnThing();
            SpawnFilth();
            SpawnMote();
            //SpawnCorpse();

            SayGoodBye();
            
        }
        public void MyTick(int tickNum)
        {
            if(pawn != null && pawn.Dead)
            {
                Tools.Warn("found dead pawn", myDebug);
                DestroyProcedure();
                return;
            }
                

            if (pawn.Negligeable())
            {
                Tools.Warn("found negligeable pawn", myDebug);
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
                result = "LifespanExpiry".Translate() + " " + num.ToStringTicksToPeriod();
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