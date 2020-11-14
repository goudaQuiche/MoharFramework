using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace MoharGamez
{
    public static class Tools
    {
        public static void Warn(string warning, bool debug = false)
        {
            if (debug)
                Log.Warning(warning);
        }
    }
}
