using RimWorld;
using Verse;
using System.Reflection;

namespace OLB
{
    public static class SetupCheck
    {
        public static bool CheckParentIsBuilding(this CompDecorate comp) {
            return comp.parent is Building;
        }

        public static bool CheckPropertiesNotEmpty(this CompDecorate comp)
        {
            return !comp.EmptyParameters;
        }

        public static bool CheckMoteDecoration(this CompDecorate comp)
        {
            bool OneError = false;

            foreach(MoteDecoration md in comp.ItemList)
            {
                OneError |= md.IsInvalid;
                OneError |= comp.ItemList.Any(IL => IL != md && IL.label == md.label);
                OneError |= md.transformation == null;
            }

            return !OneError;
        }

        public static bool CheckGeneral(this CompDecorate comp)
        {
            bool OneError = false;

            if (OneError |= !comp.CheckParentIsBuilding())
                Tools.Warn("this comp is meant for a building, this will fail", comp.DebugCheck);

            if (OneError |= !comp.CheckPropertiesNotEmpty())
                Tools.Warn("Properties are empty, this will fail", comp.DebugCheck);

            if (OneError |= !comp.CheckMoteDecoration())
                Tools.Warn("At least one item lacks a proper <moteDef></moteDef> <label></label> or has non unique label or no <tranformation/>", comp.DebugCheck);

            if(OneError)
                Tools.Warn("At least one error, it will most likely fail", comp.DebugCheck);
            else
                Tools.Warn("No error in the parameters, congraturations", comp.DebugCheck);


            return OneError;
        }
    }
}
