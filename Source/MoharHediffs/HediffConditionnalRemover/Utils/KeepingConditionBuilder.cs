using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public static class HediffRemovalConditionBuilder
    {
        public static void CopyHediffKeepingCondition(HediffKeepingCondition source, HediffKeepingCondition dest, bool debug = false)
        {
            string debugStr = debug ? "CopyHediffCondition - " : "";

            if (source.HasTemperatureCondition)
            {
                Tools.Warn(debugStr + "found HasTemperatureCondition, copying", debug);
                    dest.temperature = source.temperature;
            }

            if (source.HasLightCondition)
            {
                Tools.Warn(debugStr + "found HasLightCondition, copying", debug);
                dest.light = new LightCondition(source.light);
            }
            if (source.HasNeedCondition)
            {
                Tools.Warn(debugStr + "found HasNeedCondition, copying", debug);
                foreach(NeedCondition nc in source.needs)
                {
                    if (dest.needs.Any(n => n.needDef == nc.needDef))
                        dest.needs.Where(n => n.needDef == nc.needDef).First().level = nc.level;
                    else
                        dest.needs.Add(new NeedCondition(nc));
                }
            }

            if (source.HasDestroyingHediffs)
            {
                Tools.Warn(debugStr + "found HasDestroyingHediffs, copying", debug);
                foreach (HediffSeverityCondition hsc in source.destroyingHediffs)
                {
                    if (dest.destroyingHediffs.Any(dh => dh.hediffDef== hsc.hediffDef))
                        dest.destroyingHediffs.Where(dh => dh.hediffDef == hsc.hediffDef).First().acceptableSeverity = hsc.acceptableSeverity;
                    else
                        dest.destroyingHediffs.Add(new HediffSeverityCondition(hsc));
                }
            }
        }

        public static HediffKeepingCondition GetDefaultPlusSpecificHediffCondition(HediffKeepingCondition defaultHKC, HediffKeepingCondition specificHKC, bool debug = false)
        {
            string debugStr = debug ? "GetDefaultPlusSpecificHediffCondition - " : "";
            Tools.Warn(debugStr + "allocating answerHC", debug);
            HediffKeepingCondition answerHKC = new HediffKeepingCondition {
                needs = new List<NeedCondition>()
            };

            if (defaultHKC != null)
            {
                Tools.Warn(debugStr + "found defaultHKC, copying", debug);
                CopyHediffKeepingCondition(defaultHKC, answerHKC, debug);
            }

            if (specificHKC != null)
            {
                Tools.Warn(debugStr + "found specificHKC, copying", debug);
                CopyHediffKeepingCondition(specificHKC, answerHKC, debug);
            }

            Tools.Warn(
                debugStr +
                $"HasDestroyingHediffs:{answerHKC.HasDestroyingHediffs} - " + (answerHKC.HasDestroyingHediffs?answerHKC.destroyingHediffs.Count():0) +
                $"HasLightCondition:{answerHKC.HasLightCondition} - " + 
                (answerHKC.HasLightCondition? ("reqIn:"+answerHKC.light.requiresInside+ " reqOut:" + answerHKC.light.requiresOutside) : "") +
                $"HasNeedCondition:{answerHKC.HasNeedCondition}" + (answerHKC.HasNeedCondition ? answerHKC.needs.Count() : 0) +
                $"HasTemperatureCondition:{answerHKC.HasTemperatureCondition}"
                , debug);

            return answerHKC;
        }

    }
}
