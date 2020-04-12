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
        public ThingDef thingToSpawn; //= "UnfertilizedEgg";
        public int spawnCount = 1;

        public bool animalThing = false;
        public string animalName = "Chicken";
        public bool factionOfPlayerAnimal = false;

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
        public float randomGrace = 0f;
        public bool mandatoryGrace = false;

        //AgeWieghted
        public bool ageWeighted = false;
        //normal behavior = the older, the less often, the smaller quantities
        // normal 
        public bool olderMoreOften = false;
        public bool olderBiggerQuantity = false;

        // if relativeQuantity false, quantity to spawn goes from spawnCount to ageRatio*spawnCount (often less than 2)
        // [spawnCount; 2*spawnCount]
        // if relativeQuantity true, quantity = spawnCount*ageRatio, quantity will always be lower than spawnCount
        // [spawnCount*ageRatio;spawnCount] 
        // pawns always have a positive age and are often adult
        public bool relativeQuantity = false;
        // if true, quantity goes from spawnCount to spawnCount * (1/ageRatio) (meaning if ageRatio == .2 => quantity = spawnCount*5)
        public bool exponentialQuantity = false;
        public int exponentialRatioLimit = 15;

        //log
        public string spawnVerb = "delivery";

        public HediffCompProperties_Spawner()
        {
            this.compClass = typeof(HediffComp_Spawner);
        }
    }
}
