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
    public class LizardColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();
        Color bodyColor;
        //蜥蜴颜色处理
        public void SetLizardColor(LizardGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                LizardColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (i >= self.SpriteHeadStart && i < self.SpriteHeadEnd)
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor_head);
                else
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
            sLeaser.sprites[self.SpriteTail].color = ColorChange(bodyColor, freezeColor);
        }

        public void LizardColorSave(LizardGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
            bodyColor = sLeaser.sprites[self.SpriteBodyMesh].color;
        }

        //蜥蜴颜色处理
        static public void LizardGraphics_DrawSprites(On.LizardGraphics.orig_DrawSprites orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.lizard, out var colorRecord))
            {
                var cr = (LizardColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetLizardColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.lizard);
                }
            }
        }

        static public void Lizard_Update(On.Lizard.orig_Update orig, Lizard self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }

        
    }
}
    
