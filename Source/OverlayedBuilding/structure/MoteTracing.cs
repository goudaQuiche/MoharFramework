using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{
    public class MoteTracing
    {
        public Thing mote = null;
        public MyDefs.DisplayCondition condition = MyDefs.DisplayCondition.Undefined;
        public MyDefs.DisplayOrigin origin = MyDefs.DisplayOrigin.Undefined;
        public int graceTime = 0;

        public MoteTracing(MoteDecoration moteProps, bool myDebug=false)
        {
            condition = GetCondition(moteProps, myDebug);
            origin = GetOrigin(moteProps, myDebug);
            Tools.Warn("Initialized tracer for " + moteProps.moteDef.defName, myDebug);
        }

        MyDefs.DisplayCondition GetCondition(MoteDecoration props, bool myDebug = false)
        {
            MyDefs.DisplayCondition result = MyDefs.DisplayCondition.Undefined;

            if (props.whenFueled)
                result = MyDefs.DisplayCondition.WhenFueled;

            if (props.whenPowered )
            {
                if (result != MyDefs.DisplayCondition.Undefined)
                    return ErrorDisplayCondition(myDebug);
                result = MyDefs.DisplayCondition.WhenPowered;
            }

            if (props.whenFueledAndPowered)
            {
                if (result != MyDefs.DisplayCondition.Undefined)
                    return ErrorDisplayCondition(myDebug);
                result = MyDefs.DisplayCondition.WhenFueledAndPowered;
            }

            if (props.whenWorker)
            {
                if (result != MyDefs.DisplayCondition.Undefined)
                    return ErrorDisplayCondition(myDebug);
                result = MyDefs.DisplayCondition.WhenWorker;
            }
            if (props.noCondition)
            {
                if (result != MyDefs.DisplayCondition.Undefined)
                    return ErrorDisplayCondition(myDebug);
                result = MyDefs.DisplayCondition.NoCondition;
            }

            return result;
        }

        MyDefs.DisplayCondition ErrorDisplayCondition(bool myDebug=false)
        {
            Tools.Warn("you cant have 2 active display conditions", myDebug);
            return MyDefs.DisplayCondition.Undefined;
        }

        MyDefs.DisplayOrigin GetOrigin(MoteDecoration props, bool myDebug = false)
        {
            MyDefs.DisplayOrigin result = MyDefs.DisplayOrigin.Undefined;

            if (props.buildingCentered)
                result = MyDefs.DisplayOrigin.BuildingCenter;

            if (props.interactionCell)
            {
                if (result != MyDefs.DisplayOrigin.Undefined)
                    return ErrorOrigin(myDebug);
                result = MyDefs.DisplayOrigin.InteractionCell;
            }

            if (props.workerHead)
            {
                if (result != MyDefs.DisplayOrigin.Undefined)
                    return ErrorOrigin(myDebug);
                if (!props.whenWorker)
                {
                    Tools.Warn("you cant have workerHead origin if you dont have whenWorker condition", myDebug);
                    return MyDefs.DisplayOrigin.Undefined;
                }
                result = MyDefs.DisplayOrigin.WorkerHead;
            }

            return result;
        }

        MyDefs.DisplayOrigin ErrorOrigin(bool myDebug = false)
        {
            Tools.Warn("you cant have 2 mote origins", myDebug);
            return MyDefs.DisplayOrigin.Undefined;
        }
    }
}