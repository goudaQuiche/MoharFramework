using Verse;
using System.Collections.Generic;
using System.Linq;

namespace MoharDamage
{
    public static class IgnoredThingsComputer
    {
        public static List<ThingDef> GenerateThingDefsToIgnore(this DamageWorker_ExplosionOnImpact DWEOI, Pawn Instigator)
        {
            string debugStr = DWEOI.MyDebug ? "GenerateThingDefsToIgnore" : "";

            List<ThingDef> answer = new List<ThingDef>();

            if (!DWEOI.HasImmunity)
                return null;

            if (DWEOI.Immunity.HasIgnoredThingDefs)
            {
                Tools.Warn($"{debugStr} adding ignoringDamageThingDef");
                answer = DWEOI.Immunity.ignoredByDamageThingDefs;
            }

            if (DWEOI.Immunity.addAttackerThingDef && !answer.Contains(Instigator.def))
            {
                Tools.Warn($"{debugStr} adding {Instigator.def}");
                answer.Add(Instigator.def);
            }

            if (!answer.NullOrEmpty())
                Tools.Warn($"{debugStr} final count:{answer.Count}");

            return answer;

        }

        public static List<Thing> GenerateThingsToIgnore(this DamageWorker_ExplosionOnImpact DWEOI, Map map, Thing Instigator, List<ThingDef> thingDefs, int limit = 100)
        {
            int limiter = limit;
            List<Thing> answer = new List<Thing>();

            if (DWEOI.Immunity.HasIgnoredThingDefs)
            {
                List<Thing> potentialImmunizable =
                map.spawnedThings.Where(t => thingDefs.Contains(t.def)).ToList();

                foreach (Thing ignoreMe in potentialImmunizable)
                    if (!answer.Contains(ignoreMe) && limiter-- > 0)
                        answer.Add(ignoreMe);
            }

            if (DWEOI.Immunity.addPawnsFromAttackerFaction || DWEOI.Immunity.addThingsFromAttackerFaction) {
                List<Thing> potentialImmunizable =
                    map.spawnedThings.Where(
                        t =>
                        t.Faction != null && Instigator.Faction != null &&
                        Instigator.Faction == t.Faction &&
                        (DWEOI.Immunity.addPawnsFromAttackerFaction?t is Pawn:true) &&
                        (DWEOI.Immunity.addThingsFromAttackerFaction ? !(t is Pawn) : true)
                    ).ToList();

                foreach (Thing ignoreMe in potentialImmunizable)
                    if (!answer.Contains(ignoreMe) && limiter-- > 0)
                        answer.Add(ignoreMe);
            }

            if (DWEOI.HasImmunity && DWEOI.Immunity.addAttacker && !answer.Contains(Instigator) && limiter > 0)
            {
                //limiter--;
                answer.Add(Instigator);
            }

            return answer;
        }
    }
}
