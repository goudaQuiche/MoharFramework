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
    public class HeDiffComp_HediffExclusive : HediffComp
    {
        const int tickLimiterModulo = 60;
        bool MyDebug => Props.debug;

        public HeDiffCompProperties_HediffExclusive Props => (HeDiffCompProperties_HediffExclusive)this.props;

        public bool HasHediffToNullify => (!Props.hediffToNullify.NullOrEmpty());
        public bool HasHediffPatternToNullify =>!Props.hediffPatternToNullify.NullOrEmpty();
        public bool HasHediffToApply => Props.hediffToApply != null;

        bool HasWhiteList => !Props.bodyDefWhiteList.NullOrEmpty();
        bool HasBlackList => !Props.bodyDefBlackList.NullOrEmpty();

        bool WhiteListCompliant => HasWhiteList ? Props.bodyDefWhiteList.Contains(Pawn.def.race.body) : true;
        bool BlackListCompliant => HasBlackList ? !Props.bodyDefBlackList.Contains(Pawn.def.race.body) : true;
        bool HasAccessList => HasWhiteList || HasBlackList;

        string DebugStr => MyDebug ? $"{Pawn.LabelShort} HediffExclusive {parent.def.defName} - " : "";

        private bool PatternMatch(string MyHediffDefname)
        {
            foreach(string cur in Props.hediffPatternToNullify)
            {
                if (MyHediffDefname.Contains(cur))
                    return true;
            }
            return false;
        }

        public void NullifyHediff()
        {
            int i = 0;

            foreach (Hediff curHediff in Pawn.health.hediffSet.hediffs)
            {
                Tools.Warn(Pawn.Label + " hediff #" + i + ": " + curHediff.def.defName, MyDebug);

                int j = 0;
                foreach (HediffDef curHediffToNullify in Props.hediffToNullify)
                {
                    Tools.Warn(" Props.hediffToNullify #" + j + ": " + curHediffToNullify, MyDebug);

                    if (curHediff.def == curHediffToNullify && Props.hediffToApply != curHediffToNullify)
                    {
                        //pawn.health.RemoveHediff(curHediff);
                        curHediff.Severity = 0;
                        Tools.Warn(curHediff.def.defName + " removed", MyDebug);
                    }
                    j++;
                }
                i++;
            }
        }

        public void PatternNullifyHediff()
        {
            int i = 0;

            foreach (Hediff curHediff in Pawn.health.hediffSet.hediffs)
            {
                if(MyDebug)
                    Log.Warning(Pawn.LabelShort + " hediff #" + i + ": " + curHediff.def.defName);

                int j = 0;
                foreach (string curHediffToNullify in Props.hediffPatternToNullify)
                {
                    if (MyDebug)
                        Log.Warning(" Props.hediffPatternToNullify #" + j + ": " + curHediffToNullify);

                    if (PatternMatch(curHediff.def.defName))
                    {
                        curHediff.Severity = 0;
                        Tools.Warn(curHediff.def.defName + " severity = 0", MyDebug);
                    }
                    j++;
                }
                i++;
            }
        }

        public void ApplyHediff()
        {
            HediffDef hediff2use = Props.hediffToApply;
            if (hediff2use == null)
            {
                if (MyDebug)
                    Log.Warning("cant find hediff called: " + Props.hediffToApply);

                return;
            }

            BodyPartDef myBPDef = DefDatabase<BodyPartDef>.AllDefs.Where((BodyPartDef b) => b == Props.bodyPartDef).RandomElementWithFallback();

            BodyPartRecord myBP = null;
            if (myBPDef != null)
            {
                myBP = Pawn.RaceProps.body.GetPartsWithDef(myBPDef).RandomElementWithFallback();
                if (myBP == null)
                {
                    if (MyDebug)
                        Log.Warning("cant find body part record called: " + Props.bodyPartDef.defName);
                    return;
                }
            }

            Hediff hediff2apply = HediffMaker.MakeHediff(hediff2use, Pawn, myBP);
            if (hediff2apply == null)
            {
                if (MyDebug)
                    Log.Warning("cant create hediff " + hediff2use.defName + " to apply on " + Props.bodyPartDef.defName);
                return;
            }

            Pawn.health.AddHediff(hediff2apply, myBP, null);
        }

        public bool CheckProps()
        {
            string fctN = DebugStr + "ApplyHediff - ";

            if (Props.bodyDef != null)
                if (Pawn.def.race.body != Props.bodyDef)
                {
                    if (MyDebug)
                        Log.Warning(Pawn.Label + " has not a bodyDef like required: " + Pawn.def.race.body.ToString() + "!=" + Props.bodyDef.ToString());
                    return false;
                }

            if (HasAccessList)
            {
                bool BlackIsOk = BlackListCompliant;
                bool WhiteIsOk = WhiteListCompliant;
                if (!BlackIsOk || !WhiteIsOk)
                {
                    if (MyDebug)
                    {
                        Log.Warning(
                            fctN +
                            (HasWhiteList ? $"Props.BodyDefWhiteList contains {Props.bodyDefWhiteList.Count} elements" : "No whitelist") + ", compliant: " + WhiteIsOk +
                            "; " + (HasBlackList ? $"Props.BodyDefBlackList contains {Props.bodyDefBlackList.Count} elements" : "No blacklist") + ", compliant:" + BlackIsOk
                        );
                    }
                    return false;
                }
                else
                {
                    if (MyDebug)
                        Log.Warning(fctN + " AccessList compliant ok");
                }
            }
            return true;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Negligible())
                return;

            if (CheckProps())
            {
                if (HasHediffToNullify)
                    NullifyHediff();
                if (HasHediffPatternToNullify)
                    PatternNullifyHediff();

                if (HasHediffToApply)
                {
                    ApplyHediff();
                }
            }

            // suicide
            Tools.DestroyParentHediff(parent, MyDebug);
        }

        public override string CompTipStringExtra
        {
            get
            {
                string result = string.Empty;
                
                result += "This should disappear very fast";
                
                return result;
            }
        }
    }
}
