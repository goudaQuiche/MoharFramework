using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoharGfx
{
    public class Graphic_CutoutCollection : Graphic
    {
        protected Graphic[] subGraphics;
        //protected Material mat;

        public override void TryInsertIntoAtlas(TextureAtlasGroup groupKey)
        {
            Graphic[] array = subGraphics;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].TryInsertIntoAtlas(groupKey);
            }
        }

        // Token: 0x06001C1A RID: 7194 RVA: 0x000ABC14 File Offset: 0x000A9E14
        public override void Init(GraphicRequest req)
        {
            this.data = req.graphicData;
            if (req.path.NullOrEmpty())
            {
                throw new ArgumentNullException("folderPath");
            }
            if (req.shader == null)
            {
                throw new ArgumentNullException("shader");
            }

            this.path = req.path;
            this.maskPath = req.maskPath;
            this.color = req.color;
            this.colorTwo = req.colorTwo;
            this.drawSize = req.drawSize;
            List<ValueTuple<Texture2D, string>> list = 
                (from x in ContentFinder<Texture2D>.GetAllInFolder(req.path)
                where !x.name.EndsWith(Graphic_Single.MaskSuffix)
                orderby x.name
                select new ValueTuple<Texture2D, string>(x, x.name.Split(new char[]{'_'})[0])
                ).ToList<ValueTuple<Texture2D, string>>();
            if (list.NullOrEmpty<ValueTuple<Texture2D, string>>())
            {
                Log.Error("Graphic_CutoutCollection - Collection cannot init: No textures found at path " + req.path);
                this.subGraphics = new Graphic[]
                {
                    BaseContent.BadGraphic
                };
                return;
            }
            List<Graphic> list2 = new List<Graphic>();
            foreach (IGrouping<string, ValueTuple<Texture2D, string>> grouping in from s in list
                                                                                  group s by s.Item2)
            {
                List<ValueTuple<Texture2D, string>> list3 = grouping.ToList<ValueTuple<Texture2D, string>>();
                string singlePath = req.path + "/" + grouping.Key;
                string singleMaskPath = singlePath + Graphic_Single.MaskSuffix;
                //Log.Warning("singlePath:" + singlePath + "; singleMaskPath:" + singleMaskPath);

                MaterialRequest req2 = default(MaterialRequest);
                req2.mainTex = req.texture ?? ContentFinder<Texture2D>.Get(singlePath, true);

                //Log.Warning("req2.mainTex:" + (req2.mainTex==null).ToString() + " => singlePath"+ singlePath);
                req2.shader = req.shader;
                req2.color = color;
                req2.colorTwo = colorTwo;
                req2.renderQueue = req.renderQueue;
                req2.shaderParameters = req.shaderParameters;
                //Log.Warning("req.shader.name => " + req.shader.name + " - req.shader.SupportsMaskTex()" + req.shader.SupportsMaskTex());

                if (req.shader.SupportsMaskTex())
                {
                    //req2.maskTex = ContentFinder<Texture2D>.Get(this.maskPath.NullOrEmpty() ? (this.path + Graphic_Single.MaskSuffix) : this.maskPath, false);
                    req2.maskTex = ContentFinder<Texture2D>.Get(singleMaskPath, true);
                    //Log.Warning("SupportMaskTex");
                }

                //this.mat = MaterialPool.MatFrom(req2);

                if (list3.Count > 0)
                {
                    foreach (ValueTuple<Texture2D, string> valueTuple in list3)
                    {
                        //Log.Warning("; req.path + / + valueTuple.Item1.name" + req.path + "/" + valueTuple.Item1.name + ";");
                        //list2.Add(GraphicDatabase.Get(typeof(Graphic_Single), req.path + "/" + valueTuple.Item1.name, req.shader, this.drawSize, this.color, this.colorTwo, this.data, req.shaderParameters, null));
                        list2.Add(GraphicDatabase.Get(typeof(Graphic_Single), singlePath, req.shader, drawSize, color, colorTwo, data, req.shaderParameters, singleMaskPath));
                        //Log.Warning($" p:{singlePath}, s:{req.shader}, ds:{this.drawSize}, c:{this.color}, c2:{this.colorTwo}, mp:{singleMaskPath}");
                    }
                }
            }

            subGraphics = list2.ToArray();
        }
    }
}