using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoharJoy
{
    public class JobDriver_ThrowRocks : JobDriver
    {

        readonly string MyName = "JobDriver_ThrowRocks";
        bool MyDebug => gameSettings?.debug ?? false;

        public string PawnLabel => pawn.LabelShort;

        //Available parameters
        public GameSettings gameSettings = null;

        //Random weighted parameter
        public ProjectileOption projectileOption = null;
        public MoteParameter PickedMoteParam = null;

        public IEnumerable<ThingDef> NaturalRocksInCell = new List<ThingDef>();
        public bool HasStuff => !NaturalRocksInCell.EnumerableNullOrEmpty();

        public bool HasPickedMoteParam => PickedMoteParam != null;

        // Common to all projectiles parameters
        public SkillDef SkillDefScaling => gameSettings.skillDefScaling;

        // period
        public int ThrowInterval => gameSettings.throwInterval;
        public float PlayedTogetherRatio => gameSettings.playedTogetherRatio;

        // throw interval
        public bool IsTimeToThrow => pawn.IsHashIntervalTick(ThrowInterval);

        // Depending on Randomly picker option; depends on the type of the mote
        public FloatRange Speed => PickedMoteParam.speed;
        public FloatRange Rotation => PickedMoteParam.rotation;
        
        //Lambda things
        public bool HasGameSettings => gameSettings != null;
        public bool HasProjectileOption => projectileOption != null;
        public bool HasPickedOption => HasProjectileOption && projectileOption.IsShadowMoteType;

        int AttemptNum = 0;
        int thrownRockNum = 0;

        public IntVec3 TargetCell => TargetB.Cell;

        /*
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref gameSettings, "gameSettings");
        }
        */

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!ParameterInitialization())
                return false;

            return true;
        }

        public ThingDef MoteDef {
            get
            {
                if (PickedMoteParam.HasRegularMoteDef)
                {
                    if(PickedMoteParam.moteDef == null)
                    {
                        Tools.Warn("PickedMoteParam.moteDef is empty, still doing it, but will fail", MyDebug);
                    }
                    return PickedMoteParam.moteDef;
                }
                else if (PickedMoteParam.HasStuffMotePool && HasStuff)
                    return RetrieveStuffMoteDef;

                return null;
            }
        }

        bool SetParameters()
        {
            string DebugStr =  PawnLabel + " " + MyName + " SetParameters";

            Tools.Warn(DebugStr + " - Entering", MyDebug);

            if (pawn.CurJob == null)
            {
                Tools.Warn(DebugStr + " - no job for pawn", MyDebug);
                return false;
            } else
                Tools.Warn(
                    DebugStr + 
                    " - pawn job: " + pawn.CurJobDef +
                    " - GetType(): " + GetType()
                    , MyDebug
                );

            IEnumerable<GameSettingsDef> myGSDList =
                DefDatabase<GameSettingsDef>.AllDefs
                .Where(gsd =>
                    gsd.gameSettingsList.Any(gs => gs.driverClass == GetType() && pawn.CurJobDef == gs.jobDef)
                );
            if (myGSDList.EnumerableNullOrEmpty())
            {
                Tools.Warn(DebugStr + " - 00 no GameProjectileDef found", MyDebug);
                return false;
            }
            else
                Tools.Warn(DebugStr + " - found " + myGSDList.EnumerableCount() + " GSD", MyDebug);

            foreach (GameSettingsDef curGSD in myGSDList)
            {
                IEnumerable<GameSettings> myGPDItem = curGSD.gameSettingsList;
                foreach (GameSettings curGS in myGPDItem)
                {
                    if ((curGS.driverClass == GetType()) && (pawn.CurJobDef == curGS.jobDef))
                    {
                        gameSettings = curGS;
                        Tools.Warn(DebugStr + " - found JobDef" + curGS.jobDef, MyDebug);
                        break;
                    }
                    Tools.Warn(DebugStr +
                        " - GetType():" + GetType() +
                        " curGP.driverClass: " + curGS.driverClass +
                        " curGP.jobDef: " + curGS.jobDef +
                        "  pawn.CurJobDef: " + pawn.CurJobDef
                        , MyDebug
                    );
                }
            }

            if (!HasGameSettings)
            {
                Tools.Warn(DebugStr + " - 01 no HasGameSettings item found", MyDebug);
                return false;
            }

            bool Didit = this.RetrieveProjectileParam();

            Tools.Warn(
                DebugStr +
                " - 02 RetrieveProjectileParam:" + Didit +
                " - HasPickedOption: " + HasPickedOption +
                " - Pickedmote: " + MoteDef?.defName
                , MyDebug);

            return Didit;
        }

        public ThingDef RetrieveStuffMoteDef
        {
            get
            {
                if (!HasStuff)
                {
                    Tools.Warn("RetrieveStuffMoteDef natural rocks stuff not found => null ", MyDebug);
                    return null;
                }

                List<ThingDef> StuffMoteDef = new List<ThingDef>();
                //= PickedMoteParam.stuffMotePool.Where(m => NaturalRocksInCell.  )
                foreach (ThingDef curStuffMoteDef in PickedMoteParam.stuffMotePool)
                {
                    foreach (ThingDef curRock in NaturalRocksInCell)
                    {
                        if (curStuffMoteDef.graphicData.color == curRock.graphicData.color)
                            StuffMoteDef.Add(curStuffMoteDef);
                    }
                }
                if (!StuffMoteDef.NullOrEmpty())
                {
                    return StuffMoteDef.RandomElement();
                }

                Tools.Warn("RetrieveStuffMoteDef found nothing, default=" + PickedMoteParam.stuffMotePool[0], MyDebug);

                return PickedMoteParam.stuffMotePool[0];
            }
        }

        void SetNaturalRocksStuff()
        {
            string DebugStr = MyDebug ? PawnLabel + " " + MyName + " SetNaturalRocksStuff" : "";

            foreach (ThingDef RockDef in Find.World.NaturalRockTypesIn(pawn.Map.Tile))
            {
                Tools.Warn(DebugStr + " => " + RockDef, MyDebug);
            };

            NaturalRocksInCell = Find.World.NaturalRockTypesIn(pawn.Map.Tile);
        }

        bool ParameterInitialization()
        {
            string DebugStr = MyDebug ? PawnLabel + " " + MyName + " WatchTickAction" : "";

            if (HasGameSettings)
                return true;

            if (AttemptNum > 20)
                return false;

            Tools.Warn(DebugStr + " Trying to ParameterInitialization - SetParameters",MyDebug);
            AttemptNum++;

            bool DidIt = SetParameters();
            Tools.Warn(DebugStr + " DidIt:" + DidIt + " ; attempts:" + AttemptNum, MyDebug);

            if (gameSettings.stuffDependsOnNaturalRocks)
                SetNaturalRocksStuff();

            return DidIt;
        }

        bool PickThrowOption()
        {
            this.ResetPickedOption();
            return HasPickedOption;
        }

        // depending on the projectile option, use the adaptated mote creator
        bool ThrowProjectile()
        {
            if (!HasPickedOption)
                return false;
            
            return ( this.ShadowMoteSpawner_ThrowObjectAt() != null);
            
        }

        void TryThrowProjectile()
        {
            if (!HasGameSettings)
                if(!ParameterInitialization())
                    return;

            // depending on 
            if (!IsTimeToThrow)
                return;

            if(!PickThrowOption())
                return;

            pawn.rotationTracker.FaceCell(TargetCell);

            if (ThrowProjectile())
                thrownRockNum++;
        }

        public void TickAction()
        {
            float extraJoyGainFactor = 1 - pawn.Map.weatherManager.RainRate;
            JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.EndJob, extraJoyGainFactor);

            pawn.rotationTracker.FaceCell(TargetA.Cell);
            pawn.GainComfortFromCellIfPossible();

            //Tools.Warn( MyName + " WatchTickAction - Entering WatchTickAction", myDebug);
            TryThrowProjectile();
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            Toil toil = new Toil
            {
                
                initAction = delegate
                {
                    //pawn.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
                    pawn.rotationTracker.FaceCell(TargetB.Cell);
                    //pawn.jobs.
                },
                
                tickAction = delegate
                {
                    float extraJoyGainFactor = thrownRockNum * .2f;
                    JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.EndJob, extraJoyGainFactor);
                   
                    TickAction();
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = job.def.joyDuration
            };
            toil.FailOn(() => pawn.Position.Roofed(pawn.Map));
            toil.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn));
            yield return toil;
        }

        public override string GetReport()
        {
            string defaultReport = "Throwing rocks";
            if (!HasGameSettings || !gameSettings.HasMoodDependantReport || pawn.needs.mood == null)
                return defaultReport;

            foreach(MoodDependantReport MDR in gameSettings.moodDependantReport)
            {
                if (MDR.mood.Includes(pawn.needs.mood.CurLevel))
                {
                    return MDR.keyString.Translate( pawn.Possessive() );
                }
                    
            }

            MoodDependantReport DefaultMDR = gameSettings.moodDependantReport.Where(mdr => mdr.defaultOption).FirstOrDefault();
            if(DefaultMDR != null)
                return DefaultMDR.keyString.Translate();

            return defaultReport;
        }

    }
}