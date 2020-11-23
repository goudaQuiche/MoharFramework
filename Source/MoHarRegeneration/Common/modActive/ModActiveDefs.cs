using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoHarRegeneration
{
    public static class ModActiveDefs
    {
        public static readonly string MoharHediffModPackageId = "goudaquiche.MoharFramework";
        public static readonly string MoharHediffModName = "MoHAR framework";
        public static readonly string MoharHediffMissingMessage = 
            "Please consider using the MoharHediffs mod. "+
            "Picking assemblies to add to your mod may hurt the player experience. "+
            "Developing C# assemblies takes time. By denying my efforts, by picking assemblies, you dont help me. Why should I help in return?";

        public static int OneYearTicks = 3600000;
    }
}
