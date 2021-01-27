using System;
using UnityEngine;

using Verse;

namespace MoharHediffs
{
    public static class MoteColorChangeUtils
    {
        public static int GetProgressSign(float limA, float limB, float val)
        {
            if (val <= limA && limA < limB)
                return 1;
            else if (val >= limB && limB > limA)
                return -1;

            return Rand.Chance(.5f) ? 1 : -1;
        }

        public static Color RandomPickColor(this ColorRange colorRange, Color oldColor, bool debug = false)
        {
            //float iterationDice = Rand.Range(0, colorRange.variationPerIteration);
            float iterationDice = colorRange.variationPerIteration;
            float rW = Rand.Range(0, iterationDice);
            float gW = Rand.Range(0, iterationDice - rW);
            float bW = iterationDice - rW - gW;

            int rM = GetProgressSign(colorRange.colorA.r, colorRange.colorB.r, oldColor.r);
            int bM = GetProgressSign(colorRange.colorA.g, colorRange.colorB.g, oldColor.g);
            int gM = GetProgressSign(colorRange.colorA.b, colorRange.colorB.b, oldColor.b);

            float rC = Math.Abs(colorRange.colorA.r - colorRange.colorB.r) * rW * rM;
            float gC = Math.Abs(colorRange.colorA.g - colorRange.colorB.g) * gW * gM;
            float bC = Math.Abs(colorRange.colorA.b - colorRange.colorB.b) * bW * bM;

            Color newColor =
                new Color(
                    (oldColor.r + rC).Clamp(colorRange.colorA.r, colorRange.colorB.r),
                    (oldColor.g + gC).Clamp(colorRange.colorA.g, colorRange.colorB.g),
                    (oldColor.b + bC).Clamp(colorRange.colorA.b, colorRange.colorB.b)
                );
            /*
            if (debug)
            {
                Log.Warning(
                    "iD:" + iterationDice +
                    " rW:" + rW.ToString("0.00") +" gW:" + gW.ToString("0.00") + " bW:" + bW.ToString("0.00") +
                    " rM:" + rM + " gM:" + gM + " bM:" + bM +
                    " rC:" + rC.ToString("0.00") + " gC:" + gC.ToString("0.00") + " bC:" + bC.ToString("0.00") +
                    " oldColor:"+ oldColor.ToString() + " newColor:" + newColor.ToString()
                    );
            }
            */
            return newColor;
        }

        

        public static void ChangeMoteColor(this HediffComp_TrailLeaver comp, Mote mote)
        {
            if (!comp.Props.HasColorRange || mote == null)
                return;

            if (comp.lastColor == Color.black)
                comp.lastColor = comp.Props.colorRange.colorA;

            comp.lastColor = comp.Props.colorRange.RandomPickColor(comp.lastColor, comp.MyDebug);

            mote.instanceColor = comp.lastColor;
        }
        
    }
}
