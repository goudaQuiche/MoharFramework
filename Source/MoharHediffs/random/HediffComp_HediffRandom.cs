/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using RimWorld;
using System;
using System.Linq;
using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
    public class HediffComp_HediffRandom : HediffComp
    {
        bool myDebug = false;

        public HediffCompProperties_HediffRandom Props => (HediffCompProperties_HediffRandom)this.props;

        public override void CompPostMake()
        {
            //base.CompPostMake();
            if (Props.hideBySeverity)
                parent.Severity = .05f;
            myDebug = Props.debug;
        }

        public bool HasHediff
        {
            get
            {
                return (!Props.hediffPool.NullOrEmpty());
            }
        }

        public void ApplyHediff(Pawn pawn)
        {
            if (Props.bodyDef != null)
                if (pawn.def.race.body != Props.bodyDef)
                {
                    Tools.Warn(pawn.Label + " has not a bodyDef like required: " + pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString(), myDebug);
                    return;
                }

            int randomElementIndex = Rand.RangeInclusive(0, Props.hediffPool.Count());

            HediffDef hediff2use = Props.hediffPool[randomElementIndex];
            if (hediff2use == null)
            {
                Tools.Warn("cant find hediff", myDebug);
                return;
            }

            /*
            IEnumerable<BodyPartDef> myBPDefIE = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b == Props.bodyPartDef);
            if (myBPDefIE.EnumerableNullOrEmpty())
            {
                Tools.Warn("cant find body part def called: " + Props.bodyPartDef.defName, myDebug);
                return;
            }
            
            BodyPartDef myBPDef = myBPDefIE.RandomElement();
            */

            BodyPartDef myBPDef = Props.bodyPartDef[randomElementIndex];

            IEnumerable<BodyPartRecord> myBPIE = pawn.RaceProps.body.GetPartsWithDef(myBPDef);
            if (myBPIE.EnumerableNullOrEmpty())
            {
                Tools.Warn("cant find body part record called: " + myBPDef.defName, myDebug);
                return;
            }
            BodyPartRecord myBP = myBPIE.RandomElement();

            Hediff hediff2apply = HediffMaker.MakeHediff(hediff2use, pawn, myBP);
            if (hediff2apply == null)
            {
                Tools.Warn("cant create hediff "+ hediff2use.defName + " to apply on " + myBPDef.defName, myDebug);
                return;
            }

            pawn.health.AddHediff(hediff2apply, myBP, null);
            Tools.Warn("Succesfully applied " + hediff2use.defName + " to apply on " + myBPDef.defName, myDebug);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            Pawn pawn = parent.pawn;
            if (!Tools.OkPawn(pawn))
                return;

            if (HasHediff)
            {
                ApplyHediff(pawn);
            }

            // suicide
            Tools.DestroyParentHediff(parent, myDebug);
        }
    }
}
