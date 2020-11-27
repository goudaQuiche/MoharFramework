using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace MoharGamez
{
    public static class ThrowThoughtUtils
    {
        public static float CalculateThrowDistance(this Pawn pawn, Vector3 targetBuildingCoordinates, Vector3 destinationCell, bool myDebug = false)
        {
            Vector3 buildingCenter = targetBuildingCoordinates;
            destinationCell.y = 0;
            float distance = Vector3.Distance(destinationCell, buildingCenter);

            Tools.Warn(
                pawn.LabelShort + "CalculateThrowDistance" +
                " buildingCenter: " + buildingCenter +
                " destinationCell: " + destinationCell +
                " distance: " + distance
                , myDebug
            );

            return distance;
        }

        public static void ComputeThrowQuality(this JobDriver_PlayGenericTargetingGame TG, float throwDistance, bool myDebug = false)
        {
            string myDebugStr = TG.PawnLabel + " - ComputeThrowQuality - ";
            if (!TG.HasGameSettings)
                return;

            foreach (ThoughtParameter TP in TG.gameSettings.thoughtList.thoughtOptionPerShot)
            {
                ThoughtDef TD = TP.thoughtPool.RandomElement();
                Tools.Warn(myDebugStr + " browsing >" + TD.defName + " - " + TP.distanceThreshold, myDebug);
                if (TP.distanceThreshold.Includes(throwDistance))
                {
                    Tools.Warn(myDebugStr + ">Throw is in range : " + TP.distanceThreshold, myDebug);
                    float TriggerChange = TP.triggerChance + TG.OtherPlayersNum * TP.rivalryChancePerOpponent;

                    if (!Rand.Chance(TriggerChange))
                    {
                        Tools.Warn(myDebugStr + "> RNG gods do no want: " + TP.triggerChance, myDebug);
                        return;
                    }

                    Tools.Warn(myDebugStr + "> Trying to apply thought: ", myDebug);

                    TryApplySoloThought(
                        TG.pawn, TP.thoughtPool.RandomElement(),
                        ContentFinder<Texture2D>.Get(TP.iconPool.RandomElement()),
                        TP.bubblePool.RandomElement(),
                        TG.DestroyingMotes,
                        TG.ResistantMotes
                    );

                    return;
                }
            }
        }

        public static bool TryApplySoloThought(Pawn pawn, ThoughtDef thoughtDef, Texture2D icon, ThingDef bubble, List<ThingDef> DestroyingBubbles = null, List<ThingDef> ResistantBubbles = null)
        {
            // victim shame
            Thought_Memory newthought = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef);
            if (newthought == null)
            {
                Tools.Warn("victim thought null", true);
                return false;
            }

            pawn.needs.mood.thoughts.memories.TryGainMemory(newthought);
            pawn.MakeMoodThoughtBubble(newthought, icon, bubble, DestroyingBubbles, ResistantBubbles);

            return true;
        }
    }
}
