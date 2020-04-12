/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using Verse;

namespace MoharHediffs
{
    public class HediffCompProperties_Steamer : HediffCompProperties
    {

        public int MinTicksBetweenSprays = 200;
        public int MaxTicksBetweenSprays = 400;

        public int MinSprayDuration = 60;
        public int MaxSprayDuration = 120;

        //public float SprayThickness = 0.6f;

        public float puffingChance = 1f;

        public HediffCompProperties_Steamer()
        {
            this.compClass = typeof(HediffComp_Steamer);
        }
    }
}