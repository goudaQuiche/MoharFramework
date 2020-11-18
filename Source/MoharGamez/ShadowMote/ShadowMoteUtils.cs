using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace MoharGamez
{
    public static class ShadowMoteUtils
    {
        public static void Initialization(this ShadowMote SM, JobDriver_PlayGenericTargetingGame TG, Vector3 DestinationCell)
        {
            SM.TG_parent = TG;

            SM.InitCoordinates(DestinationCell);
            SM.InitGroundShadowGraphic();
        }
        public static void InitCoordinates(this ShadowMote SM, Vector3 DestinationCell)
        {
            SM.flatOrigin = SM.origin = SM.TG_parent.pawn.DrawPos;
            SM.destination = DestinationCell;
            SM.flatOrigin.y = DestinationCell.y = 0;

            SM.BaseDistance = Vector3.Distance(SM.flatOrigin, DestinationCell);

            SM.targetBuildingCoordinates = SM.TG_parent.PetanqueSpotCell.ToVector3Shifted();
        }

        public static void InitGroundShadowGraphic(this ShadowMote SM)
        {
            if (SM.HasGroundShadowGraphic)
                return;
            if (!SM.HasGroundShadowData)
                return;

            SM.GroundShadowGraphic = new Graphic_Shadow(SM.GroundShadowData);
        }
        /*
        private void InitCoordinates(Vector3 nOrigin, Vector3 nDestination, Vector3 nTarget, Pawn p)
        {
            origin = nOrigin;
            destination = nDestination;
            flatOrigin = origin;

            flatOrigin.y = nDestination.y = 0;
            BaseDistance = Vector3.Distance(flatOrigin, nDestination);

            targetBuildingCoordinates = nTarget;
            pawn = p;
        }
        */

    }
}
