using System.Linq;
using Verse;

namespace OLB
{
    [StaticConstructorOnStartup]
    public class ModCompatibilityCheck
    {
        public static bool MoharActiveCheck
        {
            get
            {
                if (Prefs.DevMode) return true;
                /*
                Log.Error( "steamId:"+
                        ModsConfig.ActiveModsInLoadOrder.Where(
                        m =>
                        m.Name == MyDefs.MoharHediffModName).FirstOrDefault().GetPublishedFileId.ToString()
                    );
                */
                return
                    ModsConfig.IsActive(ModActiveData.MoharHediffModPackageId) &&
                    ModsConfig.ActiveModsInLoadOrder.Any(
                        m =>
                        m.Name == ModActiveData.MoharHediffModName
                        //m.GetPublishedFileId()).ToString() == MyDefs.MoharPublishedId
                        //&& m.SteamAppId == MyDefs.MoharPublishedId
                    );
                
            }
        }

        public static void DisplayMessage()
        {
            Log.ErrorOnce(ModActiveData.MoharHediffMissingMessage, 655481347);
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
