using Verse;
using UnityEngine;


namespace MoharBlood
{
    [StaticConstructorOnStartup]
    public static class HealthCardUtility_DrawHediffRow_Utils
    {

        private static readonly string bloodDropPath = "UI/Icons/Medical/BloodDrop";
        private static readonly Texture2D bleedingIcon = ContentFinder<Texture2D>.Get(bloodDropPath);


        //GUI.DrawTexture(r.ContractedBy(GenMath.LerpDouble(0f, 0.6f, 5f, 0f, Mathf.Min(this.totalBleedRate, 1f))), this.bleedingIcon);
        public static void DisplayBloodDrop(Rect rect, Texture2D originTex, Pawn pawn)
        {
            bool foundCache = StaticCollections.HealthCardGetCache(pawn, out bool isEligible, out Texture cachedTexture, out Material cachedMaterial);

            if (foundCache)
            {
                    Log.Warning(
                        pawn.LabelShort + " - HealthCardGetCache - found cache : "
                        + " isEligible : " + isEligible
                    );

                if (!isEligible)
                {
                    GUI.DrawTexture(rect, originTex);
                    return;
                }

                Graphics.DrawTexture(rect, cachedTexture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, Color.white, cachedMaterial);
                return;
            }

            if (!pawn.GetHealthTabBleeding(out HealthTabBleeding htb, out Color color))
            {
                StaticCollections.HealthCardAddIneligibleCache(pawn);
            }

            //GUI.DrawTexture(rect, GetBloodDropMaterial(htb.replacementTex, color));
            GetBloodDropMaterial(htb.replacementTex, color, out Texture tex, out Material mat);
            StaticCollections.HealthCardAddCacheAddEligibleCache(pawn, tex, mat);

            //Log.Warning("DisplayBloodDrop - GUI.color - " + GUI.color );

            Graphics.DrawTexture(rect, tex, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, Color.white, mat);

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
    }
}
