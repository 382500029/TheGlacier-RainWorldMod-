using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TheGlacier2;
using MoreSlugcats;

namespace TheGlacier2
{
    public class StowawayBugColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();

        //偷渡虫颜色处理
        public void SetStowawayBugColor(StowawayBugGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                StowawayBugColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
        }

        public void StowawayBugColorSave(StowawayBugGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //偷渡虫颜色处理
        static public void StowawayBugGraphics_DrawSprites(On.MoreSlugcats.StowawayBugGraphics.orig_DrawSprites orig, MoreSlugcats.StowawayBugGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.myBug, out var colorRecord))
            {
                var cr = (StowawayBugColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetStowawayBugColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.myBug);
                }
            }
        }

        static public void StowawayBug_Update(On.MoreSlugcats.StowawayBug.orig_Update orig, MoreSlugcats.StowawayBug self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
