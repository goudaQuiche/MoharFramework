using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace MoharHediffs
{
    public class CopyPawnSettings
    {
        public bool name = false;
        public bool pawnKind = false;
        public bool age = false;
        public bool gender = false;

        public bool melanin = false;
        public bool skinColorChannel = false;

        public bool bodyType = false;
        public bool crownType = false;

        public bool hair = false;
        public bool hairColor = false;

        public bool hediff = false;
        public List<HediffDef> excludeHediff;
        public bool excludeTendableHediffs = false;
        public bool excludePermanentHediffs = false;

        public bool childBS = false;
        public bool adultBS = false;

        public bool skills = false;
        public FloatRange skillDecay = new FloatRange(1,1);
        public bool passions = false;

        public bool traits = false;

        //public bool HasSkillDecay => skillDecay != null;
        public bool HasHediffExclusion => !excludeHediff.NullOrEmpty();
    }
}