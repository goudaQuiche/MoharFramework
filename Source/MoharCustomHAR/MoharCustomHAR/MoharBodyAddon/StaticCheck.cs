﻿using Verse;
using CheckModActive;

namespace MoharCustomHAR
{
    [StaticConstructorOnStartup]
    public static class StaticCheck
    {
        public static readonly bool IsOk;
        static StaticCheck()
        {
            string myNS = typeof(StaticCheck).Namespace;
            IsOk = ModCompatibilityCheck.MoharCheckAndDisplay(myNS);
        }
    }
}
