using Verse;


namespace YAHA
{
    public enum TriggerEvent
    {
        [Description("apparel changed")]
        apparel,

        [Description("draft toggle")]
        draft,

        [Description("hediff added/removed")]
        hediff,
        
        [Description("weapon changed")]
        weapon,

        [Description("got wounded")]
        anyWound,
        [Description("got wounded by enemy")]
        enemyWound,
        [Description("got wounded by ally")]
        friendlyFire,


        [Description("empty event")]
        empty
    }


}
