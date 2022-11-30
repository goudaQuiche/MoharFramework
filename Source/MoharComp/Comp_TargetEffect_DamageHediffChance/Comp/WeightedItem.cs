using Verse;
using RimWorld;

namespace MoharComp
{
    public class DamageItem
    {
        public DamageDef damageDef;
        public FloatRange damageAmount = new FloatRange(.25f, .5f);
        public float weight = 1;
        public BodyPartTagDef bodyPartTag;

        public bool HasBodyPartTag => bodyPartTag != null;
    }

    public class HediffItem
    {
        public HediffDef hediffDef;
        public FloatRange severity = new FloatRange(.25f, .5f);
        public float weight = 1;
        public BodyPartTagDef bodyPartTag;

        public bool HasBodyPartTag => bodyPartTag != null;
    }
}