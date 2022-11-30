using Verse;
using System;

namespace OLB
{
    public static class FuelAndPowerConditions
    {
        public static bool FuelAndPowerValidation(this CompDecorate comp)
        {
            if (comp.CurItem.RequiresNeitherFuelNorPowerCheck)
            {
                Tools.Warn(comp.CurItem.label + " does not require fuel or power check; ok", comp.CurItem.debug);
                return true;
            }

            if (comp.RequiresFuelAndPowerCheck && (!comp.HasPowerOn || !comp.IsFueled))
            {
                Tools.Warn(comp.CurItem.label + " requires fuel or power check; but one is not ok; ko", comp.CurItem.debug);
                return false;
            }
                
            if (comp.RequiresFuelCheck && !comp.IsFueled)
            {
                Tools.Warn(comp.CurItem.label + " requires fuel check; but not fueled; ko", comp.CurItem.debug);
                return false;
            }
                

            if (comp.RequiresPowerCheck && !comp.HasPowerOn)
            {
                Tools.Warn(comp.CurItem.label + " requires power check; but no power; ko", comp.CurItem.debug);
                return false;
            }

            Tools.Warn(comp.CurItem.label + " fuel and power check; ok", comp.CurItem.debug);
            return true;
        }
    }
}
