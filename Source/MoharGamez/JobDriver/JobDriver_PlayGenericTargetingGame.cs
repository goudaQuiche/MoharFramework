using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoharGamez
{
    public class JobDriver_PlayGenericTargetingGame : JobDriver_WatchBuilding
    {
        /*
         MoteMaker.MakeMoodThoughtBubble
         Grenade ArcHeightFactor 
         C:\Program Files (x86)\Steam\SteamApps\common\RimWorld\Data\Core\Defs\ThingDefs_Misc\Weapons\RangedIndustrialGrenades.xml
         https://github.com/Chillu1/RimWorldDecompiled/blob/a917643cda263dc432279afa8b9aa7ed8936eaf2/Verse/Projectile.cs
         make ball detect other balls ?
         */
        readonly string MyName = "JobDriver_PlayGenericTargetingGame";
        bool MyDebug => gameSettings?.debug ?? false;
        bool MyTogetherThoughtDebug => gameSettings?.debugTogetherThought ?? false;
        public string PawnLabel => pawn.LabelShort;

        //Available parameters
        //public List<GameSettings> gameSettingsList = null;
        public GameSettings gameSettings = null;

        //Random weighted parameter
        public ProjectileOption projectileOption = null;
        public MoteParameter PickedMoteParam = null;

        // Common to all projectiles parameters
        public SkillDef SkillDefScaling => gameSettings.skillDefScaling;

        // period
        public int ThrowInterval => gameSettings.throwInterval;
        public float PlayedTogetherRatio => gameSettings.playedTogetherRatio;

        // play together
        public int PlayedTogetherThreshold => (int)(PlayedTogetherRatio * ThrowInterval);
        List<ThoughtParameter> ThoughtsParams => (!HasPlayingTogetherThoughts ? null : gameSettings?.thoughtList?.thoughtOptionPlayingTogether);
        public ThoughtParameter SingleTogetherThought => ThoughtsParams?.FirstOrFallback();

        public ThoughtDef PlayedTogetherThought => SingleTogetherThought?.thoughtPool?.FirstOrFallback();
        public Texture2D PlayedTogetherIcon => ContentFinder<Texture2D>.Get( SingleTogetherThought?.iconPool.RandomElementWithFallback() );
        public ThingDef PlayedTogetherBubble => SingleTogetherThought.bubblePool.FirstOrFallback();
        public float TogetherThoughtChance => SingleTogetherThought.triggerChance;
        public float MinThoughtChance => SingleTogetherThought.minTriggerChance;

        public int TogetherThoughtsNum = 0;

        // throw interval
        public bool IsTimeToThrow => pawn.IsHashIntervalTick(ThrowInterval);

        // Depending on Randomly picker option; depends on the type of the mote
        public FloatRange Speed => PickedMoteParam.speed;
        public FloatRange Rotation => PickedMoteParam.rotation;

        public Thing JoyBuilding => TargetA.Thing;

        //
        public List<ThingDef> DestroyingMotes => HasGameSettings && gameSettings.HasDestroyingMotes ? gameSettings.bubbleInteraction.destroyingMotes : null;
        public List<ThingDef> ResistantMotes => HasGameSettings && gameSettings.HasResistantMotes ? gameSettings.bubbleInteraction.resistantMotes : null;

        //
        public IEnumerable<IntVec3> WatchCells;
        public int OtherPlayersNum = 0;

        //Lambda things

        public bool HasGameSettings => gameSettings != null;
        public bool HasPlayingTogetherThoughts => HasGameSettings && gameSettings.HasPlayingTogetherThought;

        public bool HasProjectileOption => projectileOption != null;

        public bool HasPickedMoteOption => HasProjectileOption && projectileOption.IsMoteType;
        public bool HasPickedShadowMoteOption => HasProjectileOption && projectileOption.IsShadowMoteType;

        public bool HasAtLeastOneOption => HasPickedShadowMoteOption || HasPickedMoteOption;

        public bool HasPickedOption => HasProjectileOption && HasAtLeastOneOption;
        public bool HasWatchCells => !WatchCells.EnumerableNullOrEmpty();

        public IntVec3 PetanqueSpotCell => TargetA.Cell;

        public ThingDef JoyBuildingStuff;
        public bool HasStuff => JoyBuildingStuff != null;

        int AttemptNum = 0;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref TogetherThoughtsNum, "TogetherThoughtsNum");
        }

        public ThingDef MoteDef {
            get
            {
                if (PickedMoteParam.HasRegularMoteDef)
                    return PickedMoteParam.moteDef;
                else if (PickedMoteParam.HasStuffMotePool)
                    return RetrieveStuffMoteDef;

                return null;
            }
        }

        public ThingDef RetrieveStuffMoteDef
        {
            get
            {
                if (!HasStuff)
                {
                    Tools.Warn("RetrieveStuffMoteDef building stuff not found => null ", MyDebug);
                    return null;
                }

                foreach(ThingDef curStuffMoteDef in PickedMoteParam.stuffMotePool)
                    if (curStuffMoteDef.graphicData.color == JoyBuildingStuff.stuffProps.color)
                    {
                        Tools.Warn("RetrieveStuffMoteDef found " + curStuffMoteDef, MyDebug);
                        return curStuffMoteDef;
                    }

                Tools.Warn("RetrieveStuffMoteDef found nothing, default=" + PickedMoteParam.stuffMotePool[0], MyDebug);

                return PickedMoteParam.stuffMotePool[0];
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
                " - HasPickedMoteOption: " + HasPickedMoteOption +
                " - HasPickedShadowMoteOption: " + HasPickedShadowMoteOption
                , MyDebug);

            WatchCells = WatchBuildingUtility.CalculateWatchCells(JoyBuilding.def, PetanqueSpotCell, JoyBuilding.Rotation, Map);

            return Didit;
        }
        /*
        public override void Notify_Starting()
        {
            base.Notify_Starting();
            //make building hide postdraw balls
        }
        */

        void SetStuff()
        {
            string DebugStr = PawnLabel + " " + MyName + " SetStuff";

            if (JoyBuilding == null)
            {
                Tools.Warn(DebugStr + " null TargetA.Thing", MyDebug);
                return;
            }

            //Thing JoyBuilding = TargetA.Thing;

            if (!JoyBuilding.def.MadeFromStuff || JoyBuilding.Stuff == null)
            {
                Tools.Warn(DebugStr + " JoyBuilding not made from stuff or stuff = null", MyDebug);
                return;
            }

            Tools.Warn(DebugStr + "found stuff=" + JoyBuilding.Stuff, MyDebug);

            JoyBuildingStuff = JoyBuilding.Stuff;
        }

        bool ParameterInitialization()
        {
            string DebugStr = PawnLabel + " " + MyName + " WatchTickAction";

            if (HasGameSettings)
                return true;

            if (AttemptNum > 20)
                return false;

            Tools.Warn(DebugStr + " Trying to ParameterInitialization - SetParameters",MyDebug);
            AttemptNum++;

            bool DidIt = SetParameters();
            Tools.Warn(DebugStr + " DidIt:" + DidIt + " ; attempts:" + AttemptNum, MyDebug);

            SetStuff();
            return DidIt;
        }

        bool PickThrowOption()
        {
            this.ResetPickedOption();
            return HasAtLeastOneOption;
        }

        // depending on the projectile option, use the adaptated mote creator
        void ThrowProjectile()
        {
            if (HasPickedMoteOption)
            {
                this.MoteSpawner_ThrowObjectAt();
            }
            else if (HasPickedShadowMoteOption)
            {
                this.ShadowMoteSpawner_ThrowObjectAt();
            }
        }

        void TryThrowProjectile()
        {
            // depending on 
            if (!IsTimeToThrow)
                return;

            if(!PickThrowOption())
                return;

            pawn.rotationTracker.FaceCell(PetanqueSpotCell);

            ThrowProjectile();

            this.IsSomeonePlayingWithMe(out OtherPlayersNum, MyTogetherThoughtDebug);
        }

        protected override void WatchTickAction()
        {
            //Tools.Warn( MyName + " WatchTickAction - Entering WatchTickAction", myDebug);
            if (!ParameterInitialization())
                return;

            TryThrowProjectile();
        }

        /*
        void ManageOldMotes()
        {
            List<int> MotesToForget = new List<int>();
            //Tools.Warn("ManageOldMotes - Browsing " + MoteThrownList.Count + "motes", myDebug);
            for (int i = 0; i < MoteThrownList.Count; i++)
            {
                MoteThrown curMote = MoteThrownList[i];
                if (curMote == null || !curMote.Spawned)
                {
                    MotesToForget.Add(i);
                    //Tools.Warn(MyName + "ManageOldMotes - Deadmote - Adding mote n°" + i + " to MotesToForget" , myDebug);
                    continue;
                }

                if (curMote.airTimeLeft <= 0)
                //if (curMote.Speed <= projectileOption.triggerImpactSpeedLimit)
                {
                    if (HasImpactMote)
                        this.ThrowImpactMote(curMote);

                    MotesToForget.Add(i);
                    //Tools.Warn(MyName + "ManageOldMotes - Non moving mote - Adding mote n°" + i + " to MotesToForget", myDebug);
                }
            }
            for (int i = MoteThrownList.Count - 1; i >= 0; i--)
            {
                if (MotesToForget.Contains(i))
                {
                    MoteThrownList.RemoveAt(i);
                    //Tools.Warn(MyName + "ManageOldMotes - Deleted mote n°" + i, myDebug);
                }
                    
            }
        }
        */

    }
}