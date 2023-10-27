using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoharGfx
{
    public class AlphaScaleLayer_MoteDef : ThingDef
    {
        public AlphaScaleLayer alphaScaleLayer;
        public bool HasASL => alphaScaleLayer != null;

        //scale
        public bool HasAlpha => HasASL && alphaScaleLayer.HasAlpha;
        public bool HasScale => HasASL && alphaScaleLayer.HasScale;
        public bool HasLayer => HasASL && alphaScaleLayer.HasLayer;

        public bool debug = false;

        public static AlphaScaleLayer_MoteDef MyNamed(string defName) => DefDatabase<AlphaScaleLayer_MoteDef>.GetNamed(defName, false);
    }

    public class AlphaScaleLayer
    {
        public SimpleCurve alpha;
        public SimpleCurve scale;

        public List<LayerSet> layerSets;

        public bool HasAlpha => !alpha.EnumerableNullOrEmpty();
        public bool HasScale => !scale.EnumerableNullOrEmpty();
        public bool HasLayer => !layerSets.NullOrEmpty();
       
    }

    public class LayerSet
    {
        public FloatRange lifeRange;
        public AltitudeLayer layer;
    }

}
