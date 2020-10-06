using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class BodyPartTechHediff
    {
        public static bool TryRegrowProsthetic(this HediffComp_Regeneration RegenHComp, HediffDef ProstheticHediff)
        {
            if (ProstheticHediff == null)
                return false;

            Pawn p = RegenHComp.Pawn;
            BodyPartRecord BPR = RegenHComp.currentHediff.Part;

            Tools.Warn(p.LabelShort + " TryRegrowProsthetic - hediffdef: " + ProstheticHediff?.defName + "; BP: " + BPR?.Label);

            float BPRMaxHealth = BPR.def.GetMaxHealth(RegenHComp.Pawn);
            float PawnBodyPartRatio = BPRMaxHealth / RegenHComp.BodyPartsHealthSum;

            MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(p, BPR, p.Position, p.Map);
            p.health.AddHediff(ProstheticHediff, BPR);

            if(RegenHComp.HasLimits)
                RegenHComp.TreatmentPerformedQuality += PawnBodyPartRatio * 10;

            return p.health.hediffSet.HasHediff(ProstheticHediff, BPR);
        }

        public static HediffDef TryFindBodyPartProsthetic(this HediffComp_Regeneration RegenHComp)
        {
            if (RegenHComp.Props.BodyPartRegenParams.techHediffTag.NullOrEmpty())
                return null;

            string techHediffTag = RegenHComp.Props.BodyPartRegenParams.techHediffTag;
            BodyPartRecord BPR = RegenHComp.currentHediff.Part;

            Tools.Warn("Looking for one recipe with techHediff=" + techHediffTag + " and BP=" + BPR?.Label, RegenHComp.MyDebug);

            IEnumerable<ThingDef> Prosthetics = DefDatabase<ThingDef>.AllDefs.Where(
                    TD => 
                    !TD.techHediffsTags.NullOrEmpty() &&
                    TD.techHediffsTags.Contains(techHediffTag)
            );

            if (Prosthetics.EnumerableNullOrEmpty())
            {
                Tools.Warn("TryFindBodyPartProsthetic - found no prosthetic with techHediff=" + techHediffTag, RegenHComp.MyDebug);
            }

            IEnumerable <RecipeDef> recipes = DefDatabase<RecipeDef>.AllDefs.Where(
                r =>
                !r.appliedOnFixedBodyParts.NullOrEmpty() &&
                r.appliedOnFixedBodyParts.Contains(BPR.def) &&

                !r.fixedIngredientFilter.AllowedThingDefs.EnumerableNullOrEmpty() &&
                r.fixedIngredientFilter.AllowedThingDefs.Intersect(Prosthetics).Count()>0
            );

            if (recipes.EnumerableNullOrEmpty())
            {
                Tools.Warn("TryFindBodyPartProsthetic - empty recipes ", RegenHComp.MyDebug);
                return null;
            }
                

            if(recipes.Count() > 1)
            {
                Tools.Warn("Found more than one recipe with techHediff=" + techHediffTag + " and BP=" + BPR.Label, RegenHComp.MyDebug);
                foreach (RecipeDef RD in recipes)
                    Tools.Warn(RD.defName, RegenHComp.MyDebug);
            }

            HediffDef answer = recipes.RandomElement().addsHediff;

            Tools.Warn("TryFindBodyPartProsthetic - Found " + answer.defName, RegenHComp.MyDebug);

            return answer;
        }

        public static void RecursiveHediffRemoval(this Pawn p, BodyPartRecord BPR, HediffDef hediffDef, bool myDebug=false)
        {
            IEnumerable<Hediff> hl = p.health.hediffSet.hediffs.Where(
                hd =>
                hd.Part != null
                && hd.Part == BPR &&
                hd.def == hediffDef
            );

            if (hl.EnumerableNullOrEmpty())
                return;

            Hediff h = hl.FirstOrFallback();
            if(h !=null)
                p.health.RemoveHediff(h);

            Tools.Warn(p.LabelShort + " RecursiveHediffRemoval - Removing " + h.def.defName + " from " + BPR.Label, myDebug);

            foreach (BodyPartRecord allChildren in BPR.parts)
            {
                p.RecursiveHediffRemoval(allChildren, hediffDef, myDebug);
            }
        }

        public static void RecursiveHediff(this Pawn p, BodyPartRecord BPR, float severity, HediffDef hediffDef)
        {
            Hediff BarelyAliveBP = HediffMaker.MakeHediff(hediffDef, p, BPR);
            BarelyAliveBP.Severity = severity;

            p.health.AddHediff(BarelyAliveBP, BPR);

            if (BPR.parts.NullOrEmpty())
                return;

            foreach (BodyPartRecord allChildren in BPR.parts)
            {
                p.RecursiveHediff(allChildren, severity, hediffDef);
            }
        }

        public static float GetAllChildrenHealth(this Pawn p, BodyPartRecord BPR)
        {
            float answer = BPR.def.GetMaxHealth(p);

            if (BPR.parts.NullOrEmpty())
                return answer;

            foreach(BodyPartRecord allChildren in BPR.parts)
            {
                answer += p.GetAllChildrenHealth(allChildren);
            }

            return answer;
        }

        public static float GetAllBodyPartsHealthSum(this Pawn p)
        {
            BodyPartRecord CoreBP = p.RaceProps.body.corePart;

            return p.GetAllChildrenHealth(CoreBP);
        }
    }
}
