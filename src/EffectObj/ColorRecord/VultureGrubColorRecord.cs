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
    public class VultureGrubColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //射线虫颜色处理
        public void SetVultureGrubColor(VultureGrubGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                VultureGrubColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (i == self.HeadSprite ||
                    i == 2 ||
                    i == 3 ||
                    i == self.EyeSprite)
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor_head);
                else
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }

        }

        public void VultureGrubColorSave(VultureGrubGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //射线虫颜色处理
        static public void VultureGrubGraphics_DrawSprites(On.VultureGrubGraphics.orig_DrawSprites orig, VultureGrubGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.worm, out var colorRecord))
            {
                var cr = (VultureGrubColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetVultureGrubColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.worm);
                }
            }
        }

        static public void VultureGrub_Update(On.VultureGrub.orig_Update orig, VultureGrub self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    