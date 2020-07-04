using System.Collections.Generic;
using Verse;
using RimWorld;

namespace FuPoSpa
{
    public class CompFuelPoweredSpawner : ThingComp
    {
        private int ticksUntilSpawn;

        public CompProperties_FuelPoweredSpawner Props => (CompProperties_FuelPoweredSpawner)props;


        private bool FuelOk
        {
            get
            {
                if (Props.requiresFuel)
                {
                    CompRefuelable comp = parent.TryGetComp<CompRefuelable>();
                    if (comp == null)
                        return false;
                    else
                        return comp.HasFuel;
                }
                return true;
            }
        }
        private bool PowerOn
        {
            get
            {
                if (Props.requiresPower)
                {
                    return parent.GetComp<CompPowerTrader>()?.PowerOn ?? false;
                }
                else
                    return true;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad)
            {
                ResetCountdown();
            }
        }

        public override void CompTick()
        {
            TickInterval(1);
        }

        public override void CompTickRare()
        {
            TickInterval(250);
        }

        private void TickInterval(int interval)
        {
            if (!parent.Spawned)
            {
                return;
            }
            CompCanBeDormant comp = parent.GetComp<CompCanBeDormant>();
            if (comp != null)
            {
                if (!comp.Awake)
                {
                    return;
                }
            }
            else if (parent.Position.Fogged(parent.Map))
            {
                return;
            }
            if (PowerOn && FuelOk)
            {
                ticksUntilSpawn -= interval;
                CheckShouldSpawn();
            }
        }

        private void CheckShouldSpawn()
        {
            if (ticksUntilSpawn <= 0)
            {
                TryDoSpawn();
                ResetCountdown();
            }
        }

        public bool TryDoSpawn()
        {
            if (!parent.Spawned)
            {
                return false;
            }
            if (Props.spawnMaxAdjacent >= 0)
            {
                int num = 0;
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 c = parent.Position + GenAdj.AdjacentCellsAndInside[i];
                    if (!c.InBounds(parent.Map))
                    {
                        continue;
                    }
                    List<Thing> thingList = c.GetThingList(parent.Map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j].def == Props.thingToSpawn)
                        {
                            num += thingList[j].stackCount;
                            if (num >= Props.spawnMaxAdjacent)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            if (TryFindSpawnCell(parent, Props.thingToSpawn, Props.spawnCount, out IntVec3 result))
            {
                Thing thing = ThingMaker.MakeThing(Props.thingToSpawn);
                thing.stackCount = Props.spawnCount;
                if (thing == null)
                {
                    Log.Error("Could not spawn anything for " + parent);
                }
                if (Props.inheritFaction && thing.Faction != parent.Faction)
                {
                    thing.SetFaction(parent.Faction);
                }
                GenPlace.TryPlaceThing(thing, result, parent.Map, ThingPlaceMode.Direct, out Thing lastResultingThing);
                if (Props.spawnForbidden)
                {
                    lastResultingThing.SetForbidden(value: true);
                }
                if (Props.showMessageIfOwned && parent.Faction == Faction.OfPlayer)
                {
                    Messages.Message("MessageCompSpawnerSpawnedItem".Translate(Props.thingToSpawn.LabelCap), thing, MessageTypeDefOf.PositiveEvent);
                }
                return true;
            }
            return false;
        }

        public static bool TryFindSpawnCell(Thing parent, ThingDef thingToSpawn, int spawnCount, out IntVec3 result)
        {
            foreach (IntVec3 item in GenAdj.CellsAdjacent8Way(parent).InRandomOrder())
            {
                if (item.Walkable(parent.Map))
                {
                    Building edifice = item.GetEdifice(parent.Map);
                    if (edifice == null || !thingToSpawn.IsEdifice())
                    {
                        Building_Door building_Door = edifice as Building_Door;
                        if ((building_Door == null || building_Door.FreePassage) && (parent.def.passability == Traversability.Impassable || GenSight.LineOfSight(parent.Position, item, parent.Map)))
                        {
                            bool flag = false;
                            List<Thing> thingList = item.GetThingList(parent.Map);
                            for (int i = 0; i < thingList.Count; i++)
                            {
                                Thing thing = thingList[i];
                                if (thing.def.category == ThingCategory.Item && (thing.def != thingToSpawn || thing.stackCount > thingToSpawn.stackLimit - spawnCount))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                result = item;
                                return true;
                            }
                        }
                    }
                }
            }
            result = IntVec3.Invalid;
            return false;
        }

        private void ResetCountdown()
        {
            ticksUntilSpawn = Props.spawnIntervalRange.RandomInRange;
        }

        public override void PostExposeData()
        {
            string str = Props.saveKeysPrefix.NullOrEmpty() ? null : (Props.saveKeysPrefix + "_");
            Scribe_Values.Look(ref ticksUntilSpawn, str + "ticksUntilSpawn", 0);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "DEBUG: Spawn " + Props.thingToSpawn.label;
                command_Action.icon = TexCommand.DesirePower;
                command_Action.action = delegate
                {
                    TryDoSpawn();
                    ResetCountdown();
                };
                yield return command_Action;
            }
        }

        public override string CompInspectStringExtra()
        {
            if (Props.writeTimeLeftToSpawn && (!Props.requiresPower || PowerOn))
            {
                return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(Props.thingToSpawn, null, Props.spawnCount)) + ": " + ticksUntilSpawn.ToStringTicksToPeriod();
            }
            return null;
        }
    }
}
