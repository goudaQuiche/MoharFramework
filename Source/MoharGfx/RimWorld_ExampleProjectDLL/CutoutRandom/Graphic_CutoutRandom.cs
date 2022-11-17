using UnityEngine;
using Verse;
using RimWorld;

namespace MoharGfx
{
    public class Graphic_CutoutRandom : Graphic_CutoutCollection
    {
        
        public override Material MatSingle => this.subGraphics[Rand.Range(0, SubGraphicsCount)].MatSingle;
        public int SubGraphicsCount => this.subGraphics.Length;
        

        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            return GraphicDatabase.Get<Graphic_Random>(this.path, newShader, this.drawSize, newColor, newColorTwo, this.data, null);
        }

        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            if (thing == null)
            {
                return this.MatSingle;
            }
            return this.MatSingleFor(thing);
        }

        public override Material MatSingleFor(Thing thing)
        {
            if (thing == null)
            {
                return this.MatSingle;
            }
            return this.SubGraphicFor(thing).MatSingle;
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            Graphic graphic;
            if (thing != null)
            {
                graphic = this.SubGraphicFor(thing);
            }
            else
            {
                graphic = this.subGraphics[0];
            }
            graphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
            if (base.ShadowGraphic != null)
            {
                base.ShadowGraphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
            }
        }

        public Graphic SubGraphicFor(Thing thing)
        {
            if (thing == null)
            {
                return this.subGraphics[0];
            }
            int num = thing.overrideGraphicIndex ?? thing.thingIDNumber;
            return this.subGraphics[num % this.subGraphics.Length];
        }

        public Graphic SubGraphicAtIndex(int index)
        {
            return this.subGraphics[index % this.subGraphics.Length];
        }

        public Graphic FirstSubgraphic()
        {
            return this.subGraphics[0];
        }

        public override void Print(SectionLayer layer, Thing thing, float extraRotation)
        {
            Graphic graphic;
            if (thing != null)
            {
                graphic = this.SubGraphicFor(thing);
            }
            else
            {
                graphic = this.subGraphics[0];
            }
            graphic.Print(layer, thing, extraRotation);
            if (base.ShadowGraphic != null && thing != null)
            {
                base.ShadowGraphic.Print(layer, thing, extraRotation);
            }
        }

        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "Graphic_CutoutRandom(path=",
                this.path,
                ", count=",
                this.subGraphics.Length,
                ")"
            });
        }

    }
}