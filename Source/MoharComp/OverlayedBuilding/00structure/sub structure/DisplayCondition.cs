using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{

    public class DisplayCondition
    {
        public bool ifFueled = false;
        public bool ifPowered = false;
        
        public bool ifWorker = false;
        public bool ifWorkerOnInteractionCell = false;
        public bool ifWorkerTouch = false;
        public List<JobDef> includeJob;
        public List<JobDef> excludeJob;
        public List<RecipeDef> includeRecipe;
        public List<RecipeDef> excludeRecipe;

        public bool ifSelected = false;

        public bool NoCondition => !ifFueled && !ifPowered && ! ifWorker && !ifSelected;
        public bool IfFueledAndPowered => ifFueled && ifPowered;

        public bool HasIncludedJob => !includeJob.NullOrEmpty();
        public bool HasExcludedJob => !excludeJob.NullOrEmpty();

        public bool HasIncludedRecipe => !includeRecipe.NullOrEmpty();
        public bool HasExcludedRecipe => !excludeRecipe.NullOrEmpty();
    }
}