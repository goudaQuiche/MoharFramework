using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace MoharDamage
{
    public class DamageWorker_ExplosionOnImpact : DamageWorker_Blunt
    {
        public ExplosionOnImpact_DamageDef Def => def as ExplosionOnImpact_DamageDef;

        public bool MyDebug => Def.debug;

        public bool HasVictimEffect => Def.HasVictimEffect;

        public bool HasNature => Def.HasNature;
        public bool HasImmunity => Def.HasImmunity;
        public bool HasChance => Def.HasChance;

        public bool HasDestroyProcedure => Def.HasDestroyProcedure;

        public bool HasPreExplosionSpawn => Def.HasPreExplosionSpawn;
        public bool HasPostExplosionSpawn => Def.HasPostExplosionSpawn;
        public bool HasDecoration => Def.HasDecoration;
        
        public VictimEffectOnImpact VictimEffect => Def.victimEffect;
        public ExplosionNature Nature => Def.nature;
        public ExplosionImmunity Immunity => Def.immunity;
        public ExplosionChance Chance => Def.chance;
        public DestroyedProcedure DestroyedProcedure => Def.destroyedProcedure;
        public List<ExplosionSpawnItem> PreExplosionSpawn => Def.preExplosionSpawn;
        public List<ExplosionSpawnItem> PostExplosionSpawn => Def.postExplosionSpawn;
        public ExplosionDecoration ExplosionDecoration => Def.decoration;

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            string debugStr = MyDebug ? $"{victim.def.defName} Apply - " : "";

            Pawn pVictim = victim as Pawn;
            Pawn pAgressor = dinfo.Instigator as Pawn;

            IntVec3 IntVec3Origin = pVictim.Position;
            //IntVec3Origin.ToVector3Shifted();
            Vector3 Vector3Origin = pVictim.DrawPos;

            bool DidSomething = false;
            bool DidApplyEffect = false;
            bool DidExplosionEffect = false;
            Map map = victim.Map;

            DamageResult damageResult = base.Apply(dinfo, victim);

            DidSomething |= DidApplyEffect = this.TryApplyEffect(pVictim);
            if (DidApplyEffect && HasDecoration && ExplosionDecoration.HasPawnEffectMote)
                MoteThrower.ThrowPawnEffectMote(pVictim.DrawPos, map, ExplosionDecoration.pawnEffectMote);

            Tools.Warn($"{debugStr}DidApplyEffect: {DidApplyEffect}", MyDebug);

            float ExplosionChance = this.ChanceToTriggerExplosion(pAgressor);
            Tools.Warn($"{debugStr}ExplosionChance: {ExplosionChance}", MyDebug);

            if (Rand.Chance(ExplosionChance))
            {
                Tools.Warn($"{debugStr}Trying to explode", MyDebug);

                ExplosionSpawnItem preExplosionSpawnItem = HasPreExplosionSpawn ? PreExplosionSpawn.PickRandomWeightedItem() : null;
                ExplosionSpawnItem postExplosionSpawnItem = HasPostExplosionSpawn ? PostExplosionSpawn.PickRandomWeightedItem() : null;

                List<Thing> explosionImmuneThings = null;
                if (HasImmunity)
                {
                    explosionImmuneThings = new List<Thing>();
                    explosionImmuneThings = this.GenerateThingsToIgnore(map, dinfo.Instigator, this.GenerateThingDefsToIgnore(pAgressor), Immunity.limit);

                    Tools.Warn($"{debugStr}explosionImmuneThings: {explosionImmuneThings?.Count}", MyDebug);
                    if (!explosionImmuneThings.NullOrEmpty() && MyDebug)
                    {
                        foreach (Thing t in explosionImmuneThings)
                            Tools.Warn(t.def.defName, MyDebug);
                    }
                }

                Tools.Warn($"{debugStr}trying to construct pp", MyDebug);
                PickedParams pp = new PickedParams()
                {
                    Instigator = dinfo.Instigator,

                    Origin = IntVec3Origin,
                    Map = map,
                    Radius = this.PawnWeightedExplosionRadius(pAgressor),

                    DamageType = Nature.damageType,
                    DamageAmount = this.PawnWeightedExplosionDamage(pAgressor, damageResult.totalDamageDealt),

                    ExplosionSound = (!HasDecoration || !ExplosionDecoration.HasSoundOnImpact)?null: ExplosionDecoration.soundOnImpact,
                    Weapon = dinfo.Weapon,

                    IntendedTarget = victim,

                    PreExplosionSpawnChance = preExplosionSpawnItem != null ? preExplosionSpawnItem.chance.RandomInRange : 0,
                    PreExplosionSpawnThingCount = preExplosionSpawnItem != null ? preExplosionSpawnItem.stack.RandomInRange : 0,
                    PreExplosionSpawnThingDef = preExplosionSpawnItem != null ? preExplosionSpawnItem.thingDef : null,

                    PostExplosionSpawnChance = postExplosionSpawnItem != null ? postExplosionSpawnItem.chance.RandomInRange : 0,
                    PostExplosionSpawnThingCount = postExplosionSpawnItem != null ? postExplosionSpawnItem.stack.RandomInRange : 0,
                    PostExplosionSpawnThingDef = postExplosionSpawnItem != null ? postExplosionSpawnItem.thingDef : null,

                    ChanceToStartFire = Nature.chanceToStartFire.RandomInRange,
                    DamageFallOff = Nature.damageFallOff,

                    IgnoredByExplosion = explosionImmuneThings
                };

                Tools.Warn($"{debugStr}constructed pp", MyDebug);

                DidSomething |= DidExplosionEffect = this.TryExplode(pp);

                Tools.Warn($"{debugStr}DidExplosionEffect : {DidExplosionEffect}", MyDebug);
                if (DidExplosionEffect && HasDecoration)
                {
                    float myRadius = pp.Radius;
                    
                    if (ExplosionDecoration.HasSmokeExplosionMote)
                    {
                        int smokeNum = ExplosionDecoration.smokeNum.RandomInRange;
                        for (int i = 0; i < smokeNum; i++)
                            MoteThrower.ThrowSmokeCustom(Vector3Origin + Gen.RandomHorizontalVector(myRadius * 0.7f), map, myRadius * 0.6f, ExplosionDecoration.smokeExplosionMote);
                    }
                    if (ExplosionDecoration.HasBlastExplosionMote)
                    {
                        int num = Mathf.RoundToInt((float)Math.PI * myRadius * myRadius / ExplosionDecoration.blastNumLimiter.RandomInRange);
                        for (int j = 0; j < num; j++)
                        {
                            MoteMaker.ThrowExplosionInteriorMote(Vector3Origin + Gen.RandomHorizontalVector(myRadius * 0.7f), map, ExplosionDecoration.blastExplosionMote);
                        }
                    }
                }
            }

            if (HasDestroyProcedure && victim.Destroyed && map != null)
            {
                if(DestroyedProcedure.HasNonTree)
                foreach (IntVec3 item in victim.OccupiedRect())
                    FilthMaker.TryMakeFilth(item, map, DestroyedProcedure.filthLeft);

                if (DestroyedProcedure.HasTree && victim is Plant plant && victim.def.plant.IsTree && plant.LifeStage != 0 && victim.def != DestroyedProcedure.thingSpawnedForTrees)
                    ((DeadPlant)GenSpawn.Spawn(DestroyedProcedure.thingSpawnedForTrees, IntVec3Origin, map)).Growth = plant.Growth;

            }

            if (DidSomething && Nature.forceNormalSpeedIfTriggered)
                if (pVictim != null && pVictim.Faction == Faction.OfPlayer)
                    Find.TickManager.slower.SignalForceNormalSpeedShort();

            return damageResult;
        }
    }
}
