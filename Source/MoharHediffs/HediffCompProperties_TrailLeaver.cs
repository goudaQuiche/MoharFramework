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
    public class HediffCompProperties_TrailLeaver : HediffCompProperties
    {

        public int period = 15;
        public List <ThingDef> moteDef;
        public FloatRange scale = new FloatRange(.5f, .8f);

        public bool debug = false;
        public bool hideBySeverity = true;

        public HediffCompProperties_TrailLeaver()
        {
            this.compClass = typeof(HediffComp_TrailLeaver);
        }
    }
}