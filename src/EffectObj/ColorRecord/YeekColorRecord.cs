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
    public class YeekColorRecord : ColorRecord
    {
        Color bodyColor;
        //跳跳蛙颜色处理
        public void SetYeekColor(YeekGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                YeekColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
                sLeaser.sprites[i].color = ColorChange(bodyColor, freezeColor);
        }

        public void YeekColorSave(YeekGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            bodyColor.r = 86.0f / 255.0f;
            bodyColor.g = 77.0f / 255.0f;
            bodyColor.b = 82.0f / 255.0f;
            bodyColor.a = 1.0f;
        }

        //跳跳蛙颜色处理
        static public void YeekGraphics_DrawSprites(On.MoreSlugcats.YeekGraphics.orig_DrawSprites orig, MoreSlugcats.YeekGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.myYeek, out var colorRecord))
            {
                var cr = (YeekColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetYeekColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.myYeek);
                }
            }
        }

        static public void Yeek_Update(On.MoreSlugcats.Yeek.orig_Update orig, MoreSlugcats.Yeek self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
