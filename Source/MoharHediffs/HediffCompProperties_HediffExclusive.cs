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
	public class HeDiffCompProperties_HediffExclusive : HediffCompProperties
	{
        //what
        public List<string> hediffToNullify;
        public string hediffToApply;
        public string bodyPartName;

        public HeDiffCompProperties_HediffExclusive()
		{
			this.compClass = typeof(HeDiffComp_HediffExclusive);
		}
	}
}
