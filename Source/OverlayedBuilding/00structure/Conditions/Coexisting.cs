using Verse;
using System;

namespace OLB
{
    public static class CoexistingCondition
    {
        public static bool NonCoexistingMoteInTracer(this CompDecorate comp)
        {
            return
                comp.HasLivingMotes &&
                comp.LivingMotes.Any(
                    LM => LM.ForbidsCoexistWithAny
                );
        }

        public static bool DifferentMoteExists(this CompDecorate comp)
        {
            return
                comp.HasLivingMotes &&
                comp.LivingMotes.Any(
                    LM => LM.LabelsNoMatch(comp.CurItem)
                );
        }

        public static bool CoexistingValidation(this CompDecorate comp)
        {
            if (comp.CurItem.allowsCoexistWithAny)
            {
                Tools.Warn(comp.CurItem.label + " coexists with all motes; ok", comp.CurItem.debug);
                return true;
            }

            if (comp.CurItem.forbidsCoexistWithOther && comp.DifferentMoteExists())
            {
                Tools.Warn(comp.CurItem.label + " forbids other motes and some exist; ko", comp.CurItem.debug);
                return false;
            }

            if (comp.CurItem.forbidsCoexistWithSame && comp.SameMoteAlreadyExists())
            {
                Tools.Warn(comp.CurItem.label + " forbids same motes and some exist; ko", comp.CurItem.debug);
                return false;
            }

            Tools.Warn(comp.CurItem.label + " coexist check ok", comp.CurItem.debug);
            return true;
        }
    }
}
