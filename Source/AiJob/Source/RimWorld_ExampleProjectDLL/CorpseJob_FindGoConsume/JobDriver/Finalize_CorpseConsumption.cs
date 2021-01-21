using Verse;
using UnityEngine;
using Verse.AI;
using RimWorld;
using System.Linq;

namespace MoharAiJob
{
    public static class Finalize_CorpseConsumption
    {
        public static Toil SpawnProductDespawnCorpse(this CorpseProduct CP, IntVec3 SpawnPos, Corpse corpse, Map map, bool MyDebug = false)
        {
            return new Toil {
                initAction = delegate
                {
                    CP.SpawnConsumptionProduct(SpawnPos, map, MyDebug);
                    corpse.DestroyCorpse();
                },
                atomicWithPrevious = true
            };
        }

        public static void DestroyCorpse(this Corpse corpse)
        {
            Log.Warning("DestroyCorpse");
            corpse.Destroy(DestroyMode.KillFinalize);
        }
        public static void SpawnConsumptionProduct(this CorpseProduct CP, IntVec3 SpawnPos, Map map, bool MyDebug = false)
        {
            Log.Warning("SpawnConsumptionProduct");
            //PawnKindDef PKD = CP.pawnKind.RandomElementByWeight()
            if (CP == null)
            {
                Log.Warning("No CP found");
            }
            PawnGenOption PGO = CP.pawnKind.RandomElementWithFallback(null);
            Log.Warning("PGO:" + ((PGO == null)? "null":"Ok"));

            PawnKindDef PKD = PGO.kind;

            Faction RandomFaction = CP.forcedFaction?.GetFaction();

            Log.Warning("PKD:" + PKD.label + " faction:" + RandomFaction);

            PawnGenerationRequest request =
                new PawnGenerationRequest(
                    kind: PKD, faction: RandomFaction, context: PawnGenerationContext.NonPlayer, tile: -1, forceGenerateNewPawn: false,
                    newborn: false, colonistRelationChanceFactor: 0, allowAddictions: false, allowFood: false, relationWithExtraPawnChanceFactor: 0
                );
            Pawn NewPawn = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(NewPawn, SpawnPos, map, WipeMode.Vanish);
        }

        public static Faction GetFaction(this FactionDef factionDef)
        {
            if (factionDef == null)
                return null;
            return Find.FactionManager.AllFactions.Where(F => F.def == factionDef).FirstOrFallback();
        }
    }
}
