namespace ConPoDra
{
    public static class DisplayedUtils
    {
        public static bool ShouldBeDisplayed(this CompConditionalPostDraw comp)
        {
            bool MyDebug = comp.ItsDebugTime;
            string debugStr = MyDebug ? comp.parent.Label + " ShouldBeDisplayed - ": "";

            if (comp.RequiresSupplyCheck)
            {
                if (comp.RequiresFuelCheck && !comp.HasFuel)
                    return false;
                if (comp.RequiresPowerCheck && !comp.HasPower)
                    return false;
            }
            Tools.Warn(debugStr + " supply check ok", MyDebug);

            if (comp.RequiresWorkCheck)
            {
                if (comp.RequiresNoWorker) {
                    if (comp.HasWorker)
                        return false;
                }

                if (comp.RequiresWorker && !comp.HasWorker)
                    return false;

                if (comp.RequiresWorkerWatching && !comp.HasWorkerInWatchArea)
                    return false;

                if (comp.RequiresWorkerTouching && !comp.HasWorkerTouchingBuilding)
                    return false;

                if (comp.RequiresInteractionCellCheck && !comp.HasWorkerOnInteractionCell)
                    return false;
            }
            Tools.Warn(debugStr + " work check ok", MyDebug);

            if (comp.RequiresSelection && !comp.IsSelected)
                return false;
            Tools.Warn(debugStr + " select check ok", MyDebug);

            if (comp.RequiresThingCheck)
            {
                if (!comp.IsModuloOk)
                    return false;
                if (!comp.IsThingDefOk)
                    return false;
            }
            Tools.Warn(debugStr + " thing check ok", MyDebug);

            return true;
        }
    }
}
