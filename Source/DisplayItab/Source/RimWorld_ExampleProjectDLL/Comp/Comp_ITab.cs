using UnityEngine;
using RimWorld;
using Verse.Sound;
using Verse;
using Verse.AI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DisplayITab
{
    public class Comp_ITab : ThingComp
    {
        public CompProperties_ITab Props => (CompProperties_ITab)props;

        public int titleIndex = -1;
        public int index = -1;

        public bool IsOnTitle => index == titleIndex;

        public bool IsProcessedTex => Props.displayWay == DisplayWay.ProcessedTexture;
        public bool IsRawTex => Props.displayWay == DisplayWay.RawTexture;
        public bool HasNoPage => Props.Pages.NullOrEmpty();

        public void ResetIndex()
        {
            index = 0;
        }
        public void NextIndex()
        {
            index += 1;
            if (index >= Props.Pages.Count)
                index = titleIndex;
        }
        public void PreviousIndex()
        {
            index -= 1;
            if (index < titleIndex)
                index = Props.Pages.Count-1;
        }
    }
}