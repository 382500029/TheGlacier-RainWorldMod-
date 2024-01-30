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
    public class DropBugColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();
        Color ShineColor;
        Color skinColor;
        //落网虫颜色处理
        public void SetDropBugColor(DropBugGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                DropBugColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            sLeaser.sprites[self.ShineMeshSprite].color = ColorChange(ShineColor, freezeColor);
            for (int l = 0; l < 2; l++)
                sLeaser.sprites[self.AntennaSprite(l)].color = ColorChange(skinColor, freezeColor);
        }

        public void DropBugColorSave(DropBugGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            ShineColor.r = 34.0f / 255.0f;
            ShineColor.g = 28.0f / 255.0f;
            ShineColor.b = 53.0f / 255.0f;
            ShineColor.a = 1.0f;
            skinColor = self.currSkinColor;
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //落网虫颜色处理
        static public void DropBugGraphics_DrawSprites(On.DropBugGraphics.orig_DrawSprites orig, DropBugGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.bug, out var colorRecord))
            {
                var cr = (DropBugColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetDropBugColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.bug);
                }
            }
        }

        static public void DropBug_Update(On.DropBug.orig_Update orig, DropBug self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
   
