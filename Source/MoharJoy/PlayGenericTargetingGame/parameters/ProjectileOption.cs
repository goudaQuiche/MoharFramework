﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace MoharJoy
{
    public class ProjectileOption
    {
        public string label;

        public MoteParameter mote = null;
        public MoteParameter shadowMote = null;

        public float weight;

        public bool IsMoteType => mote != null;
        public bool IsShadowMoteType => shadowMote != null;
    }

    public class MoteParameter
    {
        public ThingDef moteDef;
        public List<ThingDef> stuffMotePool;
        public FloatRange speed;
        public FloatRange rotation;

        public bool HasRegularMoteDef => moteDef != null;
        public bool HasStuffMotePool => !stuffMotePool.NullOrEmpty();
    }

}
