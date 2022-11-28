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
        //public Color pawnColor = Color.white;
        public Color pawnColor = Color.magenta;

        public override Color DrawColor {
            get
            {
                if (Stuff != null)
                {
                    return def.GetColorForStuff(Stuff);
                }
                if (def.graphicData != null)
                {
                    //return pawnColor;
                    return pawnColor != Color.white ? pawnColor : def.graphicData.color;
                }
                return Color.white;
            }

            set => base.DrawColor = value;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Color>(ref pawnColor, "pawnColor", Color.white);
        }
    }


}
