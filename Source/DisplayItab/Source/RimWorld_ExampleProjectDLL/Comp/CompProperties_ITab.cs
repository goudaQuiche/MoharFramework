using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace DisplayITab
{
	public class CompProperties_ITab : CompProperties
	{
        public List<ThingDef> Pages;

        public Vector2 imgSize = new Vector2(512,512);
        public DisplayWay displayWay = DisplayWay.RawTexture;

        public CompProperties_ITab()
		{
			compClass = typeof(Comp_ITab);
		}
	}

    public enum DisplayWay
    {
        RawTexture = 0,
        ProcessedTexture = 1
    }
}