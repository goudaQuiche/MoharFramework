using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharThoughts
{
    public class MTW_TUJ_Def : Def
    {
        public ThoughtDef thought;

        public ThingDef race;

        public List<LifeStageDef> lifeStages;

        public List<JobDef> applyThoughtList;
        public List<JobDef> ignoreThoughtList;

        public bool HasLifeStages => !lifeStages.NullOrEmpty();
        public bool HasApplyList => !applyThoughtList.NullOrEmpty();
        public bool HasIgnoreList => !ignoreThoughtList.NullOrEmpty();

        public bool HasAccessList => HasApplyList || HasIgnoreList;

        public bool debug = false;

        public override string ToString() => defName;
        public MTWDef Named(string searchedDN) => DefDatabase<MTWDef>.GetNamed(searchedDN);
        public override int GetHashCode() => defName.GetHashCode();
    }
}
