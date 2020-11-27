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
        public SoundMaterial soundMaterialPool = null;

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
        public bool HasSoundMaterialPool => soundMaterialPool != null && soundMaterialPool.AtleastOne;
        public bool HasStuffMaterialPool => !stuffMaterialPool.NullOrEmpty();
    }

    public class SoundMaterial
    {
        public SoundDef soundOnStart = null;
        public SoundDef soundOnStop = null;
        public SoundDef soundSustain = null;

        public bool HasStartSound => soundOnStart != null;
        public bool HasStopSound => soundOnStop != null;
        public bool HasSustainSound => soundSustain != null;

        public bool AtleastOne => HasStartSound || HasStopSound || HasSustainSound;
    }

    public class Conditions
    {
        public bool ifFueled = false;
        public bool ifPowered = false;

        public bool ifWorker = false;
        public bool ifNoWorker = false;

        public bool ifWorkerOnInteractionCell = false;
        public bool ifWorkerOnWatchArea = false;
        public bool ifWorkerTouch = false;
        public bool ifSelected = false;

        public List<JobDef> includeJob;
        public List<JobDef> excludeJob;
        public List<RecipeDef> includeRecipe;
        public List<RecipeDef> excludeRecipe;

        public bool noCondition = false;

        public bool HasIncludedJob => !includeJob.NullOrEmpty();
        public bool HasExcludedJob => !excludeJob.NullOrEmpty();

        public bool HasIncludedRecipe => !includeRecipe.NullOrEmpty();
        public bool HasExcludedRecipe => !excludeRecipe.NullOrEmpty();
    }

    public class StuffMaterialItem
    {
        public ThingDef material;
        public ThingDef stuff;
    }
}