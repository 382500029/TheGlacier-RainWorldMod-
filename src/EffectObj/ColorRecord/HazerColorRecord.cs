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
    public class HazerColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //墨鱼颜色处理
        public void SetHazerColor(HazerGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                HazerColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
        }

        public void HazerColorSave(HazerGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //墨鱼颜色处理
        static public void HazerGraphics_DrawSprites(On.HazerGraphics.orig_DrawSprites orig, HazerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.bug, out var colorRecord))
            {
                var cr = (HazerColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetHazerColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.bug);
                }
            }
        }

        static public void Hazer_Update(On.Hazer.orig_Update orig, Hazer self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
