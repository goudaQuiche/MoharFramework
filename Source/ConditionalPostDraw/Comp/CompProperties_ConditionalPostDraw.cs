using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

/*
 * Todo
 * 
 * Fade IN-Out
 * Rotate
 * Offset
 * *No condition
 * browse if debug
 * check if claimant in placeworker
 * Browse title
 * Job blacklist whitelist
 */

namespace ConPoDra
{
	public class CompProperties_ConditionalPostDraw : CompProperties
	{
        public List<PostDrawTask> postDraw;

        public int workerReservationUpdateFrequency = 60;

        public bool debug = false;

        public CompProperties_ConditionalPostDraw()
		{
			compClass = typeof(CompConditionalPostDraw);
		}
	}

    public class PostDrawTask
    {
        public string label;

        public List<ThingDef> materialPool;

        public Conditions condition;

        public bool allowMaterialBrowse = false;
        public bool allowMaterialBrowseIfDevMode = false;

        public float scale = 1f;
        public bool vanillaPulse = false;
    }

    public class Conditions
    {
        public bool ifFueled = false;
        public bool ifPowered = false;

        public bool ifReserved = false;
        public bool ifNotReserved = false;

        public bool ifClaimantOnInteractionCell = false;
        public bool ifSelected = false;

        public bool noCondition = false;
    }
}