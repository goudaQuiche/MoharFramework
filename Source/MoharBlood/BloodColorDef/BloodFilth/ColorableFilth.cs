using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AlienRace;
using System.Reflection;

namespace MoharBlood
{
    public class ColorableFilth : Filth
    {
        private Color pawnColor = Color.cyan;
        public override Color DrawColor
        {
            get => pawnColor;
            set => pawnColor = value;
        }

        public void Init(Color newColor) {
            DrawColor = newColor;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Color>(ref pawnColor, "pawnColor", Color.white);
        }
    }
}
