using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoharHediffs
{
    public class HediffCompProperties_InnerShine : HediffCompProperties
    {
        public List<InnerShineItem> innerShineList;

        public bool debug => innerShineList.Any(i => i.debug);

        public HediffCompProperties_InnerShine()
        {
            compClass = typeof(HediffComp_TrailLeaver);
        }
    }
    public class InnerShineItem
    {
        public string label;
        public IntRange period = new IntRange(15, 25);

        public List<ThingDef> motePool;
        public List<BodyTypeSpecificities> bodyTypeSpecs;

        public Vector3 defaultOffset = new Vector3(0, 0, -.32f);
        public FloatRange defaultRandomScale = new FloatRange(.5f, .8f);
        public Vector2 defaultDrawSize = new Vector2(1, 1);

        public ActivityRestriction activity;

        public bool debug = false;

        public bool HasMotePool => !motePool.NullOrEmpty();
        public bool HasOffset => !bodyTypeSpecs.NullOrEmpty();
    }

    public class BodyTypeSpecificities
    {
        public BodyTypeDef bodyTypeDef;

        public FloatRange randomScale = new FloatRange(.5f, .8f);
        public Vector2 drawSize;
        public Vector3 offset;
    }

    public class ActivityRestriction
    {
        public bool onlyWhenMoving = true;
        public List<PawnPosture> allowedPostures;
        public List<JobDef> allowedJobs;
        
        public bool HasPostureRestriction => !allowedPostures.NullOrEmpty();
        public bool HasJobRestriction => !allowedJobs.NullOrEmpty();
    }

}