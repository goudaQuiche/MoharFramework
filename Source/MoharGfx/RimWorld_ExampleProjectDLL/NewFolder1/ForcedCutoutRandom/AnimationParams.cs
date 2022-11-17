using UnityEngine;
using Verse;
using System;

namespace MoharGfx
{
    public class AnimationParams
    {
        public int TicksPerFrame = 7;
        public int FrameOffset = 0;
        public IndexEngine.TickEngine Engine = IndexEngine.TickEngine.synced;
    }
}
