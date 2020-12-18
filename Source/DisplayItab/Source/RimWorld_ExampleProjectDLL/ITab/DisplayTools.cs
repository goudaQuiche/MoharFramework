using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DisplayITab
{
	public static class DisplayTools
    {
        public static Texture GetRealMaterial(this Thing thing)
        {
            return thing.Graphic.ExtractInnerGraphicFor(thing).MatAt(thing.def.defaultPlacingRot).mainTexture;
        }
	}
}