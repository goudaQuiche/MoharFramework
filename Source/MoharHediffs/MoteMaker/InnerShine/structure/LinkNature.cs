using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using AlienRace;
using UnityEngine;


namespace MoharHediffs
{
    public static class MoteLink
    {
        public enum Nature
        {
            head,
            body
        }

        public static Vector3 GetLinkOffset(this Pawn p, Nature linkType)
        {

            switch (linkType)
            {
                case Nature.head:
                    //return p.Drawer.renderer.BaseHeadOffsetAt((p.GetPosture() == PawnPosture.Standing) ? Rot4.North : p.Drawer.renderer.LayingFacing()).RotatedBy(p.Drawer.renderer.BodyAngle());
                    return p.Drawer.renderer.BaseHeadOffsetAt((p.GetPosture() == PawnPosture.Standing) ? Rot4.North : p.Drawer.renderer.LayingFacing()).RotatedBy(p.Drawer.renderer.BodyAngle(PawnRenderFlags.Cache));
                case Nature.body:
                default:
                    return Vector3.zero;
            }
        }
    }

}
