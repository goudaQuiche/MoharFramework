using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;


namespace MoharGamez
{
    public static class TogetherThoughtUtils
    {
        public static bool IsSomeonePlayingWithMe(this JobDriver_PlayGenericTargetingGame TG, out int playerNum, bool debug = false)
        {
            string myDebugStr = debug? TG.PawnLabel + " - IsSomeonePlayingWithMe - " : "";
            playerNum = 0;

            Tools.Warn(myDebugStr + " - Entering", debug);

            if (!TG.HasPlayingTogetherThoughts || !TG.HasWatchCells)
            {
                Tools.Warn(myDebugStr + " - Found no together thought, or no watch cells", debug);
                return false;
            }

            IEnumerable<Pawn> foundPawns = TG.pawn.Map.reservationManager.ReservationsReadOnly.Where(
                r =>
                r.Claimant != TG.pawn &&
                r.Job.def == TG.pawn.CurJobDef &&
                TG.WatchCells.Contains(r.Claimant.Position) &&
                r.Claimant.CurJob != null && r.Claimant.CurJob.targetA == TG.JoyBuilding
            ).Select(r => r.Claimant).Distinct();

            if (foundPawns.EnumerableNullOrEmpty())
                return false;
            playerNum = foundPawns.Count();
            Tools.Warn(myDebugStr + " - found " + playerNum + " other players", debug);
            if (debug)
                for (int i = 0; i < foundPawns.Count(); i++)
                    Tools.Warn(myDebugStr + i + " - " + foundPawns.ElementAt(i).LabelShort, debug);

            if (!TG.SingleTogetherThought.IsUnlimitedThought && TG.TogetherThoughtsNum >= TG.SingleTogetherThought.numberOfThoughtsPerJob)
                return false;

            if (!TG.pawn.PlayedLongEnough(TG.PlayedTogetherThreshold))
            {
                Tools.Warn(myDebugStr + " - did not play long enough : " + TG.PlayedTogetherThreshold, debug);
                return false;
            }

            foreach (Pawn player in foundPawns.InRandomOrder())
            {
                Tools.Warn(myDebugStr + " - browsing : " + player.LabelShort, debug);

                if (player.PlayedLongEnough(TG.PlayedTogetherThreshold))
                {
                    float baseChance = TG.TogetherThoughtChance;
                    float finalChance = Math.Max(TG.MinThoughtChance, AgregatedOpinion(TG.pawn, player) * baseChance);
                        
                    if (Rand.Chance(finalChance))
                    {
                        Tools.Warn(myDebugStr + " - RNG agreed(" + TG.TogetherThoughtChance + "), trying to apply " + TG.PlayedTogetherThought.defName, debug);

                        TryApplyTogetherThought(TG.pawn, player, TG.PlayedTogetherThought, TG.PlayedTogetherIcon, TG.PlayedTogetherBubble, TG.DestroyingMotes, TG.ResistantMotes, debug);
                        TG.TogetherThoughtsNum++;

                        JobDriver_PlayGenericTargetingGame OtherPlayerTG = (JobDriver_PlayGenericTargetingGame)(player.jobs.curDriver);
                        if(OtherPlayerTG!=null)
                            OtherPlayerTG.TogetherThoughtsNum++;

                        return true;
                    } else
                        Tools.Warn(myDebugStr + " - RNG did not agree " + TG.TogetherThoughtChance, debug);
                }
                else
                    Tools.Warn(myDebugStr + " - opponent did not play long enough : " + TG.PlayedTogetherThreshold, debug);
            }

            Tools.Warn(myDebugStr + " - found nothing to do", debug);

            return false;
        }

        public static float AgregatedOpinion(Pawn p1, Pawn p2)
        {
            return (p1.relations.OpinionOf(p2) + p2.relations.OpinionOf(p1)) / 125f;
        }

        public static void TryApplyTogetherThought(Pawn player1, Pawn player2, ThoughtDef thoughtDef, Texture2D icon, ThingDef bubble, List<ThingDef> DestroyingBubbles = null, List<ThingDef> ResistantBubbles = null, bool debug = false)
        {
            string myDebugStr = debug ? " - TryApplyTogetherThought " + player1.LabelShort + "-" + player2.LabelShort : "";

            Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef);

            if (thought_Memory == null)
            {
                Tools.Warn("thought_Memory null", true);
                return;
            }

            player1.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, player2);
            //player2.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, player1);

            Pawn motePawn = Rand.Chance(.5f) ? player1 : player2;
            motePawn.MakeMoodThoughtBubble(thought_Memory, icon, bubble, DestroyingBubbles, ResistantBubbles);
            
        }

        public static bool PlayedLongEnough(this Pawn p, int threshold)
        {
            return Find.TickManager.TicksGame - p.CurJob.startTick > threshold;
        }
    }
}
