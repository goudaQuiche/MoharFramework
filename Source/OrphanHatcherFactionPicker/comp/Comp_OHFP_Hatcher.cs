using RimWorld.Planet;
using UnityEngine;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;

namespace OHFP
{
    //Todo
    // hatcheeFaction and hatcherPawn could be set when spawning :/
    public class Comp_OHFP_Hatcher : ThingComp
    {
        private float gestateProgress;
        public Pawn hatcheeParent;
        public Pawn otherParent;
        public Faction hatcheeFaction;
        public PawnKindDef hatcheePawnKind;

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

            if (!StaticCheck.IsOk)
                return;

            Scribe_Values.Look(ref gestateProgress, "gestateProgress", 0f);
            Scribe_References.Look(ref hatcheeParent, "hatcheeParent");
            Scribe_References.Look(ref otherParent, "otherParent");
            Scribe_References.Look(ref hatcheeFaction, "hatcheeFaction");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!StaticCheck.IsOk)
                return;

            if (HasForcedFaction)
            {
                hatcheeFaction = Find.FactionManager.AllFactions.Where(F => F.def == Props.forcedFaction).FirstOrFallback();
                hatcheeParent = null;
                Tools.Warn("Faction forced ok", myDebug);
            }
            else if (SetFactionAndParent())
                Tools.Warn("SetFactionAndParent ok", myDebug);
        }

        private bool SetPawnKind()
        {
            if (Props.hatcherPawnList.NullOrEmpty())
                return false;

            hatcheePawnKind = Props.hatcherPawnList.RandomElement();
            return (hatcheePawnKind != null);
        }

        private bool SetFactionAndParent()
        {
            float totalChance = Props.colonyAdoptedChance + Props.neutralAdoptedChance + Props.enemyAdoptedChance;

            float DiceRoll = Rand.Range(0, totalChance);
            // got enemy
            if ((DiceRoll -= Props.enemyAdoptedChance) < 0)
            {
                hatcheeFaction = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && !f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElementWithFallback();

                if (hatcheeFaction != null)
                    hatcheeParent = Find.WorldPawns.AllPawnsAlive.Where(p => p.Faction != null && p.Faction == hatcheeFaction).RandomElementWithFallback();
            }
            // got neutral
            else if ((DiceRoll -= Props.neutralAdoptedChance) < 0)
            {
                hatcheeFaction = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElementWithFallback();

                if (hatcheeFaction != null)
                    hatcheeParent = Find.WorldPawns.AllPawnsAlive.Where(p => p.Faction != null && p.Faction == hatcheeFaction).RandomElementWithFallback();
            }
            // got player faction
            else
            {
                hatcheeFaction = Faction.OfPlayer;
                hatcheeParent = parent.Map.mapPawns.AnyFreeColonistSpawned ? parent.Map.mapPawns.FreeColonists.RandomElementWithFallback() : null;
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
            //Tools.Warn("hatcherPawn == null", hatcheePawnKind == null && myDebug);
            Tools.Warn("hatcheeFaction == null", hatcheeFaction == null && myDebug);
            Tools.Warn("hatcheeParent == null", hatcheeParent == null && myDebug);

            PawnGenerationContext pGenContext = PawnGenerationContext.NonPlayer;

            /*
            bool TryingToSpawnMechanoid = hatcheePawnKind.RaceProps.IsMechanoid;
            if (TryingToSpawnMechanoid) hatcheeFaction = Faction.OfAncients;
            */

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

                for (int i = 0; i < parent.stackCount; i++)
                {
                    bool newBorn = Rand.Chance(Props.newBornChance);
                    if (SetPawnKind())
                        Tools.Warn("SetPawnKind: " + hatcheePawnKind.label, myDebug);
                    else continue;

                    PawnGenerationRequest request;
                    /*
                    if (TryingToSpawnMechanoid)
                    {
                        request = new PawnGenerationRequest(
                            kind: hatcheePawnKind, faction: hatcheeFaction, context: pGenContext,
                            tile: -1, forceGenerateNewPawn: false, newborn: newBorn, allowDead: false,
                            allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: true, colonistRelationChanceFactor: 0,
                            forceAddFreeWarmLayerIfNeeded: false, allowGay: false
                            );
                         would require to set faction
                    }
                    else
                    {
                        */
                    request = new PawnGenerationRequest(
                    kind: hatcheePawnKind, faction: hatcheeFaction, context: pGenContext, tile: -1,
                    forceGenerateNewPawn: false, newborn: newBorn
                    );

                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    if (parent.MyTrySpawnHatchedOrBornPawn(pawn))
                    {
                        if (pawn == null)
                            continue;

                        if (hatcheeParent != null)
                        {
                            pawn.InheritParentSettings(hatcheeParent, hatcheeFaction);
                            pawn.AddParentRelations(hatcheeParent);
                        }
                        pawn.AddOtherParentRelations(hatcheeParent, otherParent);

                        if (parent.Spawned)
                        {
                            FilthMaker.TryMakeFilth(parent.Position, parent.Map, ThingDefOf.Filth_AmnioticFluid);
                        }

                        if (Rand.Chance(Props.manhunterChance))
                            pawn.MakeManhunter(myDebug);
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
