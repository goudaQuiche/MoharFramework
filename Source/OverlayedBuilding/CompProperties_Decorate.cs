using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{
	public class CompProperties_Decorate : CompProperties
	{
        public List<MoteDecoration> moteDecorations;
        public int workerReservationUpdateFrequency = 60;

        public bool debug = false;

        public CompProperties_Decorate()
		{
			compClass = typeof(CompDecorate);
		}
	}
}