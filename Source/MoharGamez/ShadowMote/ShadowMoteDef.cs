﻿using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharGamez
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
        public ImpactMoteParameter impactMote;

        public bool HasImpactMote => impactMote != null;
        public bool HasThrowSound => throwSound != null;
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