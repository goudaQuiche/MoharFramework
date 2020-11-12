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

        public bool HasForcedFaction => Props.forcedFaction != null;

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

            if (HasForcedFaction)
            {
                hatcheeFaction = Find.FactionManager.AllFactions.Where(F => F.def == Props.forcedFaction).FirstOrFallback();
                hatcheeParent = null;
                Tools.Warn("Faction forced ok", myDebug);
            }
            else if (SetFactionAndParent())
                Tools.Warn("SetFactionAndParent ok", myDebug);

            if (SetPawnKind())
                Tools.Warn("SetPawnKind ok", myDebug);
            if (enemyAdopted)
                Tools.Warn("Enemy adopted", myDebug);
            else if (neutralAdopted)
                Tools.Warn("Neutral adopted", myDebug);
            else if (playerAdopted)
                Tools.Warn("Player adopted", myDebug);
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
            if ((DiceRoll -= Props.enemyAdoptedChance) < 0)
            {
                hatcheeFaction = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && !f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElementWithFallback();

                if (hatcheeFaction != null)
                    hatcheeParent = Find.WorldPawns.AllPawnsAlive.Where(p => p.Faction != null && p.Faction == hatcheeFaction).RandomElementWithFallback();
                enemyAdopted = true;
            }
            // got neutral
            else if ((DiceRoll -= Props.neutralAdoptedChance) < 0)
            {
                hatcheeFaction = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElementWithFallback();

                if (hatcheeFaction != null)
                    hatcheeParent = Find.WorldPawns.AllPawnsAlive.Where(p => p.Faction != null && p.Faction == hatcheeFaction).RandomElementWithFallback();
                neutralAdopted = true;
            }
            // got player faction
            else
            {
                hatcheeFaction = Faction.OfPlayer;
                hatcheeParent = parent.Map.mapPawns.AnyFreeColonistSpawned ? parent.Map.mapPawns.FreeColonists.RandomElementWithFallback() : null;
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

            //PawnGenerationContext pGenContext = playerAdopted ? PawnGenerationContext.NonPlayer : PawnGenerationContext.PlayerStarter;
            PawnGenerationContext pGenContext = PawnGenerationContext.NonPlayer;
            bool newBorn = Rand.Chance(Props.newBornChance);

            bool noRelation = (hatcherPawn.race.defName == "Mohar_Scyther") ?true:false;
            if (noRelation)
                hatcheeFaction = Faction.OfAncients;

            try
            {
                //PawnGenerationRequest request = new PawnGenerationRequest(
                //    Props.hatcherPawn, hatcheeFaction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, newborn: true);
                /*
                public PawnGenerationRequest(
                    PawnKindDef kind, Faction faction = null, PawnGenerationContext context = PawnGenerationContext.NonPlayer,
                    int tile = -1, bool forceGenerateNewPawn = false, bool newborn = false, bool allowDead = false,
                    bool allowDowned = false, bool canGeneratePawnRelations = true, bool mustBeCapableOfViolence = true, float colonistRelationChanceFactor = 1,
                    bool forceAddFreeWarmLayerIfNeeded = false, bool allowGay = true, bool allowFood = true, bool allowAddictions = true,
                    bool inhabitant = false, bool certainlyBeenInCryptosleep = false, bool forceRedressWorldPawnIfFormerColonist = false, bool worldPawnFactionDoesntMatter = false,
                    float biocodeWeaponChance = 0, Pawn extraPawnForExtraRelationChance = null, float relationWithExtraPawnChanceFactor = 1, Predicate<Pawn> validatorPreGear = null, 
                    Predicate<Pawn> validatorPostGear = null, IEnumerable<TraitDef> forcedTraits = null, IEnumerable<TraitDef> prohibitedTraits = null, float? minChanceToRedressWorldPawn = null, 
                    float? fixedBiologicalAge = null, float? fixedChronologicalAge = null, Gender? fixedGender = null, float? fixedMelanin = null,
                    string fixedLastName = null, string fixedBirthName = null, RoyalTitleDef fixedTitle = null);
                */

                PawnGenerationRequest request;
                if(noRelation)
                request = new PawnGenerationRequest(
                    kind: hatcherPawn, faction: hatcheeFaction, context: pGenContext, 
                    tile: -1, forceGenerateNewPawn: false, newborn: newBorn, allowDead: false,
                    allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: true, colonistRelationChanceFactor: 0,
                    forceAddFreeWarmLayerIfNeeded: false, allowGay: false
                    );
                else 
                    request = new PawnGenerationRequest(
                    kind: hatcherPawn, faction: hatcheeFaction, context: pGenContext, tile: -1,
                    forceGenerateNewPawn: false, newborn: newBorn
                    );

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

                        if (noRelation)
                        {
                            pawn.SetFaction(Faction.OfPlayer);
                            
                        }
                            


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
