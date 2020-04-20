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
	public class HeDiffCompProperties_MultipleHediff : HediffCompProperties
	{
        //conditionnal body
        public BodyDef bodyDef;
        //what
        public List<HediffDef> hediffToApply;
        // where is it applied
        public List<string> bodyPartDefName;

        public bool debug = false;

        public HeDiffCompProperties_MultipleHediff()
		{
			this.compClass = typeof(HeDiffComp_MultipleHediff);
		}
	}
}
