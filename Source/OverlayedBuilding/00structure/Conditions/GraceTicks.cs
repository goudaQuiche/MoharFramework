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

        public static bool DyingMoteInTracer(this CompDecorate comp)
        {

            if(comp.MyDebug)
                Log.Warning("DyingMoteInTracer");

            return 
                comp.LivingMotes.Any(
                    LM =>
                        LM.HasRemainingGraceTicks &&
                        LM.LabelsMatch(comp.CurItem) &&
                        ((Mote)LM.EmittedMote).AgeSecs >= LM.EmittedMote.def.mote.Lifespan -2
                    )
                ;
        }

        public static bool GraceValidation(this CompDecorate comp)
        {
            if (!comp.CurItem.HasGraceTicks)
            {
                if(comp.CurItem.debug)
                    Log.Warning(comp.CurItem.label + " does not require gracetick check; ok");
                return true;
            }

            if (comp.DyingMoteInTracer())
            {
                return true;
            }

            if (comp.SameMoteWithGraceInTracer())
            {
                if(comp.CurItem.debug)
                    Log.Warning(comp.CurItem.label + " found same item type with graceTicks; ko");
                return false;
            }

            if(comp.CurItem.debug)
                Log.Warning(comp.CurItem.label + " grace check ok");

            return true;
        }
    }
}
