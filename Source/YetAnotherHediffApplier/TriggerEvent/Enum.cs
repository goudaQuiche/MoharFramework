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

        [Description("empty event")]
        empty
    }


}
