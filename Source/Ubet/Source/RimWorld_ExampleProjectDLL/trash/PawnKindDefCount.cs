using System;
using RimWorld;
using Verse;

namespace MoharAiJob
{
    public struct PawnKindDefCount : IEquatable<PawnKindDefCount>, IExposable
    {
        private PawnKindDef pawnKindDef;
        private int count;

        public PawnKindDef PawnKindDef => pawnKindDef;
        public int Count => count;

        //public string Label => GenLabel.ThingLabel(pawnKindDef, null, count);
        //public string Label => GenLabel.BestKindLabel(PawnKindDef, Gender.Male);
        public string Label => PawnKindDef.label;
        public string LabelCap => Label.CapitalizeFirst(pawnKindDef);

        public PawnKindDefCount(PawnKindDef pawnKindDef, int count)
        {
            if (count < 0)
            {
                Log.Warning("Tried to set PawnKindDefCount count to " + count + ". pawnKindDef=" + pawnKindDef);
                count = 0;
            }
            this.pawnKindDef = pawnKindDef;
            this.count = count;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref pawnKindDef, "pawnKindDef");
            Scribe_Values.Look(ref count, "count", 1);
        }

        public PawnKindDefCount WithCount(int newCount)
        {
            return new PawnKindDefCount(pawnKindDef, newCount);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PawnKindDefCount))
            {
                return false;
            }
            return Equals((PawnKindDefCount)obj);
        }

        public bool Equals(PawnKindDefCount other)
        {
            return this == other;
        }

        public static bool operator ==(PawnKindDefCount a, PawnKindDefCount b)
        {
            if (a.pawnKindDef == b.pawnKindDef)
            {
                return a.count == b.count;
            }
            return false;
        }

        public static bool operator !=(PawnKindDefCount a, PawnKindDefCount b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Gen.HashCombine(count, pawnKindDef);
        }

        public override string ToString()
        {
            return "(" + count + "x " + ((pawnKindDef != null) ? pawnKindDef.defName : "null") + ")";
        }

        public static implicit operator PawnKindDefCount(PawnKindDefCountClass t)
        {
            if (t == null)
            {
                return new PawnKindDefCount(null, 0);
            }
            return new PawnKindDefCount(t.pawnKindDef, t.count);
        }
    }
}
