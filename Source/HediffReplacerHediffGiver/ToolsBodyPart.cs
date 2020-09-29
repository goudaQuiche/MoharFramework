using RimWorld;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;


namespace HEREHEGI
{
    public static class ToolsBodyPart
    {
        public static bool HasHediff(this Pawn pawn, HediffDef hediffDef, BodyPartRecord BPR)
        {
            if (BPR == null)
                return pawn.health.hediffSet.HasHediff(hediffDef);

            if (pawn.health.hediffSet.HasHediff(HediffDefOf.MissingBodyPart, BPR))
                return false;

            return pawn.health.hediffSet.HasHediff(hediffDef, BPR);
        }

        public static BodyPartRecord GetBPRecord(this Pawn pawn, string BPPartDefName, bool myDebug = false)
        {
            IEnumerable<BodyPartDef> BPDefIE = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b.defName == BPPartDefName);
            if (BPDefIE.EnumerableNullOrEmpty())
            {
                Tools.Warn(pawn.Label+" - GetBPRecord - did not find any " + BPPartDefName, myDebug);
                return null;
            }
                
            BodyPartDef BPDef = BPDefIE.RandomElement();
            pawn.RaceProps.body.GetPartsWithDef(BPDef).TryRandomElement(out BodyPartRecord bodyPart);

            Tools.Warn(pawn.Label + "GetBPRecord - DID find " + BPPartDefName, myDebug);
            return bodyPart;
        }

        public static bool ApplyHediffOnBodyPartTag(Pawn pawn, BodyPartTagDef BPTag, HediffDef hediffDef, bool myDebug)
        {
            pawn.RaceProps.body.GetPartsWithTag(BPTag).TryRandomElement(out BodyPartRecord bodyPart);
            if (bodyPart == null)
            {
                Tools.Warn("null body part", myDebug);
                return false;
            }

            Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn, bodyPart);
            if (hediff == null)
            {
                Tools.Warn("hediff maker null", myDebug);
                return false;
            }

            pawn.health.AddHediff(hediff, bodyPart, null);

            return true;
        }

    }
}
