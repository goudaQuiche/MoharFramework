using Verse;
using System;
using RimWorld;

namespace HEREHEGI
{
    public class HediffComp_DataHediff : HediffComp
    {
        public HediffCompProperties_DataHediff Props
        {
            get
            {
                return (HediffCompProperties_DataHediff)this.props;
            }
        }

        bool MyDebug => Props.debug;
        public bool IsValid => !Props.InputHediffPool.NullOrEmpty() && !Props.OutputHediffPool.NullOrEmpty() && Props.InputHediffPool.Count == Props.OutputHediffPool.Count;
        public bool HasChances => !Props.HediffReplacementChance.NullOrEmpty() && Props.InputHediffPool.Count == Props.HediffReplacementChance.Count;

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;

                if (!MyDebug)
                    return result;

                if (!IsValid)
                {
                    result += "HediffCompProperties are invalid : empty InputHediffPool/OutputHediffPool or count differs";
                    return result;
                }

                for (int i = 0; i < Props.InputHediffPool.Count; i++)
                    result += 
                        ' ' + i.ToString("00")+ Props.InputHediffPool[i].label + 
                        " => " + 
                        (Props.OutputHediffPool[i].IsNullHediff()?"Removed": Props.OutputHediffPool[i].label) + 
                        (HasChances? "("+Props.HediffReplacementChance[i].ToStringPercent()+")" : "") +
                        ";\n";

                return result;
            }
        }
    }
}
