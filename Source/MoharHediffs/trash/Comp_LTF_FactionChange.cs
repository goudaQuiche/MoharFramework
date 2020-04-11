using RimWorld;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace NewHatcher
{
    // Main
    public class Comp_LTF_FactionChange : ThingComp
    {
        private int ticksLeft=-9999;

        public CompProperties_LTF_FactionChange Props
        {
            get
            {
                return (CompProperties_LTF_FactionChange)this.props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Log.Warning("PostSpawnSetup start");

            if (parent == null)
            {
                Log.Warning("null parent");
                return;
            }
            if (Props.ownFaction == null)
            {
                Log.Warning("null own");
                return;
            }
            if(Props.forcedFaction == null)
            {
                Log.Warning("null forced");
                return;
            }
            if (parent.Faction == null)
            {
                Log.Warning("null parent faction");
                return;
            }

            if (parent.Faction == Props.forcedFaction)
            {
                Log.Warning("No need to do");
            }
            else
            if(parent.Faction == Props.forcedFaction){
                Log.Warning("LEts enslave");
                Enslave();
            }
            else
            {
                Log.Warning("How the f");
            }
            

            Log.Warning("PostSpawnSetup start");
        }

        public override void CompTick()
        {
            if ((this.parent == null) || (this.parent.Map == null))
            {
                Log.Warning(parent.Label + " NUll tick ");
                return;
            }
            this.ticksLeft--;

            Log.Warning(parent.Label + " tick " + ticksLeft);
            if (ticksLeft <= 0)
            {
                Log.Warning(parent.Label + " releasing ");
                Release();
            }
            //CheckDespawn();
        }

        public override void CompTickRare()
        {
            Log.Warning("whatever tick rare");
        }

        public override void PostExposeData()
        {
            Scribe_References.Look(ref Props.ownFaction, "LTF_own");
            Scribe_References.Look(ref Props.forcedFaction, "LTF_forced");

            Scribe_Values.Look(ref ticksLeft, "LTF_ticksLeft");
        }

        [DebuggerHidden]
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if ((parent != null) && (Props.ownFaction != null) && (Props.forcedFaction != null))
            {
                yield return new Command_Action
                {
                    action = new Action(this.Release),
                    defaultLabel = "Spare",
                    defaultDesc = "Reset the target",
                    icon = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true)
                };
            }
        }

        public override string CompInspectStringExtra()
        {
            string text = base.CompInspectStringExtra();
            string result = string.Empty;
            int num = this.ticksLeft % 60000;

            if (parent == null)
            {
                result += "null parent";
                return result;
            }

            if (num > 0)
            {
                result = num.ToStringTicksToPeriod(true, false, true) + " before release";
            }

            if (!text.NullOrEmpty())
            {
                result = "\n" + text;
            }

            return result;
        }

        //Faction own, Faction forced, int duration
        public void Enslave()
        {
            if ((parent == null) || (parent.Map == null) || (parent.Faction == null) || (Props.ownFaction == null) || (Props.forcedFaction == null))
            {
                Log.Warning("null parent enslave");
                return;
            }
            Log.Warning("Trying to enslave " + parent.LabelShort);
            parent.SetFaction(Props.forcedFaction);
            Log.Warning("Enslaving Ok");
        }

        private void Release()
        {
            if ((parent == null) || (parent.Map == null) || (parent.Faction == null) || (Props.ownFaction == null) || (Props.forcedFaction == null))
            {
                Log.Warning("null parent release");
                return;
            }
            Log.Warning(this.parent.Label + " : " + Props.ownFaction + " " + Props.forcedFaction);
            Log.Warning("release");
            parent.SetFaction(Props.ownFaction);
            Log.Warning("sepuku");
            PostDeSpawn(this.parent.Map);
        }

        public void Init(Faction own, Faction forced, int duration)
        {
            //parent=
            if (this.parent == null)
            {
                Log.Warning("null parent Init");
                return;
            }

            if(parent.Map == null)
            {
                Log.Warning("null map Init");
                return;
            }

            Log.Warning("asking " + parent.Label + own.Name + "->" + forced.Name + "(" + duration + ")");

            Faction almostOwnFaction = null;
            if(parent.Faction == null)
            {
                Log.Warning("null faction Init");
                almostOwnFaction = FactionUtility.DefaultFactionFrom(FactionDefOf.Tribe);
            }
            else
            {
                almostOwnFaction = own;
            }
            Log.Warning("faction found : " + almostOwnFaction.Name);

            Props.ownFaction = own;
            Props.forcedFaction = forced;
            ticksLeft = duration;

            Log.Warning("Init done, enslaving");
            Enslave();
        }
    }

    public class HediffComp_FactionChange : HediffComp
    {
        private int ticksLeft = 0;
        public HediffCompProperties_FactionChange Props
        {
            get
            {
                return (HediffCompProperties_FactionChange)this.props;
            }
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (this.parent == null)
            {
                Log.Warning("null comp");
                return;
            }

            if (this.parent.pawn == null)
            {
                Log.Warning("null pawn");
                return;
            }

            if(this.parent.pawn.Map == null)
            {
                Log.Warning(parent.Label + " null map");
                return;
            }

            ticksLeft--;
            Log.Warning(parent.Label + " tick " + ticksLeft);

            if (ticksLeft <= 0)
            {
                Log.Warning(parent.Label + " releasing ");
                Release();
            }
        }
        //Faction own, Faction forced, int duration
        public void Enslave()
        {
            if ((parent == null) || (parent.Map == null) || (parent.Faction == null) || (Props.ownFaction == null) || (Props.forcedFaction == null))
            {
                Log.Warning("null parent enslave");
                return;
            }
            Log.Warning("Trying to enslave " + parent.LabelShort);
            parent.SetFaction(Props.forcedFaction);
            Log.Warning("Enslaving Ok");
        }

        private void Release()
        {
            if ((parent == null) || (parent.Map == null) || (parent.Faction == null) || (Props.ownFaction == null) || (Props.forcedFaction == null))
            {
                Log.Warning("null parent release");
                return;
            }
            Log.Warning(this.parent.Label + " : " + Props.ownFaction + " " + Props.forcedFaction);
            Log.Warning("release");
            parent.SetFaction(Props.ownFaction);
            Log.Warning("sepuku");
            PostDeSpawn(this.parent.Map);
        }

        public void Init(Faction own, Faction forced, int duration)
        {
            //parent=
            if (this.parent == null)
            {
                Log.Warning("null parent Init");
                return;
            }

            if (parent.Map == null)
            {
                Log.Warning("null map Init");
                return;
            }

            Log.Warning("asking " + parent.Label + own.Name + "->" + forced.Name + "(" + duration + ")");

            Faction almostOwnFaction = null;
            if (parent.Faction == null)
            {
                Log.Warning("null faction Init");
                almostOwnFaction = FactionUtility.DefaultFactionFrom(FactionDefOf.Tribe);
            }
            else
            {
                almostOwnFaction = own;
            }
            Log.Warning("faction found : " + almostOwnFaction.Name);

            Props.ownFaction = own;
            Props.forcedFaction = forced;
            ticksLeft = duration;

            Log.Warning("Init done, enslaving");
            Enslave();
        }

    }
}