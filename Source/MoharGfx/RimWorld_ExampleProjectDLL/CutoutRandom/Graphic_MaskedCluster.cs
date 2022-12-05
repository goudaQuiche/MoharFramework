using Verse;
using RimWorld;
using UnityEngine;

namespace MoharGfx
{
    public class Graphic_MaskedCluster : Graphic_CutoutRandom
    {
        private const float PositionVariance = 0.45f;
        private const float SizeVariance = 0.2f;
        private const float SizeFactorMin = 0.8f;
        private const float SizeFactorMax = 1.2f;

        public override void Print(SectionLayer layer, Thing thing, float extraRotation)
        {
            Log.Warning(thing + " Called Graphic_MaskedCluster print");

            Vector3 a = thing.TrueCenter();
            Rand.PushState();
            Rand.Seed = thing.Position.GetHashCode();
            //int num = (thing as Filth)?.thickness ?? 3;
            int num = (thing as Filth)?.thickness ?? 3;
            for (int i = 0; i < num; i++)
            {
                //Log.Warning(thing + " Called Graphic_MaskedCluster print" + $"[{i}]");
                Material matSingle = MatSingle;
                Vector3 center = a + new Vector3(Rand.Range(-PositionVariance, PositionVariance), 0f, Rand.Range(-PositionVariance, PositionVariance));
                Vector2 size = new Vector2(Rand.Range(data.drawSize.x * SizeFactorMin, data.drawSize.x * SizeFactorMax), Rand.Range(data.drawSize.y * SizeFactorMin, data.drawSize.y * SizeFactorMax));
                float rot = Rand.RangeInclusive(0, 360) + extraRotation;
                bool flipUv = Rand.Value < 0.5f;
                TryGetTextureAtlasReplacementInfo(matSingle, thing.def.category.ToAtlasGroup(), flipUv, true, out matSingle, out Vector2[] uvs, out Color32 color);
                Printer_Plane.PrintPlane(layer, center, size, matSingle, rot, flipUv, uvs, new Color32[] { color, color, color, color }, 0.01f, 0f);
                //Printer_Plane.PrintPlane(layer, center, size, matSingle, rot, flipUv, uvs, null, 0.01f, 0f);

            }
            Rand.PopState();
        }
        /*
        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            Log.Warning($"Graphic_MaskedCluster GetColoredVersion p:{path}, s:{newShader}, ds:{drawSize}, c:{newColor}, c2:{newColorTwo}, mp:{path} ... {MatSingle.mainTexture.name}");
            //return GraphicDatabase.Get<Graphic_Random>(path, newShader, drawSize, newColor, newColorTwo, data, null);
            //return GraphicDatabase.Get<Graphic_MaskedCluster>(path, newShader, drawSize, newColor, newColorTwo, data, null);
            //return GraphicDatabase.Get<Graphic_CutoutCollection>(path, newShader, drawSize, newColor, newColorTwo, data, path);
            //return GraphicDatabase.Get<Graphic_MaskedCluster>(path + "/" + MatSingle.mainTexture.name, newShader, drawSize, newColor, newColorTwo, data, maskPath + "/" + MatSingle.mainTexture.name + Graphic_Single.MaskSuffix);

            return GetColoredV(newColor);
        }

        public Graphic_MaskedCluster GetColoredV(Color newColor)
        {
            Graphic_MaskedCluster value = new Graphic_MaskedCluster();
            value.Init(new GraphicRequest(typeof(Graphic_MaskedCluster), path, data.shaderType.Shader, drawSize, newColor, colorTwo, data, 0, null, path));
            return value;
        }

        //public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;
        public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;
        */
        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            Log.ErrorOnce("Graphic_MaskedCluster cannot draw realtime.", 9432243);
        }
    }

}
