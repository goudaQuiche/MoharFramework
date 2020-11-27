using RimWorld;
using HarmonyLib;
using Verse;
using System.Reflection;
using UnityEngine;
using AlienRace;

namespace AddonPerBody
{
    [StaticConstructorOnStartup]
    static class HarmonyPatchAll
    {

        static HarmonyPatchAll()
        {

            Harmony har = new Harmony("AddonPerBody");
            har.PatchAll(Assembly.GetExecutingAssembly());

            if (Prefs.DevMode)
                Log.Warning("Applied AddonPerBody harmony patch");

        }

    }

    public class BodyTypeAddon
    {

        //[HarmonyAfter(new string[] { "net.example.plugin2" })]
        [HarmonyPatch(typeof(AlienPartGenerator.BodyAddon), "GetPath")]
        class HARPatch
        {
            public static void Postfix(Pawn pawn, ref Graphic __result)
            {
                if (__result != null)
                {
                    if (pawn == null)
                        return;
                    string originalPath = __result.path;
                    bool validTexture = false;

                    //Body typed texture
                    if (pawn.story.bodyType == BodyTypeDefOf.Hulk || pawn.story.bodyType == BodyTypeDefOf.Fat)
                    {
                        if (pawn.story.bodyType == BodyTypeDefOf.Hulk)
                        {
                            if ((ContentFinder<Texture2D>.Get(originalPath + "_Hulk" + "_south", false) != null))
                            {
                                Graphic newGraphic = GraphicDatabase.Get<Graphic_Multi>(originalPath + "_Hulk", __result.Shader, __result.drawSize, __result.color, __result.colorTwo);
                                __result = newGraphic;
                                validTexture = true;
                            }
                        }
                        else if (pawn.story.bodyType == BodyTypeDefOf.Fat)
                        {
                            if ((ContentFinder<Texture2D>.Get(originalPath + "_Fat" + "_south", false) != null))
                            {
                                Graphic newGraphic = GraphicDatabase.Get<Graphic_Multi>(originalPath + "_Fat", __result.Shader, __result.drawSize, __result.color, __result.colorTwo);
                                __result = newGraphic;
                                validTexture = true;
                            }
                        }
                        if (validTexture == false)
                        {
                            if ((ContentFinder<Texture2D>.Get(originalPath + "_Wide" + "_south", false) != null))
                            {
                                Graphic newGraphic = GraphicDatabase.Get<Graphic_Multi>(originalPath + "_Wide", __result.Shader, __result.drawSize, __result.color, __result.colorTwo);
                                __result = newGraphic;
                                validTexture = true;
                            }
                        }
                    }
                    else if (pawn.story.bodyType == BodyTypeDefOf.Thin)
                    {
                        if ((ContentFinder<Texture2D>.Get(originalPath + "_Thin" + "_south", false) != null))
                        {
                            Graphic newGraphic = GraphicDatabase.Get<Graphic_Multi>(originalPath + "_Thin", __result.Shader, __result.drawSize, __result.color, __result.colorTwo);
                            __result = newGraphic;
                            validTexture = true;
                        }
                    }
                    else if (pawn.story.bodyType == BodyTypeDefOf.Male)
                    {
                        if ((ContentFinder<Texture2D>.Get(originalPath + "_Male" + "_south", false) != null))
                        {
                            Graphic newGraphic = GraphicDatabase.Get<Graphic_Multi>(originalPath + "_Male", __result.Shader, __result.drawSize, __result.color, __result.colorTwo);
                            __result = newGraphic;
                            validTexture = true;
                        }
                    }
                    else if (pawn.story.bodyType == BodyTypeDefOf.Female)
                    {
                        if ((ContentFinder<Texture2D>.Get(originalPath + "_Female" + "_south", false) != null))
                        {
                            Graphic newGraphic = GraphicDatabase.Get<Graphic_Multi>(originalPath + "_Female", __result.Shader, __result.drawSize, __result.color, __result.colorTwo);
                            __result = newGraphic;
                            validTexture = true;
                        }
                    }
                    else
                    {
                        string bodyname = pawn.story.bodyType.defName;
                        if ((ContentFinder<Texture2D>.Get(originalPath + "_" + bodyname + "_south", false) != null))
                        {
                            Graphic newGraphic = GraphicDatabase.Get<Graphic_Multi>(originalPath + "_" + bodyname, __result.Shader, __result.drawSize, __result.color, __result.colorTwo);
                            __result = newGraphic;
                            validTexture = true;
                        }
                    }
                }
            }
        }

    }


}
