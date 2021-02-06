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

            if(comp.MyDebug)Log.Warning(debugStr + " searching total weights thing list");

            float total = 0;

            for (int i = 0; i < TSList.Count; i++)
                total += TSList[i].weight;

            if(comp.MyDebug)Log.Warning(debugStr + " found: " + total);

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

            if(comp.MyDebug)Log.Warning(debugStr + " creating thing list with color");

            if (!comp.HasColorCondition || !comp.Pawn.IsAlien())
            {
                if(comp.MyDebug)Log.Warning(debugStr + "Found no color condition or pawn is not alien");
                return null;
            }

            AlienPartGenerator.AlienComp alienComp = Tools.GetAlien(comp.Pawn);
            if (alienComp == null)
            {
                if(comp.MyDebug)Log.Warning(debugStr + "Found no AlienPartGenerator.AlienComp");
                return null;
            }

            if(comp.MyDebug)Log.Warning(
                debugStr + "colors=>" +
                " skin.first:" + alienComp.GetChannel("skin").first +
                " skin.second:" + alienComp.GetChannel("skin").second
                );

            List<ThingSettings> debugList = comp.FullOptionList.Where(
                t =>
                t.IsThingSpawner && t.HasColorCondition
                ).ToList();

            if(comp.MyDebug)Log.Warning("Option num:" + debugList.Count);

            Color PawnColor = alienComp.GetChannel("skin").first;
            //Color PawnColor = PickAlienColor(alienComp, t.colorCondition.channelName, t.colorCondition.channelNum)
            
            foreach (ThingSettings TS in debugList)
            {
                if(comp.MyDebug)Log.Warning(
                    " TS.Def: " + TS.thingToSpawn.defName +
                    "; TS.color: " + TS.thingToSpawn.PickStuffColor() +
                    "; P.color: " + PawnColor +
                    "; equals: " + PawnColor.SameColorAs(TS.thingToSpawn.PickStuffColor())
                    //"; equals what:" + PawnColor.SameColorAsWhat(TS.thingToSpawn.graphicData.color),
                    );
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

            if(comp.MyDebug)Log.Warning(debugStr + "Found " + answer.Count + " things with color");

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
                    if(comp.MyDebug)Log.Warning("GetWeightedRandomIndex : returning thing " + i);
                    if(AlreadyPickedOptions.NullOrEmpty() && !comp.HasColorCondition)
                        return i;
                    else
                    {
                        int normalizedIndex = comp.Props.settings.things.IndexOf(TSList[i]);
                        if(comp.MyDebug)Log.Warning("GetWeightedRandomIndex : returning thing " + i + " normalized:" + normalizedIndex);
                        return normalizedIndex;
                    }
                }
            }

            if(comp.MyDebug)Log.Warning("GetWeightedRandomIndex : failed to return proper index, returning -1");

            return -1;
        }
    }
}