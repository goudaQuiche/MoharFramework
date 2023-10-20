using RimWorld;
using Verse;

namespace CSSU
{
    public static class JobDriver_Meditate_Utils
    {
        public static void AddProgress(IntVec3 cell, Map map)
        {
            foreach (ThingDef td in StaticDef.AllDefs)
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

        /*
        public static void RawAddProgress(IntVec3 cell, Map map)
        {

            Thing t = null;
            t = cell.GetFirstBuilding(map);

            if (t == null) return;

            CompSpawnSubplant compSpawnSubplant = t.TryGetComp<CompSpawnSubplant>();
            if (compSpawnSubplant == null)
                return;

            compSpawnSubplant.AddProgress(JobDriver_Meditate.AnimaTreeSubplantProgressPerTick, false);
        }
        */
    }
}
