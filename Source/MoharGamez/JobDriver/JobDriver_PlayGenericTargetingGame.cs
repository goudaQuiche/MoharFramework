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
        string MyName = "JobDriver_PlayGenericTargetingGame";
        readonly bool myDebug = false;

        //Available parameters
        public List<GameProjectile> gameProjectileList = null;
        public GameProjectile gameProjectile = null;

        //Random weighted parameter
        public ProjectileOption projectileOption = null;
        public MoteParameter PickedMoteParam = null;

        // Common to all projectiles parameters
        public SkillDef SkillDefScaling => gameProjectile.skillDefScaling;

        // period
        public IntRange ThrowInterval => gameProjectile.throwInterval;
        public bool IsTimeToThrow => pawn.IsHashIntervalTick(ThrowInterval.RandomInRange);

        // Depending on Randomly picker option; depends on the type of the mote
        public FloatRange Speed => PickedMoteParam.speed;
        public FloatRange Rotation => PickedMoteParam.rotation;
        

        public bool HasGameProjectile => gameProjectile != null;
        public bool HasProjectileOption => projectileOption != null;

        public bool HasPickedMoteOption => HasProjectileOption && projectileOption.IsMoteType;
        public bool HasPickedShadowMoteOption => HasProjectileOption && projectileOption.IsShadowMoteType;

        public bool HasAtLeastOneOption => HasPickedShadowMoteOption || HasPickedMoteOption;

        public bool HasPickedOption => HasProjectileOption && HasAtLeastOneOption;

        public IntVec3 PetanqueSpotCell => TargetA.Cell;

        public ThingDef JoyBuildingStuff;
        public bool HasStuff => JoyBuildingStuff != null;

        int AttemptNum = 0;

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
                    Tools.Warn("RetrieveStuffMoteDef building stuff not found => null ", myDebug);
                    return null;
                }

                foreach(ThingDef curStuffMoteDef in PickedMoteParam.stuffMotePool)
                    if (curStuffMoteDef.graphicData.color == JoyBuildingStuff.stuffProps.color)
                    {
                        Tools.Warn("RetrieveStuffMoteDef found " + curStuffMoteDef, myDebug);
                        return curStuffMoteDef;
                    }

                Tools.Warn("RetrieveStuffMoteDef found nothing, default=" + PickedMoteParam.stuffMotePool[0], myDebug);

                return PickedMoteParam.stuffMotePool[0];
            }
        }

        bool SetParameters()
        {
            string DebugStr = pawn.LabelShort + " " + MyName + " SetParameters";

            Tools.Warn(DebugStr + " - Entering", myDebug);

            if (pawn.CurJob == null)
            {
                Tools.Warn(DebugStr + " - no job for pawn", myDebug);
                return false;
            } else
                Tools.Warn(
                    DebugStr + 
                    " - pawn job: " + pawn.CurJobDef +
                    " - GetType(): " + GetType()
                    , myDebug
                );

            IEnumerable<GameProjectileDef> myGPDList =
                DefDatabase<GameProjectileDef>.AllDefs
                .Where(gpd => 
                    gpd.gameProjectileList.Any(gp => gp.driverClass == GetType() && pawn.CurJobDef == gp.jobDef)
                );
            if (myGPDList.EnumerableNullOrEmpty())
            {
                Tools.Warn(DebugStr + " - 00 no GameProjectileDef found", myDebug);
                return false;
            }
            else
                Tools.Warn(DebugStr + " - found " + myGPDList.EnumerableCount() + " GPD", myDebug);

            foreach (GameProjectileDef curGPD in myGPDList)
            {
                IEnumerable<GameProjectile> myGPDItem = curGPD.gameProjectileList;
                foreach (GameProjectile curGP in myGPDItem)
                {
                    if ((curGP.driverClass == GetType()) && (pawn.CurJobDef == curGP.jobDef))
                    {
                        gameProjectile = curGP;
                        Tools.Warn(DebugStr + " - found JobDef" + curGP.jobDef, myDebug);
                        break;
                    }
                    Tools.Warn(DebugStr +
                        " - GetType():" + GetType() +
                        " curGP.driverClass: " + curGP.driverClass +
                        " curGP.jobDef: " + curGP.jobDef +
                        "  pawn.CurJobDef: " + pawn.CurJobDef
                        , myDebug
                    );
                }
            }


            if (!HasGameProjectile)
            {
                Tools.Warn(DebugStr + " - 01 no GameProjectile item found", myDebug);
                return false;
            }

            bool Didit = this.RetrieveProjectileParam();

            Tools.Warn(
                DebugStr +
                " - 02 RetrieveProjectileParam:" + Didit +
                " - HasPickedMoteOption: " + HasPickedMoteOption +
                " - HasPickedShadowMoteOption: " + HasPickedShadowMoteOption
                //" - HasPickedEffecterOption: " + HasPickedEffecterOption
                , myDebug);

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
            string DebugStr = pawn.LabelShort + " " + MyName + " SetStuff";

            if (TargetA.Thing == null)
            {
                Tools.Warn(DebugStr + " null TargetA.Thing", myDebug);
                return;
            }

            Thing JoyBuilding = TargetA.Thing;

            if (!JoyBuilding.def.MadeFromStuff || JoyBuilding.Stuff == null)
            {
                Tools.Warn(DebugStr + " JoyBuilding not made from stuff or stuff = null", myDebug);
                return;
            }

            Tools.Warn(DebugStr + "found stuff=" + JoyBuilding.Stuff, myDebug);

            JoyBuildingStuff = JoyBuilding.Stuff;
        }

        bool ParameterInitialization()
        {
            string DebugStr = pawn.LabelShort + " " + MyName + " WatchTickAction";

            if (HasGameProjectile)
                return true;

            if (AttemptNum > 20)
                return false;

            Tools.Warn(DebugStr + " Trying to ParameterInitialization - SetParameters", myDebug);
            AttemptNum++;

            bool DidIt = SetParameters();
            Tools.Warn(DebugStr + " DidIt:" + DidIt + " ; attempts:" + AttemptNum, myDebug);

            SetStuff();
            return DidIt;
        }

        void ThrowProjectile()
        {
            if (!IsTimeToThrow)
                return;

            this.ResetPickedOption();
            if (!HasAtLeastOneOption)
                return;

            pawn.rotationTracker.FaceCell(PetanqueSpotCell);

            if (HasPickedMoteOption)
            {
                this.MoteSpawner_ThrowObjectAt();
            }
            else if (HasPickedShadowMoteOption)
            {
                this.ShadowMoteSpawner_ThrowObjectAt();
                //this.CalculateThrowDistance(destinationCell);
            }
        }

        protected override void WatchTickAction()
        {
            //Tools.Warn( MyName + " WatchTickAction - Entering WatchTickAction", myDebug);
            if (!ParameterInitialization())
                return;

            ThrowProjectile();
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