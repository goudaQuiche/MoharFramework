using System.Linq;
using Verse;

namespace Ubet
{
    [StaticConstructorOnStartup]
    public class ModCompatibilityCheck
    {
        private const string alienRacesMod_ModName = "Humanoid Alien Races 2.0";

        public static bool AlienRacesIsActive
        {
            get
            {
                return ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == alienRacesMod_ModName);
            }
        }
    }
}