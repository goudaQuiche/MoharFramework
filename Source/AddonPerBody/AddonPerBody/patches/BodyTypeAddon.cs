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
                Log.Message("MoharFramework - Applied AddonPerBody harmony patch");

        }

    }

    public class BodyTypeAddon
    {
        static class Utils
        {

            public static bool IsThereContent(string newPath)
            {
                return ContentFinder<Texture2D>.Get(newPath + "_south", false) != null;
            }

            public static string InsertBodyTypeDirectoryBeforeFileName(string originalPath, string bodyType)
            {
                int lastSlashPos = originalPath.LastIndexOf("/") + 1;
                string path = originalPath.Substring(0, lastSlashPos);
                string fileName = originalPath.Substring(lastSlashPos, originalPath.Length - lastSlashPos);

                return path + bodyType + "/" + fileName;
            }
        }

        [HarmonyPatch(typeof(AlienPartGenerator.BodyAddon), "GetPath")]
        class HARPatch
        {
            public static void Postfix(Pawn pawn, ref Graphic __result)
            {
                if (__result == null)
                    return;

                if (pawn == null)
                    return;

                string originalPath = __result.path;

                string bodyname = pawn.story.bodyType.defName;
                //string newPath = Utils.InsertBodyTypeBeforeFileName(originalPath, bodyname);
                string newPath = Utils.InsertBodyTypeDirectoryBeforeFileName(originalPath, bodyname);

                //if (Prefs.DevMode && pawn.story.bodyType == BodyTypeDefOf.Hulk && originalPath.Contains("Arm"))
                //    Log.Warning("bodytype: " + bodyname + "; oPath:" + originalPath + "; nPath:" + newPath);

                if (Utils.IsThereContent(newPath))
                {
                    //if (Prefs.DevMode)Log.Warning("=>>>AlienPartGenerator.BodyAddon Foundcontent nPath:" + newPath);
                    Graphic newGraphic = GraphicDatabase.Get<Graphic_Multi>(newPath, __result.Shader, __result.drawSize, __result.color, __result.colorTwo);
                    __result = newGraphic;
                }
            }

        }

    }


}
