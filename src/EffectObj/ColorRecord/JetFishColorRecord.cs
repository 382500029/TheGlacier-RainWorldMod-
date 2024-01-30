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
    public class JetFishColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //蛙鱼颜色处理
        public void SetJetFishColor(JetFishGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                JetFishColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
            for (int k = 0; k < 2; k++)
                for (int l = 0; l < 2; l++)
                    sLeaser.sprites[self.EyeSprite(k, l)].color = ColorChange(colorList[self.EyeSprite(k, l)], freezeColor_head);
            for (int k = 0; k < 2; k++)
                sLeaser.sprites[self.TentacleSprite(k)].color = ColorChange(colorList[self.BodySprite], freezeColor);

        }

        public void JetFishColorSave(JetFishGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //蛙鱼颜色处理
        static public void JetFishGraphics_DrawSprites(On.JetFishGraphics.orig_DrawSprites orig, JetFishGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.fish, out var colorRecord))
            {
                var cr = (JetFishColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetJetFishColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.fish);
                }
            }
        }

        static public void JetFish_Update(On.JetFish.orig_Update orig, JetFish self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
