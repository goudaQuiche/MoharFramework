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
        public List<TriggerEvent> triggerEvent;

        public bool IsTriggered => !triggerEvent.NullOrEmpty();

        
        public bool HasLimit => applyNumLimit > 0;
        public bool HasDiscard => discard != null;
        public bool HasGrace => grace != null;
    }

    public class Grace
    {
        public GraceSettings uponApply;
        public GraceSettings uponRemove;

        public bool HasUponApplyGrace => uponApply != null;
        public bool HasUponRemoveGrace => uponRemove != null;
    }
    public class GraceSettings
    {
        public FloatRange chance = new FloatRange(1, 1);
        public IntRange tickAmount = new IntRange(0, 0);
    }
    public class Discard
    {
        public DiscardSettings uponApply;
        public DiscardSettings uponRemove;

        public bool HasUponApplyDiscard => uponApply != null;
        public bool HasUponRemoveDiscard => uponRemove != null;
    }
    public class DiscardSettings
    {
        public FloatRange chance = new FloatRange(1, 1);
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
