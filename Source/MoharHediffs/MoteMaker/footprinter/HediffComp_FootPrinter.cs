using RimWorld;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace MoharHediffs
{
    public class HediffComp_FootPrinter : HediffComp
    {
        Pawn myPawn = null;
        Map myMap = null;
        ThingDef moteFootPrintDef = null;

        private Vector3 lastFootprintPlacePos;
        private bool lastFootprintRight;
        private const float FootprintIntervalDist = 0.632f;
        private static readonly Vector3 FootprintOffset = new Vector3(0f, 0f, -0.3f);
        private const float LeftRightOffsetDist = 0.17f;
        private const float FootprintSplashSize = 2f;

        private int ticksUntilFootPrint = 500;
        private int footPrintTicksLeft;

        private bool myDebug = false;

        public HediffCompProperties_FootPrinter Props
        {
            get
            {
                return (HediffCompProperties_FootPrinter)this.props;
            }
        }

        public override void CompPostMake()
        {
            myDebug = Props.debug;
            Init();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (myPawn == null)
            {
                Tools.Warn("pawn null", myDebug);
                return;
            }
            if (myPawn.Map == null)
            {
                //Tools.Warn(myPawn.Label + " - pawn.Map null", myDebug);
                return;
            }
            if (Props.moteFootPrintDef == null)
                return;

            TerrainDef terrain = myPawn.Position.GetTerrain(myPawn.Map);
            
            if (terrain == null || terrain.IsWater || myPawn.Map.snowGrid.GetDepth(myPawn.Position) >= 0.4f)
            {
                return;
            }

            // Puff
            if (this.footPrintTicksLeft <= 0)
            {
                //Action
                TryPlaceFootprint();
                //Reset
                Reset();
            }
            // decrease ticks
            else
            {
                this.footPrintTicksLeft --;
            }
        }
        public void Init()
        {
            myPawn = parent.pawn;
            myMap = myPawn.Map;
            if (Props.moteFootPrintDef == null)
                moteFootPrintDef = ThingDefOf.Mote_Footprint;
            else
                moteFootPrintDef = Props.moteFootPrintDef;
        }

        public static void PlaceFootprint(Vector3 loc, Map map, float rot, ThingDef Mote_FootprintDef)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(Mote_FootprintDef, null);
            moteThrown.Scale = 0.5f;
            moteThrown.exactRotation = rot;
            moteThrown.exactPosition = loc;
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }

        private void TryPlaceFootprint()
        {

            Vector3 drawPos = myPawn.Drawer.DrawPos;
            Vector3 normalized = (drawPos - lastFootprintPlacePos).normalized;
            float rot = normalized.AngleFlat();
            float angle = (float)((!lastFootprintRight) ? -90 : 90);
            Vector3 b = normalized.RotatedBy(angle) * 0.17f * Mathf.Sqrt(myPawn.BodySize);
            Vector3 vector = drawPos + FootprintOffset + b;
            IntVec3 c = vector.ToIntVec3();
            if (c.InBounds(myMap))
            {
                TerrainDef terrain = c.GetTerrain(myPawn.Map);
                if (terrain != null)
                {
                    PlaceFootprint(vector, myMap, rot, moteFootPrintDef);
                }
            }
            lastFootprintPlacePos = drawPos;
            lastFootprintRight = !lastFootprintRight;
        }

        void Reset()
        {
            footPrintTicksLeft = ticksUntilFootPrint = Props.period;
        }

    }
}
