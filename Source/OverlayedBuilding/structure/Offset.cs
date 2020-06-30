using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{

    public class Offset
    {
        public static Vector2 North = new Vector2(0, 0);
        public static Vector2 East = new Vector2(0, 0);
        public static Vector2 South = new Vector2(0, 0);
        public static Vector2 West = new Vector2(0, 0);

        public static Vector2 GetOffset(Rot4 rot)
        {
            switch (rot.AsInt)
            {
                case 0:
                    return North;
                case 1:
                    return East;
                case 2:
                    return South;
                case 3:
                    return West;
                default:
                    return North;
            }
        }
    }
}