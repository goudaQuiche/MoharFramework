using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using AlienRace;

namespace DefGen
{
    public class ApparelBlacklistDef : Def
    {
        public List<RaceRestrictionItem> restrictions;
        
        public override string ToString() => defName;
        public ApparelBlacklistDef Named(string searchedDN) => DefDatabase<ApparelBlacklistDef>.GetNamed(searchedDN);
        public override int GetHashCode() => defName.GetHashCode();
    }
    
    public class RaceRestrictionItem
    {
        public ThingDef_AlienRace race;

        public bool debug = false;
        public bool reportResult = false;

        public List<ApparelLayerDef> apparelLayerDef;
        public List<string> apparelTags;
        public List<BodyPartGroupDef> bodyPartGroupDefs;
        public List<ThingCategoryDef> thingCategoryDefs;

        public bool HasLayerDef => !apparelLayerDef.NullOrEmpty();
        public bool HasTags => !apparelTags.NullOrEmpty();
        public bool HasBodyPartGroupDef => !bodyPartGroupDefs.NullOrEmpty();
        public bool HasThingCategoryDef => !thingCategoryDefs.NullOrEmpty();
    }

}
