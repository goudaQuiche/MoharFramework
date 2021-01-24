using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoharHediffs
{
    public class HediffCompProperties_TrailLeaver : HediffCompProperties
    {
        public int period = 20;
        public List<ThingDef> motePool;
        public FloatRange scale = new FloatRange(.5f, .8f);

        public bool allowedInWater = false;
        public FloatRange allowedWithSnowDepth = new FloatRange(0, 0.4f);

        public List<BodyTypeOffset> offSetPerBodyType;

        public Vector3 defaultOffset = new Vector3(0, 0, -.32f);

        public bool debug = false;

        public bool HasMotePool => !motePool.NullOrEmpty();
        public bool HasOffset => !offSetPerBodyType.NullOrEmpty();
        public HediffCompProperties_TrailLeaver()
        {
            compClass = typeof(HediffComp_TrailLeaver);
        }
    }

    public class BodyTypeOffset
    {
        public BodyTypeDef bodyType;
        public Vector3 offset;
    }

    public class ColorRange
    {
        public Color minColor;
        public Color maxColor;

        public float variationPerIteration;
    }
}