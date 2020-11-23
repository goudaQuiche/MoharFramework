using Verse;
using System;

namespace OLB
{
    public static class GraceCondition
    {
        public static bool SameMoteWithGraceInTracer(this CompDecorate comp)
        {
            return
                comp.CurItem.HasGraceTicks && 
                comp.LivingMotes.Any(
                    LM =>
                    LM.HasRemainingGraceTicks &&
                    LM.LabelsMatch(comp.CurItem)
                );
        }

        public static bool GraceValidation(this CompDecorate comp)
        {
            if (!comp.CurItem.HasGraceTicks)
            {
                Tools.Warn(comp.CurItem.label + " does not require gracetick check; ok", comp.CurItem.debug);
                return true;
            }

            if (comp.SameMoteWithGraceInTracer())
            {
                Tools.Warn(comp.CurItem.label + " found same item type with graceTicks; ko", comp.CurItem.debug);
                return false;
            }

            Tools.Warn(comp.CurItem.label + " grace check ok", comp.CurItem.debug);
            return true;
        }
    }
}
