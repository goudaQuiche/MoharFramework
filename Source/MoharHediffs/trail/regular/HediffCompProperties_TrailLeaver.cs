using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoharHediffs
{
    public class HediffCompProperties_TrailLeaver : HediffCompProperties
    {
        public IntRange period = new IntRange(15,25);

        public List<ThingDef> motePool;

        public List<BodyTypeOffset> offSetPerBodyType;
        public Vector3 defaultOffset = new Vector3(0, 0, -.32f);

        public Restriction restriction;
        public Footprint footprint;
        public ColorRange colorRange;

        public float rotationOffset = 0;
        public bool dynamicRotation = true;
        public FloatRange randomScale = new FloatRange(.5f, .8f);
         
        public bool debug = false;

        public bool HasRestriction => restriction != null;
        public bool HasColorRange => colorRange != null;
        public bool UsesFootPrints => footprint != null;
        public bool HasMotePool => !motePool.NullOrEmpty();
        public bool HasOffset => !offSetPerBodyType.NullOrEmpty();
        public bool HasRotationOffset => rotationOffset != 0;
        
        public HediffCompProperties_TrailLeaver()
        {
            compClass = typeof(HediffComp_TrailLeaver);
        }
    }

    public class Footprint
    {
        public float intervalDistance = .632f;
        public Vector3 offset = new Vector3(0, 0, -.3f);
        public float distanceBetweenFeet = .17f;

        public string Dump()
        {
            return
                "intervalDistance:" + intervalDistance +
                "; offset:" + offset +
                "; distanceBetweenFeet:" + distanceBetweenFeet;
        }
    }

    public class Restriction
    {
        public bool onlyWhenMoving = true;
        public List<PawnPosture> allowedPostures;

        public TerrainRestriction terrain;

        public bool HasTerrainRestriction => terrain != null;
        public bool HasPostureRestriction => !allowedPostures.NullOrEmpty();
    }

    public class TerrainRestriction
    {
        public bool allowedInWater = false;
        public FloatRange allowedSnowDepth = new FloatRange(0, 0.4f);
        public List<TerrainDef> forbiddenTerrains;

        public bool HasForbiddenTerrains => !forbiddenTerrains.NullOrEmpty();
        public bool HasRelevantSnowRestriction => allowedSnowDepth.min != 0 && allowedSnowDepth.min != 1;
    }

    public class BodyTypeOffset
    {
        public BodyTypeDef bodyType;
        public Vector3 offset;
    }

    public class ColorRange
    {
        public Color colorA;
        public Color colorB;

        public float variationPerIteration;
    }
}