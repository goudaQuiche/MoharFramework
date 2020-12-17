using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DisplayITab
{
	public class DisplayITabUtility
	{
        public static float WindowWidth = 600;
        public static float WindowHeight = 600;

        public static Vector2 DefaultImgSize = new Vector2(512,512);

        public static float ButtonWidth = 60;
        public static float ButtonHeight = 32;

        public static int BrowsingNumberCharWidth = 20;
        public static float Margin = 5;

        public static Vector2 WindowSize = new Vector2(WindowWidth, WindowHeight);

        public static void DrawCard(Rect rect, Thing thing)
        {
            Comp_ITab comp = thing.TryGetComp<Comp_ITab>();
            Vector2 ChosenImgSize = (comp == null) ? DefaultImgSize : comp.Props.imgSize;

            Rect ChosenImgRect = new Rect((WindowWidth - ChosenImgSize.x) / 2, (WindowHeight - ChosenImgSize.y) / 2, ChosenImgSize.x, ChosenImgSize.y);

            if (comp == null)
            {
                //Widgets.ThingIcon(ChosenImgRect, thing);
                Widgets.DrawTextureFitted(ChosenImgRect, thing.Graphic.MatSingle.mainTexture, 1);
                return;
            }else if (comp.HasNoPage)
            {
                if (comp.IsProcessedTex)
                {
                    Widgets.ThingIcon(ChosenImgRect, thing);
                }
                else if (comp.IsRawTex)
                {
                    Widgets.DrawTextureFitted(ChosenImgRect, thing.Graphic.MatSingle.mainTexture, 1);
                }
                return;
            }

            if (comp.IsOnTitle)
            {
                if (comp.IsProcessedTex)
                {
                    Widgets.ThingIcon(ChosenImgRect, thing);
                }
                else if (comp.IsRawTex)
                {
                    Widgets.DrawTextureFitted(ChosenImgRect, thing.Graphic.MatSingle.mainTexture, 1);
                }
            }
            else
                Widgets.ThingIcon(ChosenImgRect, comp.Props.Pages[comp.index]);

            if (Widgets.ButtonText(new Rect(Margin, WindowSize.y - ButtonHeight - Margin, ButtonWidth, ButtonHeight), "Previous"))
                comp.PreviousIndex();

            string browsingNum = (comp.IsOnTitle ? "(" : "") + "Title" + (comp.IsOnTitle ? ")" : "") + ' ';
            for (int i = 0; i < comp.Props.Pages.Count; i++)
            {
                browsingNum += (i == comp.index ? "(" : "") + (i + 1).ToString("D2") + (i == comp.index ? ")" : "") + ' ';
            }

            Widgets.TextArea(new Rect(ButtonWidth + Margin * 2, WindowSize.y - ButtonHeight - Margin, WindowWidth - ButtonWidth * 2 - Margin * 4, ButtonHeight), browsingNum);

            if (Widgets.ButtonText(new Rect(WindowSize.x - ButtonWidth, WindowSize.y - ButtonHeight - Margin, ButtonWidth, ButtonHeight), "Next"))
                comp.NextIndex();
        }
	}
}