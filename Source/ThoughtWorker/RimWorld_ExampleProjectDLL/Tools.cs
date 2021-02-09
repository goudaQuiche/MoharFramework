using Verse;
using UnityEngine;

namespace MoharThoughts
{
    public static class Tools
    {
        public static bool Includes(this IntRange intRange, int value)
        {
            return value >= intRange.min && value <= intRange.max;
        }
    }
}
