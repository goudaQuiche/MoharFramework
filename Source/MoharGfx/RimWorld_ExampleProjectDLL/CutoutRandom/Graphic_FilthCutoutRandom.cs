using UnityEngine;
using Verse;
using RimWorld;

namespace MoharGfx
{
    public class Graphic_FilthCutoutRandom : Graphic_CutoutCollection
    {
        
        public override Material MatSingle => subGraphics[Rand.Range(0, SubGraphicsCount)].MatSingle;
        public int SubGraphicsCount => subGraphics.Length;
        

        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            return GraphicDatabase.Get<Graphic_Random>(path, newShader, drawSize, newColor, newColorTwo, data, null);
        }

        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            if (thing == null)
            {
                return MatSingle;
            }
            return MatSingleFor(thing);
        }

        public override Material MatSingleFor(Thing thing)
        {
            if (thing == null)
            {
                return MatSingle;
            }
            return SubGraphicFor(thing).MatSingle;
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

        public Graphic FirstSubgraphic()
        {
            return subGraphics[0];
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