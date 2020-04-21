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
	public class HeDiffCompProperties_HediffRandom : HediffCompProperties
	{
        //what
        public List<HediffDef> hediffPool;

        public BodyDef bodyDef;
        public BodyPartDef bodyPartDef;

        public bool debug = false;
        public bool hideBySeverity=true;

        public HeDiffCompProperties_HediffRandom()
		{
			this.compClass = typeof(HeDiffComp_HediffRandom);
		}
	}
}
