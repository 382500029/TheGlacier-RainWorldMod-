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
    public class TentaclePlantColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //红树颜色处理
        public void SetTentaclePlantColor(TentaclePlantGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                TentaclePlantColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
        }

        public void TentaclePlantColorSave(TentaclePlantGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //红树颜色处理
        static public void TentaclePlantGraphics_DrawSprites(On.TentaclePlantGraphics.orig_DrawSprites orig, TentaclePlantGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.plant, out var colorRecord))
            {
                var cr = (TentaclePlantColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetTentaclePlantColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.plant);
                }
            }
        }

        static public void TentaclePlant_Update(On.TentaclePlant.orig_Update orig, TentaclePlant self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
