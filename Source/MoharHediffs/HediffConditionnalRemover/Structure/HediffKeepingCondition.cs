using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public class HediffKeepingCondition
    {
        public FloatRange? temperature = null;
        public LightCondition light;
        public List<NeedCondition> needs;
        public List<HediffSeverityCondition> destroyingHediffs;

        public bool HasTemperatureCondition => temperature != null;
        public bool HasLightCondition => light != null;
        public bool HasNeedCondition => !needs.NullOrEmpty();
        public bool HasDestroyingHediffs => !destroyingHediffs.NullOrEmpty() && destroyingHediffs.Any(dh => !dh.HasHediffDef);
    }

    public class LightCondition
    {
        public bool requiresOutside = true;
        public bool requiresInside = false;
        public FloatRange? level;

        public bool RequiresLightLevel => level != null;

        public LightCondition(LightCondition copyMe)
        {
            requiresInside = copyMe.requiresInside;
            requiresOutside = copyMe.requiresOutside;
            level = copyMe.level;
        }
    }

    public class HediffSeverityCondition
    {
        public HediffDef hediffDef;
        public FloatRange acceptableSeverity = new FloatRange(0, .5f);

        public bool HasHediffDef => hediffDef != null;

        public HediffSeverityCondition(HediffSeverityCondition copyMe)
        {
            hediffDef = copyMe.hediffDef;
            acceptableSeverity = copyMe.acceptableSeverity;
        }
        public HediffSeverityCondition() { }
    }

    public class NeedCondition
    {
        public NeedDef needDef;
        public FloatRange level;

        public NeedCondition(NeedCondition copyMe)
        {
            needDef = copyMe.needDef;
            level = copyMe.level;
        }
        public NeedCondition() { }
    }

}
