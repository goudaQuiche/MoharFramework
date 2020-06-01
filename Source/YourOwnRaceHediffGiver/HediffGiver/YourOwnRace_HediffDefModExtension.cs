using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace YORHG
{
    /*
    public class YourOwnRace_HediffDef : HediffDef
    {
        public string race;
        public bool debug;

        public override void ResolveReferences()
        {
            race = "Human";
            debug = false;
        }
    }
    */
    public class HediffDefModExtension : DefModExtension
    {
        public string race;
        public bool keepLowSeverity;
        public BodyPartDef partToAffect;

        public bool debug;
    }
}