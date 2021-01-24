using RimWorld;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Verse;

namespace MoharHediffs
{
    public class HediffComp_TrailLeaver : HediffComp
    {
        private Vector3 lastMotePos;
        private Color lastColor = Color.black;
        private bool toTheTop = true;

        private Map MyMap => Pawn.Map;
        private bool TerrainDoesNotAllowMotes(TerrainDef terrain) 
            => 
            terrain == null || 
            (!Props.allowedInWater && terrain.IsWater) || 
            !Props.allowedWithSnowDepth.Includes(MyMap.snowGrid.GetDepth(Pawn.Position));

        private bool MyDebug => Props.debug;
        public HediffCompProperties_TrailLeaver Props => (HediffCompProperties_TrailLeaver)this.props;

        public override void CompPostMake()
        {
            Init();
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Negligible())
            {
                if (MyDebug) Log.Warning("null pawn");
                return;
            }

            if (!Pawn.IsHashIntervalTick(Props.period))
                return;

            TerrainDef terrain = Pawn.Position.GetTerrain(MyMap);
            if (TerrainDoesNotAllowMotes(terrain))
            {
                if (MyDebug) Log.Warning("terrain does not allow motes");
                return;
            }

            //if (MyDebug) Log.Warning(Pawn.ThingID + " trying to spawn mote - bodytype:" + Pawn.story?.bodyType?.defName);
            TryPlaceMote();
        }
        public void Init()
        {
            if (!MyDebug)
                return;

            if (!Props.HasMotePool )
                Log.Warning("Mote pool is empty, trailLeaver needs at least 1 mote");

            if (!Props.HasOffset)
            {
                Log.Warning("no offset found, will use default:" + Props.defaultOffset);
            }
            else
            {
                foreach(BodyTypeOffset BTO in Props.offSetPerBodyType)
                {
                    Log.Warning(BTO.bodyType.defName + ":" + BTO.offset);
                }
            }
        }

        private void TryPlaceMote()
        {
            Vector3 drawPos = Pawn.DrawPos;
            Vector3 normalized = (drawPos - lastMotePos).normalized;
            float rot = normalized.AngleFlat();

            Vector3 vector = drawPos + GetOffset;
            //if (MyDebug) Log.Warning(Pawn.ThingID + " dp:" + drawPos + " offset:" + GetOffset + " => vector:"+vector);
            if (Pawn.Position.InBounds(MyMap))
            {
                if (vector.TryMoteSpawn(MyMap, rot, Props.scale.RandomInRange, Props.motePool.RandomElementWithFallback(), MyDebug) is Mote mote)
                {
                    if (Props.useColorRange)
                    {
                        if (lastColor == Color.black)
                            lastColor = Props.colorRange.colorA;

                        lastColor = Props.colorRange.RandomPickColor(lastColor, MyDebug);

                        mote.instanceColor = lastColor;
                    }
                }
            }
            lastMotePos = drawPos;
        }

        public Vector3 GetOffset
        {
            get
            {
                if (Pawn.story?.bodyType == null || !Props.HasOffset)
                    return Props.defaultOffset;

                BodyTypeOffset BTO = Props.offSetPerBodyType.Where(b => b.bodyType == Pawn.story.bodyType).FirstOrFallback();
                return BTO == null ? Props.defaultOffset : BTO.offset;
            }
        }

    }
}
