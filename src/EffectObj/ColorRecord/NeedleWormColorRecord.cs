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
    public class NeedleWormColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();
        Color bodyColor;
        //面条蝇颜色处理
        public void SetNeedleWormColor(NeedleWormGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                NeedleWormColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = ColorChange(bodyColor, freezeColor);
            }
        }

        public void NeedleWormColorSave(NeedleWormGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
            bodyColor = self.bodyColor;
        }

        //面条蝇颜色处理
        static public void NeedleWormGraphics_DrawSprites(On.NeedleWormGraphics.orig_DrawSprites orig, NeedleWormGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.worm, out var colorRecord))
            {
                var cr = (NeedleWormColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetNeedleWormColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.worm);
                }
            }
        }

        static public void NeedleWorm_Update(On.NeedleWorm.orig_Update orig, NeedleWorm self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
