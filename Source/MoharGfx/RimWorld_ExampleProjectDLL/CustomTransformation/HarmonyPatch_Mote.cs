using RimWorld;
using System.Collections.Generic;
using Verse;
using System;
using System.Reflection;
using HarmonyLib;

namespace MoharGfx
{
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {
        static HarmonyPatchAll()
        {
            Harmony MoharGfx_HarmonyPatch = new Harmony("MoharFW.MoharGfx");
            
            if (HarmonyPatch_Mote.Try_MoteSpawnSetup_PostfixPatch(MoharGfx_HarmonyPatch))
                Log.Message(MoharGfx_HarmonyPatch.Id + " patched Mote.SpawnSetup successfully.");
        }
    }

    public class HarmonyPatch_Mote
    {
        public static bool Try_MoteSpawnSetup_PostfixPatch(Harmony myPatch)
        {
            try
            {
                MethodBase Method = AccessTools.Method(typeof(Mote), "SpawnSetup");
                HarmonyMethod Prefix = null;
                HarmonyMethod Postfix = new HarmonyMethod(typeof(Mote_HarmonyPatch), "Mote_SpawnSetup_Postfix");

                myPatch.Patch(Method, Prefix, Postfix);
            }
            catch (Exception e)
            {
                Log.Warning("MoharFramework.MoharGfx failed Try_MoteSpawnSetup_PostfixPatch" + e);
                return false;
            }

            return true;
        }

        static class Mote_HarmonyPatch
        {
            static void Mote_SpawnSetup_Postfix(Mote __instance)
            {
                if (__instance.Graphic is Graphic_AnimatedMote GAM &&
                    CustomTransformation_MoteDef.MyNamed(__instance.def.defName) is CustomTransformation_MoteDef CTMD)
                {
                    GAM.MyDebug = CTMD.debug;

                    if (CTMD.HasMisc)
                    {
                        GAM.Flipped = CTMD.transformation.misc.flipped;
                    }

                    if (CTMD.HasAnimatedMote)
                    {
                        GAM.FrameOffset = CTMD.transformation.animatedMote.frameOffset;
                        //GAM.FrameOffset += CTMD.transformation.animatedMote.randomFrameOffset.RandomInRange;
                        GAM.TicksPerFrame = CTMD.transformation.animatedMote.ticksPerFrame;
                        GAM.Engine = CTMD.transformation.animatedMote.engine;
                    }
                    if (CTMD.HasRandomRotationRate)
                        __instance.rotationRate = CTMD.transformation.rotation.randRotRate.range.RandomInRange;

                    if (CTMD.HasPulsingScale)
                    {
                        GAM.PulsingScaleRange = CTMD.transformation.scale.pulsingScale.range;
                        GAM.PulsingScaleSpeed = CTMD.transformation.scale.pulsingScale.speed;
                    }

                    //if(CTMD.alp)
                }
            }
        }
    }
}