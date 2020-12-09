using Verse;

namespace MoharCustomHAR
{
    public static class BodyPartTools
    {
        public static bool IsSearchedBodyPart (this BodyPartRecord bpr, string HarBodyPart)
        {
            return bpr.untranslatedCustomLabel == HarBodyPart || bpr.def.defName == HarBodyPart;
        }
    }
}
