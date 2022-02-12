using Verse;


namespace YAHA
{
    public static class Tools
    {
        public static bool OkPawn(this Pawn pawn)
        {
            return ((pawn != null) && (pawn.Map != null));
        }

    }
}
