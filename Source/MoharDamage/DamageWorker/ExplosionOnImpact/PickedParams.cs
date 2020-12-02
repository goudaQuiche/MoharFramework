using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;

namespace MoharDamage
{
    /*
     GenExplosion.DoExplosion(instigator: (instigator == null || instigator.HostileTo(parent.Faction)) ? parent : instigator,
     center: parent.PositionHeld, map: map, radius: num, damType: props.explosiveDamageType, damAmount: props.damageAmountBase, 
     armorPenetration: props.armorPenetrationBase, explosionSound: props.explosionSound, weapon: null, projectile: null, 
     intendedTarget: null, postExplosionSpawnThingDef: props.postExplosionSpawnThingDef, postExplosionSpawnChance: props.postExplosionSpawnChance,
     postExplosionSpawnThingCount: props.postExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: props.applyDamageToExplosionCellsNeighbors,
     preExplosionSpawnThingDef: props.preExplosionSpawnThingDef, preExplosionSpawnChance: props.preExplosionSpawnChance, 
     preExplosionSpawnThingCount: props.preExplosionSpawnThingCount, chanceToStartFire: props.chanceToStartFire, damageFalloff: props.damageFalloff,
     direction: null, ignoredThings: thingsIgnoredByExplosion);
    */

    public class PickedParams
    {
        public Thing Instigator;

        public IntVec3 Origin;
        public Map Map;
        public float Radius;

        public DamageDef DamageType;
        public float DamageAmount;

        //armorPenetration
        public SoundDef ExplosionSound;
        public ThingDef Weapon;
        //projectile
        public Thing IntendedTarget;

        public ThingDef PostExplosionSpawnThingDef;
        public float PostExplosionSpawnChance;
        public int PostExplosionSpawnThingCount;
        //applyDamageToExplosionCellsNeighbors

        public ThingDef PreExplosionSpawnThingDef;
        public float PreExplosionSpawnChance;
        public int PreExplosionSpawnThingCount;

        public float ChanceToStartFire;
        public bool DamageFallOff;
        //direction

        public List<Thing> IgnoredByExplosion = new List<Thing>();

        public PickedParams()
        {

        }
        /*
        PickedParams(
            Pawn nInstigator, IntVec3 nOrigin, Map nMap, float nRadius, DamageDef nDamageType, 
            float nDamageAmount, SoundDef nExplosionSound, ThingDef nWeapon, ThingDef nIntendedTarget, 
            ThingDef nPostExplosionSpawnThingDef, float nPostExplosionSpawnChance, int nPostExplosionSpawnThingCount, 
            ThingDef nPreExplosionSpawnThingDef, float nPreExplosionSpawnChance, int nPreExplosionSpawnThingCount, 
            float nChanceToStartFire, bool nDamageFallOff)
        {

        }
        */
    }
}
