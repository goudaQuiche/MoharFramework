using UnityEngine;
using Verse;
using RimWorld;

namespace MoharGfx
{
    public class Graphic_CutoutRandom : Graphic_CutoutCollection
    {
        //public override Material MatSingle => subGraphics[thing.thingIDNumber % subGraphics.Length];

        public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;

        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            //Log.Warning("Graphic_CutoutRandom GetColoredVersion" + this.data.texPath);
            //return GraphicDatabase.Get<Graphic_Random>(path, newShader, drawSize, newColor, newColorTwo, data, null);
            return GraphicDatabase.Get<Graphic_CutoutRandom>(path, newShader, drawSize, newColor, newColorTwo, data);
        }

        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            return thing == null ? MatSingle : MatSingleFor(thing);
        }

        public override Material MatSingleFor(Thing thing)
        {
            if (thing == null)
            {
                return MatSingle;
            }
            //Log.Warning("Graphic_CutoutRandom MatSingleFor" + this.data.texPath);
            return SubGraphicFor(thing).MatSingle;
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            //Log.Warning("Graphic_CutoutRandom DrawWorker" + this.data.texPath);
            Graphic graphic = (thing == null) ? subGraphics[0] : SubGraphicFor(thing);
            graphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
            if (ShadowGraphic == null)
                return;

            ShadowGraphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
        }

        public Graphic SubGraphicFor(Thing thing)
        {
            if (thing == null)
            {
                return subGraphics[0];
            }
            int num = thing.overrideGraphicIndex ?? thing.thingIDNumber;
            return subGraphics[num % subGraphics.Length];
        }

        public Graphic SubGraphicAtIndex(int index)
        {
            return subGraphics[index % subGraphics.Length];
        }

        public Graphic FirstSubgraphic()
        {
            return subGraphics[0];
        }

        public override void Print(SectionLayer layer, Thing thing, float extraRotation)
        {
            Graphic graphic = (thing == null) ? subGraphics[0] : SubGraphicFor(thing);
            graphic.Print(layer, thing, extraRotation);
            if (ShadowGraphic == null || thing == null)
                return;

            ShadowGraphic.Print(layer, thing, extraRotation);
        }

        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "Graphic_CutoutRandom(path=",
                path,
                ", count=",
                subGraphics.Length,
                ")"
            });
        }

    }
}