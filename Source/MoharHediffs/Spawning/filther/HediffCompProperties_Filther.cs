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
    public class HediffCompProperties_Filther : HediffCompProperties
    {

        public int MinTicksBetweenSprays = 60;
        public int MaxTicksBetweenSprays = 120;

        public ThingDef filthDef = null;

        public bool debug = false;

        public HediffCompProperties_Filther()
        {
            this.compClass = typeof(HediffComp_Filther);
        }
    }
}