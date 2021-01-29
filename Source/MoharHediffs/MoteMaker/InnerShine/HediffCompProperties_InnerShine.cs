using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoharHediffs
{
    public class HediffCompProperties_InnerShine : HediffCompProperties
    {
        public List<InnerShineItem> innerShinePool;

        public bool HasShinePool => !innerShinePool.NullOrEmpty();

        public bool debug;
        // => innerShineList.Any(i => i.debug);

        public HediffCompProperties_InnerShine()
        {
            compClass = typeof(HediffComp_InnerShine);
        }
    }
    public class InnerShineItem
    {
        public string label;

        public SpawnRules spawningRules;

        public List<ThingDef> motePool;
        public MoteLink.Nature linkType;

        public List<BodyTypeSpecificities> bodyTypeDrawRules;
        public DrawingSpecificities defaultDrawRules;

        public ActivityRestriction restriction;

        public ColorRange colorRange;
        
        public bool debug = false;

        public bool HasColorRange => colorRange != null;
        public bool HasRestriction => restriction != null;
        public bool HasMotePool => !motePool.NullOrEmpty();
        public bool HasBodyTypeDrawRules => !bodyTypeDrawRules.NullOrEmpty();
        public bool HasDefaultDrawRules => defaultDrawRules != null;

        public string Dump()
        {
            return 
                $"label:{label} HasColorRange:{HasColorRange} HasRestriction:{HasRestriction}"+
                $" HasMotePool:{HasMotePool} HasBodyTypeDrawRules:{HasBodyTypeDrawRules} HasDefaultDrawRules:{HasDefaultDrawRules}"+
                $" debug:{debug}";
        }
    }

    public class SpawnRules
    {
        public int spawnedMax = 2;
        public IntRange period = new IntRange(15, 25);
        public bool IsUnlimited => spawnedMax <= 0;
    }

    public class BodyTypeSpecificities
    {
        public BodyTypeDef bodyTypeDef;
        public DrawingSpecificities drawRules;
    }

    public class DrawingSpecificities
    {
        public FloatRange randomScale = new FloatRange(.5f, .8f);
        //public Vector2 drawSize;
        public Vector3 offset;
    }

    public class ActivityRestriction
    {
        public bool onlyWhenMoving = true;
        public List<PawnPosture> allowedPostures;
        public List<JobDef> allowedJobs;
        public List<Rot4> allowedRotation;

        public bool HasPostureRestriction => !allowedPostures.NullOrEmpty();
        public bool HasJobRestriction => !allowedJobs.NullOrEmpty();
        public bool HasAllowedRotation => !allowedRotation.NullOrEmpty();
    }


}