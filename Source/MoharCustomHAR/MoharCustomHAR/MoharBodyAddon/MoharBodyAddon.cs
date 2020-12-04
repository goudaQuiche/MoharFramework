using Verse;
using RimWorld;
using AlienRace;
using System.Linq;
using System.Collections.Generic;

namespace MoharCustomHAR
{
    public class MoharBodyAddon : AlienPartGenerator.BodyAddon
    {
        public bool drawIfDirectMissingChild = true;

        public override bool CanDrawAddon(Pawn pawn)
        {
            return 
                this.ApparelCondition(pawn) && 
                this.PostureCondition(pawn) &&
                this.BedCondition(pawn) && 
                this.BackstoryCondition(pawn) &&
                this.BodyPartCondition(pawn) &&
                this.BodyPartChildrenCondition(pawn) &&
                this.GenderCondition(pawn) &&
                this.BodyTypeCondition(pawn)
               ;
        }
    }
    
}
