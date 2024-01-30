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
    public class DaddyLongLegsColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();
        Color color;
        //蘑菇颜色处理
        public void SetDaddyLongLegsColor(DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                DaddyLongLegsColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            foreach (var p in self.danglers)
                sLeaser.sprites[p.firstSprite].color = ColorChange(color, freezeColor_head);
            foreach (var p in self.deadLegs)
                sLeaser.sprites[p.firstSprite].color = ColorChange(color, freezeColor_head);
            foreach (var p in self.legGraphics)
                sLeaser.sprites[p.firstSprite].color = ColorChange(color, freezeColor_head);
        }

        public void DaddyLongLegsColorSave(DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            color = self.blackColor;
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //蘑菇颜色处理
        static public void DaddyGraphics_DrawSprites(On.DaddyGraphics.orig_DrawSprites orig, DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.daddy, out var colorRecord))
            {
                var cr = (DaddyLongLegsColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetDaddyLongLegsColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.daddy);
                }
            }
        }

        static public void DaddyLongLegs_Update(On.DaddyLongLegs.orig_Update orig, DaddyLongLegs self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
