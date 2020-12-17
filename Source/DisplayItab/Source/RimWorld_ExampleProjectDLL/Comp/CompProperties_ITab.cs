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

        public CompProperties_ITab()
		{
			compClass = typeof(Comp_ITab);
		}
	}

}