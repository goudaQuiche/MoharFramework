using System.Linq;
using Verse;

namespace MoharHediffs
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
                    ModsConfig.IsActive(MyDefs.MoharHediffModPackageId) &&
                    ModsConfig.ActiveModsInLoadOrder.Any(
                        m =>
                        m.Name == MyDefs.MoharHediffModName
                        //m.GetPublishedFileId()).ToString() == MyDefs.MoharPublishedId
                        //&& m.SteamAppId == MyDefs.MoharPublishedId
                    );
                
            }
        }

        public static void DisplayMessage()
        {
            Log.ErrorOnce(MyDefs.MoharHediffMissingMessage, 655481347);
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
