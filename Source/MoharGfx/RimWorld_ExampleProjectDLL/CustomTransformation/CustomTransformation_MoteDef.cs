using Verse;
using UnityEngine;

namespace MoharGfx
{
    public class CustomTransformation_MoteDef : ThingDef
    {
        public CustomTransformation transformation;
        public bool HasTransformation => transformation != null;

        public bool HasAnimatedMoteSpecifics => HasTransformation && transformation.HasAnimatedMoteSpecifics;

        public bool HasRandomRotation => HasTransformation && transformation.HasRandomRotation;
        public bool HasRotationSpecifics => HasTransformation && transformation.HasRotationSpecifics;

        public bool HasScaleSpecifics => HasTransformation && transformation.HasScaleSpecifics;
        public bool HasPulsingScale => HasScaleSpecifics && transformation.scaleSpecifics.pulsingScale != null;

        public bool HasSpacialSpecifics => HasTransformation && transformation.HasSpacialSpecifics;

        public bool debug = false;

        public static CustomTransformation_MoteDef MyNamed(string defName) => DefDatabase<CustomTransformation_MoteDef>.GetNamed(defName, false);
    }

    public class CustomTransformation
    {
        public RandomRotationTransformation randomRotation;
        public bool HasRandomRotation => randomRotation != null;

        public AnimatedMoteSpecifics animatedMoteSpecifics;
        public bool HasAnimatedMoteSpecifics => animatedMoteSpecifics != null;

        public RotationSpecifics rotationSpecifics;
        public bool HasRotationSpecifics => rotationSpecifics != null;

        public ScaleSpecifics scaleSpecifics;
        public bool HasScaleSpecifics => scaleSpecifics != null;

        public SpacialSpecifics spacialSpecifics;
        public bool HasSpacialSpecifics => spacialSpecifics != null;
    }

    public class SpacialSpecifics
    {
        //public Vector3 offSet;
        public bool flipped = false;
    }

    public class RandomRotationTransformation
    {
        public FloatRange randomAngle;
        public float chance;
        public IntRange period;
    }

    public class RotationSpecifics
    {
        public FloatRange rotationRange;
    }

    public class ScaleSpecifics
    {
        public PulsingScale pulsingScale;
    }

    public class AnimatedMoteSpecifics
    {
        public IntRange randomFrameOffset;
        public int frameOffset;
        public int ticksPerFrame;
    }
    public class PulsingScale
    {
        public Vector2 range;
        public float speed;
    }
}
