using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CSSU
{
    public static class JobDriver_Meditate_Utils
    {
        public static void AddProgress(IntVec3 cell, Map map)
        {
            foreach(ThingDef td in MyDefs.AllDefsWithCSS)
            {
                Thing t = null;
                if (td.category == ThingCategory.Building)
                {
                    t = cell.GetFirstBuilding(map);
                }
                else if (td.category == ThingCategory.Plant)
                {
                    t = cell.GetPlant(map);
                }

                if (t == null) continue;

                CompSpawnSubplant compSpawnSubplant = t.TryGetComp<CompSpawnSubplant>();
                if (compSpawnSubplant == null)
                    continue;
                
                compSpawnSubplant.AddProgress(JobDriver_Meditate.AnimaTreeSubplantProgressPerTick, false);
            }
        }
    }
}
