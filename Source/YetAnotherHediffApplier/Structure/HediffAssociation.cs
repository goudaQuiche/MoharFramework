using System.Collections.Generic;
using Verse;
using RimWorld;
using Ubet;

namespace YAHA
{
    public class HediffAssociation
    {
        public List<HediffItem> hediffPool;
        public List<RandomHediffItem> randomHediffPool;

        public List<string> bodyPart;
        public UbetDef condition;
        public ApplySpecifics specifics;

        public bool HasHediffPool => !hediffPool.NullOrEmpty();
        public bool HasRandomHediffPool => !randomHediffPool.NullOrEmpty();

        public bool HasBodyPartToApplyHediff => !bodyPart.NullOrEmpty();
    }

    public class ApplySpecifics
    {
        // -1 unlimited
        public int applyNumLimit = 1;
        public bool removeIfFalse = false;
        //public bool removeOthersIfFalse = false;
        //public bool removeIfOtherTrue = false;

        public Grace grace;
        public Discard discard;
        public bool triggered = false;

        
        public bool HasLimit => applyNumLimit > 0;
    }

    public class Grace
    {
        public FloatRange chance = new FloatRange(1, 1);
        public IntRange uponApply = new IntRange(0, 0);
        public IntRange uponRemove = new IntRange(0, 0);
    }
    public class Discard
    {
        public FloatRange chance = new FloatRange(1, 1);
        public FloatRange uponApply = new FloatRange(0, 0);
        public FloatRange uponRemove = new FloatRange(0, 0);
    }

    public class RandomHediffItem : HediffItem
    {
        public float weight;
    }
    public class HediffItem
    {
        public FloatRange severity = new FloatRange(1, 1);
        public HediffDef hediff;

        public bool HasSeverity => !(severity.min == 1 && severity.max == 1);
    }

}
