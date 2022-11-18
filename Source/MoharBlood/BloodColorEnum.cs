using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoharBlood
{
    public enum BloodColor
    {
        [Description("Skin first color")]
        SkinFirst = 1,
        [Description("Skin second color")]
        SkinSecond = 2,

        [Description("Insect")]
        Insect = 3,
        [Description("Human")]
        Human = 4,

        [Description("BloodDef")]
        Blood = 5,

        [Description("DefaultWoundColor")]
        DefaultWoundColor = 6
    }
}
