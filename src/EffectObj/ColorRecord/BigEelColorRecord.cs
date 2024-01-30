using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGlacier2
{
    public class BigEelColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //利维坦颜色处理
        public void SetBigEelColor(BigEelGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                BigEelColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
        }

        public void BigEelColorSave(BigEelGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //利维坦颜色处理
        static public void BigEelGraphics_DrawSprites(On.BigEelGraphics.orig_DrawSprites orig, BigEelGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.eel, out var colorRecord))
            {
                var cr = (BigEelColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetBigEelColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.eel);
                }
            }
        }

        static public void BigEel_Update(On.BigEel.orig_Update orig, BigEel self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
