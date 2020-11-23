using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace OLB
{

    public class Offset
    {
        public static Vector2 north = new Vector2(0, 0);
        public static Vector2 east = new Vector2(0, 0);
        public static Vector2 south = new Vector2(0, 0);
        public static Vector2 west = new Vector2(0, 0);

        public static Vector2 GetOffset(Rot4 rot)
        {
            switch (rot.AsInt)
            {
                case 0:
                    return north;
                case 1:
                    return east;
                case 2:
                    return south;
                case 3:
                    return west;
                default:
                    return north;
            }
        }
    }
}