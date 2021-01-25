using System.Xml;
using Verse;

namespace MoharAiJob
{
    public sealed class PawnKindDefCountClass : IExposable
    {
        public PawnKindDef pawnKindDef;

        public int count;
        public string Label => pawnKindDef.label;
        public string LabelCap => Label.CapitalizeFirst(pawnKindDef);
        public string Summary => count + "x " + ((pawnKindDef != null) ? pawnKindDef.label : "null");

        public PawnKindDefCountClass()
        {
        }

        public PawnKindDefCountClass(PawnKindDef pawnKindDef, int count)
        {
            if (count < 0)
            {
                Log.Warning("Tried to set PawnKindDefCountClass count to " + count + ". pawnKindDef=" + pawnKindDef);
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

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count != 1)
            {
                Log.Error("Misconfigured PawnKindDefCountClass: " + xmlRoot.OuterXml);
                return;
            }
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
            count = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
        }

        public override string ToString()
        {
            return "(" + count + "x " + ((pawnKindDef != null) ? pawnKindDef.defName : "null") + ")";
        }

        public override int GetHashCode()
        {
            return pawnKindDef.shortHash + count << 16;
        }

        public static implicit operator PawnKindDefCountClass(PawnKindDefCount t)
        {
            return new PawnKindDefCountClass(t.PawnKindDef, t.Count);
        }
    }
}
