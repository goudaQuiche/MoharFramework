/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using Verse;
using System.Collections.Generic;
using RimWorld;

namespace MoharHediffs
{
    public class HediffCompProperties_RainbowTrail : HediffCompProperties
    {

        public int period = 15;

        public List <ThingDef> motePurpleDef;
        public List<ThingDef> moteBlueDef;
        public List<ThingDef> moteGreenDef;
        public List<ThingDef> moteYellowDef;
        public List<ThingDef> moteOrangeDef;
        public List<ThingDef> moteRedDef;

        public float staySameColorChance = .5f;
        public int maxTimesSameColor = 3;
        public int minTimesSameColor = 1;

        public FloatRange scale = new FloatRange(.5f, .8f);

        public bool debug = false;
        public bool hideBySeverity = true;

        public HediffCompProperties_RainbowTrail()
        {
            this.compClass = typeof(HediffComp_RainbowTrail);
        }
    }
}