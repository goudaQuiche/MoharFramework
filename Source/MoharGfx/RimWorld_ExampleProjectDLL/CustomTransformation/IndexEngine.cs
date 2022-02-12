using UnityEngine;
using Verse;
using System;

namespace MoharGfx
{
    public class IndexEngine
    {
        public enum TickEngine
        {
            [Description("synced")]
            synced = 0,
            [Description("moteLifespan")]
            moteLifespan = 1,
            [Description("relativeMoteLifespan")]
            relativeMoteLifespan = 2,
        }
    }
}
