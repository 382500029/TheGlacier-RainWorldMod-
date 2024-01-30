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
    public class VultureColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //秃鹫颜色处理
        public void SetVultureColor(VultureGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                VultureColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (i == self.MaskSprite ||
                    i == self.HeadSprite)
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor_head);
                else
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
        }

        public void VultureColorSave(VultureGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //秃鹫颜色处理
        static public void VultureGraphics_DrawSprites(On.VultureGraphics.orig_DrawSprites orig, VultureGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.vulture, out var colorRecord))
            {
                var cr = (VultureColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetVultureColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.vulture);
                }
            }
        }

        static public void Vulture_Update(On.Vulture.orig_Update orig, Vulture self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    