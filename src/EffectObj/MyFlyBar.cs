using RWCustom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGlacier2
{
    public class MyFlyBar : CosmeticSprite
    {
        Player player;
        int numGodPips;
        Color color;
        float radius = 6f;
        float alpha = 0;
        float alpha_count;
        const float alpha_count_max = 40;
        public MyFlyBar(Player player,int numGodPips = 8)
        {
#if MYDEBUG
            try
            {
#endif
            this.player = player;
            this.numGodPips = numGodPips;
            color.r = 192.0f / 255.0f;
            color.g = 237.0f / 255.0f;
            color.b = 249.0f / 255.0f;
            color.a = 1.0f;
#if MYDEBUG
            }
            catch (Exception e)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                var sr = sf.GetFileName().Split('\\');
                MyDebug.outStr = sr[sr.Length - 1] + "\n";
                MyDebug.outStr += sf.GetMethod() + "\n";
                MyDebug.outStr += e;
                UnityEngine.Debug.Log(e);
            }
#endif
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
#if MYDEBUG
            try
            {
#endif
            sLeaser.sprites = new FSprite[numGodPips];
            for (int i = 0; i < numGodPips; i++)
                sLeaser.sprites[i] = new FSprite("WormEye");
            FContainer fcontainer = rCam.ReturnFContainer("HUD");
            //显示图像
            for (int i = 0; i < numGodPips; i++)
                fcontainer.AddChild(sLeaser.sprites[i]);
#if MYDEBUG
            }
            catch (Exception e)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                var sr = sf.GetFileName().Split('\\');
                MyDebug.outStr = sr[sr.Length - 1] + "\n";
                MyDebug.outStr += sf.GetMethod() + "\n";
                MyDebug.outStr += e;
                UnityEngine.Debug.Log(e);
            }
#endif
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
#if MYDEBUG
            try
            {
#endif
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            //如果玩家在管道则不绘制图像
            for (int i = 0; i < numGodPips; i++)
                sLeaser.sprites[i].isVisible = !player.inShortcut;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);

            float eachPipsTime = pv.flyAbility.counter_max / numGodPips;
            for (int i = 0; i < numGodPips; i++)
            {
                float thisPipsTime_start = eachPipsTime * (float)i;
                float thisPipsTime_end = eachPipsTime * (float)(i + 1);
                if (pv.flyAbility.counter <= thisPipsTime_start)
                {
                    sLeaser.sprites[i].scale = 0f;
                }
                else if (pv.flyAbility.counter >= thisPipsTime_end)
                {
                    sLeaser.sprites[i].scale = 1f;
                }
                else
                {
                    sLeaser.sprites[i].scale = (pv.flyAbility.counter - thisPipsTime_start) / eachPipsTime;
                }
                sLeaser.sprites[i].color = color;
                sLeaser.sprites[i].alpha = alpha;
                var markPos = Vector2.Lerp(player.bodyChunks[1].lastPos, player.bodyChunks[1].pos, timeStacker);
                markPos.y += 60f;
                markPos -= Vector2.Lerp(rCam.lastPos, rCam.pos, timeStacker);

                markPos += Custom.rotateVectorDeg(Vector2.one * radius, (float)(i - 15) * (360f / (float)numGodPips));
                sLeaser.sprites[i].x = markPos.x;
                sLeaser.sprites[i].y = markPos.y;
            }
#if MYDEBUG
            }
            catch (Exception e)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                var sr = sf.GetFileName().Split('\\');
                MyDebug.outStr = sr[sr.Length - 1] + "\n";
                MyDebug.outStr += sf.GetMethod() + "\n";
                MyDebug.outStr += e;
                UnityEngine.Debug.Log(e);
            }
#endif
        }

        public override void Update(bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            //如果玩家在管道则不绘制图像
            if (player.inShortcut)
                return;
            base.Update(eu);
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);

            if (pv.flyAbility.counter < pv.flyAbility.counter_max)
            {
                if (alpha_count < alpha_count_max)
                    alpha_count++;
            }
            else
            {
                if (alpha_count > 0)
                    alpha_count--;
            }
            alpha = alpha_count / alpha_count_max;
#if MYDEBUG
            }
            catch (Exception e)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                var sr = sf.GetFileName().Split('\\');
                MyDebug.outStr = sr[sr.Length - 1] + "\n";
                MyDebug.outStr += sf.GetMethod() + "\n";
                MyDebug.outStr += e;
                UnityEngine.Debug.Log(e);
            }
#endif
        }
    }
}
