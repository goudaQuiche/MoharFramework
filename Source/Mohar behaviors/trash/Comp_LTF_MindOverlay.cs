/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace NewHatcher
{

    namespace NewHatcher
    {
        [StaticConstructorOnStartup]
        public class Comp_LTF_MindOverlay : ThingComp
        {
            private static readonly Material femaleMat = MaterialPool.MatFrom("Things/Building/MindcontrolBench/icon/gender/female", ShaderDatabase.Transparent);
            private static readonly Material powerOffMat = MaterialPool.MatFrom("Things/Building/MindcontrolBench/icon/powerOff", ShaderDatabase.Transparent);

            public CompProperties_LTF_MindOverlay Props
            {
                get
                {
                    return (CompProperties_LTF_MindOverlay)this.props;
                }
            }

            public override void PostDraw()
            {
                base.PostDraw();
                Log.Warning("base.PostDraw();");
                bool gottaDraw = true;

                Thing bench = null;
                bench = this.parent;
                if (bench == null) { return; }

                CompFlickable compFlickable = bench.TryGetComp<CompFlickable>();
                if (compFlickable == null) { return; }
                if (!compFlickable.SwitchIsOn) gottaDraw = false;

                /*
                if (Props.refuelable)
                {
                    CompRefuelable compRefuelable = t.TryGetComp<CompRefuelable>();
                    if (compRefuelable == null) { return; }
                    if (!compRefuelable.HasFuel) gottaDraw = false;
                }
                */

                Log.Warning("will draw");
                if (gottaDraw)
                {
                    Vector3 benchPos = this.parent.DrawPos;
                    Vector3 femalePos = benchPos;

                    Vector3 blueS = new Vector3(.1f, 1f, .1f);
                    Matrix4x4 blueMatrix = default(Matrix4x4);
                    /*
                    float randomAngle = (float)Rand.Range(0, 360);
                    blueMatrix.SetTRS(femalePos, Quaternion.AngleAxis(randomAngle, Vector3.up), blueS);
                    */
                    blueMatrix.SetTRS(femalePos, Quaternion.AngleAxis(0f, Vector3.up), blueS);
                    //blueMatrix.SetTRS(titlePos, Quaternion.AngleAxis(0, Vector3.up), blueS);
                    Log.Warning("lol drawing");



                    Graphics.DrawMesh(MeshPool.plane03, blueMatrix, femaleMat, 0);
                    Graphics.DrawMesh(MeshPool.plane03, blueMatrix, femaleMat, 0);
                }
            }
        }
    }
}
