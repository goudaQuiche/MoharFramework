using UnityEngine;
using Verse;
using RimWorld;

namespace DisplayITab
{
	public class ITab_DisplayPicture : ITab
	{
        public Thing SelectedThing
        {
            get
            {
                if (base.SelThing != null)
                {
                    return base.SelThing;
                }
                return null;
            }
        }

        public ITab_DisplayPicture()
		{
			Vector2 offsets = new Vector2(17, 17);
			this.size = DisplayITabUtility.WindowSize + offsets;
			this.labelKey = "ITab_DisplayPicture";
		}

		protected override void FillTab()
		{
			Thing thing = this.SelectedThing;
			if (thing == null)
			{
				Log.Error("display tab found no selected pawn to display.");
				return;
			}
			Rect rect = new Rect(17, 17, DisplayITabUtility.WindowWidth, DisplayITabUtility.WindowHeight);
            DisplayITabUtility.DrawCard(rect, thing);
		}
	}
}