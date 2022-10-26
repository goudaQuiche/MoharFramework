using Verse;

namespace MoharCustomHAR
{
    public static class BodyPartTools
    {
        public static bool IsSearchedBodyPartString (this BodyPartRecord bpr, string HarBodyPart)
        {
            return bpr.untranslatedCustomLabel == HarBodyPart || bpr.def.defName == HarBodyPart;
        }

        public static bool IsSearchedBodyPartBodyPartDef(this BodyPartRecord bpr, BodyPartDef HarBodyPart)
        {
            return bpr.def == HarBodyPart;
        }
    }
}
