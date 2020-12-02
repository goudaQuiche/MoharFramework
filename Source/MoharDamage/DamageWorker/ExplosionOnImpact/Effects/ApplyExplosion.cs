using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;

namespace MoharDamage
{
    public static class ApplyExplosion
    {
        /*
         * public static void DoExplosion(
	        IntVec3 center, Map map, float radius, DamageDef damType, Thing instigator, 
	        int damAmount = -1, float armorPenetration = -1, SoundDef explosionSound = null, ThingDef weapon = null, ThingDef projectile = null, Thing intendedTarget = null, 
	        ThingDef postExplosionSpawnThingDef = null, float postExplosionSpawnChance = 0, int postExplosionSpawnThingCount = 1, 
	        bool applyDamageToExplosionCellsNeighbors = false, 
	        ThingDef preExplosionSpawnThingDef = null, float preExplosionSpawnChance = 0, int preExplosionSpawnThingCount = 1, 
	        float chanceToStartFire = 0, bool damageFalloff = false, float? direction = null, List<Thing> ignoredThings = null
        );*/
        public static bool TryExplode(this DamageWorker_ExplosionOnImpact DWEOI, PickedParams pp)
        {
            GenExplosion.DoExplosion(
                center: pp.Origin, map: pp.Map, radius: pp.Radius, damType: pp.DamageType, instigator: pp.Instigator,
                damAmount: (int)pp.DamageAmount, explosionSound: pp.ExplosionSound, weapon: pp.Weapon, intendedTarget: pp.IntendedTarget,
                postExplosionSpawnThingDef: pp.PostExplosionSpawnThingDef, postExplosionSpawnChance: pp.PostExplosionSpawnChance, postExplosionSpawnThingCount: pp.PostExplosionSpawnThingCount,

                preExplosionSpawnThingDef: pp.PreExplosionSpawnThingDef, preExplosionSpawnChance: pp.PreExplosionSpawnChance, preExplosionSpawnThingCount: pp.PreExplosionSpawnThingCount,
                chanceToStartFire: pp.ChanceToStartFire, damageFalloff: pp.DamageFallOff, ignoredThings: pp.IgnoredByExplosion
            );

            return true;
        }
    }
}
