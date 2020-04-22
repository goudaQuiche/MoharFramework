/*
 * Created by SharpDevelop.
 * User: Etienne
 * Date: 22/11/2017
 * Time: 16:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Verse;               // RimWorld universal objects are here (like 'Building')
using Verse.Sound;

using UnityEngine;

namespace MoharHediffs
{
    [StaticConstructorOnStartup]
    public class MyGfx
    {
        public static Color Purple =    new Color(1, 0, 1);
        public static Color Blue =      new Color(0, 0, 1);
        public static Color Cyan =      new Color(0, 1, 1);
        public static Color Green =     new Color(0, 1, 0);
        public static Color Yellow =    new Color(1, 1, 0);
        public static Color Orange =    new Color(1, .6f, 0);
        public static Color Red =       new Color(1, 0, 1);
    }
}
