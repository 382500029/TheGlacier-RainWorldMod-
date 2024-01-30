using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGlacier2
{
    public class CentipedeColorRecord : ColorRecord
    {
        public List<Color> colorList = new List<Color>();
        /*public Dictionary<int, Color> colorDic = new Dictionary<int, Color>();*/
        //蜈蚣颜色处理
        public void SetCentipedeColor(CentipedeGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            //保存原本的颜色
            if (!setColor)
            {
                setColor = true;
                CentipedeColorSave(self, sLeaser);
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
                sLeaser.sprites[i].color = ColorChange(colorList[i], freezeColor);
        }

        public void CentipedeColorSave(CentipedeGraphics self, RoomCamera.SpriteLeaser sLeaser)
        {
            foreach (var p in sLeaser.sprites)
                colorList.Add(p.color);
        }

        //蜈蚣颜色处理
        static public void CentipedeGraphics_DrawSprites(On.CentipedeGraphics.orig_DrawSprites orig, CentipedeGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            var freezeCreature = GlobalVar.freezeCreature;

            if (freezeCreature.TryGetValue(self.centipede, out var colorRecord))
            {
                var cr = (CentipedeColorRecord)colorRecord;
                if (cr.meltTime > 0)
                    cr.SetCentipedeColor(self, sLeaser);
                else
                {
                    self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                    freezeCreature.Remove(self.centipede);
                }
            }
        }

        static public void Centipede_Update(On.Centipede.orig_Update orig, Centipede self, bool eu)
        {
            var freezeCreature = GlobalVar.freezeCreature;
            if (freezeCreature.TryGetValue(self, out var colorRecord))
                colorRecord.Step();
            else
                orig.Invoke(self, eu);
        }
    }
}
    
