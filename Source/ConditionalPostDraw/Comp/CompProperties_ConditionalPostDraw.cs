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
        public string label = "browse materials";

        public List<ThingDef> materialPool;
        public List<StuffMaterialItem> stuffMaterialPool;

        public Conditions condition;

        public bool allowMaterialBrowse = false;
        public bool allowMaterialBrowseIfDevMode = false;

        public Vector3 offset;

        public Vector2 scale = Vector2.one;
        public bool tickDrivenScale = false;
        public FloatRange xScaleRange = new FloatRange(1, 1);
        public FloatRange yScaleRange = new FloatRange(1, 1);

        public bool tickDrivenRotation = false;
        public float rotationSpeed = 1f;

        public bool vanillaPulse = false;

        public bool HasRegularMaterialPool => !materialPool.NullOrEmpty();
        public bool HasStuffMaterialPool => !stuffMaterialPool.NullOrEmpty();
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

    public class StuffMaterialItem
    {
        public ThingDef material;
        public ThingDef stuff;
    }
}