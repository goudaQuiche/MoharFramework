using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharJoy
{
    //[StaticConstructorOnStartup]

    public class ShadowMoteDef : ThingDef
    {
        public MoteSubEffect moteSubEffect;
        //public bool HasMSE => moteSubEffect != null;
    }

    public class MoteSubEffect
    {
        public ThingDef flyingShadowRessource;
        public SoundDef throwSound;
        public SoundDef skiddingSustainSound;

        public SoundDef groundLandSound;
        public SoundDef waterLandSound;

        public ImpactMoteParameter impactMote;

        public bool makeWaterSplashOnImpact = false;
        public bool destroyParentOnImpact = false;

        public bool HasImpactMote => impactMote != null;

        public bool HasThrowSound => throwSound != null;
        public bool HasGroundLandSound => groundLandSound != null;
        public bool HasWaterLandSound => waterLandSound != null;
        public bool HasSkiddingSound => skiddingSustainSound != null;

        public bool HasflyingShadowRessource => flyingShadowRessource != null;

        public bool debug = false;
    }

    public class ImpactMoteParameter
    {
        public ThingDef moteDef;
        public FloatRange speed;
        public FloatRange angle;
        public FloatRange scale;
        public FloatRange rotationRate;
    }

}
