using Verse;
using RimWorld;
using AlienRace;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MoharCustomHAR
{
    using BAP = AlienPartGenerator.BodyAddonPrioritization;
    using BAHG = AlienPartGenerator.BodyAddonHediffGraphic;
    using BAHSG = AlienPartGenerator.BodyAddonHediffSeverityGraphic;
    using BABSG = AlienPartGenerator.BodyAddonBackstoryGraphic;
    using AlienComp = AlienPartGenerator.AlienComp;
    using ExposableValueTuple = AlienPartGenerator.ExposableValueTuple<Color, Color>;

    public static class OverrideSubMethods
    {
        public static BABSG PawnHasBackstoryGraphics(this List<BABSG> backstoryGraphics, Pawn pawn)
        {
            return backstoryGraphics?.FirstOrDefault(babgs => pawn.story.AllBackstories.Any(bs => bs.identifier == babgs.backstory));
        }

        public static bool HediffPartIsNullOrBodyPartIsNullOrIsSearchedBodyPart(this Hediff h, string HarBodyPart)
        {
            if (h.Part == null)
                return true;
            if (HarBodyPart.NullOrEmpty())
                return true;
            if (h.Part.IsSearchedBodyPart(HarBodyPart))
                return true;

            return false;
        }
        public static bool IsRelevantHediff(this Hediff h, BAHG bahg, string HarBodyPart)
        {
            return h.def == bahg.hediff && h.HediffPartIsNullOrBodyPartIsNullOrIsSearchedBodyPart(HarBodyPart);
        }
        
        public static Shader DetermineShaderType(this string path, Shader shaderParameter)
        {
            if (ContentFinder<Texture2D>.Get(path + "_northm", reportFailure: false) == null)
                return shaderParameter;

            return ShaderDatabase.CutoutComplex;
        }
        public static int DetermineSharedIndex(this bool linkVariantIndexWithPrevious, int sharedIndex, int variantCounting, out int newSharedIndex)
        {
            if (linkVariantIndexWithPrevious)
            {
                newSharedIndex = sharedIndex;
                return sharedIndex % variantCounting;
            }

            newSharedIndex = Rand.Range(0, variantCounting);
            return newSharedIndex ;
        }

        public static int DetermineVariantIndex(this int? savedIndex, int sharedIndex, int variantCounting, bool linkVariantIndexWithPrevious, out int newSharedIndex)
        {
            if (savedIndex.HasValue)
            {
                newSharedIndex = savedIndex.Value % variantCounting;
                return newSharedIndex;
            }

            return linkVariantIndexWithPrevious.DetermineSharedIndex(sharedIndex, variantCounting, out newSharedIndex);
        }

        public static string VariantIndexToString(this bool linkVariantIndexWithPrevious, int? savedIndex, int sharedIndex, int variantCounting, out int newSharedIndex, out int newVariantIndex)
        {
            if ((newVariantIndex = savedIndex.DetermineVariantIndex(sharedIndex, variantCounting, linkVariantIndexWithPrevious, out newSharedIndex)) == 0)
                return "";

            return newVariantIndex.ToString();
        }

        public static bool HandlePrioritization(this MoharBodyAddon MBA, Pawn pawn, out string returnPath, out int variantCounting)
        {
            returnPath = "";
            variantCounting = 0;

            foreach (BAP prio in MBA.Prioritization)
            {
                switch (prio)
                {
                    case BAP.Backstory:
                        if (MBA.backstoryGraphics.PawnHasBackstoryGraphics(pawn) is BABSG babg)
                        {
                            returnPath = babg.path;
                            variantCounting = babg.variantCount;
                        }
                        break;

                    case BAP.Hediff:
                        if (MBA.hediffGraphics.NullOrEmpty())
                            break;

                        foreach (BAHG bahg in MBA.hediffGraphics)
                        {
                            foreach (Hediff h in pawn.health.hediffSet.hediffs.Where(h => h.IsRelevantHediff(bahg, MBA.bodyPart)))
                            {
                                returnPath = bahg.path;
                                variantCounting = bahg.variantCount;

                                if (bahg.severity.NullOrEmpty())
                                    break;

                                foreach (BAHSG bahsg in bahg.severity.Where(s => h.Severity >= s.severity))
                                {
                                    returnPath = bahsg.path;
                                    variantCounting = bahsg.variantCount;
                                    break;
                                }
                                break;
                            }
                        }

                        break;

                    default:
                        throw new ArrayTypeMismatchException();
                }

                if (!returnPath.NullOrEmpty())
                    break;
            }

            return !returnPath.NullOrEmpty();
        }
    }
}
