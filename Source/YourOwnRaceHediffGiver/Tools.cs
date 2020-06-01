using AlienRace;
using Verse;

namespace YORHG
{
    public static class Tools
    {
        public static bool CheckPawn(Pawn pawn)
        {
            //return (pawn != null && pawn.Map != null && pawn.Position != null);
            return (pawn != null && pawn.Map != null);
        }
        public static void Warn(string warning, bool debug = false)
        {
            if(debug)
                Log.Warning(warning);
        }

        public static bool IsRaceMember(this Pawn pawn, string raceName)
        {
            return (pawn.def.defName == raceName);
        }

    }
}
