using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;
using UnityEngine;
using System;

namespace MoharHediffs
{
    public class InnerShineDef : Def
    {
        public InnerShineItem item;

        public override string ToString() => defName;
        public InnerShineDef Named(string name) => DefDatabase<InnerShineDef>.GetNamed(name);
        public override int GetHashCode() => defName.GetHashCode();
    }
}
