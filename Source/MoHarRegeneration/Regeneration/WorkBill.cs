using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;

namespace MoHarRegeneration
{
    public static class RegenBill
    {
        public static bool CanPayHungerBill(this Pawn p, float cost, bool myDebug = false)
        {
            if (!p.HasFoodNeed())
                return true;

            if (p.needs.food.CurLevel < cost)
                return false;

            return true;
        }

        public static void PayHungerBill(this Pawn p, float cost, bool myDebug = false)
        {
            if (!p.HasFoodNeed())
                return;

            p.needs.food.CurLevel -= cost;
        }

        public static bool HungerTransaction(this Pawn p, float CostRatio, float WorkDone, bool myDebug = false)
        {
            if (!p.HasFoodNeed())
                return true;

            if (CostRatio > 0)
            {
                float HungerCost = WorkDone * CostRatio;
                if (!p.CanPayHungerBill(HungerCost))
                    return false;
                else
                {
                    p.PayHungerBill(HungerCost);
                    return true;
                }
            }
            return true;
        }

        public static bool CanPayRestBill(this Pawn p, float cost, bool myDebug = false)
        {
            if (!p.HasRestNeed())
                return true;

            if (p.needs.rest.CurLevel < cost)
                return false;

            return true;
        }

        public static void PayRestBill(this Pawn p, float cost, bool myDebug = false)
        {
            if (!p.HasRestNeed())
                return;

            p.needs.rest.CurLevel -= cost;
        }

        public static bool RestTransaction(this Pawn p, float CostRatio, float WorkDone, bool myDebug = false)
        {
            if (CostRatio > 0 && p.HasRestNeed())
            {
                float RestCost = WorkDone * CostRatio;
                if (!p.CanPayRestBill(RestCost))
                    return false;
                else
                {
                    p.PayRestBill(RestCost);
                    return true;
                }
            }
            return true;
        }

        public static bool HungerAndRestTransaction(this Pawn p, float HungerCostRatio, float RestCostRatio, float WorkDone, bool myDebug = false)
        {
            float RestCost = WorkDone * RestCostRatio;
            float HungerCost = WorkDone * HungerCostRatio;

            if(myDebug)
            Log.Warning(
                p.LabelShort + " HungerAndRestTransaction " +
                " Quality:" + WorkDone +
                "; RestCost: " + RestCost +
                "; p.rest: " + p.needs.rest?.CurLevel +
                "; HungerCost: " + HungerCost +
                "; p.hunger: " + p.needs.food?.CurLevel
            );

            if (HungerCostRatio > 0 && RestCostRatio > 0)
            {
                if (!p.CanPayRestBill(RestCost) || !p.CanPayHungerBill(HungerCost))
                {
                    if(myDebug)
                        Log.Warning(p.LabelShort + " cant pay either RestCost or HungerCost ");

                    return false;
                }
                else
                {
                    if (myDebug)
                        Log.Warning(p.LabelShort + " will pay both RestCost/HungerCost ");

                    if(p.needs.rest != null)
                        p.PayRestBill(RestCost);

                    if (p.needs.food != null)
                        p.PayHungerBill(HungerCost);

                    return true;
                }
            }
            else if (HungerCostRatio > 0 && RestCostRatio <= 0)
                return p.HungerTransaction(HungerCostRatio, WorkDone);
            else if (HungerCostRatio <= 0 && RestCostRatio > 0)
                return p.RestTransaction(HungerCostRatio, WorkDone);

            return true;
        }
    }
}
