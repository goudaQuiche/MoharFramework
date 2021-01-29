using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;


namespace MoharHediffs
{
    public class HediffCondition
    {
        public PawnCondition pawn;
        public BodyPartCondition bodyPart;

        public bool HasPawnCondition => pawn != null;
        public bool HasBodypartCondition => bodyPart != null && bodyPart.HasBPCondition;
        /*
        public HediffCondition( PawnCondition nPawn,  BodyPartCondition nBPC)
        {
            pawn = nPawn;
            bodypart = nBPC;
        }
        */
    }
    public class PawnCondition
    {
        public List<ThingDef> race;
        public FloatRange ageRange = new FloatRange(0, 999);
        public List<Gender> gender;

        public bool HasRace => !race.NullOrEmpty();
        public bool HasGender => !gender.NullOrEmpty();

        /*
        public PawnCondition(List<ThingDef> nRace, FloatRange nAgeRange, List<Gender> nGender)
        {
            race = nRace;
            ageRange = nAgeRange;
            gender = nGender;
        }
        */
        
    }
    public class BodyPartCondition
    {
        public List<string> bodyPartLabel;
        public List<BodyPartDef> bodyPartDef;
        public List<BodyPartTagDef> bodyPartTag;

        public bool HasLabel => !bodyPartLabel.NullOrEmpty();
        public bool HasDef => !bodyPartDef.NullOrEmpty();
        public bool HasTag => !bodyPartTag.NullOrEmpty();

        public bool HasBPCondition => HasLabel || HasDef || HasTag;
        /*
        public BodyPartCondition(List<string> nBodyPartLabel, List<BodyPartDef> nBodyPartDef, List<string> nBodyPartTag)
        {
            bodyPartLabel = nBodyPartLabel;
            bodyPartDef = nBodyPartDef;
            bodyPartTag = nBodyPartTag;
        }
        */
   
    }
}
