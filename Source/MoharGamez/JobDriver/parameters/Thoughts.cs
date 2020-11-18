using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace MoharGamez
{
    public class GameThoughts
    {
        public List<ThoughtParameter> thoughtOptionPerShot;
        public List<ThoughtParameter> thoughtOptionPlayingTogether;
        
    }

    public class ThoughtParameter
    {
        public List<ThoughtDef> thoughtPool;
        public List<string> iconPool;
        public List<ThingDef> bubblePool;

        public float triggerChance;
        public float rivalryChancePerOpponent;

        public FloatRange distanceThreshold;

        public bool onlyOneThoughtPerJob = false;

        //private Texture2D iconInt;
    }

}
