using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using AlienRace;
using UnityEngine;

namespace MoharHediffs
{
    public static class RandyPickerUtils
    {
        public static float ThingsTotalWeight(this HediffComp_RandySpawnUponDeath comp, List<ThingSettings> TSList)
        {
            string debugStr =
               comp.MyDebug ?
               comp.Pawn.LabelShort + " ThingsTotalWeight " :
               "";

            Tools.Warn(debugStr + " searching total weights thing list", comp.MyDebug);

            float total = 0;

            for (int i = 0; i < TSList.Count; i++)
                total += TSList[i].weight;

            Tools.Warn(debugStr + " found: " + total, comp.MyDebug);

            return total;
        }

        public static bool SameColorAs(this Color colorA, Color colorB)
        {
            bool sameR = Math.Abs(colorA.r - colorB.r) < 0.01;
            bool sameG = Math.Abs(colorA.g - colorB.g) < 0.01;
            bool sameB = Math.Abs(colorA.b - colorB.b) < 0.01;
            bool sameA = Math.Abs(colorA.a - colorB.a) < 0.01;
            return sameR && sameG && sameB && sameA;
        }
        /*
        public static bool DebugSameColorAsWhat(this Color colorA, Color colorB)
        {
            bool sameR = Math.Abs(colorA.r - colorB.r) < 0.01;
            bool sameG = Math.Abs(colorA.g - colorB.g) < 0.01;
            bool sameB = Math.Abs(colorA.b - colorB.b) < 0.01;
            bool sameA = Math.Abs(colorA.a - colorB.a) < 0.01;

            Log.Warning(
                " ColorA:" + colorA +
                " ColorB:" + colorB +
                " R:" + sameR +
                " G:" + sameG+
                " B:" + sameB +
                " A:" + sameA
            );

            return sameR && sameG && sameB && sameA;
        }
        */

        public static Color PickAlienColor(this AlienPartGenerator.AlienComp a, string channelName, int channelNum)
        {
            return channelNum == 1 ? 
                    a.GetChannel(channelName).first :
                    (channelNum == 2 ? a.GetChannel(channelName).second : Color.white)
                    ;
        }

        public static Color PickStuffColor(this ThingDef tDef)
        {
            if (tDef.stuffProps?.color != null)
                return tDef.stuffProps.color;

            ThingDefCountClass butcher = tDef.butcherProducts.FirstOrDefault() ?? null;
            ThingDef butcherProduct = null;

            if (butcher == null)
                return Color.black;

            butcherProduct = butcher.thingDef;

            if ( butcherProduct?.stuffProps?.color == null)
                return Color.black;

            return butcherProduct.stuffProps.color;
        }

        public static List<ThingSettings> ThingSettingsWithColor(this HediffComp_RandySpawnUponDeath comp)
        {
            string debugStr =
                comp.MyDebug ?
                comp.Pawn.LabelShort + " ThingSettingsWithColor -" :
                "";

            Tools.Warn(debugStr + " creating thing list with color", comp.MyDebug);

            if (!comp.HasColorCondition || !comp.Pawn.IsAlien())
            {
                Tools.Warn(debugStr + "Found no color condition or pawn is not alien", comp.MyDebug);
                return null;
            }

            AlienPartGenerator.AlienComp alienComp = Tools.GetAlien(comp.Pawn);
            if (alienComp == null)
            {
                Tools.Warn(debugStr + "Found no AlienPartGenerator.AlienComp", comp.MyDebug);
                return null;
            }

            Tools.Warn(
                debugStr + "colors=>" +
                " skin.first:" + alienComp.GetChannel("skin").first +
                " skin.second:" + alienComp.GetChannel("skin").second
                , comp.MyDebug);

            List<ThingSettings> debugList = comp.FullOptionList.Where(
                t =>
                t.IsThingSpawner && t.HasColorCondition
                ).ToList();

            Tools.Warn(
                    "Option num:" + debugList.Count,
                    comp.MyDebug);

            Color PawnColor = alienComp.GetChannel("skin").first;
            //Color PawnColor = PickAlienColor(alienComp, t.colorCondition.channelName, t.colorCondition.channelNum)
            
            foreach (ThingSettings TS in debugList)
            {
                Tools.Warn(
                    " TS.Def: " + TS.thingToSpawn.defName +
                    "; TS.color: " + TS.thingToSpawn.PickStuffColor() +
                    "; P.color: " + PawnColor +
                    "; equals: " + PawnColor.SameColorAs(TS.thingToSpawn.PickStuffColor())
                    //"; equals what:" + PawnColor.SameColorAsWhat(TS.thingToSpawn.graphicData.color),
                    , comp.MyDebug);
            }
            
            

            List<ThingSettings> answer = new List<ThingSettings>();
            answer = comp.FullOptionList.Where(
                t =>
                t.IsThingSpawner && t.HasColorCondition &&
                PawnColor.SameColorAs(t.thingToSpawn.PickStuffColor())
                /*
                 PickAlienColor(alienComp, t.colorCondition.channelName, t.colorCondition.channelNum).SameColorAs(t.thingToSpawn.graphicData.color)
                */
                ).ToList();

            Tools.Warn(debugStr + "Found " + answer.Count + " things with color", comp.MyDebug);

            return answer;
        }

        public static List<ThingSettings> ThingSettingsWithExclusion(this HediffComp_RandySpawnUponDeath comp, List<ThingSettings> TSList, List<int> AlreadyPickedOptions)
        {
            List<ThingSettings> answer = new List<ThingSettings>();

            answer = comp.Props.settings.things.ListFullCopy();
            foreach(int i in AlreadyPickedOptions)
                answer.RemoveAt(i);

            return answer;
        }

        public static int GetWeightedRandomIndex(this HediffComp_RandySpawnUponDeath comp, List<int> AlreadyPickedOptions)
        {
            if (!comp.Props.settings.HasSomethingToSpawn)
                return -1;

            List<ThingSettings> TSList;
            if(comp.HasColorCondition)
                TSList = comp.ThingSettingsWithColor();
            else
                TSList = comp.FullOptionList;

            if (!AlreadyPickedOptions.NullOrEmpty())
                TSList = comp.ThingSettingsWithExclusion(TSList, AlreadyPickedOptions);

            float DiceThrow = Rand.Range(0, comp.ThingsTotalWeight(TSList));

            for (int i = 0; i < TSList.Count; i++)
            {
                if ((DiceThrow -= TSList[i].weight) < 0)
                {
                    Tools.Warn("GetWeightedRandomIndex : returning thing " + i, comp.MyDebug);
                    if(AlreadyPickedOptions.NullOrEmpty() && !comp.HasColorCondition)
                        return i;
                    else
                    {
                        int normalizedIndex = comp.Props.settings.things.IndexOf(TSList[i]);
                        Tools.Warn("GetWeightedRandomIndex : returning thing " + i + " normalized:" + normalizedIndex, comp.MyDebug);
                        return normalizedIndex;
                    }
                }
            }

            Tools.Warn("GetWeightedRandomIndex : failed to return proper index, returning -1", comp.MyDebug);

            return -1;
        }
    }
}