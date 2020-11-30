using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace MoharJoy
{
    public static class BubbleMoteMaker
    {
        private static IntVec3[] UpRightPattern = new IntVec3[4]
        {
            new IntVec3(0, 0, 0),
            new IntVec3(1, 0, 0),
            new IntVec3(0, 0, 1),
            new IntVec3(1, 0, 1)
        };

        private static MoteBubble ExistingMoteBubbleOn(Pawn pawn)
        {
            if (!pawn.Spawned)
            {
                return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (!(pawn.Position + UpRightPattern[i]).InBounds(pawn.Map))
                {
                    continue;
                }
                List<Thing> thingList = pawn.Position.GetThingList(pawn.Map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    MoteBubble moteBubble = thingList[j] as MoteBubble;
                    if (moteBubble != null && moteBubble.link1.Linked && moteBubble.link1.Target.HasThing && moteBubble.link1.Target == pawn)
                    {
                        return moteBubble;
                    }
                }
            }
            return null;
        }

        public static MoteBubble MakeMoodThoughtBubble(this Pawn pawn, Thought thought, Texture2D icon, ThingDef bubble, List<ThingDef> DestroyingBubbles = null, List<ThingDef> ResistantBubbles = null)
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return null;
            }
            if (!pawn.Spawned)
            {
                return null;
            }
            MoteBubble moteBubble = ExistingMoteBubbleOn(pawn);

            if (moteBubble != null)
            {
                if (!ResistantBubbles.NullOrEmpty() && ResistantBubbles.Contains(moteBubble.def))
                    return null;

                if (!DestroyingBubbles.NullOrEmpty() && DestroyingBubbles.Contains(bubble))
                    moteBubble.Destroy();
                else
                    return null;
            }
            MoteBubble obj = (MoteBubble)ThingMaker.MakeThing(bubble);

            //obj.SetupMoteBubble(thought.Icon, null);
            obj.SetupMoteBubble(icon, null);
            obj.Attach(pawn);
            GenSpawn.Spawn(obj, pawn.Position, pawn.Map);
            return obj;
        }
    }
}
