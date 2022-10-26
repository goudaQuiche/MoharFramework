using Verse;
using RimWorld;
using AlienRace;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MoharCustomHAR
{
    
    using BAP = AlienPartGenerator.ExtendedGraphicsPrioritization;
    using BAHG = AlienPartGenerator.ExtendedHediffGraphic;
    using BAHSG = AlienPartGenerator.ExtendedHediffSeverityGraphic;
    using BABSG = AlienPartGenerator.ExtendedBackstoryGraphic;

    using AlienComp = AlienPartGenerator.AlienComp;
    using ExposableValueTuple = AlienPartGenerator.ExposableValueTuple<Color, Color>;
    

    public class MoharBodyAddon : AlienPartGenerator.BodyAddon
    {
        public bool drawIfDirectMissingChild = true;
        public bool drawIfDead = true;

        public bool drawIfDrafted = true;
        public bool drawIfUndrafted = true;

        public bool useBodyDependantBodyAddonGraphics = true;
        public JobParameters jobParams;

        public bool HasJobParams => jobParams != null && (jobParams.HasDrawJobs || jobParams.HasHideJobs);

        public bool WasDisplayedLastFrame = false;

        public override bool CanDrawAddon(Pawn pawn)
        {
            bool result =
                base.CanDrawAddon(pawn) &&

                //this.ApparelCondition(pawn) && 
                //this.PostureCondition(pawn) &&
                this.BedCondition(pawn) && 
                //this.BackstoryCondition(pawn) &&
                //this.BodyPartCondition(pawn) &&
                
                //this.GenderCondition(pawn) &&
                //this.BodyTypeCondition(pawn) &&

                this.BodyPartChildrenCondition(pawn) &&
                this.DeadCondition(pawn) &&
                this.JobCondition(pawn) &&
                this.DraftedCondition(pawn)
               ;

            //does not work
            /*
            if(WasDisplayedLastFrame != result)
                PortraitsCache.SetDirty(pawn);

            WasDisplayedLastFrame = result;
            */

            return result;
        }

        /*
        public override Graphic GetPath(Pawn pawn, ref int sharedIndex, int? savedIndex = new int?())
        {
            string returnPath = string.Empty;

            //if (!StaticCheck.IsOk) return null;

            if (!this.HandlePrioritization(pawn, out returnPath, out int variantCounting))
            {
                returnPath = path;
                variantCounting = variantCount;
            }

            if (variantCounting <= 0)
                variantCounting = 1;

            ExposableValueTuple channel = pawn.GetComp<AlienComp>().GetChannel(ColorChannel);

            //Log.Message($"{pawn.Name.ToStringFull}\n{channel.first.ToString()} | {pawn.story.hairColor}");

            if (returnPath.NullOrEmpty())
                return null;

            returnPath += linkVariantIndexWithPrevious.VariantIndexToString(savedIndex, sharedIndex, variantCounting, out sharedIndex, out int variantIndex);
            Shader shader = returnPath.DetermineShaderType(ShaderType.Shader);

            if (useBodyDependantBodyAddonGraphics)
            {
                string bodyname = pawn.story.bodyType.defName;
                string newPath = returnPath.InsertBodyTypeDirectoryBeforeFileName(bodyname);
                if (newPath.IsThereContent())
                {
                    return GraphicDatabase.Get<Graphic_Multi_RotationFromData>(
                    newPath,
                    shader,
                    drawSize * 1.5f,
                    channel.first,
                    channel.second,
                    new GraphicData
                    {
                        drawRotated = !drawRotated
                    });
                }
            }

            return
                GraphicDatabase.Get<Graphic_Multi_RotationFromData>(
                    returnPath,
                    shader, 
                    drawSize * 1.5f,
                    channel.first, 
                    channel.second, 
                    new GraphicData
                    {
                        drawRotated = !drawRotated
                    });
        }
        */
    }

    
}
