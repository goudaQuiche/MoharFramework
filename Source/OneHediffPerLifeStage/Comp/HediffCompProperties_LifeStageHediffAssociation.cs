/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using Verse;
using System.Collections.Generic;
using RimWorld;

namespace OHPLS
{
    public class HediffCompProperties_LifeStageHediffAssociation : HediffCompProperties
    {
        public string race;
        public BodyPartDef bodyPartDef;
        public string bodyPartLabel;

        public List<Association> associations;
        public bool debug = false;

        public HediffCompProperties_LifeStageHediffAssociation()
        {
            this.compClass = typeof(HediffComp_LifeStageHediffAssociation);
        }
    }
}