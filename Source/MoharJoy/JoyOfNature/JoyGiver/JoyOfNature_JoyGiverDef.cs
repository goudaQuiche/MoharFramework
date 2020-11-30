using Verse;
using System;
using RimWorld;
using System.Collections.Generic;

namespace MoharJoy
{
    //[StaticConstructorOnStartup]

    public class JoyOfNature_JoyGiverDef : JoyGiverDef
    {
        public TerrainCondition terrainCondition;
        public bool requiresLineOfSight = true;
        public bool avoidTargetAndPawnCellsBeingAdjacent = true;
        public int defaultTargetAndPawnCellsDistance = 3;

        public bool debug = false;
    }

    public class TerrainCondition
    {
        public List<TerrainDef> targetCellNature;
        public List<TerrainAffordanceDef> pawnCellAffordance;

        /*
        public float occupiedCellMaxDistance;
        public FloatRange occupiedToTargetCellDistance;
        public int maxTicksSpentToReach;
        */
    }

}
