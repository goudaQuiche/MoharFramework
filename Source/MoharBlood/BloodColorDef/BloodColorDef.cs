using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoharBlood
{
    public class BloodColorDef : Def
    {
        public List<BloodColorSet> bloodSetList;
        public bool debug = false;
    }

    public class BloodColorSet
    {
        public Restriction restriction;
        public ColorSettings defaultValues; 

        public FleshTypeWound fleshTypeWound;
        public DamageEffecter damageEffecter;

        public List<JobMote> jobMote;

        public HealthTabBleeding healthTabBleeding;
        public BloodFilth bloodFilth;

        public bool HasFleshTypeWound => fleshTypeWound != null;
        public bool HasDamageEffecter => damageEffecter != null;

        public bool HasJobMote => !jobMote.NullOrEmpty();

        public bool HasHealthTabBleeding => healthTabBleeding != null;
        public bool HasBloodFilth => bloodFilth != null;
    }

    public class Restriction
    {
        public ThingDef race;
    }
    public class DefaultValues
    {
        public ColoringWay colorWay = ColoringWay.DefaultColor;
        public Color color = Color.magenta;
    }
    public class ColorSettings
    {
        public ColoringWay colorWay = ColoringWay.Unset;
        public Color color;
    }

    // FleshTypeWound
    public class FleshTypeWound
    {
        public FleshTypeDef fleshTypeDef;
        public List<WoundColorAssociation> woundColorList;
    }
    public class WoundColorAssociation
    {
        public List<string> textureList;
        public ColorSettings colorSet;

        public bool HasColorWay => colorSet != null;
    }

    //FleshTypeDamageEffecter
    public class DamageEffecter
    {
        public EffecterDef damageEffecterDef;
        public ColorSettings colorSet;
        public List<FleckMitigatedColor> affectedFleckList;

        public bool HasColorWay => colorSet != null;
    }

    // XML Parameters to calculate color that needs to be cached
    public class FleckMitigatedColor
    {
        public FleckDef fleckDef;
        public ColorSettings colorSet;

        public ColorMitigation mitigation;

        public bool HasColorWay => colorSet != null;
        public bool HasColorMitigation => mitigation != null;
    }

    public class ColorMitigation
    {
        public ColorMitigationType type;
        public Color customMitigation = Color.white;
    }

    public enum ColorMitigationType
    {
        [Description("AirPuff like")]
        AirPuff = 1,

        [Description("BloodSplash like")]
        BloodSplash = 2,

        [Description("BodyImpact")]
        BodyImpact = 3,

        [Description("Custom")]
        Custom = 4,

        [Description("No mitigation")]
        NoMitigation = 5
    }

    // surgeryJobMote butcherCorpseJobMote
    public class JobMote
    {
        public EffecterDef effectWorking;
        public ThingDef originMote;

        public ColorSettings colorSet;
        public List<ThingDef> replacementMotePool;

        public bool HasColorWay => colorSet != null;
    }

    //HealthTabBleeding
    public class HealthTabBleeding
    {
        public ColorSettings colorSet;
        public string replacementTex;

        public bool HasColorWay => colorSet != null;
    }

    //bloodFilth
    public class BloodFilth
    {
        public ColorSettings colorSet;

        public bool HasColorWay => colorSet != null;
    }
}
