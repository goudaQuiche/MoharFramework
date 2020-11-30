using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace MoharJoy
{
    public static class ShadowMoteUtils
    {
        public static void Initialization(this ShadowMote SM, JobDriver_PlayGenericTargetingGame TG, Vector3 DestinationCell)
        {
            SM.TG_parent = TG;

            SM.InitCoordinates(DestinationCell);
            SM.InitGroundShadowGraphic();
        }

        public static void Initialization(this ShadowMote SM, JobDriver_ThrowRocks JDTR, Vector3 DestinationCell)
        {
            SM.JDTR_parent = JDTR;

            SM.InitCoordinates(DestinationCell);
            //SM.InitGroundShadowGraphic();
        }

        public static void InitCoordinates(this ShadowMote SM, Vector3 DestinationCell)
        {
            if(SM.HasTargetingGameParent)
                SM.flatOrigin = SM.throwerOrigin = SM.TG_parent.pawn.DrawPos;
            else if (SM.HasThrowRockParent)
                SM.flatOrigin = SM.throwerOrigin = SM.JDTR_parent.pawn.DrawPos;

            SM.destination = DestinationCell;
            SM.flatOrigin.y = DestinationCell.y = 0;

            SM.BaseDistance = Vector3.Distance(SM.flatOrigin, DestinationCell);

            if (SM.HasTargetingGameParent)
                SM.targetBuildingCoordinates = SM.TG_parent.PetanqueSpotCell.ToVector3Shifted();
            else if (SM.HasThrowRockParent)
                SM.targetBuildingCoordinates = SM.JDTR_parent.TargetCell.ToVector3Shifted();
        }

        public static void InitGroundShadowGraphic(this ShadowMote SM)
        {
            bool MyDebug = SM.MoteSubEffectDebug;
            string debugStr = MyDebug ? "InitGroundShadowGraphic - " : "";

            Tools.Warn("Entering", MyDebug);

            if (SM.HasGroundShadowGraphic)
            {
                Tools.Warn(" already found ground shadow graphic, exiting KO", MyDebug);
                return;
            }
                
            if (!SM.HasGroundShadowData)
            {
                Tools.Warn(" found no ground shadow data, exiting KO", MyDebug);
                return;
            }

            Tools.Warn(" creating new ground shadow graphic ok", MyDebug);
            SM.GroundShadowGraphic = new Graphic_Shadow(SM.GroundShadowData);
        }

    }
}
