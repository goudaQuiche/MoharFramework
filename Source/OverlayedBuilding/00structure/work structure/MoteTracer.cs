using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{

    public class MoteTracer
    {
        public string Label;
        public Thing EmittedMote;
        public int GraceTicks = 0;
        public bool CoexistsWithSame = false;
        public bool CoexistsWithOther = false;

        public bool HasRemainingGraceTicks => GraceTicks > 0;
        public bool ForbidsCoexistWithSame => !CoexistsWithSame;
        public bool ForbidsCoexistWithOther => !CoexistsWithOther;
        public bool ForbidsCoexistWithAny => ForbidsCoexistWithSame && ForbidsCoexistWithOther;

        public MoteTracer(string nLabel, Thing nThing, int nGrace, bool nCoexistSame, bool nCoexistOther)
        {
            Label = nLabel;
            EmittedMote = nThing;
            GraceTicks = nGrace;
            CoexistsWithSame = nCoexistSame;
            CoexistsWithOther = nCoexistOther;
        }

        public void DecreaseGraceTicks()
        {
            if(HasRemainingGraceTicks)
                GraceTicks--;
        }
    }
}