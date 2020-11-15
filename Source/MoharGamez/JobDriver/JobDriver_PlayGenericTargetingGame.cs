using RimWorld;
using Verse;
using System.Collections.Generic;
using Verse.Sound;

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

        public IntRange ThrowInterval => gameProjectile.throwInterval;
        public bool IsTimeToThrow => pawn.IsHashIntervalTick(ThrowInterval.RandomInRange);

        // Depending on Randomly picker option; depends on the type of the mote
        public FloatRange Speed => PickedMoteParam.speed;
        public FloatRange Rotation => PickedMoteParam.rotation;
        public ThingDef MoteDef => PickedMoteParam.moteDef;

        public List<MoteThrown> MoteThrownList = new List<MoteThrown>();

        public bool HasGameProjectile => gameProjectile != null;
        public bool HasProjectileOption => projectileOption != null;

        public bool HasPickedMoteOption => HasProjectileOption && projectileOption.IsMoteType;
        public bool HasPickedShadowMoteOption => HasProjectileOption && projectileOption.IsShadowMoteType;

        public bool HasAtLeastOneOption => HasPickedShadowMoteOption || HasPickedMoteOption;

        public bool HasPickedOption => HasProjectileOption && HasAtLeastOneOption;

        public IntVec3 PetanqueSpotCell => TargetA.Cell;

        int AttemptNum = 0;

        bool SetParameters()
        {
            MyName = pawn.LabelShort + " " + MyName;

            Tools.Warn(MyName + " Entering SetParameters ", myDebug);

            GameProjectileDef myGPD = DefDatabase<GameProjectileDef>.AllDefs.FirstOrFallback();
            if (myGPD == null)
            {
                Tools.Warn(MyName + " SetParameters " + " - 00 no GameProjectileDef found", myDebug);
                return false;
            }

            foreach (GameProjectile curGP in myGPD.gameProjectileList)
                if ((curGP.driverClass == this.GetType()) && (pawn.CurJob != null && pawn.CurJobDef == curGP.jobDef))
                {
                    gameProjectile = curGP;
                    break;
                }

            if (!HasGameProjectile)
            {
                Tools.Warn(MyName + " SetParameters " + " - 01 no GameProjectile item found", myDebug);
                return false;
            }

            bool Didit = this.RetrieveProjectileParam();

            Tools.Warn(
                MyName + " SetParameters " + 
                " - 02 RetrieveProjectileParam:" + Didit + 
                " - HasPickedMoteOption: "+ HasPickedMoteOption +
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

        bool ParameterInitialization()
        {
            if (HasGameProjectile)
                return true;

            if (AttemptNum > 20)
                return false;

            Tools.Warn(MyName + " WatchTickAction - Trying to ParameterInitialization - SetParameters", myDebug);
            AttemptNum++;

            bool DidIt = SetParameters();
            Tools.Warn(MyName + " WatchTickAction - DidIt:" + DidIt + " ; attempts:" + AttemptNum, myDebug);

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
                MoteThrownList.Add((MoteThrown)this.MoteSpawner_ThrowObjectAt());
            }
            else if (HasPickedShadowMoteOption)
            {
                MoteThrownList.Add((MoteThrown)this.ShadowMoteSpawner_ThrowObjectAt());
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