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
    public class DeerColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //雨鹿颜色处理
        public void SetDeerColor(DeerGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                DeerColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (i >= self.FirstAntlerSprite && i < self.LastAntlerSprite)
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor_head);
                else
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 4; j++)
                    sLeaser.sprites[self.LegSprite(j, i)].color = ColorChange(colorList[self.BodySprite(0)], freezeColor);
            sLeaser.sprites[self.BodySprite(0)].color = ColorChange(colorList[self.BodySprite(0)], freezeColor_head);

        }

        public void DeerColorSave(DeerGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //雨鹿颜色处理
        static public void DeerGraphics_DrawSprites(On.DeerGraphics.orig_DrawSprites orig, DeerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.deer, out var colorRecord))
            {
                var cr = (DeerColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetDeerColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.deer);
                }
            }
        }

        static public void Deer_Update(On.Deer.orig_Update orig, Deer self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
