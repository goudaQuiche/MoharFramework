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

        public bool HasForcedFaction => Props.HasForcedFaction;
        public bool IsRandomlyAdopted => Props.IsRandomlyAdopted;

        public bool MyDebug => Props.debug;
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
                SetForcedFaction();
                Tools.Warn("Faction forced ok", MyDebug);
            }
            else if (IsRandomlyAdopted)
            {
                SetRandomFaction();
                Tools.Warn("SetFactionAndParent ok", MyDebug);
            }
            else if (hatcheeParent != null)
                hatcheeFaction = hatcheeParent.Faction;

            if (Props.findRandomMotherIfNull && hatcheeParent == null)
                hatcheeParent = GetMother(hatcheeFaction);
            if (Props.findRandomFatherIfNull && otherParent == null)
                otherParent = GetFather(hatcheeFaction, hatcheeParent);

        }

        private bool SetPawnKind()
        {
            if (Props.hatcherPawnList.NullOrEmpty())
                return false;

            hatcheePawnKind = Props.hatcherPawnList.RandomElement();
            return hatcheePawnKind != null;
        }

        private void SetForcedFaction()
        {
            hatcheeFaction = Find.FactionManager.AllFactions.Where(F => F.def == Props.forcedFaction).FirstOrFallback();
        }

        private Pawn GetMother(Faction faction)
        {
            return Find.WorldPawns.AllPawnsAlive
                .Where(p => p.Faction != null && p.Faction == faction)
                // since pawnkind is chosen upon hatch, parents may not be from child race
                .Where(p => p.kindDef == Props.hatcherPawnList.RandomElement())
                .Where(p => p.ageTracker.CurLifeStage.reproductive)
                .Where(p => p.gender == Gender.Female)
                .RandomElementWithFallback();
        }
        private Pawn GetFather(Faction faction, Pawn Mother)
        {
            return
            Find.WorldPawns.AllPawnsAlive
            .Where(p => p.Faction != null && p.Faction == hatcheeFaction)
            // we choose same kindDef as hatcheeParent
            .Where(p => (hatcheeParent != null) ? (p.kindDef == hatcheeParent.kindDef && p != hatcheeParent) : true)
            .Where(p => p.ageTracker.CurLifeStage.reproductive)
            .Where(p => p.gender == Gender.Male)
            .RandomElementWithFallback();
        }

        private void SetRandomFaction()
        {
            RandomAdoption RA = Props.randomAdoption.RandomElementByWeightWithFallback(ra => ra.weight);

            switch (RA.factionType)
            {
                case AdoptionType.enemy:
                    hatcheeFaction = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && !f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElementWithFallback();
                    break;
                case AdoptionType.neutral:
                    hatcheeFaction = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && f.AllyOrNeutralTo(Faction.OfPlayer)).RandomElementWithFallback();
                    break;
                default:
                case AdoptionType.player:
                    hatcheeFaction = Faction.OfPlayer;
                    break;
            }
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
            
            Tools.Warn("hatcheeFaction == null", hatcheeFaction == null && MyDebug);
            Tools.Warn("hatcheeParent == null", hatcheeParent == null && MyDebug);
            Tools.Warn("otherParent == null", otherParent == null && MyDebug);

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
                        Tools.Warn("SetPawnKind: " + hatcheePawnKind.label, MyDebug);
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
                            pawn.MakeManhunter(MyDebug);
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
                    bla += "; Mother:" + hatcheeParent;
                if (otherParent != null)
                    bla += "; Father:" + otherParent;
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
