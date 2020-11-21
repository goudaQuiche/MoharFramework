using System.Linq;
using Verse;

namespace MoHarRegeneration
{
    [StaticConstructorOnStartup]
    public class ModCompatibilityCheck
    {
        public static bool MoharActiveCheck
        {
            get
            {
                if (Prefs.DevMode) return true;

                return
                    ModsConfig.IsActive(ModActiveDefs.MoharHediffModPackageId) &&
                    ModsConfig.ActiveModsInLoadOrder.Any(
                        m =>
                        m.Name == ModActiveDefs.MoharHediffModName
                    );
                
            }
        }

        public static void DisplayMessage()
        {
            Log.ErrorOnce(ModActiveDefs.MoharHediffMissingMessage, 655481347);
        }

        public static bool MoharCheckAndDisplay()
        {
            if (!MoharActiveCheck)
            {
                DisplayMessage();
                return false;
            }

            return true;
        }
    }
}
