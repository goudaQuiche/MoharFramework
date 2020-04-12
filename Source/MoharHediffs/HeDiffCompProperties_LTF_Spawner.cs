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
	public class HeDiffCompProperties_LTF_Spawner : HediffCompProperties
	{
        // tick
        public float tickerLimit = 1f;

        //what
        public ThingDef thingToSpawn ; //= "UnfertilizedEgg";
		public int spawnCount = 1;
        public bool animalThing = false;
        public string animalName = "Chicken";


        //when
        public float minDaysB4Next = 1f;
        public float maxDaysB4Next = 2f;
        public float graceDays = .5f;

        //how
        public int spawnMaxAdjacent = -1;
		public bool spawnForbidden = false;

        // condition
        public bool hungerRelative = false;
        public bool healthRelative = false;
        public bool randomGrace = false;
        public bool mandatoryGrace = false;

        //log
        public string spawnVerb = "delivery";
		
		
		public HeDiffCompProperties_LTF_Spawner()
		{
			this.compClass = typeof(HeDiffComp_LTF_Spawner);
		}
	}
}
