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

    public class ExplosionOnImpact_DamageDef : DamageDef
    {
        public VictimEffectOnImpact victimEffect;

        public ExplosionNature nature;

        public ExplosionImmunity immunity;
        public ExplosionChance chance;

        public List<ExplosionSpawnItem> preExplosionSpawn;
        public List<ExplosionSpawnItem> postExplosionSpawn;

        public ExplosionDecoration decoration;
        public DestroyedProcedure destroyedProcedure;

        public bool debug = false;

        public bool HasVictimEffect => victimEffect != null && victimEffect.HasMinimalReq;

        public bool HasNature => nature != null;
        public bool HasImmunity => immunity != null;
        public bool HasChance => chance != null;

        public bool HasDestroyProcedure => destroyedProcedure != null;

        public bool HasPreExplosionSpawn => !preExplosionSpawn.NullOrEmpty();
        public bool HasPostExplosionSpawn => !postExplosionSpawn.NullOrEmpty();
        public bool HasDecoration => decoration != null;
    }

    public class VictimEffectOnImpact
    {
        public JobDef jobDef;
        public StatDef resistanceStatDef;
		public FloatRange chanceMultiplier = new FloatRange(.2f, .5f);
        public List<ThingDef> immuneToEffectThings;
        public bool resumeJobAfterWards = true;

        public bool HasMinimalReq => jobDef != null;
        public bool HasResistance => resistanceStatDef != null;
        public bool HasImmuneThings => !immuneToEffectThings.NullOrEmpty();
    }

    public class ExplosionNature
    {
        public FloatRange radius = new FloatRange(1, 1);
        public SkillDef radiusLinkedSkill;
        public float radiusRatioMin = .1f;
        public SimpleCurve radiusCurve = new SimpleCurve {
            new CurvePoint(0f, .1f),
            new CurvePoint(.25f, .35f),
            new CurvePoint(.5f, .5f),
            new CurvePoint(.75f, 7f),
            new CurvePoint(1f, 1f)
        };

        public SkillDef damageLinkedSkill;// = SkillDefOf.Shooting;
        public DamageDef damageType;// = DamageDefOf.Bomb;
        public FloatRange damageMultiplier = new FloatRange(.2f, .5f);
        public FloatRange damageLimit = new FloatRange(.5f, 15f);
        public float damageRatioMin = .1f;
        public SimpleCurve damageCurve = new SimpleCurve { new CurvePoint(.1f, .05f), new CurvePoint(.5f, .35f), new CurvePoint(.95f, 1f) };

        public bool damageFallOff = true;
        public bool forceNormalSpeedIfTriggered = true;
        public FloatRange chanceToStartFire = new FloatRange(.3f, .5f);

        public List<ThingDef> blockTriggerExplosionThingDefs;
    }

    public class ExplosionSpawnItem
    {
        public ThingDef thingDef;
        public FloatRange chance = new FloatRange(.5f, .7f);
        public IntRange stack = new IntRange(1, 1);
        public float weight=1;
    }

    public class DestroyedProcedure
    {
        public ThingDef filthLeft;
        public ThingDef thingSpawnedForTrees;

        public bool HasNonTree => filthLeft != null;
        public bool HasTree => thingSpawnedForTrees != null;
    }

    public class ExplosionDecoration
    {
        public SoundDef soundOnImpact;

        public ThingDef pawnEffectMote;
        public ThingDef smokeExplosionMote;
        public ThingDef blastExplosionMote;

        public IntRange smokeNum = new IntRange(3, 5);
        public FloatRange blastNumLimiter = new FloatRange(4, 6);

        public bool HasSoundOnImpact => soundOnImpact != null;
        public bool HasPawnEffectMote => pawnEffectMote != null;
        public bool HasSmokeExplosionMote => smokeExplosionMote != null;
        public bool HasBlastExplosionMote => blastExplosionMote != null;
    }

    public class ExplosionChance
    {
        public SkillDef linkedSkill;// = SkillDefOf.Shooting;
        public SimpleCurve chanceCurve = new SimpleCurve { new CurvePoint(.1f, .05f), new CurvePoint(.5f, .35f), new CurvePoint(.95f, 1f) };
        public FloatRange randomChanceFactor = new FloatRange(.75f, 1.25f);

        public FloatRange limits = new FloatRange(.05f, .95f);
    }

    public class ExplosionImmunity
    {
        public List<ThingDef> ignoredByDamageThingDefs;
        public bool addAttackerThingDef = false;
        public bool addAttacker = false;
        public bool addPawnsFromAttackerFaction = false;
        public bool addThingsFromAttackerFaction = false;
        public int limit = 50;

        public bool HasIgnoredThingDefs => !ignoredByDamageThingDefs.NullOrEmpty();
    }
}
