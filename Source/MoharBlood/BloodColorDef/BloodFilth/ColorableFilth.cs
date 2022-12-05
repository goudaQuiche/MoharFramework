using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AlienRace;
using System.Reflection;

namespace MoharBlood
{
    public class ColorableFilth : Filth
    {
        private Color pawnColor = Color.cyan;

        public Vector3 offsetDrawPos;
        public Vector2 randomDrawSize;
        public float randomRotation;

        public override Color DrawColor
        {
            get => pawnColor;
            set => pawnColor = value;
        }

        public void Init(Color newColor) {
            DrawColor = newColor;
            SetOffsetDrawPos();
            SetRandomSize();
            SetRandomRot();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Color>(ref pawnColor, "pawnColor", Color.white);
            Scribe_Values.Look<Vector3>(ref offsetDrawPos, "offsetDrawPos");
            Scribe_Values.Look<Vector2>(ref randomDrawSize, "randomDrawSize");
            Scribe_Values.Look<float>(ref randomRotation, "randomRotation");
        }

        public override Vector3 DrawPos => offsetDrawPos;
        public override Vector2 DrawSize => base.DrawSize;

        public void SetRandomRot()
        {
            randomRotation = Rand.Range(0, 360);
        }
        public void SetRandomSize()
        {
            float xSize = Rand.Range(.85f, 1.15f);
            float ySize = Rand.Range(.85f, 1.15f);

            randomDrawSize = new Vector2(base.DrawSize.x * xSize, base.DrawSize.y * ySize);
            //Log.Warning($" {base.DrawSize} => {randomDrawSize}");
        }
        public void SetOffsetDrawPos()
        {
            int precision = 12;
            float seed = (thingIDNumber * 11587) % precision;

            float circleThing = (seed / precision) * 2 * 3.14f;
            float x = Mathf.Cos(circleThing);
            float y = Mathf.Sin(circleThing);

            float xStr = Rand.Range(.125f, .55f);
            float yStr = Rand.Range(.125f, .55f);
            
            Vector3 posOffset = new Vector3(x * xStr, 0, y * yStr);

            offsetDrawPos = base.DrawPos + posOffset;
            //Log.Warning($" {base.DrawPos} + ({x}, 0, {y})*({xStr};{yStr}); posOffset={posOffset} ===> {offsetDrawPos} ");
        }

        
        public override void Print(SectionLayer layer)
        {
            base.Print(layer);
            /*
            Log.Warning("ColorableFilth print : " + typeof(Graphic));

            Vector2 size;
            bool flag;
            if (Graphic.ShouldDrawRotated)
            {
                Log.Warning("ColorableFilth ShouldDrawRotated");
                size = Graphic.drawSize;
                flag = false;
            }
            else
            {
                Log.Warning("ColorableFilth ShouldDrawRotated not");
                size = (Rotation.IsHorizontal ? Graphic.drawSize.Rotated() : Graphic.drawSize);
                flag = (Rotation == Rot4.West && Graphic.WestFlipped) || (Rotation == Rot4.East && Graphic.EastFlipped);
            }
            float num = AngleFromRot( this.Graphic, Rotation);
            if (flag && Graphic.data != null)
            {
                num += Graphic.data.flipExtraRotation;
            }

            Vector3 center = this.TrueCenter() + Graphic.DrawOffset(Rotation);
            //Vector3 center = offsetDrawPos + Graphic.DrawOffset(Rotation);

            Printer_Plane.PrintPlane(layer, center, size, Graphic.MatAt(Rotation, this), num, flag);
            */
        }



    }


}
