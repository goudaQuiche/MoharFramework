using RimWorld;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using AlienRace;
using UnityEngine;
using Verse;


namespace MoHarRegeneration
{
    public static class GfxEffects
    {

        // closed match in RGB space
        public static Color ClosestColor(Pawn pawn, bool complementary = false, bool myDebug = false)
        {

            AlienPartGenerator.AlienComp alienComp = Tools.GetAlien(pawn);

            Color pColor;
            
            if(alienComp == null)
                pColor =  pawn.DrawColor;
            else
            {
                pColor = alienComp.GetChannel("skin").first;
                Tools.Warn(pawn.LabelShort +" is alien, color="+pColor,  myDebug);
                
            }
                

            Color answer = Color.blue;

            float purpleDiff, blueDiff, cyanDiff, greenDiff, yellowDiff, orangeDiff, redDiff;
            float minVal = 1000f;

            //1 0 1
            purpleDiff = (
                Math.Abs(pColor.r - MyGfx.Purple.r) +
                Math.Abs(pColor.g - MyGfx.Purple.g) / 4 +
                Math.Abs(pColor.b - MyGfx.Purple.b)
            );
            // 0 0 1
            blueDiff = (
                Math.Abs(pColor.r - MyGfx.Blue.r) / 2 +
                Math.Abs(pColor.g - MyGfx.Blue.g) / 2 +
                Math.Abs(pColor.b - MyGfx.Blue.b)
            );
            // 0 1 1
            cyanDiff = (
                Math.Abs(pColor.r - MyGfx.Cyan.r) / 4 +
                Math.Abs(pColor.g - MyGfx.Cyan.g) +
                Math.Abs(pColor.b - MyGfx.Cyan.b)
            );

            // 0 1 0
            greenDiff = (
                Math.Abs(pColor.r - MyGfx.Green.r) / 2 +
                Math.Abs(pColor.g - MyGfx.Green.g) +
                Math.Abs(pColor.b - MyGfx.Green.b) / 2
            );
            // 1 1 0
            yellowDiff = (
                Math.Abs(pColor.r - MyGfx.Yellow.r) +
                Math.Abs(pColor.g - MyGfx.Yellow.g) +
                Math.Abs(pColor.b - MyGfx.Yellow.b) / 4
            );
            // 1 .6 0
            orangeDiff = (
                Math.Abs(pColor.r - MyGfx.Orange.r) +
                Math.Abs(pColor.g - MyGfx.Orange.g) /1.6f+
                Math.Abs(pColor.b - MyGfx.Orange.b) /2.5f
            );
            // 1 0 0 
            redDiff = (
                Math.Abs(pColor.r - MyGfx.Red.r) +
                Math.Abs(pColor.g - MyGfx.Red.g) / 2 +
                Math.Abs(pColor.b - MyGfx.Red.b) / 2
            );


            Tools.Warn(
                pawn.LabelShort+"'s pColor: " + pColor
                , myDebug
            );
            Tools.Warn(
                "purpleDiff: " + purpleDiff +
                "; blueDiff: " + blueDiff +
                "; cyanDiff: " + cyanDiff +
                "; greenDiff: " + greenDiff +
                "; yellowDiff: " + yellowDiff +
                "; orangeDiff: " + orangeDiff +
                "; redDiff: " + redDiff
                , myDebug
            );


            if (purpleDiff < minVal)
            {
                minVal = purpleDiff;
                answer = MyGfx.Purple;
            }
            if (blueDiff < minVal)
            {
                minVal = blueDiff;
                answer = MyGfx.Blue;
            }
            if (cyanDiff < minVal)
            {
                minVal = cyanDiff;
                answer = MyGfx.Blue;
            }
            if (greenDiff < minVal)
            {
                minVal = greenDiff;
                answer = MyGfx.Green;
            }
            if (yellowDiff < minVal)
            {
                minVal = yellowDiff;
                answer = MyGfx.Yellow;
            }
            if (orangeDiff < minVal)
            {
                minVal = orangeDiff;
                answer = MyGfx.Orange;
            }
            if (redDiff < minVal)
            {
                minVal = redDiff;
                answer = MyGfx.Red;
            }

            if (complementary)
            {
                if (answer == MyGfx.Purple)
                    answer = MyGfx.Yellow;
                else if ((answer == MyGfx.Blue) || (answer == MyGfx.Cyan))
                    answer = MyGfx.Orange;
                else if (answer == MyGfx.Green)
                    answer = MyGfx.Red;
                else if (answer == MyGfx.Yellow)
                    answer = MyGfx.Purple;
                else if (answer == MyGfx.Orange)
                    answer = MyGfx.Blue;
                else if (answer == MyGfx.Red)
                    answer = MyGfx.Green;
            }

            Tools.Warn((complementary)?"complementary":"closest"+" Color=" + answer, myDebug);

            return answer;
        }
    }
}
