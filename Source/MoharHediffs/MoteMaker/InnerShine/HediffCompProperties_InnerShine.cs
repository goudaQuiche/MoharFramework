using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;

namespace MoharHediffs
{
    public class HediffCompProperties_InnerShine : HediffCompProperties
    {
        public List<InnerShineItem> innerShinePool;
        public List<InnerShineDef> innerShineDefPool;

        public bool HasShinePool => HasRawShinePool || HasShineDefPool;

        public bool HasRawShinePool => !innerShinePool.NullOrEmpty();
        public bool HasShineDefPool => !innerShineDefPool.NullOrEmpty();

        public bool debug;
        // => innerShineList.Any(i => i.debug);

        public HediffCompProperties_InnerShine()
        {
            compClass = typeof(HediffComp_InnerShine);
        }
    }
}