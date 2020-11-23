using Verse;
using System;

namespace OLB
{
    public static class MoteTracerUtils
    {
        public static void AddMoteTracer(this CompDecorate comp, string nLabel, Thing nThing, int nGrace, bool nCoexistSame, bool nCoexistOther)
        {
            comp.LivingMotes.Add(new MoteTracer(nLabel, nThing, nGrace, nCoexistSame, nCoexistOther));
        }
        /*
        public static void RemoveMoteTracer(this CompDecorate comp, string label)
        {
            comp.LivingMotes.RemoveAll(MT => MT.LabelsMatch(label));
        }
        */

        public static bool LabelsMatch(this MoteTracer MT, MoteDecoration MD)
        {
            return MT.Label == MD.label;
        }

        public static bool LabelsNoMatch(this MoteTracer MT, MoteDecoration MD)
        {
            return MT.Label != MD.label;
        }

        public static void MoteTracerMaintenanceRemoval(this CompDecorate comp)
        {
            comp.LivingMotes.RemoveAll(MT => MT.MoteIsDead());
        }
            
        public static bool MoteIsDead(this MoteTracer MT)
        {
            return MT.EmittedMote == null || !MT.EmittedMote.Spawned;
        }

        public static bool SameMoteAlreadyExists(this CompDecorate comp)
        {
            return 
                comp.HasLivingMotes && 
                comp.LivingMotes.Any(
                    LM => 
                    LM.LabelsMatch(comp.CurItem)
                );
        }

        public static bool AllConditionsValidation(this CompDecorate comp)
        {
            if (comp.CurItem.HasNoCondition)
                return true;

            if ( (comp.CurItem.forbidsSomeCoexist || comp.CurItem.HasGraceTicks) && comp.HasEmptyTracer)
            {
                Tools.Warn("Empty tracer -> ok", comp.CurItem.debug);
            }
            else
            {
                if (!comp.CoexistingValidation())
                {
                    Tools.Warn("coexistance invalidation -> ko", comp.CurItem.debug);
                    return false;
                }


                if (!comp.GraceValidation())
                {
                    Tools.Warn("grace invalidation -> ko", comp.CurItem.debug);
                    return false;
                }
            }

            if (!comp.FuelAndPowerValidation())
            {
                Tools.Warn("Fuel And power invalidation -> ko", comp.CurItem.debug);
                return false;
            }

            if (!comp.ReservationValidation())
            {
                Tools.Warn("reservation invalidation -> ko", comp.CurItem.debug);
                return false;
            }
                

            return true;
        }
    }
}
