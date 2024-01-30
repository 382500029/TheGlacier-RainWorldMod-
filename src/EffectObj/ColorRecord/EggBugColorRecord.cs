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
    public class EggBugColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();
        Color antennaColor;
        //蛋虫颜色处理
        public void SetEggBugColor(EggBugGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                EggBugColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
            for (int j = 0; j < 2; j++)
                sLeaser.sprites[self.AntennaSprite(j)].color = ColorChange(antennaColor, freezeColor);
        }

        public void EggBugColorSave(EggBugGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
            antennaColor = self.antennaTipColor;
        }

        //蛋虫颜色处理
        static public void EggBugGraphics_DrawSprites(On.EggBugGraphics.orig_DrawSprites orig, EggBugGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.bug, out var colorRecord))
            {
                var cr = (EggBugColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetEggBugColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.bug);
                }
            }
        }

        static public void EggBug_Update(On.EggBug.orig_Update orig, EggBug self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
