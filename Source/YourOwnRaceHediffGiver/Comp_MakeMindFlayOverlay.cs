using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Verse;               // RimWorld universal objects are here (like 'Building')
using Verse.Sound;

using UnityEngine;


namespace LTF_Slug
{

    // Main
    [StaticConstructorOnStartup]
    public class Comp_MakeIceOverlay : ThingComp
    {
        // building Setup
        Pawn pawn = null;
        Vector3 pawnPos;
        

        private float Progress = 0;
        private float Max = 65000;
        bool drawMe = false;

        static string overlayPath = "Special/";

        Mesh WarningMesh = MeshPool.plane08;
        Mesh dotsMesh = MeshPool.plane14;
        Mesh workMesh = MeshPool.plane10;

        private static readonly Graphic MakeIceGfx = GraphicDatabase.Get<Graphic_Flicker>(overlayPath + "MakeIce", ShaderDatabase.TransparentPostLight, Vector2.one * .5f, Color.white);

        public CompProperties_MakeIceOverlay Props
        {
            get
            {
                return (CompProperties_MakeIceOverlay)props;
            }
        }
        public void ResetProgress()
        {
            Progress = 0;
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (drawMe)
            {
                Log.Warning("gotta draw " + pawn.Label);
                if (pawnPos == null)
                {
                    Log.Warning("null pos draw");
                    return;
                }
                // higher than ground to be visible
                pawnPos.y += 4f;
                //pawnPos.y += 0.046875f;

                MakeIceGfx.Draw(pawnPos, Rot4.North, this.parent, 0f);
            }
            
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            pawn = (Pawn)parent;
            pawnPos = pawn.DrawPos;
            /*
            powerComp = bench.TryGetComp<CompPowerTrader>();
            qualityComp = bench.TryGetComp<CompQuality>();
            QualityWeightedBenchFactors();
            */
            DumpProps();
        }

        private void DumpProps()
        {
            Log.Warning("work:" + Props.maxTicks);
        }

        public override void CompTick()
        //public override void CompTickRare()
        {
            Progress++;
            Progress = ((Progress > Max) ? (0) : (Progress));

            if (Progress >= Props.maxTicks)
            {
                drawMe = false;
                //this.PostDestroy(vanish ,Map);
            }
            // We should not have a snap without somebody dead
            else
            {
                drawMe = true;
            }
            Log.Warning("TickerType:" + Progress + " draw?" + drawMe);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref Progress, "LTF_drawMe");
        }

    }
}