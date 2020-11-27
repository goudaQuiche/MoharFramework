using UnityEngine;
using RimWorld;
using Verse.Sound;
using Verse;
using Verse.AI;
using System;
using System.Collections.Generic;
using System.Linq;

// ConPoDra stands for Conditionnal PostDraw

    //Todo
//post expose data
namespace ConPoDra
{
    public class CompConditionalPostDraw : ThingComp
    {
        public CompProperties_ConditionalPostDraw Props => (CompProperties_ConditionalPostDraw)props;

        Building building = null;
        public Pawn Worker = null;

        public CompPowerTrader compPower = null;
        public CompRefuelable compFuel = null;

        public IEnumerable<ReservationManager.Reservation> reservations;
        List<IntVec3> WatchCells = new List<IntVec3>();
        public bool HasWatchCells => HasWatchArea && !WatchCells.EnumerableNullOrEmpty();

        List<Tracer> MaterialIndexList = new List<Tracer>();
        int CurMaterialIndex => MaterialIndexList[PostDrawIndex].Index;

        int PostDrawIndex = 0;

        public PostDrawTask CurPostDrawTask => PostDrawIndex >= Props.postDraw.Count ? null : Props.postDraw[PostDrawIndex];

        bool HasRegularMaterial => !CurPostDrawTask.materialPool.NullOrEmpty();
        bool HasStuffMaterial => !CurPostDrawTask.stuffMaterialPool.NullOrEmpty();

        ThingDef CurrentStuffThingDef => CurMaterialIndex >= CurPostDrawTask.stuffMaterialPool.Count ? null : CurPostDrawTask.stuffMaterialPool[CurMaterialIndex].material;
        ThingDef CurrentRegularThingDef => CurMaterialIndex >= CurPostDrawTask.materialPool.Count ? null : CurPostDrawTask.materialPool[CurMaterialIndex];
        Material CurrentMaterial => CurrentThingDef?.DrawMatSingle ?? null;

        public bool AnyTaskWithStuffMaterial => Props.postDraw.Any(p => p.HasStuffMaterialPool);

        AltitudeLayer CurrentAltitudeLayer => CurrentThingDef?.altitudeLayer ?? 0;

        //Nature of parent
        public bool HasFuelComp => compFuel != null;
        public bool HasPowerComp => compPower != null;
        public bool IsMadeOfStuff => parent.def.MadeFromStuff && parent.Stuff != null;

        public bool HasPower => HasPowerComp && compPower.PowerOn;
        public bool HasFuel => HasFuelComp && compFuel.Fuel > 0;
        
        public bool IsBuilding => building != null;
        public bool HasInteractionCells => IsBuilding && building.InteractionCell != null;
        public bool HasWatchArea => IsBuilding && building.def.PlaceWorkers != null && building.def.PlaceWorkers.Any((PlaceWorker y) => y is PlaceWorker_WatchArea);

        public Conditions CurCondition => CurPostDrawTask?.condition ?? null;

        //Condition
        public bool RequiresWorker => CurCondition?.ifWorker ?? false;
        public bool RequiresNoWorker => CurCondition?.ifNoWorker ?? false;

        public bool RequiresFuelCheck => CurCondition?.ifFueled ?? false && HasFuelComp ;
        public bool RequiresPowerCheck => CurCondition?.ifPowered ?? false && HasPowerComp;
        //Condition reserved
        public bool IsTimeToUpdateReservation => AnyTaskRequiresReservationCheck && (Find.TickManager.TicksGame % Props.workerReservationUpdateFrequency == 0);

        public bool AnyTaskRequiresReservationCheck => Props.postDraw.Any(p => p.condition.ifWorker || p.condition.ifNoWorker);
        public bool RequiresReservationUpdate = false;

        public bool IsReserved => !reservations.EnumerableNullOrEmpty();
        public ReservationManager.Reservation FirstReservation => reservations.FirstOrDefault() ?? null;
        public bool HasWorker => Worker != null;
        public bool HasNonMovingWorker => IsReserved && HasWorker && !Worker.pather.MovingNow;

        public bool RequiresInteractionCellCheck => CurCondition?.ifWorkerOnInteractionCell ?? false && HasInteractionCells;
        public bool RequiresWorkerTouching => CurCondition?.ifWorkerTouch?? false;
        public bool RequiresWorkerWatching => CurCondition?.ifWorkerOnWatchArea ?? false && HasWatchCells;

        public bool RequiresSelection => CurCondition?.ifSelected ?? false && parent.def.selectable;
        public bool RequiresNothing => CurCondition?.noCondition ?? false;
        
        public bool HasWorkerOnInteractionCell => HasNonMovingWorker && Worker.Position == building.InteractionCell;
        public bool HasWorkerTouchingBuilding => HasNonMovingWorker && building.OccupiedRect().AdjacentCells.Contains(Worker.Position);
        public bool HasWorkerInWatchArea => HasNonMovingWorker && HasWatchCells && WatchCells.Contains(Worker.Position);

        public bool IsTimeToUpdate => Find.TickManager.TicksGame % Props.workerReservationUpdateFrequency == 0;

        bool IsSelected => Find.Selector.IsSelected(parent);

        bool TriggersSoundActivityOnStop => CurPostDrawTask.HasSoundMaterialPool && (CurPostDrawTask.soundMaterialPool.HasStopSound || CurPostDrawTask.soundMaterialPool.HasSustainSound);
        bool TriggersSoundActivityOnStart => (CurPostDrawTask.HasSoundMaterialPool && (CurPostDrawTask.soundMaterialPool.HasStartSound || CurPostDrawTask.soundMaterialPool.HasSustainSound));

        public Vector3 CurScale => CurPostDrawTask?.scale ?? Vector3.one;

        public bool CurTickDrivenScale => CurPostDrawTask?.tickDrivenScale ?? false;
        public FloatRange CurXScaleRange => CurPostDrawTask?.xScaleRange?? new FloatRange(0,0);
        public FloatRange CurYScaleRange => CurPostDrawTask?.yScaleRange?? new FloatRange(0, 0);

        public bool CurTickDrivenRotation => CurPostDrawTask?.tickDrivenRotation ?? false;
        public float CurRotationSpeed => CurPostDrawTask?.rotationSpeed ?? 1;

        public Vector3 CurOffset => CurPostDrawTask?.offset ?? Vector3.zero;
        public bool CurVanillaPulse => CurPostDrawTask?.vanillaPulse ?? false;
        public bool CurAllowBrowse => CurPostDrawTask?.allowMaterialBrowse ?? false;
        public string CurLabel => CurPostDrawTask?.label ?? "empty";

        public bool MyDebug => Props.debug;

        public ThingDef CurrentThingDef
        {
            get
            {
                if (HasRegularMaterial)
                {
                    return CurrentRegularThingDef;
                }else if (HasStuffMaterial)
                {
                    return CurrentStuffThingDef;
                }
                return null;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();

            if (parent.Negligeable())
                return;

            for (PostDrawIndex = 0; PostDrawIndex < Props.postDraw.Count; PostDrawIndex++)
            {
                if (!MaterialIndexList[PostDrawIndex].Displayed)
                    continue;

                Vector3 drawPos = parent.DrawPos;
                drawPos.y = CurrentAltitudeLayer.AltitudeFor();
                drawPos = drawPos + CurOffset;

                Vector2 parentDrawSize = parent.def.graphicData.drawSize;
                //float drawSize = .x * CurScale;
                Vector3 MaterialSize = new Vector3(parentDrawSize.x * CurScale.x, 1f, parentDrawSize.y * CurScale.y);
                if (CurTickDrivenScale)
                {
                    float tickFactor = parent.Mirror1();
                    MaterialSize.x += CurXScaleRange.min + CurXScaleRange.Span * tickFactor;
                    MaterialSize.z += CurYScaleRange.min + CurYScaleRange.Span * tickFactor;
                }

                Material material = CurVanillaPulse ? FadedMaterialPool.FadedVersionOf(CurrentMaterial, parent.VanillaPulse()) : CurrentMaterial;

                Matrix4x4 MaterialMatrix = default(Matrix4x4);

                Quaternion quaternion;
                if (CurTickDrivenRotation) {
                    quaternion = Quaternion.AngleAxis(parent.Loop360( CurRotationSpeed), Vector3.up);
                }
                else
                {
                    quaternion = Quaternion.AngleAxis(0, Vector3.up);
                }
                MaterialMatrix.SetTRS(drawPos, quaternion, MaterialSize);

                Graphics.DrawMesh(MeshPool.plane10, MaterialMatrix, material, 0);
            }
        }

        public void SetStuffMaterialIndexes()
        {
            if (!IsMadeOfStuff)
            {
                Tools.Warn(parent.Label + " is not made of stuff", MyDebug);
                return;
            }
            ThingDef stuffIngredient = parent.Stuff;

            for (int i = 0; i < Props.postDraw.Count; i++)
            {
                PostDrawTask PDT = Props.postDraw[i];
                if (PDT.HasStuffMaterialPool)
                {
                    for (int j = 0; j < PDT.stuffMaterialPool.Count; j++)
                    {
                        if (stuffIngredient == PDT.stuffMaterialPool[j].stuff)
                            MaterialIndexList[i].Index = j;
                        Tools.Warn(
                            "Found " + PDT.stuffMaterialPool[j].stuff +
                            " => chose " + PDT.stuffMaterialPool[j].material.defName +
                            " ; i: " + i +
                            " ; j:" + j
                            , MyDebug
                        );
                    }
                }
            }
        }
        
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Collections.Look(ref WatchCells, "WatchCells", LookMode.Value, new object[0]);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && MaterialIndexList == null)
            {
                WatchCells = new List<IntVec3>();
            }

            Scribe_Collections.Look(ref MaterialIndexList, "MaterialIndexList", LookMode.Deep, new object[0]);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && MaterialIndexList == null)
            {
                MaterialIndexList = new List<Tracer>();
            }

        }
        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            string debugStr = MyDebug ? parent.LabelShort + " PostSpawnSetup -" : "";

            compFuel = parent.TryGetComp<CompRefuelable>();
            compPower = parent.TryGetComp<CompPowerTrader>();
            if (parent is Building b)
                building = b;

            RequiresReservationUpdate = AnyTaskRequiresReservationCheck;
            if (RequiresReservationUpdate)
            {
                this.UpdateReservationAndWorker();
                Tools.Warn(debugStr + "updated reservation", MyDebug);
            }

            if (respawningAfterLoad)
            {
                for (PostDrawIndex = 0; PostDrawIndex < Props.postDraw.Count; PostDrawIndex++)
                    if(MaterialIndexList[PostDrawIndex].Displayed == true && CurPostDrawTask.HasSoundMaterialPool && CurPostDrawTask.soundMaterialPool.HasSustainSound)
                        MaterialIndexList[PostDrawIndex].sustainer = CurPostDrawTask.soundMaterialPool.soundSustain.TrySpawnSustainer(new TargetInfo(parent.Position, parent.Map));

                return;
            }

            if (HasWatchArea)
            {
                WatchCells = WatchBuildingUtility.CalculateWatchCells(parent.def, parent.Position, parent.Rotation, parent.Map).ToList();
                Tools.Warn(debugStr + "updated WatchCells " + (HasWatchCells ? WatchCells.Count().ToString() : ""), MyDebug);
            }

            for (PostDrawIndex = 0; PostDrawIndex < Props.postDraw.Count; PostDrawIndex++)
            {
                //MaterialIndexList.Add(0);
                MaterialIndexList.Add( new Tracer( 0, false));
            }
            Tools.Warn("MaterialIndexList size:" + MaterialIndexList.Count(), MyDebug);

            if (AnyTaskWithStuffMaterial && IsMadeOfStuff)
            {
                Tools.Warn(parent.Label + " is made of " + parent.Stuff, MyDebug);
                SetStuffMaterialIndexes();
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            //string debugStr = MyDebug ? parent.LabelShort + " CompTick -" : "";

            if (parent.Negligeable())
                return;

            this.MaybeUpdateReservations();

            for (PostDrawIndex = 0; PostDrawIndex < Props.postDraw.Count; PostDrawIndex++)
            {
                if (CurrentMaterial == null)
                {
                    MaterialIndexList[PostDrawIndex].Displayed = false;
                    continue;
                }
                if (RequiresNothing)
                {
                    MaterialIndexList[PostDrawIndex].Displayed = true;
                    continue;
                }

                if ((RequiresFuelCheck && !HasFuel) || (RequiresPowerCheck && !HasPower) ||
                    (RequiresWorker && !HasWorker) || (RequiresNoWorker && HasWorker) ||

                    (RequiresWorkerWatching && !HasWorkerInWatchArea) ||
                    (RequiresWorkerTouching && !HasWorkerTouchingBuilding) ||
                    (RequiresInteractionCellCheck && !HasWorkerOnInteractionCell) ||

                    (RequiresSelection && !IsSelected))
                {
                    if (TriggersSoundActivityOnStop && MaterialIndexList[PostDrawIndex].Displayed)
                    {
                        if (CurPostDrawTask.soundMaterialPool.HasStopSound)
                            CurPostDrawTask.soundMaterialPool.soundOnStop.PlayOneShot(new TargetInfo(parent.Position, parent.Map));

                        if (CurPostDrawTask.soundMaterialPool.HasSustainSound)
                        {
                            if (MaterialIndexList[PostDrawIndex].sustainer != null)
                            {
                                MaterialIndexList[PostDrawIndex].sustainer.End();
                                MaterialIndexList[PostDrawIndex].sustainer = null;
                            }
                        }

                    }

                    MaterialIndexList[PostDrawIndex].Displayed = false;

                    continue;
                }

                if (TriggersSoundActivityOnStart && !MaterialIndexList[PostDrawIndex].Displayed)
                {
                    if (CurPostDrawTask.soundMaterialPool.HasStartSound)
                        CurPostDrawTask.soundMaterialPool.soundOnStart.PlayOneShot(new TargetInfo(parent.Position, parent.Map));

                    if (CurPostDrawTask.soundMaterialPool.HasSustainSound)
                    {
                        MaterialIndexList[PostDrawIndex].sustainer = CurPostDrawTask.soundMaterialPool.soundSustain.TrySpawnSustainer(new TargetInfo(parent.Position, parent.Map));
                    }

                }

                MaterialIndexList[PostDrawIndex].Displayed = true;

            }
        }

        public int NextIndex(int materialIndex, int postDrawTaskIndex)
        {
            Tools.Warn("NextIndex postDrawTaskIndex?" + postDrawTaskIndex, MyDebug);

            int newValue = materialIndex + 1;

            if (newValue >= Props.postDraw[postDrawTaskIndex].materialPool.Count)
                newValue = 0;

            Tools.Warn("NextIndex answer=" + newValue, MyDebug);

            return newValue;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            for (int i = 0; i < Props.postDraw.Count; i++)
            {
                bool curAllow = Props.postDraw[i].allowMaterialBrowse || Props.postDraw[i].allowMaterialBrowseIfDevMode && Prefs.DevMode;
                string curLabel = Props.postDraw[i].label;
                int curMatIndex = MaterialIndexList[i].Index;

                if (curAllow)
                {
                    int preEvaluate = i;
                    yield return new Command_Action
                    {
                        defaultLabel = curLabel,
                        //defaultLabel = "#" + preEvaluate + " " + curLabel + " - change material",
                        //defaultDesc = "browse #" + preEvaluate + ":"+ curMatIndex + " -> " + (curMatIndex + 1),
                        icon = TexCommand.Attack,
                        action = delegate
                        {
                            MaterialIndexList[preEvaluate].Index = NextIndex(curMatIndex, preEvaluate);
                        }
                    };
                }

            }
        }
    }
}