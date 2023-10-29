using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoharGfx
{
    public class AlphaScaleLayer_MoteDef : ThingDef
    {
        public AlphaScaleLayer alphaScaleLayer;
        public bool HasASL => alphaScaleLayer != null;

        public bool HasAlpha => HasASL && alphaScaleLayer.HasAlpha;
        public bool HasScale => HasASL && alphaScaleLayer.HasScale;
        public bool HasLayer => HasASL && alphaScaleLayer.HasLayer;

        public bool HasWeigthedAlpha => HasASL && alphaScaleLayer.HasWeigthedAlpha;
        public bool HasWeigthedScale => HasASL && alphaScaleLayer.HasWeigthedScale;

        public bool debug = false;

        public static AlphaScaleLayer_MoteDef MyNamed(string defName) => DefDatabase<AlphaScaleLayer_MoteDef>.GetNamed(defName);
    }

    public class AlphaScaleLayer
    {
        public SimpleCurve alpha;
        public SimpleCurve scale;

        public FloatRange weightedScaleRange;
        public FloatRange weightedAlphaRange;

        public List<LayerSet> layerSets;

        public bool HasAlpha => !alpha.EnumerableNullOrEmpty();
        public bool HasScale => !scale.EnumerableNullOrEmpty();
        public bool HasLayer => !layerSets.NullOrEmpty();

        public bool HasWeigthedAlpha => weightedAlphaRange != null;
        public bool HasWeigthedScale => weightedScaleRange != null;
    }

    public class LayerSet
    {
        public float lifeRange;
        public AltitudeLayer layer;
    }

}
