/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using RimWorld;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using UnityEngine;

using Verse;
using Verse.Sound;

namespace NewHatcher
{
	public class CompProperties_LTF_FactionChange : CompProperties
	{
        public Faction ownFaction;
        public Faction forcedFaction;

        public bool manHunter = false;
		
		public CompProperties_LTF_FactionChange()
		{
			this.compClass = typeof(Comp_LTF_FactionChange);
		}
	}
}