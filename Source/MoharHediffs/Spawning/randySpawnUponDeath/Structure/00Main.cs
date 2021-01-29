using Verse;
using RimWorld;
using System.Collections.Generic;

namespace MoharHediffs
{
    public class GeneralSettings
    {
        //Item - thing/pawnKind
        public List<ThingSettings> things;

        // default values
        public CommonSettings defaultSettings;

        public bool HasSomethingToSpawn => !things.NullOrEmpty();
        public bool HasDefaultSettings => defaultSettings != null;

        public void LogParams(bool myDebug = false)
        {
            Tools.Warn(
                "HasSomethingToSpawn:" + HasSomethingToSpawn + "; "  +
                "HasDefaultSettings:" + HasDefaultSettings + "; "
                , myDebug
            );
        }
        
    }
}