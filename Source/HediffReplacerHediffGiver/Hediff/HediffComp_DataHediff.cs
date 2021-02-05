using Verse;
using System;
using RimWorld;

namespace HEREHEGI
{
    public class HediffComp_DataHediff : HediffComp
    {
        public HediffCompProperties_DataHediff Props => (HediffCompProperties_DataHediff)this.props;

        bool MyDebug => Props.debug;
        public bool IsValid => !Props.replaceHediffs.NullOrEmpty() && !Props.replaceHediffs.Any(i => !i.IsValid);

        public override void CompPostMake()
        {
            base.CompPostMake();
            if (!StaticCheck.IsOk)
                parent.Severity = 0;
        }

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

                for (int i = 0; i < Props.replaceHediffs.Count; i++)
                {
                    ReplaceHediffItem RHI = Props.replaceHediffs[i];
                    result +=
                        ' ' + i.ToString("00") + RHI.inputH.label +
                        " => " +
                        (RHI.outputH!=null ? RHI.outputH.label : "destroy") +
                        (RHI.HasConsiderableChances ? "(" + RHI.chance + ")" : "") +
                        ";\n";
                }

                return result;
            }
        }
    }
}
