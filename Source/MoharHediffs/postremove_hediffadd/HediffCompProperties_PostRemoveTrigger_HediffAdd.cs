/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Collections.Generic;
using Verse;

namespace MoharHediffs
{
	public class HediffCompProperties_PostRemoveTrigger_HediffAdd : HediffCompProperties
	{
        //what
        public List<HediffDef> triggeredHediff;

        public bool debug = false;

        public HediffCompProperties_PostRemoveTrigger_HediffAdd()
		{
			this.compClass = typeof(HediffComp_PostRemoveTrigger_HediffAdd);
		}
	}
}
