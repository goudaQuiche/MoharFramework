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
	public class HediffCompProperties_HediffNullifier : HediffCompProperties
	{
        // tick
        public float tickerLimit = 1f;

        //what
        public List<string> hediffToNullify;

        public HediffCompProperties_HediffNullifier()
		{
			this.compClass = typeof(HediffComp_HediffNullifier);
		}
	}
}
