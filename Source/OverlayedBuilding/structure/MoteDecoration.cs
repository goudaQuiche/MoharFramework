using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{

    public class MoteDecoration
    {
        public ThingDef moteDef;

        public bool whenFueled =false;
        public bool whenPowered = false;
        public bool whenFueledAndPowered = false;
        public bool whenWorker = false;
        public bool noCondition = false;

        public bool buildingCentered = false;
        public bool interactionCell = false;
        public bool workerHead = false;

        public Offset offset;
        public FloatRange randomOffset = new FloatRange(0, 0);

        public FloatRange exactRotation = new FloatRange(0, 0);
        public FloatRange rotationRate = new FloatRange(0, 0);

        public FloatRange xVelocity = new FloatRange(0, 0);
        public FloatRange yVelocity = new FloatRange(0, 0);

        public FloatRange scale = new FloatRange(1, 1);

        public int graceTime = 15;
        public bool multipleMotesCoexisting = false;
        //int frequency = 0;
    }
}