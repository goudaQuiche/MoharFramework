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
    public class HediffComp_TrailLeaver : HediffComp
    {
        Pawn myPawn = null;
        Map myMap = null;
        List <ThingDef> moteDef = null;

        private Vector3 lastFootprintPlacePos;
        private bool lastFootprintRight;
        private const float FootprintIntervalDist = 0.632f;
        private static readonly Vector3 FootprintOffset = new Vector3(0f, 0f, -0.3f);
        private const float LeftRightOffsetDist = 0.17f;
        private const float FootprintSplashSize = 2f;

        private int ticksUntilFootPrint = 500;
        private int footPrintTicksLeft;

        private bool myDebug = false;

        public HediffCompProperties_TrailLeaver Props
        {
            get
            {
                return (HediffCompProperties_TrailLeaver)this.props;
            }
        }

        public override void CompPostMake()
        {
            if (Props.hideBySeverity)
                parent.Severity = .05f;
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

            TerrainDef terrain = myPawn.Position.GetTerrain(myPawn.Map);
            
            if (terrain == null || terrain.IsWater || myPawn.Map.snowGrid.GetDepth(myPawn.Position) >= 0.4f)
            {
                return;
            }

            // Puff
            if (this.footPrintTicksLeft <= 0)
            {
                //Action
                TryPlaceMote();
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

            if (Props.moteDef.NullOrEmpty())
                moteDef.Add(ThingDefOf.Mote_Footprint);
            else
                moteDef = Props.moteDef;
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

        private void TryPlaceMote()
        {

            Vector3 drawPos = myPawn.Drawer.DrawPos;
            Vector3 normalized = (drawPos - lastFootprintPlacePos).normalized;
            float rot = normalized.AngleFlat();
            /*
            float angle = (float)((!lastFootprintRight) ? -90 : 90);
            Vector3 b = normalized.RotatedBy(angle) * 0.17f * Mathf.Sqrt(myPawn.BodySize);
            Vector3 vector = drawPos + FootprintOffset + b;
            */

            Vector3 vector = drawPos;
            IntVec3 c = vector.ToIntVec3();
            if (c.InBounds(myMap))
            {
                TerrainDef terrain = c.GetTerrain(myPawn.Map);
                if (terrain != null)
                {
                    PlaceFootprint(vector, myMap, rot, Props.scale.RandomInRange, moteDef.RandomElement());
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
