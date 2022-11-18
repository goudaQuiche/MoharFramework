using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace MoharBlood
{
    public static class BloodColoring
    {
        public enum PickedColor
        {
            [Description("First color")]
            First = 1,
            [Description("Second color")]
            Second = 2,
            [Description("Insect")]
            Insect = 3,

            [Description("Human")]
            Human = 4,

            [Description("BloodDef")]
            Blood = 5

        }
    }

    /*
    public class WoundColor
    {
        FleshTypeDef.Wound Wound;
        BloodColoring.PickedColor Color;
    }
    */
}
