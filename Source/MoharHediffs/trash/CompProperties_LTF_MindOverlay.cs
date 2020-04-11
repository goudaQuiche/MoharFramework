using System;
using UnityEngine;
using Verse;

namespace NewHatcher
{
    public class CompProperties_LTF_MindOverlay : CompProperties
    {
        public bool flickable = false;
        public CompProperties_LTF_MindOverlay()
        {
            this.compClass = typeof(Comp_LTF_MindOverlay);
        }

    }
}