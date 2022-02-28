using Verse;


namespace YAHA
{
    public static class TriggerUtility
    {
        public static string GetDesc(this TriggerEvent te)
        {
            return te.DescriptionAttr();
        }
    }
}
