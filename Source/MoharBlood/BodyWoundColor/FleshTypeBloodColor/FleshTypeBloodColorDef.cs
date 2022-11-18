using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoharBlood
{
    public class FleshTypeWoundColorDef : Def
    {
        public List<FleshTypeWoundColor> fleshTypeWoundColor;
    }

    public class FleshTypeWoundColor
    {
        public FleshTypeDef fleshTypeDef;
        public BloodColor fallbackColor;
        public List<WoundColorAssociation> woundColor;
    }

    public class WoundColorAssociation
    {
        public List<string> textures;
        public BloodColor color;
    }
}
