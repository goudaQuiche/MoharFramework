using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace MoharGamez
{
    public static class ThoughtUtils
    {
        public static void CalculateThrowDistance(this Pawn pawn, Vector3 targetBuildingCoordinates, Vector3 destinationCell, bool myDebug = false)
        {
            Vector3 buildingCenter = targetBuildingCoordinates;
            destinationCell.y = 0;
            float distance = Vector3.Distance(destinationCell, buildingCenter);

            Tools.Warn(
                pawn.LabelShort + "CalculateThrowDistance" +
                " buildingCenter: " + buildingCenter +
                " destinationCell: " + destinationCell +
                " distance: " + distance
                , myDebug
            );
        }
        
    }
}
