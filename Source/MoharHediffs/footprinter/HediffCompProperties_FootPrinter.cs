/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using Verse;
using RimWorld;

namespace MoharHediffs
{
    public class HediffCompProperties_FootPrinter : HediffCompProperties
    {

        public int period = 15;

        public ThingDef moteFootPrintDef = null;

        public bool debug = false;

        public HediffCompProperties_FootPrinter()
        {
            this.compClass = typeof(HediffComp_FootPrinter);
        }
    }
}