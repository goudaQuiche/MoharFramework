using System;
using Verse;
using RimWorld;

namespace LTF_Slug
{
    public class CompProperties_UseItemAbility : CompProperties
    {
        public bool doCameraShake;

        public CompProperties_UseItemAbility()
        {
            this.compClass = typeof(CompUseEffect);
        }
    }
}