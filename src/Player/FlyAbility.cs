using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGlacier2
{
    //飞行能力变量
    public struct FlyAbility
    {
        //飞行状态
        public bool canFly = false;
        public bool onFly = false;
        //飞行时间计时条
        public float counter = 0;
        public float counter_dec = 1f;//飞行时间消耗速度
        public float counter_inc = 0.5f;//飞行时间恢复速度
        public float counter_max = 260f;//飞行时间计时条最大值
        //飞行状态恢复切换
        public float restore = 0;
        public float restore_max = 40f;
        //飞行特效物体
        public MyLightTailObj tailL = null;
        public MyLightTailObj tailR = null;
        //飞行计时条
        public MyFlyBar myFlyBar = null;
        //飞行速度
        public float wingSpeed = 2;
        public float maxSpeed = 5;

        public FlyAbility()
        {
#if MYDEBUG
            try
            {
#endif
            counter = counter_max;
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

        public void FlyUp(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.bodyChunks[1].vel.y < 0)
                self.bodyChunks[1].vel.y = 0;
            if (self.bodyChunks[1].vel.y < maxSpeed)
                self.bodyChunks[1].vel.y += wingSpeed;
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

        public void FlyDown(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.bodyChunks[1].vel.y > 0)
                self.bodyChunks[1].vel.y = 0;
            if (self.bodyChunks[1].vel.y > -maxSpeed)
                self.bodyChunks[1].vel.y -= wingSpeed;
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

        public void FlyLeft(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.bodyChunks[0].vel.x > 0)
                self.bodyChunks[0].vel.x = 0;
            if (self.bodyChunks[0].vel.x > -maxSpeed)
                self.bodyChunks[0].vel.x -= wingSpeed;
            self.bodyChunks[1].vel.x++;//身体方向矫正
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

        public void FlyRight(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.bodyChunks[0].vel.x < 0)
                self.bodyChunks[0].vel.x = 0;
            if (self.bodyChunks[0].vel.x < maxSpeed)
                self.bodyChunks[0].vel.x += wingSpeed;
            self.bodyChunks[1].vel.x--;//身体方向矫正
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

        public void Glacier2_Fly(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            //玩家是否在空中
            bool inAir = self.Consious &&
            self.bodyMode != Player.BodyModeIndex.Stand &&
            self.bodyMode != Player.BodyModeIndex.Crawl &&
            self.bodyMode != Player.BodyModeIndex.Stunned &&
            self.bodyMode != Player.BodyModeIndex.Swimming &&
            self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut &&
            self.bodyMode != Player.BodyModeIndex.CorridorClimb &&
            self.bodyMode != Player.BodyModeIndex.ClimbingOnBeam &&
            self.bodyMode != Player.BodyModeIndex.WallClimb;
            //如果落回地面则无法飞行
            if (self.canJump > 0)
            {
                canFly = false;
            }
            if (self.wantToJump > 0 && inAir)
            {
                canFly = true;
            }
            if (self.input[0].jmp &&
                canFly &&
                counter - counter_dec > 0)
            {
                GlobalVar.dbgstr += " canFly = true\n";
                //往右上飞
                if (self.input[0].x > 0 && self.input[0].y > 0)
                {
                    FlyRight(self);
                    FlyUp(self);
                }
                //往左上飞
                else if (self.input[0].x < 0 && self.input[0].y > 0)
                {
                    FlyLeft(self);
                    FlyUp(self);
                }
                //往右下飞
                else if (self.input[0].x > 0 && self.input[0].y < 0)
                {
                    FlyRight(self);
                    FlyDown(self);
                }
                //往左下飞
                else if (self.input[0].x < 0 && self.input[0].y < 0)
                {
                    FlyLeft(self);
                    FlyDown(self);
                }
                //往右飞
                else if (self.input[0].x > 0)
                {
                    FlyRight(self);
                    self.bodyChunks[0].vel.y = self.gravity;
                    self.bodyChunks[1].vel.y = self.gravity;
                }
                //往左飞
                else if (self.input[0].x < 0)
                {
                    FlyLeft(self);
                    self.bodyChunks[0].vel.y = self.gravity;
                    self.bodyChunks[1].vel.y = self.gravity;
                }
                //往下飞
                else if (self.input[0].y < 0)
                {
                    FlyDown(self);
                    self.bodyChunks[0].vel.x = 0;
                    self.bodyChunks[1].vel.x = 0;
                }
                //往上飞
                else if (self.input[0].y > 0 || self.input[0].jmp)
                {

                    FlyUp(self);
                    self.bodyChunks[0].vel.x = 0;
                    self.bodyChunks[1].vel.x = 0;
                }
                counter -= counter_dec;
                restore = restore_max;
                onFly = true;
            }
            else
                onFly = false;
            //恢复飞行计时
            if (onFly == false)
            {
                if (restore > 0)
                    restore--;
                else
                {
                    //恢复到最大值
                    if (counter < counter_max)
                        counter += counter_inc;
                    if (counter > counter_max)
                        counter = counter_max;
                }
            }
            //滑翔特效
            if (onFly == true)
            {

                if (tailL == null)
                {
                    tailL = new MyLightTailObj(GetTracePos, self, true);
                    self.room.AddObject(tailL);
                }

                if (tailR == null)
                {
                    tailR = new MyLightTailObj(GetTracePos, self, false);
                    self.room.AddObject(tailR);
                }
            }
            //滑翔特效销毁
            if (onFly == false)
            {
                if (tailL != null)
                {
                    //销毁特效物体
                    tailL.ReadyDestory();
                    tailL = null;
                }
                if (tailR != null)
                {
                    //销毁特效物体
                    tailR.ReadyDestory();
                    tailR = null;
                }
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

        //追踪轨迹函数
        public static Vector2 GetTracePos(Player self, bool left = true)
        {
#if MYDEBUG
            try
            {
#endif
            Vector2 dstPos = self.mainBodyChunk.pos;
            float rad = -self.mainBodyChunk.Rotation.GetRadians();
            if (left)
                rad += 120 * Mathf.Deg2Rad;
            else
                rad -= 120 * Mathf.Deg2Rad;
            Vector2 dir;
            dir.x = Mathf.Cos(rad);
            dir.y = Mathf.Sin(rad);
            dir *= 20f;
            dstPos += dir;
            return dstPos;
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
                return Vector2.zero;
            }
#endif
        }
    }
}
