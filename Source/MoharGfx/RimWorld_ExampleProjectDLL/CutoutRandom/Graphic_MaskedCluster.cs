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
            Vector3 a = thing.TrueCenter();
            Rand.PushState();
            Rand.Seed = thing.Position.GetHashCode();
            int num = (thing as Filth)?.thickness ?? 3;
            for (int i = 0; i < num; i++)
            {
                Material matSingle = MatSingle;
                Vector3 center = a + new Vector3(Rand.Range(-PositionVariance, PositionVariance), 0f, Rand.Range(-PositionVariance, PositionVariance));
                Vector2 size = new Vector2(Rand.Range(data.drawSize.x * SizeFactorMin, data.drawSize.x * SizeFactorMax), Rand.Range(data.drawSize.y * SizeFactorMin, data.drawSize.y * SizeFactorMax));
                float rot = Rand.RangeInclusive(0, 360) + +extraRotation;
                bool flipUv = Rand.Value < 0.5f;
                Vector2[] uvs;
                Color32 color;
                Graphic.TryGetTextureAtlasReplacementInfo(matSingle, thing.def.category.ToAtlasGroup(), flipUv, true, out matSingle, out uvs, out color);
                Printer_Plane.PrintPlane(layer, center, size, matSingle, rot, flipUv, uvs, new Color32[]
                {
                    color,
                    color,
                    color,
                    color
                }, 0.01f, 0f);
            }
            Rand.PopState();
        }
    }

}
