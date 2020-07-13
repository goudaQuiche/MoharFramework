using RimWorld.Planet;
using UnityEngine;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;

namespace OHFP
{
    public class Comp_OHFP_Hatcher : ThingComp
    {
        private float gestateProgress;
        public Pawn hatcheeParent;
        public Pawn otherParent;
        public Faction hatcheeFaction;
        public PawnKindDef hatcherPawn;

        bool playerAdopted, enemyAdopted, neutralAdopted;

        public bool myDebug => Props.debug;
        public CompProperties_OHFP_Hatcher Props => (CompProperties_OHFP_Hatcher)props;
        private CompTemperatureRuinable FreezerComp => parent.GetComp<CompTemperatureRuinable>();

        public bool TemperatureDamaged
        {
            get
            {
                if (FreezerComp != null)
                {
                    return FreezerComp.Ruined;
                }
                return false;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref gestateProgress, "gestateProgress", 0f);
            Scribe_References.Look(ref hatcheeParent, "hatcheeParent");
            Scribe_References.Look(ref otherParent, "otherParent");
            Scribe_References.Look(ref hatcheeFaction, "hatcheeFaction");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            playerAdopted = enemyAdopted = neutralAdopted = false;

            if (SetFactionAndParent())
                Tools.Warn("SetFactionAndParent ok", myDebug);
            if (SetPawnKind())
                Tools.Warn("SetPawnKind ok", myDebug);

            Tools.Warn("Enemy adopted", myDebug && enemyAdopted);
            Tools.Warn("Neutral adopted", myDebug && neutralAdopted);
            Tools.Warn("Player adopted", myDebug && playerAdopted);
        }

        private bool SetPawnKind()
        {
            if (Props.hatcherPawnList.NullOrEmpty())
                return false;

            hatcherPawn = Props.hatcherPawnList.RandomElement();
            return (hatcherPawn != null);
        }

        private bool SetFactionAndParent()
        {
            float totalChance = Props.colonyAdoptedChance + Props.neutralAdoptedChance + Props.enemyAdoptedChance;

            float DiceRoll = Rand.RangeInclusive(0, 1);
            // got enemy
            if( (DiceRoll -= Props.enemyAdoptedChance) < 0)
            {
                hatcheeFaction = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && !f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElement();

                if(hatcheeFaction!=null)
                    hatcheeParent = Find.WorldPawns.AllPawnsAlive.Where(p => p.Faction != null && p.Faction == hatcheeFaction).RandomElement();
                enemyAdopted = true;
            }
            // got neutral
            else if ((DiceRoll -= Props.neutralAdoptedChance) < 0)
            {
                hatcheeFaction = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElement();

                if (hatcheeFaction != null)
                    hatcheeParent = Find.WorldPawns.AllPawnsAlive.Where(p => p.Faction != null && p.Faction == hatcheeFaction).RandomElement();
                neutralAdopted = true;
            }
            // got player faction
            else
            {
                hatcheeFaction = Faction.OfPlayer;
                hatcheeParent = parent.Map.mapPawns.AnyFreeColonistSpawned ? parent.Map.mapPawns.FreeColonists.RandomElement() : null;
                playerAdopted = true;
            }

            return (hatcheeFaction != null && hatcheeParent != null);
        }
        

        public override void CompTick()
        {
            if (!TemperatureDamaged)
            {
                float num = 1f / (Props.hatcherDaystoHatch * 60000f);
                gestateProgress += num;
                if (gestateProgress > 1f)
                {
                    Hatch();
                }
            }
        }

        public void Hatch()
        {
            Tools.Warn("hatcherPawn == null", hatcherPawn == null && myDebug);
            Tools.Warn("hatcheeFaction == null", hatcheeFaction == null && myDebug);
            Tools.Warn("hatcheeParent == null", hatcheeParent == null && myDebug);

            try
            {
                //PawnGenerationRequest request = new PawnGenerationRequest(
                //    Props.hatcherPawn, hatcheeFaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, newborn: true);
                PawnGenerationRequest request = new PawnGenerationRequest(
                    hatcherPawn, hatcheeFaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, newborn: true);
                for (int i = 0; i < parent.stackCount; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, parent))
                    {
                        if (pawn != null)
                        {
                            if (hatcheeParent != null)
                            {
                                if (pawn.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
                                {
                                    pawn.playerSettings.AreaRestriction = hatcheeParent.playerSettings.AreaRestriction;
                                }
                                if (pawn.RaceProps.IsFlesh)
                                {
                                    pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
                                }
                            }
                            if (otherParent != null && (hatcheeParent == null || hatcheeParent.gender != otherParent.gender) && pawn.RaceProps.IsFlesh)
                            {
                                pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, otherParent);
                            }
                        }
                        if (parent.Spawned)
                        {
                            FilthMaker.TryMakeFilth(parent.Position, parent.Map, ThingDefOf.Filth_AmnioticFluid);
                        }

                        if (Rand.Chance(Props.manhunterChance))
                            MakeManhunter(pawn);


                    }
                    else
                    {
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                    }
                }
            }
            finally
            {
                parent.Destroy();
            }
        }


        //Faction own, Faction forced, int duration
        public bool MakeManhunter(Pawn p)
        {
            if (p.NegligeablePawn())
                return false;

            MentalStateDef manhunterState = null;
            manhunterState = MentalStateDefOf.Manhunter;
            Tools.Warn(p.LabelShort + " trying to go " + manhunterState.defName, myDebug);
            //mindTarget.mindState.mentalStateHandler.TryStartMentalState(chosenState, null, true, false, null);
            string reason = "because ";

            if(p.mindState == null || p.mindState.mentalStateHandler == null)
            {
                Tools.Warn(p.LabelShort + " null mindstate", myDebug);
                return false;
            }

            Tools.Warn(p.LabelShort + " got applied "+ manhunterState.defName, myDebug);
            p.mindState.mentalStateHandler.TryStartMentalState(manhunterState, reason, true, false, null);

            return true;
        }

        public override void PreAbsorbStack(Thing otherStack, int count)
        {
            float t = (float)count / (float)(parent.stackCount + count);
            float b = ((ThingWithComps)otherStack).GetComp<Comp_OHFP_Hatcher>().gestateProgress;
            gestateProgress = Mathf.Lerp(gestateProgress, b, t);
        }

        public override void PostSplitOff(Thing piece)
        {
            Comp_OHFP_Hatcher comp = ((ThingWithComps)piece).GetComp<Comp_OHFP_Hatcher>();
            comp.gestateProgress = gestateProgress;
            comp.hatcheeParent = hatcheeParent;
            comp.otherParent = otherParent;
            comp.hatcheeFaction = hatcheeFaction;
        }

        public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
        {
            base.PrePreTraded(action, playerNegotiator, trader);
            switch (action)
            {
                case TradeAction.PlayerBuys:
                    hatcheeFaction = Faction.OfPlayer;
                    break;
                case TradeAction.PlayerSells:
                    hatcheeFaction = trader.Faction;
                    break;
            }
        }

        public override void PostPostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
        {
            base.PostPostGeneratedForTrader(trader, forTile, forFaction);
            hatcheeFaction = forFaction;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {


            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "force hatch",
                    defaultDesc = "debug egg",
                    //icon = Gfx.IconDebug,
                    //icon = (myDebug)?(Gfx.DebugOnGz):(Gfx.DebugOffGz),
                    action = delegate
                    {
                        gestateProgress = Props.hatcherDaystoHatch * 60000f;
                    }
                };
            }
            
        }

        public override string CompInspectStringExtra()
        {
            string bla = string.Empty;

            if (Prefs.DevMode)
            {
                if (hatcheeFaction != null)
                    bla += "Faction:" + hatcheeFaction;
                if (hatcheeParent != null)
                    bla += "; Parent:" + hatcheeParent;
            }

            if (!TemperatureDamaged)
            {
                bla += "EggProgress".Translate() + ": " + gestateProgress.ToStringPercent();
            }

            if(bla.NullOrEmpty())
                return null;

            return bla;
        }
    }
}
