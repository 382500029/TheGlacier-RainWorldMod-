using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TheGlacier2;
using MoreSlugcats;

namespace TheGlacier2
{
    public class InspectorColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //监察者颜色处理
        public void SetInspectorColor(InspectorGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                InspectorColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if ((i >= self.SpritesBegin_wings && i < self.SpritesBegin_wings + self.SpritesTotal_wings) ||
                    (i >= self.SpritesBegin_mycelium && i < self.SpritesBegin_mycelium + self.SpritesTotal_mycelium))
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
                else
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor_head);
            }

        }

        public void InspectorColorSave(InspectorGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //监察者颜色处理
        static public void InspectorGraphics_DrawSprites(On.MoreSlugcats.InspectorGraphics.orig_DrawSprites orig, MoreSlugcats.InspectorGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.myInspector, out var colorRecord))
            {
                var cr = (InspectorColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetInspectorColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.myInspector);
                }
            }
        }

        static public void Inspector_Update(On.MoreSlugcats.Inspector.orig_Update orig, MoreSlugcats.Inspector self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
