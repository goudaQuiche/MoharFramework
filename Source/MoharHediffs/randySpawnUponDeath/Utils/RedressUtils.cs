using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using UnityEngine;

namespace MoharHediffs
{
    public static class PawnRedressUtils
    {
        public static void DestroyInventory(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.redressNewPawn.destroyInventory)
                newPawn.inventory.innerContainer.ClearAndDestroyContents();
        }

        public static void DestroyEquipment(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.redressNewPawn.destroyEquipment)
                newPawn.equipment.DestroyAllEquipment();
        }
        public static void DestroyApparel(this HediffComp_RandySpawnUponDeath comp, Pawn newPawn)
        {
            if (comp.ChosenItem.redressNewPawn.destroyApparel)
                newPawn.apparel.DestroyAll();
        }

        public static bool StripCorpse(this HediffComp_RandySpawnUponDeath comp, Corpse corpse)
        {
            bool DidIt = false;
            if (comp.Props.HasParentRedress && comp.Props.redressParent.strip && !corpse.Negligeable())
            {
                DidIt = true;
                if (corpse.AnythingToStrip())
                {
                    corpse.Strip();
                }
            }

            return DidIt;
        }

        public static bool DestroyCorpse(this HediffComp_RandySpawnUponDeath comp, Corpse corpse)
        {
            bool DidIt = false;
            if (comp.Props.HasParentRedress && comp.Props.redressParent.destroyCorpse && !corpse.Negligeable())
            {
                DidIt = true;
                corpse.DeSpawn();
            }

            return DidIt;
        }

    }
}