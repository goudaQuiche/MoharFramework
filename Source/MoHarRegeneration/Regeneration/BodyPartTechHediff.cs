using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class BodyPartTechHediff
    {
        public static bool TryRegrowProsthetic(this HediffComp_Regeneration comp, HediffDef ProstheticHediff)
        {
            if (ProstheticHediff == null)
                return false;

            Pawn p = comp.Pawn;
            BodyPartRecord BPR = comp.currentHediff.Part;

            if (comp.MyDebug)
                Log.Warning(p.LabelShort + " TryRegrowProsthetic - hediffdef: " + ProstheticHediff?.defName + "; BP: " + BPR?.Label);

            float BPRMaxHealth = BPR.def.GetMaxHealth(comp.Pawn);
            float PawnBodyPartRatio = BPRMaxHealth / comp.BodyPartsHealthSum;

            MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(p, BPR, p.Position, p.Map);
            p.health.AddHediff(ProstheticHediff, BPR);

            if(comp.Props.BodyPartRegenParams.prostheticHediff != null)
            {
                float TheoricSeverity = BPRMaxHealth * (1 - comp.Props.BodyPartRegenParams.prostheticMaxHealth);
                Hediff BarelyAliveBP = HediffMaker.MakeHediff(comp.Props.BodyPartRegenParams.prostheticHediff, comp.Pawn, BPR);
                BarelyAliveBP.Severity = TheoricSeverity;
                comp.Pawn.health.AddHediff(BarelyAliveBP, BPR);
            }
            
            if(comp.HasLimits)
                comp.TreatmentPerformedQuality += PawnBodyPartRatio * 10;

            return p.health.hediffSet.HasHediff(ProstheticHediff, BPR);
        }

        public static HediffDef TryFindBodyPartProsthetic(this HediffComp_Regeneration RegenHComp)
        {
            if (RegenHComp.Props.BodyPartRegenParams.techHediffTag.NullOrEmpty())
                return null;

            string techHediffTag = RegenHComp.Props.BodyPartRegenParams.techHediffTag;
            BodyPartRecord BPR = RegenHComp.currentHediff.Part;

            if(RegenHComp.MyDebug)
                Log.Warning("Looking for one recipe with techHediff=" + techHediffTag + " and BP=" + BPR?.Label);

            IEnumerable<ThingDef> Prosthetics = DefDatabase<ThingDef>.AllDefs.Where(
                    TD => 
                    !TD.techHediffsTags.NullOrEmpty() &&
                    TD.techHediffsTags.Contains(techHediffTag)
            );

            if (Prosthetics.EnumerableNullOrEmpty())
            {
                if (RegenHComp.MyDebug)
                    Log.Warning("TryFindBodyPartProsthetic - found no prosthetic with techHediff=" + techHediffTag);
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
                if (RegenHComp.MyDebug)
                    Log.Warning("TryFindBodyPartProsthetic - empty recipes ");
                return null;
            }
                

            if(RegenHComp.MyDebug && recipes.Count() > 1)
            {
                    Log.Warning("Found more than one recipe with techHediff=" + techHediffTag + " and BP=" + BPR.Label);
                    foreach (RecipeDef RD in recipes)
                        Log.Warning(RD.defName);
            }

            HediffDef answer = recipes.RandomElement().addsHediff;

            if (RegenHComp.MyDebug)
                Log.Warning("TryFindBodyPartProsthetic - Found " + answer.defName);

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

            if (myDebug)
                Log.Warning(p.LabelShort + " RecursiveHediffRemoval - Removing " + h.def.defName + " from " + BPR.Label);

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
