/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using Verse;

namespace LTF_Slug
{
	public class CompProperties_MakeIceOverlay : CompProperties
	{

        public int maxTicks = 1000;
        //public bool rotate = true;
        //public bool opacityPulse = true;

		
		public CompProperties_MakeIceOverlay()
		{
			this.compClass = typeof(Comp_MakeIceOverlay);
		}
	}
}
