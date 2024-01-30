using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TheGlacier2;

namespace TheGlacier2
{
    public class MirosBirdColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //钢鸟颜色处理
        public void SetMirosBirdColor(MirosBirdGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                MirosBirdColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {

                if ((i >= self.FirstBeakSprite && i < self.LastBeakSprite) ||
                    (i == self.HeadSprite))
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor_head);
                else
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
        }

        public void MirosBirdColorSave(MirosBirdGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //钢鸟颜色处理
        static public void MirosBirdGraphics_DrawSprites(On.MirosBirdGraphics.orig_DrawSprites orig, MirosBirdGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.bird, out var colorRecord))
            {
                var cr = (MirosBirdColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetMirosBirdColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.bird);
                }
            }
        }

        static public void MirosBird_Update(On.MirosBird.orig_Update orig, MirosBird self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
