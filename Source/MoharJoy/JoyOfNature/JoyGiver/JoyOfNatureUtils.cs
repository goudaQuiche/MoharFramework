using Verse;
using Verse.AI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoharJoy
{
    public static class JoyOfNatureUtils
    {
        //generating cells surrounding another centerCell
        // centerCell should be the water one
        // we want cells where a pawn can stand on, aka affordance
        public static IEnumerable<IntVec3> GetSurroundingCells(
            this IntVec3 centerCell, Map map, int distance,
            bool avoidAdjacentCells = false, bool requiresLineOfSight = false, List<TerrainAffordanceDef> TAD = null)
        {
            bool HasTerrainAffordanceRequirements = !TAD.NullOrEmpty();

            List<IntVec3> answer = new List<IntVec3>();

            for (int i = -distance; i <= distance; i++)
                for (int j = -distance; j <= distance; j++)
                {
                    // origin cell
                    if (i == 0 && j == 0)
                        continue;

                    // adjacent cell
                    if (avoidAdjacentCells && (Math.Abs(i) == 1 || Math.Abs(j) == 1))
                        continue;

                    IntVec3 checkMe = new IntVec3(centerCell.x + i, 0, centerCell.z + j);

                    // new cell not on the map
                    if (!checkMe.InBounds(map))
                        continue;

                    // no intersection between requirements and def
                    if (HasTerrainAffordanceRequirements && checkMe.GetTerrain(map).affordances.Intersect(TAD).EnumerableNullOrEmpty())
                        continue;

                    // line of sight
                    if (requiresLineOfSight && !GenSight.LineOfSight(centerCell, checkMe, map))
                            continue;

                    answer.Add(checkMe);
                }

            return answer;
        }

        public static IEnumerable<IntVec3> GetSurroundingCells(this IntVec3 centerCell, Map map, JoyOfNature_JoyGiverDef JG)
        {
            bool debug = JG.debug;
            string debugStr = debug ? centerCell.ToString() + " GetSurroundingCells - " : "";

            bool HasTerrainAffordanceRequirements = !JG.terrainCondition.pawnCellAffordance.NullOrEmpty();
            List<TerrainAffordanceDef> TAD = JG.terrainCondition.pawnCellAffordance;
            int distance = JG.defaultTargetAndPawnCellsDistance;
            bool avoidAdjacentCells = JG.avoidTargetAndPawnCellsBeingAdjacent;
            bool requiresLineOfSight = JG.requiresLineOfSight;

            List<IntVec3> answer = new List<IntVec3>();

            for (int i = -distance; i <= distance; i++)
                for (int j = -distance; j <= distance; j++)
                {
                    // origin cell
                    if (i == 0 && j == 0)
                        continue;

                    // adjacent cell
                    if (avoidAdjacentCells && (Math.Abs(i) == 1 || Math.Abs(j) == 1))
                        continue;

                    IntVec3 checkMe = new IntVec3(centerCell.x + i, 0, centerCell.z + j);

                    // new cell not on the map
                    if (!checkMe.InBounds(map))
                        continue;

                    // no intersection between requirements and def
                    if (HasTerrainAffordanceRequirements && checkMe.GetTerrain(map).affordances.Intersect(TAD).EnumerableNullOrEmpty())
                        continue;

                    // line of sight
                    if (requiresLineOfSight && !GenSight.LineOfSight(centerCell, checkMe, map))
                        continue;

                    Tools.Warn(debugStr + "is valid, adding", debug);
                    answer.Add(checkMe);
                }

            return answer;
        }

        // input water; want to avoid water cells surrounded by water cells
        // purpose: find water cells with ground next to them
        public static bool IsCellDirectlySurroundedBy(this IntVec3 centerCell, Map map, List<TerrainDef> avoidedTerrains, bool debug = false)
        {
            string debugStr = debug ? centerCell.ToString() + " IsCellDirectlySurroundedBy - " : "";

            Tools.Warn(debugStr + "Entering ", debug);

            //List<IntVec3> answer = new List<IntVec3>();
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    // origin cell
                    if (i == 0 && j == 0)
                        continue;

                    IntVec3 checkMe = new IntVec3(centerCell.x + i, 0, centerCell.z + j);

                    // cell not on the map
                    if (!checkMe.InBounds(map))
                        continue;

                    // no intersection between requirements and def
                    if (!avoidedTerrains.Contains(checkMe.GetTerrain(map)))
                    {
                        Tools.Warn(centerCell.ToString() + " not surrounded, exiting", debug);
                        return false;
                    }
                        
                }

            // if we have reached this far, it means we found no cell 
            Tools.Warn(centerCell.ToString() + " is surrounded, exiting", debug);
            return true;
        }

        static bool aze(this IntVec3 c, Map map)
        {
            return true;
        }

        public static bool TryFindPawnAndTargetCells(this JoyGiver_JoyOfNature JG, Pawn searcher, out IntVec3 pawnCell, out IntVec3 targetCell)
        {
            bool debug = JG.Def.debug;
            string debugStr = debug ? searcher.LabelShort + " TryFindPawnAndTargetCells - " : "";

            IntVec3 root = searcher.Position;
            Map map = searcher.Map;

            targetCell = root;
            pawnCell = root;

            IEnumerable<IntVec3> potentialPawnCells = new List<IntVec3>();

            bool TargetCellValidator(IntVec3 c) => 
                !c.Roofed(map) &&
                JG.Def.terrainCondition.targetCellNature.Contains(c.GetTerrain(map)) &&
                //(!c.IsCellDirectlySurroundedBy(map, JG.Def.terrainCondition.targetCellNature, debug)) &&
                !c.IsCellDirectlySurroundedBy(map, JG.Def.terrainCondition.targetCellNature) &&
                c.GetSurroundingCells(map, JG.Def).Any(
                        pc =>
                            map.reachability.CanReach(root, pc, PathEndMode.OnCell, TraverseMode.NoPassClosedDoors, Danger.None) &&
                            !pc.Roofed(map) && !pc.GetTerrain(searcher.Map).avoidWander
                        )       
                ;

            // cells with water in them, not surrounded by water


            Region pawnRegion = root.GetRegion(searcher.Map);
            TraverseParms traverseParms = TraverseParms.For(searcher);

            if (!CellFinder.TryFindClosestRegionWith(pawnRegion, traverseParms, targetCellRegionValidator, 300, out Region targetCellRegion))
            {
                Tools.Warn(debugStr + "TryFindClosestRegionWith target Cell KO", debug);
                return false;
            }else
                Tools.Warn(debugStr + "TryFindClosestRegionWith target Cell ok", debug);

            //IntVec3 result3;
            bool targetCellRegionValidator(Region r) =>
                r.Room.PsychologicallyOutdoors && !r.IsForbiddenEntirely(searcher) &&
                r.TryFindRandomCellInRegionUnforbidden(searcher, TargetCellValidator, out _);

            bool FoundTargetRegion = CellFinder.RandomRegionNear(targetCellRegion, 14, traverseParms, targetCellRegionValidator, searcher).
            TryFindRandomCellInRegionUnforbidden(searcher, TargetCellValidator, out targetCell);
            if (!FoundTargetRegion)
            {
                Tools.Warn(debugStr + "FoundTargetRegion KO", debug);
                return false;
            } else
                Tools.Warn(debugStr + "FoundTargetRegion ok, cell:" + targetCell.ToString(), debug);

            potentialPawnCells = targetCell.GetSurroundingCells(map, JG.Def).Where(
                pc => map.reachability.CanReach(root, pc, PathEndMode.OnCell, TraverseMode.NoPassClosedDoors, Danger.None)
            );

            if (potentialPawnCells.EnumerableNullOrEmpty())
            {
                Tools.Warn(debugStr + "empty potentialPawnCells KO", debug);
                return false;
            } else
                Tools.Warn(debugStr + "found potentialPawnCells ok" + potentialPawnCells.Count(), debug);

            bool pawnCellValidator(IntVec3 pc) =>
                 !pc.Roofed(searcher.Map) && !pc.GetTerrain(searcher.Map).avoidWander &&
                 potentialPawnCells.Contains(pc);
            bool pawnCellRegionValidator(Region r) =>
                r.Room.PsychologicallyOutdoors && !r.IsForbiddenEntirely(searcher) &&
                r.TryFindRandomCellInRegionUnforbidden(searcher, pawnCellValidator, out _);

            if (!CellFinder.TryFindClosestRegionWith(pawnRegion, traverseParms, pawnCellRegionValidator, 300, out Region pawnCellRegion))
            {
                Tools.Warn(debugStr + "TryFindClosestRegionWith pawn Cell KO", debug);
                return false;
            }
            else
                Tools.Warn(debugStr + "TryFindClosestRegionWith pawn Cell ok", debug);

            bool FoundPawnRegion = CellFinder.RandomRegionNear(pawnCellRegion, 14, traverseParms, pawnCellRegionValidator, searcher).
            TryFindRandomCellInRegionUnforbidden(searcher, pawnCellValidator, out pawnCell);
            if (!FoundPawnRegion)
            {
                Tools.Warn(debugStr + "FoundPawnRegion KO", debug);
                return false;
            }else
                Tools.Warn(debugStr + "FoundPawnRegion ok, cell:" + pawnCell.ToString(), debug);

            return FoundTargetRegion && FoundPawnRegion;
        }
    }
}