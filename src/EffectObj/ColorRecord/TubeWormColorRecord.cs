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
    public class TubeWormColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //管虫颜色处理
        public void SetTubeWormColor(TubeWormGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                TubeWormColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (i <= 1)
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor_head);
                else
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
        }

        public void TubeWormColorSave(TubeWormGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //管虫颜色处理
        static public void TubeWormGraphics_DrawSprites(On.TubeWormGraphics.orig_DrawSprites orig, TubeWormGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.worm, out var colorRecord))
            {
                var cr = (TubeWormColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetTubeWormColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.worm);
                }
            }
        }

        static public void TubeWorm_Update(On.TubeWorm.orig_Update orig, TubeWorm self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    