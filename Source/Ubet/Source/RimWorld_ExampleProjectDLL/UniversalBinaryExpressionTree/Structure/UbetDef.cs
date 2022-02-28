using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace Ubet
{
    public class UbetDef : Def
    {
        public Leaf trunk;
        
        public bool debug = false;

        public override string ToString() => defName;
        public UbetDef Named(string searchedDN) => DefDatabase<UbetDef>.GetNamed(searchedDN);
        public override int GetHashCode() => defName.GetHashCode();
    }
    
    public class Leaf
    {
        public Condition initialCondition;
        public Operand operand = Operand.empty;
        public Condition condition;

        public List<Leaf> leaf;

        public bool HasInitialCondition => initialCondition != null;
    }

    public class Condition
    {
        public ConditionType type = ConditionType.empty;
        public List<List<string>> stringArg;
        public List<IntRange> intArg;
        public List<FloatRange> floatArg;
        public SimpleCurve curve;

        public bool HasStringArg => !stringArg.NullOrEmpty();
        public bool Has1StringArg => HasStringArg && stringArg.Count == 1;
        public bool Has2StringArg => HasStringArg && stringArg.Count == 2;

        public bool HasIntArg => !intArg.NullOrEmpty();
        public bool HasFloatArg => !floatArg.NullOrEmpty();

        public bool HasStringFloatArg => HasStringArg && HasFloatArg;
        public bool HasCurve => !curve.EnumerableNullOrEmpty();

        public bool HasNoArg => stringArg.NullOrEmpty() && intArg.NullOrEmpty();

        public string Description => type.DescriptionAttr();
    }
}
