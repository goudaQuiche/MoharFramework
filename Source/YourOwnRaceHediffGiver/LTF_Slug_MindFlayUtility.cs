using RimWorld;
using Verse;

namespace LTF_Slug
{
    [StaticConstructorOnStartup]
    public static class MindFlayerUtility
    {
        private const float defaultFireSize = 0.5f;
        private const bool myDebug = true;
        //private static Graphic graphicInt;

        public static IntVec3 Behind(Pawn pawn)
        {
            //IntVec3 offset = new IntVec3(0,0,0);
            IntVec3 offset;// = null;
            int x, y, z;
            x = 0;y = 0; z = 0;

            if (pawn.Rotation == Rot4.North)
            {
                x = 1;
                
            }else if (pawn.Rotation == Rot4.South)
            {
                x = -1;
            }
            else if (pawn.Rotation == Rot4.West)
            {
                z = 1;
            }
            else if (pawn.Rotation == Rot4.East)
            {
                z = -1;
            }
            else
            {
                Tools.Warn("wtf rot", myDebug);
                
            }
            offset = new IntVec3(x,y,z);

            return (pawn.Position + offset);

            //return null;
        }

        public static void CreateMindFlaySpot(Thing hitThing)
        {
            Tools.Warn("Create MindFlaySpot", myDebug);
            IntVec3 destinationCell = hitThing.Position;

            //actor
            Pawn Victim = destinationCell.GetFirstPawn(hitThing.Map);

            // block
            Building mindFlaySpot = (Building)ThingMaker.MakeThing(ThingDef.Named("LTF_MindFlaySpot"), null);

            //lockBlock.SetColor(lockDowner.DrawColor);
            //lockDowner.Rotation

            //GenSpawn.Spawn( lockBlock, Behind(lockDowner), Find.VisibleMap, Rot4.North, false);
            //GenSpawn.Spawn(mindFlaySpot, Behind(lockDowner), lockDowner.Map, Rot4.North, WipeMode.Vanish);
            GenSpawn.Spawn(mindFlaySpot, destinationCell, hitThing.Map, Rot4.North, WipeMode.Vanish);

            //lockDowner.Position.
            /*
            Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire, null);
            fire.fireSize = defaultFireSize;
            if (hitThing != null)
            {
                if (hitThing is Pawn) hitThing.TryAttachFire(defaultFireSize);
                else GenSpawn.Spawn(fire, hitThing.Position, Find.VisibleMap, Rot4.North, false);
            }
            else
            {
                GenSpawn.Spawn(fire, destinationCell, Find.VisibleMap, Rot4.North, false);
            }
            */
        }

        /*
        public static Graphic XenarrowGraphic
        {
            get
            {
                if (graphicInt == null)
                {
                    GraphicData graphicData = new GraphicData()
                    {
                        texPath = "Projectile/Xenarrow",
                        graphicClass = typeof(Graphic_Single),
                        shaderType = ShaderType.TransparentPostLight
                    };
                    graphicInt = graphicData.Graphic;
                }
                return graphicInt;
            }
        }
        */
    }

}