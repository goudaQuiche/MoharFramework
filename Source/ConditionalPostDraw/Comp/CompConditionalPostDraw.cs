using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Linq;

// ConPoDra stands for Conditionnal PostDraw

    //Todo
//WatchBuildingUtility.CalculateWatchCells
namespace ConPoDra
{
    public class CompConditionalPostDraw : ThingComp
    {
        public CompProperties_ConditionalPostDraw Props => (CompProperties_ConditionalPostDraw)props;

        Building building = null;
        public Pawn worker = null;

        public CompPowerTrader compPower = null;
        public CompRefuelable compFuel = null;

        IEnumerable<ReservationManager.Reservation> reservations;

        List<int> MaterialIndexList = new List<int>();
        int CurMaterialIndex => MaterialIndexList[PostDrawIndex];

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


        public bool HasWorker => worker != null;

        public bool HasPower => HasPowerComp && compPower.PowerOn;
        public bool HasFuel => HasFuelComp && compFuel.Fuel > 0;

        public bool IsBuilding => building != null;
        public bool HasInteractionCells => IsBuilding && building.InteractionCell != null;

        public Conditions CurCondition => CurPostDrawTask?.condition ?? null;

        //Condition
        public bool RequiresFuelCheck => CurCondition?.ifFueled ?? false && HasFuelComp ;
        public bool RequiresPowerCheck => CurCondition?.ifPowered ?? false && HasPowerComp;
        //Condition reserved
        public bool RequiresReserved => (CurCondition?.ifReserved ?? false);
        public bool RequiresNotReserved => (CurCondition?.ifNotReserved ?? false);
        public bool RequiresReservationCheck => RequiresReserved || RequiresNotReserved;
        public bool IsTimeToUpdateReservation => AnyTaskRequiresReservationCheck && (Find.TickManager.TicksGame % Props.workerReservationUpdateFrequency == 0);

        public bool AnyTaskRequiresReservationCheck => Props.postDraw.Any(p => p.condition.ifReserved || p.condition.ifNotReserved);

        public bool RequiresInteractionCellCheck => CurCondition?.ifClaimantOnInteractionCell ?? false && HasInteractionCells;
        public bool RequiresSelection => CurCondition?.ifSelected ?? false && parent.def.selectable;
        public bool RequiresNothing => CurCondition?.noCondition ?? false;
        
        bool IsReserved => !reservations.EnumerableNullOrEmpty();
        bool IsOccupied => IsReserved && reservations.FirstOrDefault().Claimant.Position == building.InteractionCell;

        bool IsSelected => Find.Selector.IsSelected(parent);

        public float CurScale => CurPostDrawTask?.scale ?? 0;
        public bool CurVanillaPulse => CurPostDrawTask?.vanillaPulse ?? false;
        public bool CurAllowBrowse => CurPostDrawTask?.allowMaterialBrowse ?? false;
        public string CurLabel => CurPostDrawTask?.label ?? "empty";

        bool MyDebug => Props.debug;

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
                if (CurrentMaterial == null)
                    continue;

                if (!RequiresNothing) {
                    if (RequiresFuelCheck && !HasFuel)
                        continue;
                    if (RequiresPowerCheck && !HasPower)
                        continue;

                    if (RequiresReserved && !IsReserved)
                        continue;
                    else if (RequiresNotReserved && IsReserved)
                        continue;
                        
                    if (RequiresInteractionCellCheck && !IsOccupied)
                        continue;
                    if(RequiresSelection && !IsSelected)
                        continue;
                }

                Vector3 drawPos = parent.DrawPos;
                drawPos.y = CurrentAltitudeLayer.AltitudeFor();
                float drawSize = parent.def.graphicData.drawSize.x * CurScale;

                Vector3 MaterialSize = new Vector3(drawSize, 1f, drawSize);

                Material material = CurVanillaPulse ? FadedMaterialPool.FadedVersionOf(CurrentMaterial, parent.VanillaPulse()) : CurrentMaterial;

                Matrix4x4 MaterialMatrix = default(Matrix4x4);
                MaterialMatrix.SetTRS(drawPos, Quaternion.AngleAxis(0, Vector3.up), MaterialSize);

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
                            MaterialIndexList[i] = j;
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

            //Scribe_Values.Look(ref MaterialIndexList, "MaterialIndexList");
            //Scribe_Collections.Look(ref MaterialIndexList, "MaterialIndexList", LookMode.Reference);
            
            //Scribe_Collections.Look(ref MaterialIndexList, "MaterialIndexList", LookMode.Value, new object[0]);
            Scribe_Collections.Look(ref MaterialIndexList, "MaterialIndexList", LookMode.Value, new object[0]);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && this.MaterialIndexList == null)
            {
                MaterialIndexList = new List<int>();
            }

            //Scribe_Values.Look(ref reservations, "reservation");
            //Scribe_Collections.Look(ref reservations, "reservation");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            compFuel = parent.TryGetComp<CompRefuelable>();
            compPower = parent.TryGetComp<CompPowerTrader>();
            if (parent is Building b)
                building = b;

            if (respawningAfterLoad)
            {
                if (RequiresReservationCheck) UpdateReservation();
                return;
            }

            //MaterialIndexList = new int[Props.postDraw.Count];
            //MaterialIndexList = new List<int>();
            
            for (PostDrawIndex = 0; PostDrawIndex < Props.postDraw.Count; PostDrawIndex++)
            {
                MaterialIndexList.Add(0);
            }
            Tools.Warn("MaterialIndexList size:" + MaterialIndexList.Count(), MyDebug);

            if (AnyTaskWithStuffMaterial && IsMadeOfStuff)
            {
                Tools.Warn(parent.Label + " is made of " + parent.Stuff, MyDebug);
                SetStuffMaterialIndexes();
            }

            

            
        }

        bool UpdateReservation()
        {
            reservations = parent.Map.reservationManager.ReservationsReadOnly.Where(
                r =>
                r.Target == new LocalTargetInfo(parent) &&
                r.Job.def != JobDefOf.Maintain && r.Job.def != JobDefOf.Deconstruct && r.Job.def != JobDefOf.Repair &&
                r.Faction == Faction.OfPlayer
            ).ToList();

            if(IsReserved)
                UpdateWorker();

            return !reservations.EnumerableNullOrEmpty();
        }

        void UpdateWorker()
        {
            worker = IsOccupied ? reservations.FirstOrDefault().Claimant : null;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent.Negligeable())
                return;

            //Tools.Warn(parent?.LabelShort + "CompTick requiresreservation?" + AnyTaskRequiresReservationCheck, MyDebug);

            if (IsTimeToUpdateReservation)
            {
                bool DoHaveReservation = UpdateReservation();
                //Tools.Warn(parent?.LabelShort + " reservation: " + DoHaveReservation + " worker: " + worker?.LabelShort, MyDebug);
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
                int curMatIndex = MaterialIndexList[i];

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
                            MaterialIndexList[preEvaluate] = NextIndex(curMatIndex, preEvaluate);
                        }
                    };
                }

            }
        }
    }
}