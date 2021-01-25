using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace Ubet
{
    public class UbetDef : Def
    {
        public UbetNode trunk;
        
        public bool debug = false;

        public override string ToString() => defName;
        public UbetDef Named(string searchedDN) => DefDatabase<UbetDef>.GetNamed(searchedDN);
        public override int GetHashCode() => defName.GetHashCode();
    }
    
    public class UbetNode
    {
        public int depth;
        public bool? valueLeaf = null;
        
        public UbetOperand operand;
        public UbetCondition condition;

        public UbetNode parent;
        public UbetNode leftBranch;
        public UbetNode rightBranch;

        public bool IsTrunk => depth == 0;
    }

    public class UbetCondition
    {
        public Condition condition = Condition.Void;
        public Func<Thing, bool> conditionMethod;
    }
    public class UbetOperand
    {
        public Operand operand = Operand.Void;
        public Func<bool, bool, bool> operandMethod;
    }

    public enum Condition
    {
        IsHuman,
        IsNegligible,
        IsPawn,

        Void
    }

    public enum Operand
    {
        And,
        Or,

        Void
    }
}
