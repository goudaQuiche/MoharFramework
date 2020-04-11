/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
	public class HeDiffComp_LTF_Spawner : HediffComp
	{
		int spawnTicker=0;
				
		int hungerReset=0;
		int healthReset=0;
        int graceTicks = 0;

		Pawn pawn = null;

        Random rand = new Random();

        public HeDiffCompProperties_LTF_Spawner Props
		{
			get
			{
				return (HeDiffCompProperties_LTF_Spawner)this.props;
			}
		}


        public override void CompExposeData()
        {
            Scribe_Values.Look(ref spawnTicker, "LTF_spawnTicker");
            Scribe_Values.Look(ref hungerReset, "LTF_hungerReset");
            Scribe_Values.Look(ref healthReset, "LTF_healthReset");
            Scribe_Values.Look(ref graceTicks, "LTF_graceTicks");
        }

        public override void CompPostTick(ref float severityAdjustment)
		{
			
			pawn = parent.pawn;
			if (pawn != null)
			{
				if (pawn.Map == null )
				{
					return;
				}

                if (this.graceTicks <= 0)
                {
                    //if (pawn.IsHashIntervalTick(60))
                    if (IsHungry())
                    {
                        hungerReset++;
                        graceTicks = (int)(randomDays2wait() * 60000);
                        return;
                    }
                    else if (IsInjured())
                    {
                        healthReset++;
                        graceTicks = (int)(randomDays2wait() * 60000);
                        return;
                    }
                    else
                    {
                        //Log.Warning( pawn.Label + "OK beat goes on");
                        //pawn.drafter.Drafted
                        if ( CheckShouldSpawn())
                        {
                            ResetCountdown();

                            if ( (Props.mandatoryGrace) || ((Props.randomGrace) &&(rand.NextDouble() > .5)) )
                            {
                                graceTicks = (int)(randomGraceDays() * 60000);
                            }
                       
                        }
                    }
                }
                else
                {
                    this.graceTicks--;
                }

            }
		}


		private bool CheckShouldSpawn()
		{
			pawn = parent.pawn;
			if (pawn != null)
			{
                spawnTicker++;
                if (spawnTicker >= (Props.tickerLimit * 60000))
				{
					TryDoSpawn();
                    return true;
				}
                return false;
			}

			Log.Warning( "CheckShouldSpawn pawn Null");
            return false;
		}

		public bool TryDoSpawn()
		{
			pawn = this.parent.pawn;
			if (pawn != null)
			{
                if (this.Props.animalThing) {
                    if ( this.Props.animalName.Equals( "Megascarab" ))
                    {
                        //PawnKindDef.
                        Pawn insect = PawnGenerator.GeneratePawn(PawnKindDefOf.Megascarab, null);
                        GenSpawn.Spawn(insect, CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 4, null), pawn.Map);
                        return true;
                    }
                    else
                    {
                        
                        //PawnGenerator.GeneratePawn()
                        Pawn insect = PawnGenerator.GeneratePawn(PawnKindDefOf.Thrumbo, null);
                        GenSpawn.Spawn(insect, CellFinder.RandomClosewalkCellNear(pawn.Position, pawn.Map, 4, null), pawn.Map);
                        return true;
                    }
                    
                } else
                {
                    if (this.Props.spawnMaxAdjacent >= 0)
                    {
                        int num = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            List<Thing> thingList = (pawn.Position + GenAdj.AdjacentCellsAndInside[i]).GetThingList(pawn.Map);
                            for (int j = 0; j < thingList.Count; j++)
                            {
                                if (thingList[j].def == this.Props.thingToSpawn)
                                {
                                    num += thingList[j].stackCount;
                                    if (num >= this.Props.spawnMaxAdjacent)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    IntVec3 center;
                    if (this.TryFindSpawnCell(out center))
                    {
                        Thing thing = ThingMaker.MakeThing(this.Props.thingToSpawn, null);
                        thing.stackCount = this.Props.spawnCount;
                        Thing t;
                        GenPlace.TryPlaceThing(thing, center, pawn.Map, ThingPlaceMode.Direct, out t, null);
                        if (this.Props.spawnForbidden)
                        {
                            t.SetForbidden(true, true);
                        }
                        return true;
                    }
                    return false;
                }
				
			}else{
				//Log.Warning( "TryDoSpawn Null");
				return false;
			}
		}

		private bool TryFindSpawnCell(out IntVec3 result)
		{
			pawn = this.parent.pawn;
			if (pawn != null)
			{
				foreach (IntVec3 current in GenAdj.CellsAdjacent8Way(pawn).InRandomOrder(null))
				{
					if (current.Walkable(pawn.Map))
					{
						Building edifice = current.GetEdifice(pawn.Map);
						if (edifice == null || !this.Props.thingToSpawn.IsEdifice())
						{
							Building_Door building_Door = edifice as Building_Door;
							if (building_Door == null || building_Door.FreePassage)
							{
								if (GenSight.LineOfSight(pawn.Position, current,pawn.Map, false, null, 0, 0))
								{
									bool flag = false;
									List<Thing> thingList = current.GetThingList(pawn.Map);
									for (int i = 0; i < thingList.Count; i++)
									{
										Thing thing = thingList[i];
										if (thing.def.category == ThingCategory.Item && (thing.def != this.Props.thingToSpawn || thing.stackCount > this.Props.thingToSpawn.stackLimit - this.Props.spawnCount))
										{
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										result = current;
										return true;
									}
								}
							}
						}
					}
				}
				result = IntVec3.Invalid;
				return false;
			}else{
				Log.Warning( "TryFindSpawnCell Null");
				result = IntVec3.Invalid;
				return false;
			}
		}

		private void ResetCountdown()
		{
            spawnTicker = 0;
            Props.tickerLimit = randomDays2wait();
        }

        private float randomDays2wait()
        {
            return ((this.Props.minDaysB4Next + (this.Props.maxDaysB4Next - this.Props.minDaysB4Next) * (float)rand.NextDouble()) );
        }
        private float randomGraceDays()
        {
            return (this.Props.graceDays * (float)rand.NextDouble());
        }

        private bool IsWounded()
		{
			pawn = this.parent.pawn;
			if (pawn != null)
			{
				float num = 0f;
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
                    //if (hediffs[i] is Hediff_Injury && !hediffs[i].IsOld())
                    if (hediffs[i] is Hediff_Injury && !hediffs[i].IsPermanent())
                    {
						num += hediffs[i].Severity;
					}
				}
				//Log.Warning( pawn.Label + " is wounded ");
				return (num>0);
			}
			return false;
		}
		
		private bool IsHungry()
		{
			string bla = string.Empty;
			pawn = parent.pawn;
			
			if (pawn != null)
			{
				if (Props.hungerRelative == true){
					if (pawn.needs.food != null && pawn.needs.food.CurCategory == HungerCategory.Starving)
					{
						return true;
					}else{
						return false;
					}
				}
				else{
					return false;
				}
				
			}else{
				Log.Warning (" IsHungry Null Error ");
				return false;
			}
			
		}		
		
		private bool IsInjured()
		{
			pawn = parent.pawn;
			if (pawn != null)
			{
				if (Props.healthRelative == true){
					if ( IsWounded() )
					{
						return true;
					}
					else{
						
					}
					return false;
				}
				else
				{
					return false;
				}
			}else{
				Log.Warning (" IsInjured Null Error ");
				return false;	
			}
			
		}	
		

		public override string CompTipStringExtra
		{
			get
			{
				string result = string.Empty;
                int ticksLeft = (int)(Props.tickerLimit * 60000) - spawnTicker;

                if (this.graceTicks > 0)
                {
                    if (this.Props.animalThing)
                    {
                        result = " No " + Props.animalName + " for " + (graceTicks).ToStringTicksToPeriod();
                    }
                    else
                    {
                        result = " No " + Props.thingToSpawn.label + " for " + (graceTicks).ToStringTicksToPeriod();
                    }

                    if (hungerReset > 0)
                    {
                        result += "(hunger)";
                    }
                    else if (healthReset > 0)
                    {
                        result += "(injury)";
                    }
                    else
                    {
                        result += "(why not)";
                    }
                }
                else{

                    result = ticksLeft.ToStringTicksToPeriod() + " before ";
                    if (this.Props.animalThing)
                    {
                        result += Props.animalName;
                    }
                    else
                    {
                        result += Props.thingToSpawn.label;
                    }
                    result += " " + Props.spawnVerb + "(" + Props.spawnCount + "x)";

                }
				

				return result;
			}
		}

	}
}
