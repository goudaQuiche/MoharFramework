using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;
using UnityEngine;


namespace MoharHediffs
{
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


        public bool HasSpawningRules => spawningRules != null;

        public bool HasRestriction => restriction != null;
        public bool HasMotePool => !motePool.NullOrEmpty();

        public bool HasBodyTypeDrawRules => !bodyTypeDrawRules.NullOrEmpty();
        public bool HasDefaultDrawRules => defaultDrawRules != null;

        public bool HasColorRange => colorRange != null;

        public string Dump()
        {
            return
                $"label:{label}"+
                $" HasSpawningRules:{HasSpawningRules} HasRestriction:{HasRestriction}" +
                $" HasMotePool:{HasMotePool} HasBodyTypeDrawRules:{HasBodyTypeDrawRules} HasDefaultDrawRules:{HasDefaultDrawRules}" +
                $" HasColorRange:{HasColorRange}" +
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
        public List<RotationOffset> rotationOffset;

        public Vector3 GetRotationOffset(Pawn p)
        {
            if (rotationOffset.Where(ro => ro.rot == p.Rotation).FirstOrFallback() is RotationOffset answer)
                return answer.offset;

            //Log.Warning("rot: " + p.Rotation);

            return Vector3.zero;
        }
    }

    public class RotationOffset
    {
        public Rot4 rot;
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
