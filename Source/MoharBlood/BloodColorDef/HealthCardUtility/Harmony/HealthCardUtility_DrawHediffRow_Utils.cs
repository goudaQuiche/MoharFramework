using Verse;
using UnityEngine;


namespace MoharBlood
{
    [StaticConstructorOnStartup]
    public static class HealthCardUtility_DrawHediffRow_Utils
    {

        private static readonly string bloodDropPath = "UI/Icons/Medical/BloodDrop";
        private static readonly Texture2D bleedingIcon = ContentFinder<Texture2D>.Get(bloodDropPath);
        public static readonly Rect OneByOneRect = new Rect(0f, 0f, 1f, 1f);

        public static void DisplayUncachedBloodDrop(Rect rect, Texture2D originTex, Pawn pawn)
        {
            bool foundParams = pawn.GetHealthTabBleeding(out HealthTabBleeding htb, out Color color);

            if (!foundParams)
            {
                GUI.DrawTexture(rect, originTex);
                return;
            }
            GetBloodDropMaterial(htb.replacementTex, color, out Texture tex, out Material mat);
            Graphics.DrawTexture(rect, tex, OneByOneRect, 0, 0, 0, 0, Color.white, mat);
        }

        public static void DisplayCachedBloodDrop(Rect rect, Texture2D originTex, Pawn pawn)
        {
            bool foundCache = CachedHealthCard.GetCache(pawn.thingIDNumber, out bool isEligible, out Texture cachedTexture, out Material cachedMaterial);

            if (foundCache)
            {
                if (!isEligible)
                {
                    GUI.DrawTexture(rect, originTex);
                    return;
                }
                Graphics.DrawTexture(rect, cachedTexture, OneByOneRect, 0, 0, 0, 0, Color.white, cachedMaterial);
                return;
            }

            bool foundParams = pawn.GetHealthTabBleeding(out HealthTabBleeding htb, out Color color);

            if (!foundParams)
            {
                CachedHealthCard.AddIneligibleCache(pawn.thingIDNumber);
                GUI.DrawTexture(rect, originTex);
                return;
            }

            GetBloodDropMaterial(htb.replacementTex, color, out Texture tex, out Material mat);
            Graphics.DrawTexture(rect, tex, OneByOneRect, 0, 0, 0, 0, Color.white, mat);
            CachedHealthCard.AddEligibleCache(pawn.thingIDNumber, tex, mat);
        }

        public static void GetBloodDropMaterial(string path, Color newColor, out Texture tex, out Material mat)
        {
            MaterialRequest MR = default(MaterialRequest);

            MR.mainTex = ContentFinder<Texture2D>.Get(path, reportFailure: true);
            MR.shader = ShaderDatabase.CutoutComplex;
            MR.maskTex = ContentFinder<Texture2D>.Get(path + Graphic_Single.MaskSuffix, reportFailure: true);

            mat = MaterialPool.MatFrom(MR);
            mat.color = newColor;

            //Log.Warning(" - GetBloodDropMaterial - trying material - " + path + " - color : " + newColor);
            tex = mat.mainTexture;
        }
        /*
        public static Texture2D GetBloodDropTexture(Pawn pawn, Texture2D originTex)
        {
            MaterialRequest MR = default(MaterialRequest);

            MR.mainTex = ContentFinder<Texture2D>.Get(bloodDropPath, reportFailure: true);
            MR.shader = ShaderDatabase.CutoutComplex;
            MR.color = Color.magenta;
            MR.colorTwo = Color.white;

            //MR.renderQueue = MR.renderQueue;
            //MR.shaderParameters = MR.shaderParameters;

            MR.maskTex = ContentFinder<Texture2D>.Get(bloodDropPath + Graphic_Single.MaskSuffix, reportFailure: true);

            Material mat = MaterialPool.MatFrom(MR);
            mat.color = Color.magenta;
            //Texture2D texture2D = (Texture2D)mat.mainTexture;
            Texture2D newTex = mat.GetTexture("_MainTex") as Texture2D;

            return newTex;
        }
        */
    }
}
