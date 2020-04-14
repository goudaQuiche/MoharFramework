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
	public class HediffCompProperties_Spawner : HediffCompProperties
	{
        // tick
        //public float tickerLimit = 1f;

        //what
        public ThingDef thingToSpawn ; //= "UnfertilizedEgg";
		public int spawnCount = 1;

        public bool animalThing = false;
        public PawnKindDef animalToSpawn;
        public bool factionOfPlayerAnimal = false;
        
        //when
        public float minDaysB4Next = 1f;
        public float maxDaysB4Next = 2f;

        public float randomGrace = 0f;
        public float graceDays = .5f;

        //how
        public int spawnMaxAdjacent = -1;
		public bool spawnForbidden = false;

        // condition
        public bool hungerRelative = false;
        public bool healthRelative = false;

        //AgeWieghted
        public bool ageWeightedQuantity = false;
        public bool ageWeightedPeriod = false;
        //normal behavior = the older, the less often, the smaller quantities
        // normal 
        public bool olderSmallerPeriod = false;
        public bool olderBiggerQuantity = false;

        // quantity to spawn goes from spawnCount to spawnCount*(1+ageRatio) (almost always < 2*spawnCount)
        // [spawnCount; 2*spawnCount]
        // pawns always have a positive age and are often adult

        // if exponentialQuantity, quantity goes from spawnCount to spawnCount * (1 + 1/ageRatio) (meaning if ageRatio == .2 => quantity = spawnCount*6)
        public bool exponentialQuantity = false;
        public int exponentialRatioLimit = 15;

        //log
        public string spawnVerb = "delivery";

        public bool debug = false;

        public HediffCompProperties_Spawner()
		{
			this.compClass = typeof(HediffComp_Spawner);
		}
	}
}
