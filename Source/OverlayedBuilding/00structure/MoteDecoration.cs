using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{

    public class MoteDecoration
    {
        public string label;

        public ThingDef moteDef;

        public DisplayCondition condition;
        public DisplayOrigin.Origin origin;

        public DisplayTransformation transformation;
        
        public int graceTicks = 15;
        public bool coexistsWithSame = false;
        public bool coexistsWithOther = false;

        public bool debug = false;

        // lambda expressions
        public bool HasCondition => condition != null;
        public bool HasNoCondition => !HasCondition || condition.NoCondition;
        public bool HasMoteDef => moteDef != null;
        public bool HasLabel => !label.NullOrEmpty();
        public bool IsInvalid => !HasMoteDef || !HasLabel;

        public bool RequiresReservationUpdate =>  HasCondition && condition.ifWorker;
        public bool RequiresReservationCheck => RequiresReservationUpdate;

        public bool RequiresSelectionCheck => HasCondition && condition.ifSelected;

        public bool RequiresFuelCheck => HasCondition && condition.ifFueled;
        public bool RequiresPowerCheck => HasCondition && condition.ifPowered;
        public bool RequiresFuelAndPowerCheck => HasCondition && condition.IfFueledAndPowered;
        public bool RequiresNeitherFuelNorPowerCheck => !RequiresFuelCheck && !RequiresPowerCheck;

        public bool HasGraceTicks => graceTicks > 0;

        public bool forbidsCoexistWithSame => !coexistsWithSame;
        public bool forbidsCoexistWithOther => !coexistsWithOther;
        public bool allowsCoexistWithAny => coexistsWithSame && coexistsWithOther;

        public bool forbidsAnyCoexist => forbidsCoexistWithSame && forbidsCoexistWithOther;
        public bool forbidsSomeCoexist => forbidsCoexistWithSame || forbidsCoexistWithOther;
    }
}