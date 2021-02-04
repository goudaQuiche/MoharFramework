using Verse;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using AlienRace;

namespace DefGen
{
    [StaticConstructorOnStartup]
    public static class AddApparelRestrictionToAlienDef
    {
        static readonly bool MyDebug = false;

        static bool IsBlacklistedApparel(this ThingDef td, RaceRestrictionItem RRT)
        {
            if (RRT.HasBodyPartGroupDef && !td.apparel.bodyPartGroups.NullOrEmpty() && !td.apparel.bodyPartGroups.Intersect(RRT.bodyPartGroupDefs).EnumerableNullOrEmpty())
                return true;

            if (RRT.HasLayerDef && !td.apparel.layers.NullOrEmpty() && !td.apparel.layers.Intersect(RRT.apparelLayerDef).EnumerableNullOrEmpty())
                return true;

            if (RRT.HasTags && !td.apparel.tags.NullOrEmpty() && !td.apparel.tags.Intersect(RRT.apparelTags).EnumerableNullOrEmpty())
                return true;

            if (RRT.HasThingCategoryDef && !td.thingCategories.NullOrEmpty() && !td.thingCategories.Intersect(RRT.thingCategoryDefs).EnumerableNullOrEmpty())
                return true;

            return false;
        }

        static AddApparelRestrictionToAlienDef()
        {
            string report = "MoharFW - AddApparelRestrictionToAlienDef";

            if (!StaticCheck.IsOk)
                return;

            IEnumerable<RaceRestrictionItem> restrictions = DefDatabase<ApparelBlacklistDef>.AllDefs?.SelectMany(b => b.restrictions);
            if (restrictions.EnumerableNullOrEmpty())
            {
                if (MyDebug) Log.Warning(report + " found no restriction.");
                return;
            }

            foreach (RaceRestrictionItem cur in restrictions)
            {
                ThingDef_AlienRace MyRace = DefDatabase<ThingDef_AlienRace>.AllDefs.Where(r => r == cur.race).FirstOrFallback();
                if (MyRace == null)
                {
                    if (cur.debug) Log.Warning(report + " could not find " + cur.race.defName);
                    continue;
                }

                int apparelNum= DefDatabase<ThingDef>.AllDefs.Where(td => td.IsApparel).EnumerableCount();
                IEnumerable <ThingDef> WhitelistApparels = DefDatabase<ThingDef>.AllDefs.Where(td => td.IsApparel && !td.IsBlacklistedApparel(cur));
                if (WhitelistApparels.EnumerableNullOrEmpty())
                {
                    if (cur.debug) Log.Warning(report + " could not find any apparel to restrict for " + cur.race.defName);
                }

                if (MyRace.alienRace.raceRestriction == null)
                    MyRace.alienRace.raceRestriction = new RaceRestrictionSettings();
                if (MyRace.alienRace.raceRestriction.whiteApparelList == null)
                    MyRace.alienRace.raceRestriction.whiteApparelList = new List<ThingDef>();
                if (MyRace.alienRace.raceRestriction.apparelList == null)
                    MyRace.alienRace.raceRestriction.apparelList = new List<ThingDef>();
                
                if (!MyRace.alienRace.raceRestriction.onlyUseRaceRestrictedApparel)
                {
                    if (cur.debug) Log.Warning(report + " " + cur.race.defName + " - had to enable onlyUseRaceRestrictedApparel");
                    MyRace.alienRace.raceRestriction.onlyUseRaceRestrictedApparel = true;
                }

                int i = 0;
                //foreach (ThingDef td in WhitelistApparels.Except(MyRace.alienRace.raceRestriction.apparelList).Union(WhitelistApparels.Except(MyRace.alienRace.raceRestriction.apparelList)))
                foreach (ThingDef td in WhitelistApparels)
                {
                    if (cur.debug) Log.Warning(cur.race.defName + " adding :" + td + " to whitelist");
                    //Dictionary<ThingDef, List<ThingDef_AlienRace>> dict = myObject.GetType();

                    Dictionary<ThingDef, List<ThingDef_AlienRace>> regularDict = RaceRestrictionSettings.apparelRestrictionDict;
                    List<ThingDef_AlienRace> raceL = regularDict.TryGetValue(td);
                    if (raceL.NullOrEmpty())
                    {
                        raceL = new List<ThingDef_AlienRace>();
                    }
                    raceL.Add(MyRace);
                    RaceRestrictionSettings.apparelRestrictionDict.SetOrAdd(td, raceL);
                    //regularDict.SetOrAdd(td, raceL);

                    Dictionary<ThingDef, List<ThingDef_AlienRace>> whiteDict = RaceRestrictionSettings.apparelWhiteDict;
                    List<ThingDef_AlienRace> raceL2 = regularDict.TryGetValue(td);
                    if (raceL2.NullOrEmpty())
                    {
                        raceL2 = new List<ThingDef_AlienRace>();
                    }
                    raceL2.Add(MyRace);
                    RaceRestrictionSettings.apparelRestrictionDict.SetOrAdd(td, raceL);

                    i++;
                }
                /*
                foreach (ThingDef td in BlacklistApparels)
                {
                    if (cur.debug) Log.Warning(cur.race.defName + " adding :" + td + " to whitelist");

                    MyRace.alienRace.raceRestriction.apparelList.Add(td);
                    MyRace.alienRace.raceRestriction.whiteApparelList.Add(td);
                }
                */

                if (cur.reportResult) Log.Message(report + " whitelisted " + i + " apparel" + (i > 1 ? "s" : "") + " over " + apparelNum + " for " + cur.race.defName + "(blacklisted " + (apparelNum - i) + ")");
                if (cur.debug)
                {
                    IEnumerable<ThingDef> BlacklistApparels = DefDatabase<ThingDef>.AllDefs.Where(td => td.IsApparel && td.IsBlacklistedApparel(cur));
                    if (BlacklistApparels.EnumerableNullOrEmpty())
                    {
                        Log.Warning(report + " could not find any apparel to restrict for " + cur.race.defName);
                    }
                    foreach (ThingDef td in BlacklistApparels)
                        Log.Warning(td + " blacklisted");
                }
            }
        }
    }
}
