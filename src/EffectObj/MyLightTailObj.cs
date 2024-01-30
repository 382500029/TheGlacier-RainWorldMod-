using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheGlacier2
{
    public class MyLightTailObj : CosmeticSprite
    {
        readonly int section;//分段数量
        public List<Vector2> track;//目标移动轨迹
        public List<Vector2> trackVec;//每个点的向量
        public List<Vector2> trackVecAverage;//每个点的平均向量
        public List<Vector2> verticalVec;//每个点的垂直向量
        public Vector2[] track_finally;//轨道
        public Vector2[] track_finally_last;//轨道
        public float tail_width;//尾巴宽度
        public TriangleMesh tm;//拖尾网格

        public delegate Vector2 GetTracePos(Player self, bool left);
        private GetTracePos getTracePos;
        private Player getTracePos_arg1;
        private bool getTracePos_arg2;

        private bool readyDestory = false;
        private int readyDestory_counter;
        private Vector2 tailLastPos;

        public MyLightTailObj(GetTracePos getTracePos, Player getTracePos_arg1, bool getTracePos_arg2, int section = 20, float tail_width = 2)
        {
#if MYDEBUG
            try
            {
#endif
            this.section = section;
            this.tail_width = tail_width;
            track = new List<Vector2>();
            trackVec = new List<Vector2>();
            trackVecAverage = new List<Vector2>();
            verticalVec = new List<Vector2>();
            track_finally = new Vector2[section * 2];
            track_finally_last = new Vector2[section * 2];
            readyDestory_counter = section;
            this.getTracePos = getTracePos;
            this.getTracePos_arg1 = getTracePos_arg1;
            this.getTracePos_arg2 = getTracePos_arg2;
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
            //设置顶点
            var trinum = (section - 1) * 2;
            TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[trinum];
            for (int i = 0; i < trinum; i++)
                tris[i] = new TriangleMesh.Triangle(i, i + 1, i + 2);
            tm = new TriangleMesh("Futile_White", tris, true, false);
            //设置uv坐标
            for (int i = 0; i < section; i++)
            {
                float temp = section - 1;
                tm.UVvertices[i * 2] = new Vector2(0.0f, (1f / temp) * (temp - i));
                tm.UVvertices[i * 2 + 1] = new Vector2(1.0f, (1f / temp) * (temp - i));
            }
            //设置颜色
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = tm;
            FContainer fcontainer = rCam.ReturnFContainer("Background");
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

        //更新
        public override void Update(bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            base.Update(eu);
            if (readyDestory == false)
                tailLastPos = getTracePos(getTracePos_arg1, getTracePos_arg2);
            while (track.Count < section)
                track.Add(tailLastPos);
            HeadMoveNewPos(tailLastPos);
            if (readyDestory)
            {
                if (readyDestory_counter > 0)
                {
                    readyDestory_counter--;
                }
                else
                {
                    Destroy();
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

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
#if MYDEBUG
            try
            {
#endif
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            Vector2 v;
            for (int i = 0; i < track_finally.Length; i++)
            {
                v = Vector2.Lerp(track_finally_last[i], track_finally[i], timeStacker);
                v -= Vector2.Lerp(rCam.lastPos, rCam.pos, timeStacker);
                tm.MoveVertice(i, v);
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

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
#if MYDEBUG
            try
            {
#endif
            Color c;
            c.r = 183.0f / 255.0f;
            c.g = 240.0f / 255.0f;
            c.b = 255.0f / 255.0f;
            c.a = 1.0f;
            for (int i = 0; i < section; i++)
            {
                //c.a = ((float)section - i) / section;
                tm.verticeColors[i * 2] = c;
                tm.verticeColors[i * 2 + 1] = c;
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

        //准备销毁
        public void ReadyDestory()
        {
#if MYDEBUG
            try
            {
#endif
            readyDestory = true;
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

        //拖尾的头移动到一个新位置
        public void HeadMoveNewPos(Vector2 pos)
        {
#if MYDEBUG
            try
            {
#endif
            //在头部加入一个新坐标，再把尾部的移除
            track_finally.CopyTo(track_finally_last, 0);
            track.Insert(0, pos);
            track.RemoveAt(track.Count - 1);
            CalcPos();
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

        //计算每个点的坐标
        public void CalcPos()
        {
#if MYDEBUG
            try
            {
#endif
            //补到一定的长度
            while (track.Count < section)
                track.Add(Vector2.zero);
            //计算向量
            trackVec.Clear();
            Vector2 v;
            Vector3 c1, c2, cOut;
            for (int i = 0; i < track.Count - 1; i++)
            {
                v = track[i + 1] - track[i];
                trackVec.Add(v);
            }
            trackVec.Add(trackVec[trackVec.Count - 1]);//加尾
                                                       //向量平均化
            trackVecAverage.Clear();
            trackVecAverage.Add(trackVec[0]);//加头
            for (int i = 0; i < trackVec.Count - 1; i++)
            {
                v = trackVec[i] + trackVec[i + 1];
                trackVecAverage.Add(v);
            }
            //计算垂直向量
            verticalVec.Clear();
            foreach (var vec in trackVecAverage)
            {
                c1.x = vec.x;
                c1.y = vec.y;
                c1.z = 0;
                c2.x = 0;
                c2.y = 0;
                c2.z = 1;
                cOut = Vector3.Cross(c1, c2);
                v.x = cOut.x;
                v.y = cOut.y;
                verticalVec.Add(v.normalized);
            }
            //计算最终顶点坐标
            for (int i = 0; i < verticalVec.Count; i++)
            {
                float w = ((float)verticalVec.Count - i) / verticalVec.Count;
                track_finally[i * 2] = track[i] - verticalVec[i] / 2 * tail_width * w;
                track_finally[i * 2 + 1] = track[i] + verticalVec[i] / 2 * tail_width * w;
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
    }
}
