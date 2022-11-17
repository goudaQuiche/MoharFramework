using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoharGfx
{
    public class CustomTransformation_MoteDef : ThingDef
    {
        public CustomTransformation transformation;
        public bool HasTransformation => transformation != null;

        // animation
        public bool HasAnimatedMote => HasTransformation && transformation.HasAnimatedMote;

        // color
        public bool HasColor => HasTransformation && transformation.HasColor;
        public bool HasAlpha => HasColor && transformation.color.HasArbitraryAlpha;

        // misc - flipped
        public bool HasMisc => HasTransformation && transformation.HasMisc;

        //rotation
        public bool HasRotation => HasTransformation && transformation.HasRotation;
        public bool HasPeriodicRandomRotation => HasRotation && transformation.rotation.HasPeriodicRandRot;
        public bool HasRandomRotationRate => HasRotation && transformation.rotation.HasRandRotRate;
        public bool HasStraightenUp => HasRotation && transformation.rotation.HasStraightenUp;

        //scale
        public bool HasScale => HasTransformation && transformation.HasScale;
        public bool HasPulsingScale => HasScale && transformation.scale.HasPulsingScale;



        public bool debug = false;

        public static CustomTransformation_MoteDef MyNamed(string defName) => DefDatabase<CustomTransformation_MoteDef>.GetNamed(defName, false);
    }

    public class CustomTransformation
    {
        public Rotation rotation;
        public AnimatedMote animatedMote;
        public Scale scale;
        public ColorAltering color;
        public Misc misc;

        public bool HasAnimatedMote => animatedMote != null;
        public bool HasColor => color != null;
        public bool HasMisc => misc != null;
        public bool HasRotation => rotation != null;
        public bool HasScale => scale != null;
        
    }

    public class ColorAltering
    {
        public float arbitraryAlpha = 1f;

        public bool HasArbitraryAlpha => arbitraryAlpha != 1;
    }

    public class Misc
    {
        //public Vector3 offSet;
        public bool flipped = false;
    }

    public class Rotation
    {
        public PeriodicRandomRotation periodicRandRot;
        public RandomRotationRate randRotRate;
        public StraightenUpRotation straightenUp;

        public bool HasPeriodicRandRot => periodicRandRot != null;
        public bool HasRandRotRate => randRotRate != null;
        public bool HasStraightenUp => straightenUp != null;
    }

    public class PeriodicRandomRotation
    {
        public FloatRange randomAngle;
        public float chance;
        public IntRange period;
    }

    public class RandomRotationRate
    {
        public FloatRange range;
    }

    public class StraightenUpRotation
    {
        public float aimedRotation;
        public float goalLifeSpanRatio;

        public List<FloatRange> gracePeriod;

        public float tolerance;

        public bool HasGracePeriod => !gracePeriod.NullOrEmpty();

        public bool IsWithinGracePeriod(float curLifeSpanRatio)
        {
            if (!HasGracePeriod)
                return false;

            return gracePeriod.Any(fr => fr.Includes(curLifeSpanRatio));
        }
    }

    public class Scale
    {
        public PulsingScale pulsingScale;
        public bool HasPulsingScale => pulsingScale != null;
    }
    public class PulsingScale
    {
        public Vector2 range;
        public float speed;
    }

    public class AnimatedMote
    {
        //public IntRange randomFrameOffset;
        public int frameOffset;
        public int ticksPerFrame;
        public IndexEngine.TickEngine engine = IndexEngine.TickEngine.synced;
    }

}
