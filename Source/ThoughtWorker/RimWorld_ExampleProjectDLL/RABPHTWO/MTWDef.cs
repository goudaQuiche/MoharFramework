using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharThoughts
{
    public class MTWDef : Def
    {
        public ThoughtDef thought;

        public ThingDef race;

        public BodyPartDef bodyPart;
        public string bodyPartLabel;
        
        public HediffDef hediff;
        public IntRange bpNum = new IntRange(0, 0);
        //public List<LifeStageDef> lifeStages;

        public List<HediffDef> applyThoughtHediffList;
        public List<HediffDef> ignoreThoughtHediffList;
        public bool ignoreThoughtIfAddedPart = false;

        public bool debug = false;

        public override string ToString() => defName;
        public MTWDef Named(string searchedDN) => DefDatabase<MTWDef>.GetNamed(searchedDN);
        public override int GetHashCode() => defName.GetHashCode();

        public bool HasBodyPartDefTarget => bodyPart != null;
        public bool HasBodyPartLabelTarget => !bodyPartLabel.NullOrEmpty();
        public bool HasTarget => HasBodyPartDefTarget || HasBodyPartLabelTarget;

        public bool HasRequiredBpNum => bpNum.min != 0 || bpNum.max != 0;
        //public bool HasLifeStages => !lifeStages.NullOrEmpty();
        public bool HasApplyList => !applyThoughtHediffList.NullOrEmpty();
        public bool HasIgnoreList => !ignoreThoughtHediffList.NullOrEmpty();

        public bool HasAccessList => HasApplyList || HasIgnoreList;
        //public bool IgnoreForAnyAddedPart => !ignoreWhenHediff.NullOrEmpty();
    }
}
