﻿using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharGamez
{
    //[StaticConstructorOnStartup]

    public class GameProjectileClass : Thing
    {
        public GameProjectileDef Def
        {
            get
            {
                return def as GameProjectileDef;
            }
        }
    }

    public class GameProjectileDef : ThingDef
    {
        public List<GameProjectile> gameProjectileList;
    }
    
    public class GameProjectile
    {
        public Type driverClass;
        public JobDef jobDef;

        public SkillDef skillDefScaling;
        public IntRange throwInterval;

        public List<ProjectileOption> projectileOptionList;
        public bool debug = false;
    }

    public class ProjectileOption
    {
        public MoteParameter mote = null;
        public MoteParameter shadowMote = null;

        public ThoughtParameter thought = null;

        public float weight;

        public bool IsMoteType => mote != null;
        public bool IsShadowMoteType => shadowMote != null;

        public bool HasThought => thought != null;
    }

    public class ThoughtParameter
    {
        public PlayPool goodPlayThoughtPool;
        public PlayPool badPlayThoughtPool;

        public List<string> goodPlayIconPool;
        public List<string> badPlayIconPool;
    }
    public class PlayPool
    {
        public float triggerChance;
        public float rivalryAdditionnalChance;
        public float distanceThreshold;

        public List<ThoughtDef> thoughtList;
    }

    public class MoteParameter
    {
        public ThingDef moteDef;
        public FloatRange speed;
        public FloatRange rotation;
    }

    //
    //public class MoteRecord
    

}
