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
            if (!pawn.GetHealthTabBleeding(out HealthTabBleeding htb, out Color color))
            {
                GUI.DrawTexture(rect, originTex);
                return;
            }
            else
            {

                //GUI.DrawTexture(rect, GetBloodDropMaterial(htb.replacementTex, color));
                GetBloodDropMaterial(htb.replacementTex, color, out Texture tex, out Material mat);

                Log.Warning("DisplayBloodDrop - GUI.color - " + GUI.color );
                //Color guiColor = GUI.color; GUI.color = color;
                //GUI.DrawTexture(rect, tex);
                //GenUI.DrawTextureWithMaterial(rect, tex, mat);
                //Graphics.DrawTexture(rect, tex, mat);
                //GUI.DrawTexture(rect, mat.mainTexture);
                //GUI.color = guiColor;
                Graphics.DrawTexture(rect, tex, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, Color.white, mat);
            }


            //default
            /*
            GUI.DrawTexture(rect, originTex);
            GUI.DrawTexture(rect, originTex);
            */
        }

        public static void GetBloodDropMaterial(string path, Color newColor, out Texture tex, out Material mat)
        {
            MaterialRequest MR = default(MaterialRequest);

            //MR.mainTex = ContentFinder<Texture2D>.Get(bloodDropPath, reportFailure: true);
            MR.mainTex = ContentFinder<Texture2D>.Get(path, reportFailure: true);
            MR.shader = ShaderDatabase.CutoutComplex;
            //MR.shader = ShaderDatabase.Cutout;

            //MR.color = Color.magenta;
            //MR.color = Color.white;
            //MR.color = newColor;
            //MR.colorTwo = Color.white;

            //MR.renderQueue = MR.renderQueue;
            //MR.shaderParameters = MR.shaderParameters;

            MR.maskTex = ContentFinder<Texture2D>.Get(bloodDropPath + Graphic_Single.MaskSuffix, reportFailure: true);

            mat = MaterialPool.MatFrom(MR);

            mat.color = newColor;
            //mat.color = Color.cyan;

            //Log.Warning(" - GetBloodDropMaterial - trying material - " + path + " - color : " + newColor);
            tex = mat.mainTexture;
        }

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
