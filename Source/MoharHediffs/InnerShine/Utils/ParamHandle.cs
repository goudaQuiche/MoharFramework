using UnityEngine;
using System.Linq;
using RimWorld;
using Verse;

namespace MoharHediffs
{
    public static class ParametersHandlingsUtils
    {
        public static bool IsTerrainAllowed(this HediffComp_TrailLeaver comp, TerrainDef terrain)
        {
            if (terrain == null || comp.NullMap)
                return false;

            if (!comp.HasTerrainRestriction)
                return true;

            TerrainRestriction terrainRestriction = comp.TerrainRestriction;

            if (!terrainRestriction.allowedInWater && terrain.IsWater)
                return false;
            if (terrainRestriction.HasRelevantSnowRestriction && !terrainRestriction.allowedSnowDepth.Includes(comp.MyMap.snowGrid.GetDepth(comp.Pawn.Position)))
                return false;
            if (terrainRestriction.HasForbiddenTerrains && terrainRestriction.forbiddenTerrains.Contains(terrain))
                return false;

            return true;
        }

        public static bool IsPawnActivityCompatible(this HediffComp_TrailLeaver comp)
        {
            if (!comp.HasPawnRestriction)
                return true;

            Restriction pawnRestriction = comp.PawnRestriction;

            if (pawnRestriction.HasPostureRestriction && !pawnRestriction.allowedPostures.Contains(comp.Pawn.GetPosture()))
                return false;

            if (pawnRestriction.onlyWhenMoving && !comp.Pawn.pather.MovingNow)
                return false;

            return true;
        }

        public static Vector3 GetBodyTypeOffset(this HediffComp_TrailLeaver comp)
        {
            if (comp.Pawn.story?.bodyType == null || !comp.Props.HasOffset)
                return comp.Props.defaultOffset;

            BodyTypeOffset BTO = comp.Props.offSetPerBodyType.Where(b => b.bodyType == comp.Pawn.story.bodyType).FirstOrFallback();
            return BTO == null ? comp.Props.defaultOffset : BTO.offset;
        }
    }
}
