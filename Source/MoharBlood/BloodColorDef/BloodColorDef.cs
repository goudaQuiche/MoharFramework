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
    }

    public class BloodColorSet
    {
        public Restriction restriction;
        public ColorSettings defaultValues; 

        public FleshTypeWound fleshTypeWound;
        public DamageEffecter damageEffecter;
        public JobMote surgeryJobMote;
        public JobMote butcherCorpseJobMote;
        public HealthTabBleeding healthTabBleeding;
        public BloodFilth bloodFilth;

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
    }

    public enum ColorMitigation
    {
        [Description("AirPuff like")]
        AirPuff = 1,

        [Description("BloodSplash like")]
        BloodSplash = 2,

        [Description("BodyImpact")]
        BodyImpact = 3,

        [Description("No mitigation")]
        NoMitigation = 4
    }

    // surgeryJobMote butcherCorpseJobMote
    public class JobMote
    {
        public ColorSettings colorSet;
        public ThingDef mote;

        public bool HasColorWay => colorSet != null;
    }

    //HealthTabBleeding
    public class HealthTabBleeding
    {
        public ColorSettings colorSet;

        public bool HasColorWay => colorSet != null;
    }

    //bloodFilth
    public class BloodFilth
    {
        public ColorSettings colorSet;

        public bool HasColorWay => colorSet != null;
    }
}
