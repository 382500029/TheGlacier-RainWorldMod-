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
    public class ScavengerColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();
        Color bodyColor;
        Color headColor;
        /*List<Color> scavengerColor_Eyes = new List<Color>();
        List<Color> scavengerColor_FirstBckCosmeticSprite = new List<Color>();//
        Color scavengerColor_ChestSprite;
        Color scavengerColor_HipSprite;
        Color scavengerColor_WaistSprite;
        Color scavengerColor_ChestPatchSprite;
        Color scavengerColor_FirstFrntCosmeticSprite;
        Color scavengerColor_HeadSprite;
        Color scavengerColor_TeethSprite;
        Color scavengerColor_MaskSprite;
        Color scavengerColor_ShellSprite;*/
        //拾荒颜色处理
        public void SetScavengerColor(ScavengerGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                ScavengerColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (i == self.HeadSprite ||
                    i == self.TeethSprite)
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor_head);
                else
                    sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
            }
            sLeaser.sprites[self.NeckSprite].color = ColorChange(bodyColor, freezeColor);
            foreach (var p in self.legs)
                sLeaser.sprites[p.firstSprite].color = ColorChange(bodyColor, freezeColor);
            foreach (var p in self.hands)
                sLeaser.sprites[p.firstSprite].color = ColorChange(bodyColor, freezeColor);
            for (int i = self.eartlers.firstSprite; i < self.eartlers.firstSprite + self.eartlers.TotalSprites; i++)
                sLeaser.sprites[i].color = ColorChange(headColor, freezeColor_head);
        }

        public void ScavengerColorSave(ScavengerGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
            bodyColor = sLeaser.sprites[self.ChestSprite].color;
            headColor = sLeaser.sprites[self.HeadSprite].color;
        }

        //拾荒颜色处理
        static public void ScavengerGraphics_DrawSprites(On.ScavengerGraphics.orig_DrawSprites orig, ScavengerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.scavenger, out var colorRecord))
            {
                var cr = (ScavengerColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetScavengerColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.scavenger);
                }
            }
        }

        static public void Scavenger_Update(On.Scavenger.orig_Update orig, Scavenger self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
