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
    public class SlugcatColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //蛞蝓猫颜色处理
        public void SetSlugcatColor(global::PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                SlugcatColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
        }

        public void SlugcatColorSave(global::PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //蛞蝓猫颜色处理
        static public void SlugcatGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, global::PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.player, out var colorRecord))
            {
                var cr = (SlugcatColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetSlugcatColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.player);
                }
            }
        }

        static public void Slugcat_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
