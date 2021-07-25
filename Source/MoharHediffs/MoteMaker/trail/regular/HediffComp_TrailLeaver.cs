using UnityEngine;
using Verse;
using MoharGfx;

namespace MoharHediffs
{
    //MoteMaker.MakeWaterSplash(vector, pawn.Map, Mathf.Sqrt(pawn.BodySize) * 2f, 1.5f);

    public class HediffComp_TrailLeaver : HediffComp
    {
        public int currentPeriod = 0;

        public Vector3 lastMotePos;
        public Color lastColor = Color.black;
        public bool lastFootprintRight;

        public Map MyMap => Pawn.Map;
        public bool NullMap => MyMap == null;

        public bool MyDebug => Props.debug;
        public HediffCompProperties_TrailLeaver Props => (HediffCompProperties_TrailLeaver)props;
        public bool HasMotePool => Props.HasMotePool;

        public TerrainRestriction TerrainRestriction => (!Props.HasRestriction || !Props.restriction.HasTerrainRestriction) ? null : Props.restriction.terrain;
        public bool HasTerrainRestriction => TerrainRestriction != null;

        public Restriction PawnRestriction => (!Props.HasRestriction ) ? null : Props.restriction;
        public bool HasPawnRestriction => TerrainRestriction != null;

        public override void CompPostMake()
        {
            PropsCheck();
        }

        public void NewPeriod()
        {
            currentPeriod = Props.period.RandomInRange;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Negligible())
            {
                if (MyDebug) Log.Warning("null pawn");
                return;
            }
            if(currentPeriod == 0) NewPeriod();

            if (!Pawn.IsHashIntervalTick(currentPeriod))
                return;

            if (!this.IsTerrainAllowed(Pawn.Position.GetTerrain(MyMap)))
            {
                if (MyDebug) Log.Warning("terrain does not allow motes");
                return;
            }
            if (!this.IsPawnActivityCompatible())
            {
                if (MyDebug) Log.Warning("pawn activity does not allow motes");
                return;
            }
                //
            //if (MyDebug) Log.Warning(Pawn.ThingID + " trying to spawn mote - bodytype:" + Pawn.story?.bodyType?.defName);
            TryPlaceMote();

            NewPeriod();
        }
        public void PropsCheck()
        {
            if (!MyDebug)
                return;

            if (!HasMotePool)
            {
                Log.Warning("Mote pool is empty, trailLeaver needs at least 1 mote");
                /*
                parent.Severity = 0;
                Pawn.health.RemoveHediff(parent);
                */
            }

            if (!Props.HasOffset)
            {
                Log.Warning("no offset per body type found, will use default:" + Props.defaultOffset);
            }
            else
            {
                string dump = "BodyTypeOffsets dump => ";
                foreach(BodyTypeOffset BTO in Props.offSetPerBodyType)
                    dump += BTO.bodyType.defName + ":" + BTO.offset +"; ";

                Log.Warning(dump);
            }

            if (Props.UsesFootPrints)
            {
                Log.Warning("footprints => " + Props.footprint.Dump());
            }
        }

        private void TryPlaceMote()
        {
            if (!HasMotePool)
                return;

            Vector3 drawPos = Pawn.DrawPos;

            // wont exit because we want to record lastFootPos + use old value before recording it
            if (Pawn.Position.InBounds(MyMap))
            {
                float rot = this.GetMoteRotation(drawPos, out Vector3 normalized);
                Vector3 drawPosWithOffset = drawPos + this.GetBodyTypeOffset() + this.GetFootPrintOffset(normalized);

                if (MyDebug) Log.Warning(
                    Pawn.ThingID +" "+ parent.def.defName + " TryPlaceMote - " +
                    " dynRot:" + Props.dynamicRotation + " footprint:" + Props.UsesFootPrints +
                    " drawPos:" + drawPos + " offset:" + drawPosWithOffset + 
                    " rot:" + rot + " normalized:" + normalized
                );

                float scale = Props.randomScale.RandomInRange;
                ThingDef moteDef = Props.motePool.RandomElementWithFallback();
                
                if (drawPosWithOffset.TryAnyMoteSpawn(MyMap, rot, scale, moteDef, MyDebug) is Mote mote)
                {
                    this.ChangeMoteColor(mote);
                }
                
                //drawPosWithOffset.TryAnyMoteSpawn(MyMap, rot, scale, CustomTransformation_MoteDef.MyNamed("Custom_AnimatedMote"));
            }

            this.RecordMotePos(drawPos);
        }
    }
}
