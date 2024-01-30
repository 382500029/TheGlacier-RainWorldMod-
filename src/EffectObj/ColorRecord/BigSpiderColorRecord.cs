using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGlacier2
{
    public class BigSpiderColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //狼蛛颜色处理
        public void SetBigSpiderColor(BigSpiderGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                BigSpiderColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
        }

        public void BigSpiderColorSave(BigSpiderGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //狼蛛颜色处理
        static public void BigSpiderGraphics_DrawSprites(On.BigSpiderGraphics.orig_DrawSprites orig, BigSpiderGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.bug, out var colorRecord))
            {
                var cr = (BigSpiderColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetBigSpiderColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.bug);
                }
            }
        }

        static public void BigSpider_Update(On.BigSpider.orig_Update orig, BigSpider self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
