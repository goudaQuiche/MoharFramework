﻿using RimWorld;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace MoharHediffs
{
    public class HediffComp_RainbowTrail : HediffComp
    {
        public enum RainbowColor
        {
            [Description("purple")]
            purple = 0,
            [Description("blue")]
            blue = 1,
            [Description("green")]
            green = 2,
            [Description("yellow")]
            yellow = 3,
            [Description("orange")]
            orange = 4,
            [Description("red")]
            red = 5,

        }

        Pawn myPawn = null;
        Map myMap = null;

        private Vector3 lastFootprintPlacePos;
        private static readonly Vector3 PuddleOffset = new Vector3(0f, 0f, -0.3f);
        private int ticksLeft;

        List<ThingDef> moteDef = null;
        RainbowColor curColor = RainbowColor.purple;
        int sameColorInRow = 0;

        private bool blockAction = false;
        private bool myDebug = false;

        public HediffCompProperties_RainbowTrail Props
        {
            get
            {
                return (HediffCompProperties_RainbowTrail)this.props;
            }
        }
        public override void CompPostMake()
        {
            if(Props.hideBySeverity)
                parent.Severity = .05f;
            myDebug = Props.debug;
            Init(myDebug);
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksLeft, "ticksLeft");
            Scribe_Values.Look(ref curColor, "curColor");

            Scribe_Values.Look(ref myDebug, "debug");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            SetPawnAndMap();
            if (myPawn == null || myMap == null || blockAction)
            {
                Tools.Warn(
                    "null tick - " +
                    "pawn: " + (myPawn == null) +
                    "myMap: " + (myMap == null) +
                    "blockAction: " + blockAction
                    , myDebug);
                return;
            }
            if (moteDef.NullOrEmpty())
            {
                Tools.Warn("empty moteDef", myDebug);
                SetMoteDef();
            }

            TerrainDef terrain = myPawn.Position.GetTerrain(myPawn.Map);
            if (terrain == null || terrain.IsWater || myPawn.Map.snowGrid.GetDepth(myPawn.Position) >= 0.4f)
            {
                Tools.Warn(myPawn + "'s " + parent.def.defName + " is blocked by terrain", myDebug);
                return;
            }

            // puddle
            if (ticksLeft <= 0)
            {
                Tools.Warn(myPawn + "'s " + parent.def.defName + " wants to puddle", myDebug);
                //Action and rainbow colors
                if (TryPlaceMote())
                {
                    sameColorInRow++;

                    if (sameColorInRow >= Props.minTimesSameColor)
                        if (sameColorInRow > Props.maxTimesSameColor || !Rand.Chance(Props.staySameColorChance) || !myPawn.Position.InBounds(myMap))
                        {
                            Tools.Warn(myPawn + "'s " + parent.def.defName + " wants to change of color", myDebug);
                            NextColor();
                            SetMoteDef();
                            sameColorInRow = 0;
                        }

                    //Reset tick count
                    Reset();
                }
                else
                {
                    Tools.Warn(myPawn + "'s " + parent.def.defName + "failed to TryPlaceMote", myDebug);
                }
            }
            // decrease ticks
            else
            {
                //Tools.Warn(myPawn + "'s " + parent.def.defName + "is ticking: "+ticksLeft+" left", myDebug);
                ticksLeft--;
            }
        }

        public void SetMoteDef()
        {
            switch (curColor)
            {
                case RainbowColor.purple:
                    moteDef = Props.motePurpleDef;
                    break;
                case RainbowColor.blue:
                    moteDef = Props.moteBlueDef;
                    break;
                case RainbowColor.green:
                    moteDef = Props.moteGreenDef;
                    break;
                case RainbowColor.yellow:
                    moteDef = Props.moteYellowDef;
                    break;
                case RainbowColor.orange:
                    moteDef = Props.moteOrangeDef;
                    break;
                case RainbowColor.red:
                    moteDef = Props.moteRedDef;
                    break;
            }
        }

        public void NextColor()
        {
                curColor++;
                if (curColor > RainbowColor.red)
                    curColor = RainbowColor.purple;
        }

        private void SetPawnAndMap()
        {
            myPawn = parent.pawn;
            myMap = myPawn.Map;
        }

        public void Init(bool myDebug = false)
        {
            SetPawnAndMap();

            Tools.Warn(myPawn + "'s " + parent.def.defName + " Init", myDebug);

            if (
                Props.motePurpleDef.NullOrEmpty() 
                || Props.moteBlueDef.NullOrEmpty() 
                || Props.moteGreenDef.NullOrEmpty() 
                || Props.moteYellowDef.NullOrEmpty() 
                || Props.moteOrangeDef.NullOrEmpty() 
                || Props.moteRedDef.NullOrEmpty())
            {
                blockAction = true;
                parent.Severity = 0;
            }

            curColor = (RainbowColor)Rand.Range(0, 6);
            SetMoteDef();

            Tools.Warn(myPawn + "'s " + parent.def.defName + " Init success", myDebug);
        }

        public static void PlaceFootprint(Vector3 loc, Map map, float rot, float scale, ThingDef Mote_FootprintDef)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(Mote_FootprintDef, null);
            moteThrown.Scale = scale;
            moteThrown.exactRotation = rot;
            moteThrown.exactPosition = loc;
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }

        private bool TryPlaceMote()
        {

            Vector3 drawPos = myPawn.Drawer.DrawPos;
            Vector3 normalized = (drawPos - lastFootprintPlacePos).normalized;
            float rot = normalized.AngleFlat();

            //Vector3 vector = drawPos + FootprintOffset;
            Vector3 vector = myPawn.TrueCenter() + PuddleOffset;
            IntVec3 c = vector.ToIntVec3();
            if (c.InBounds(myMap))
            {
                TerrainDef terrain = c.GetTerrain(myPawn.Map);
                if (terrain != null)
                {
                    PlaceFootprint(vector, myMap, rot, Props.scale.RandomInRange, moteDef.RandomElement());
                    return true;
                }
            }
            lastFootprintPlacePos = drawPos;
            return false;
        }

        void Reset()
        {
            ticksLeft = Props.period;
        }

    }
}
