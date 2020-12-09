using Verse;
using RimWorld;
using System.Linq;

namespace MoharCustomHAR
{
    public static class BaseHARConditions
    {
        public static bool ApparelCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            // pawns wears nothing
            if (pawn.Drawer.renderer.graphics.apparelGraphics.NullOrEmpty())
                return true;

            // no restriction in bodyaddon parameters
            if (bodyAddon.hiddenUnderApparelTag.NullOrEmpty() && bodyAddon.hiddenUnderApparelFor.NullOrEmpty())
                return true;

            // pawns wears nothing that invalidates bodyaddon parameters
            if (!pawn.apparel.WornApparel.Any(
                    ap =>
                    ap.def.apparel.bodyPartGroups.Any(
                        bpgd =>
                        bodyAddon.hiddenUnderApparelFor.Contains(bpgd)) ||
                        ap.def.apparel.tags.Any(
                            s => bodyAddon.hiddenUnderApparelTag.Contains(s)
                        )
                    )
                )
                return true;

            return false;
        }

        public static bool PostureCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            // Pawn is standing
            if (pawn.GetPosture() == PawnPosture.Standing)
                return true;

            // Pawn is downed/grounded
            if (bodyAddon.drawnOnGround)
                return true;

            return false;
        }

        public static bool BedCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            // pawn is either in bed, we use core method, or is not in bed and it's ok to draw it
            if (pawn.CurrentBed()?.def.building.bed_showSleeperBody ?? true)
                return true;

            if (bodyAddon.drawnInBed)
                return true;

            return false;
        }

        public static bool BackstoryCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            // no backstory requirement
            if (bodyAddon.backstoryRequirement.NullOrEmpty())
                return true;

            // match with backstory requirments
            if (pawn.story.AllBackstories.Any(b => b.identifier == bodyAddon.backstoryRequirement))
                return true;

            return false;
        }

        public static bool BodyPartCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            // no bodypart linked to the body addon, should always be displayed
            if (bodyAddon.bodyPart.NullOrEmpty())
                return true;

            // bodypart selected dy def or custom label is not missing
            if (pawn.health.hediffSet.GetNotMissingParts().Any(
                    bpr =>
                        bpr.untranslatedCustomLabel == bodyAddon.bodyPart ||
                        bpr.def.defName == bodyAddon.bodyPart
                    )
                )
                return true;

            // if bodyaddon has not a specific display for missing hediff, by default, no display
            if (bodyAddon.hediffGraphics?.Any(bahg => bahg.hediff == HediffDefOf.MissingBodyPart) ?? false)
                return true;

            return false;
        }

        public static bool GenderCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            return pawn.gender == Gender.Female ? bodyAddon.drawForFemale : bodyAddon.drawForMale;
        }

        public static bool BodyTypeCondition(this MoharBodyAddon bodyAddon, Pawn pawn)
        {
            if (bodyAddon.bodyTypeRequirement.NullOrEmpty())
                return true;

            if (pawn.story.bodyType.ToString() == bodyAddon.bodyTypeRequirement)
                return true;

            return false;
        }
    }
}
