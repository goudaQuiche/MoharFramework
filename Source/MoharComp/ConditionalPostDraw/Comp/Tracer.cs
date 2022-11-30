using Verse.Sound;
using System.Collections.Generic;
using Verse;
using RimWorld;

// ConPoDra stands for Conditionnal PostDraw

    //Todo
//WatchBuildingUtility.CalculateWatchCells
namespace ConPoDra
{
    public class Tracer : IExposable
    {
        public bool Displayed = false;
        public int Index = 0;
        public Sustainer sustainer = null;

        public Tracer(int nIndex, bool nDisplayed)
        {
            Index = nIndex;
            Displayed = nDisplayed;
        }
        public Tracer()
        {
        }

        public void ExposeData()
        {
            
            Scribe_Values.Look(ref Index, "Index", defaultValue: 0);
            Scribe_Values.Look(ref Displayed, "Displayed", defaultValue: false);
            //Scribe_References.Look(ref sustainer, "sustainer");
            //Scribe_References.Look(ref sustainer, "sustainer");
        }
    }
}