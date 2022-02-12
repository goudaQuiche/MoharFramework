using HarmonyLib;
using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;


namespace OHPLS
{
    public class OHPLS_Settings : ModSettings
    {
        public bool SafeRemoval = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref SafeRemoval, "SafeRemoval");
        }

    }

    public class OHPLS_Mod : Mod
    {
        OHPLS_Settings settings;

        public OHPLS_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<OHPLS_Settings>();
        }

        public override string SettingsCategory()
        {
            return "MOHAR - OHPLS stands for One Hediff Per Life Stage";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("Safe mod removal");
            listing.GapLine();
            listing.Label(
                " 1. Check this if you want to remove the mod"+
                "\n 2. Load your saved game, wait a few seconds"+
                "\n 3. Save, preferably in another slot"+
                "\n 4. Quit and remove the mod"+
                "\n 5. Load your last save"
            );

            listing.CheckboxLabeled("SafeModRemoval: ", ref settings.SafeRemoval);

            listing.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
