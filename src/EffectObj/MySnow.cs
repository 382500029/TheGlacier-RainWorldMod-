using RWCustom;
using System.Diagnostics;
using System;
using UnityEngine;

namespace TheGlacier2
{
    public class MySnow : CosmeticSprite
    {
        public static string[] snowName = { "snow1", "snow2", "snow3", "snow4", "snow5", "snow6", "snow7" };
        private float lifeTime;
        private float lifeTime_max = 20;
        private float disappearTime = 0.2f;
        private LightSource lightSource = null;
        private bool reverse = false;//反转雪花运动 向目标中心汇聚
        private Vector2 reverseCenterPos;
        //参数
        public delegate Vector2 GetTracePos_Snow(Player self);
        private GetTracePos_Snow getTracePos = null;
        private Player getTracePos_arg1;
        public static void HookTexture()
        {
#if MYDEBUG
            try
            {
#endif
            //加载雪花贴图
            Futile.atlasManager.LoadAtlas("atlases/snow1");
            Futile.atlasManager.LoadAtlas("atlases/snow2");
            Futile.atlasManager.LoadAtlas("atlases/snow3");
            Futile.atlasManager.LoadAtlas("atlases/snow4");
            Futile.atlasManager.LoadAtlas("atlases/snow5");
            Futile.atlasManager.LoadAtlas("atlases/snow6");
            Futile.atlasManager.LoadAtlas("atlases/snow7");
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


        public MySnow(Vector2 setPos, bool reverse = false, GetTracePos_Snow getTracePos = null, Player getTracePos_arg1 = null)
        {
#if MYDEBUG
            try
            {
#endif
            this.reverse = reverse;
            this.getTracePos = getTracePos;
            this.getTracePos_arg1 = getTracePos_arg1;
            Vector2 tempVel;
            tempVel.x = UnityEngine.Random.Range(-10f, 10f);
            tempVel.y = UnityEngine.Random.Range(-10f, 10f);
            if (reverse)
            {
                //反向雪花
                reverseCenterPos = setPos;
                //从周围随机半径角度产生雪花
                pos = tempVel * lifeTime_max;
                pos += reverseCenterPos;
                vel = -tempVel;
            }
            else
            {
                pos = setPos;
                vel = tempVel;
            }
            lifeTime = lifeTime_max;
            lastPos = setPos;
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

        public MySnow(Vector2 pos, Vector2 decVel, bool reverse = false)
        {
#if MYDEBUG
            try
            {
#endif
            this.reverse = reverse;
            lifeTime = lifeTime_max;
            this.pos = pos;
            lastPos = pos;
            vel.x = UnityEngine.Random.Range(-10f, 10f);
            vel.y = UnityEngine.Random.Range(-10f, 10f);
            vel -= decVel;
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

        public static string GetSnowStringRandom()
        {
            return snowName[UnityEngine.Random.Range(0, 6)];
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
#if MYDEBUG
            try
            {
#endif
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite(GetSnowStringRandom());
            FContainer fcontainer = rCam.ReturnFContainer("HUD");
            fcontainer.AddChild(sLeaser.sprites[0]);
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
            Vector2 v = Vector2.Lerp(lastPos, pos, timeStacker);
            v -= Vector2.Lerp(rCam.lastPos, rCam.pos, timeStacker);
            sLeaser.sprites[0].x = v.x;
            sLeaser.sprites[0].y = v.y;
            if (reverse && getTracePos != null)
                sLeaser.sprites[0].alpha = (lifeTime_max - lifeTime) / (lifeTime_max * disappearTime);
            else
                sLeaser.sprites[0].alpha = lifeTime / (lifeTime_max * disappearTime);
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

        private bool DisappearDistance()
        {
            Vector2 dstPos = getTracePos(getTracePos_arg1);
            return (dstPos - pos).sqrMagnitude <= Mathf.Max(1.0f, vel.sqrMagnitude);
        }

        public override void Update(bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            base.Update(eu);
            bool ad = reverse && getTracePos != null;
            if (ad)
            {
                //改变能量方向
                Vector2 dstPos = getTracePos(getTracePos_arg1);
                //追踪坐标
                Vector2 v1 = (dstPos - pos).normalized;
                vel = vel.magnitude * v1;
            }
            if (lightSource == null)
            {
                lightSource = new LightSource(pos, false, new Color(41f / 255f, 69f / 255f, 83f / 255f), null);
                room.AddObject(lightSource);
            }
            if (lightSource != null)
            {
                lightSource.requireUpKeep = true;
                lightSource.setPos = pos;
                if (reverse && ad)
                    lightSource.setRad = (lifeTime_max - lifeTime) * 50f / lifeTime_max;
                else
                    lightSource.setRad = lifeTime * 50f / lifeTime_max;
                lightSource.stayAlive = true;
                lightSource.setAlpha = 1f;
                if (lightSource.slatedForDeletetion || lightSource.room != room)
                    lightSource = null;
            }
            if (lifeTime > 0)
                lifeTime--;
            //没到时间但是已经在距离内
            if (ad && DisappearDistance())
                Disappear();
            if (lifeTime <= 0)
            {
                if (ad)
                {
                    if (getTracePos == null || DisappearDistance())
                        Disappear();
                    else
                        vel *= 1.1f;
                }
                else
                    Disappear();
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

        private void Disappear()
        {
#if MYDEBUG
            try
            {
#endif
                if (lightSource != null)
                    lightSource.Destroy();
                Destroy();
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