using Verse;
using UnityEngine;


namespace MoharBlood
{
    [StaticConstructorOnStartup]
    public static class HealthCardUtility_DrawHediffRow_Utils
    {
        private static readonly string bloodDropPath = "UI/Icons/Medical/BloodDrop";
        private static readonly Texture2D bleedingIcon = ContentFinder<Texture2D>.Get(bloodDropPath);

        public static Texture2D GetBloodDropTexture(Pawn pawn, Texture2D originTex)
        {

            //Texture2D test = ContentFinder<Texture2D>.

            
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
            
            
            /*
            Texture2D newTex = new Texture2D(originTex.width, originTex.height);
            newTex = ContentFinder<Texture2D>.Get(bloodDropPath, reportFailure: true); ;

            var fillColor = Color.magenta;
            var fillColorArray = newTex.GetPixels();

            for (var i = 0; i < fillColorArray.Length; ++i)
            {
                fillColorArray[i] = fillColor;
            }

            newTex.SetPixels(fillColorArray);

            newTex.Apply();
            */

            return newTex;
        }
    }
}
