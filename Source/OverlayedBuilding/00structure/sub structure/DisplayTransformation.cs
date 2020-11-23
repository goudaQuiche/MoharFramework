using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{

    public class DisplayTransformation
    {
        public Offset offset;

        public FloatRange randomXOffset = new FloatRange(0, 0);
        public FloatRange randomYOffset = new FloatRange(0, 0);

        public FloatRange exactRotation = new FloatRange(0, 0);
        public FloatRange rotationRate = new FloatRange(0, 0);

        public FloatRange xVelocity = new FloatRange(0, 0);
        public FloatRange yVelocity = new FloatRange(0, 0);

        public FloatRange randomHalfRotation = new FloatRange(0, 0);

        public FloatRange scale = new FloatRange(1, 1);
    }
}