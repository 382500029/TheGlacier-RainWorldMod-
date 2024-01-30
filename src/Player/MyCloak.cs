
using System;
using UnityEngine;

namespace TheGlacier2
{
    public class MyCloak
    {
        //布料节点
        public class Pt
        {
            public MyCloak cloak;
            public Vector2 pos;
            public Vector2 prePos;
            public bool locked;
            public Pt(MyCloak cloak, Vector2 pos, bool locked = false)
            {
                this.cloak = cloak;
                this.pos = pos;
                prePos = pos;
                this.locked = locked;
            }
            public void Update()
            {
                if (locked)
                    return;
                Vector2 tempPos = pos;
                pos += pos - prePos;
                pos.y -= cloak.player.room.gravity * 0.05f;
                prePos = tempPos;
            }
        }
        //布料连接节点
        public class Jt
        {
            public MyCloak cloak;
            public Pt a;
            public Pt b;
            public float len;
            public Jt(MyCloak cloak, Pt a, Pt b, float len)
            {
                this.cloak = cloak;
                this.a = a;
                this.b = b;
                this.len = len;
            }
            public void Update()
            {
                Vector2 centrePos = (a.pos + b.pos) * 0.5f;
                Vector2 dir = (b.pos - a.pos).normalized;
                if(!a.locked)
                    a.pos = centrePos - dir * len * 0.5f;
                if (!b.locked)
                    b.pos = centrePos + dir * len * 0.5f;
            }
        }

        public Player player;
        private const int count_w = 21;
        private const int count_h = 19;
        private static int[] lockPt = 
            { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,
            24, 25, 26,
            45,
            66,
            87,
            108,
            128,129,
            149,
            169,170,
            190,
            211,
            231,232,
            252,
            273,
            294,
            315,
            336,
            357
        };
        private Pt[] ptArr_model;
        private Pt[] ptArr_show;//显示用的衣服坐标
        private Pt[] ptArr_show_last;//显示用的衣服坐标
        private Jt[] jtArr;
        private TriangleMesh tm;
        //矩阵
        Matrix4x4 matrix_rotate = new Matrix4x4();
        Matrix4x4 matrix_translate = new Matrix4x4();
        Matrix4x4 matrix_finally = new Matrix4x4();//图像最终显示矩阵
        private int sprite_index;
        private int reset = 0;

        public static void HookTexture()
        {
            //加载披风贴图
            Futile.atlasManager.LoadAtlas("atlases/icecloak");
        }

        public MyCloak(Player player)
        {
            this.player = player;
            //坐标变换
            var matrix_out = Matrix4x4.Translate(new Vector3(-count_w / 2, -8)) * Matrix4x4.Scale(new Vector3(1,-1,1));
            //设置布料节点
            ptArr_model = new Pt[count_w * count_h];
            ptArr_show = new Pt[count_w * count_h];
            ptArr_show_last = new Pt[count_w * count_h];
            for (int y = 0; y < count_h; y++)
            {
                for (int x = 0; x < count_w; x++)
                {
                    Vector4 v4_in, v4_out;
                    v4_in.x = (float)x * 20 / (count_w - 1);
                    v4_in.y = (float)y * 18 / (count_h - 1);
                    v4_in.z = 0;
                    v4_in.w = 1;
                    v4_out = matrix_out * v4_in;
                    ptArr_model[y * count_w + x] = new Pt(this, new Vector2(v4_out.x, v4_out.y));
                    ptArr_show[y * count_w + x] = new Pt(this, new Vector2(v4_out.x, v4_out.y));
                    ptArr_show_last[y * count_w + x] = new Pt(this, new Vector2(v4_out.x, v4_out.y));
                }
            }
            //设置布料连接节点
            jtArr = new Jt[(count_w - 1) * count_h + (count_h - 1) * count_w];
            //水平
            int index = 0;
            for (int y = 0; y < count_h; y++)
            {
                for (int x = 0; x < count_w - 1; x++)
                {
                    jtArr[index] = new Jt(this, ptArr_show[y * count_w + x], ptArr_show[y * count_w + x + 1], 1);
                    index++;
                }
            }
            //垂直
            for (int y = 0; y < count_h - 1; y++)
            {
                for (int x = 0; x < count_w; x++)
                {
                    jtArr[index] = new Jt(this, ptArr_show[y * count_w + x], ptArr_show[(y + 1) * count_w + x], 1);
                    index++;
                }
            }
            //给特定的布料节点上锁
            foreach (var i in lockPt)
            {
                ptArr_model[i].locked = true;
                ptArr_show[i].locked = true;
            }
        }

        //在PlayerGraphics_InitiateSprites里调用
        public void InitiateSprites(PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            const int sqnum_w = count_w - 1;
            const int sqnum_h = count_h - 1;
            const int sqnum = sqnum_w * sqnum_h;//正方形数量
            const int trinum = sqnum * 2;//三角形数量
            TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[trinum];
            int index = 0;
            for (int y = 0; y < sqnum_h; y++)
            {
                for (int x = 0; x < sqnum_w; x++)
                {
                    int dy = y * count_w;
                    tris[index] = new TriangleMesh.Triangle(x + dy, x + 1 + dy, x + 1 + count_w + dy);
                    index++;
                    tris[index] = new TriangleMesh.Triangle(x + dy, x + 1 + count_w + dy, x + count_w + dy);
                    index++;
                }
            }
            tm = new TriangleMesh("icecloak", tris, false, true);
            //设置uv坐标
            for (int y = 0; y < count_h; y++)
                for (int x = 0; x < count_w; x++)
                    tm.UVvertices[y * count_w + x] = new Vector2((float)x / sqnum_w, (1f - (float)y / sqnum_h));//垂直翻转一下uv 使图像显示正确
                                                                                                                //图像扩容和设置图像
            sprite_index = sLeaser.sprites.Length;
            Array.Resize<FSprite>(ref sLeaser.sprites, sLeaser.sprites.Length + 1);
            sLeaser.sprites[sprite_index] = tm;
            reset = 1;
        }
        //在PlayerGraphics_DrawSprites里调用
        public void DrawSprites(PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if(reset >0)
            {
                reset--;
                ResetPoint(sLeaser);
            }
            var headPos = sLeaser.sprites[3].GetPosition();
            var headRotation = -sLeaser.sprites[3].rotation;
            //模型坐标到世界坐标的转换
            matrix_rotate = Matrix4x4.Rotate(Quaternion.AngleAxis(headRotation, Vector3.forward));
            matrix_translate = Matrix4x4.Translate(new Vector3(headPos.x, headPos.y, 0));
            matrix_finally = matrix_translate * matrix_rotate;
            for (int y = 0; y < count_h; y++)
            {
                for (int x = 0; x < count_w; x++)
                {
                    //计算模型移动的顶点
                    if (ptArr_model[y * count_w + x].locked == false)
                        continue;
                    Vector4 v4_in = ptArr_model[y * count_w + x].pos;
                    v4_in.w = 1;
                    Vector4 v4_out = matrix_finally * v4_in;
                    ptArr_show[y * count_w + x].pos = v4_out;
                }
            }
            //显示
            for (int i = 0; i < ptArr_show.Length; i++)
                tm.MoveVertice(i, Vector2.Lerp(ptArr_show_last[i].pos, ptArr_show[i].pos, timeStacker));
        }
        //在PlayerGraphics_AddToContainer里调用
        public void AddToContainer(PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            //防止重复添加
            if (!(sprite_index > 0 && sLeaser.sprites.Length > sprite_index))
                return;
            //添加到图层
            FContainer fcontainer = (newContatiner == null) ? rCam.ReturnFContainer("Midground") : newContatiner;
            fcontainer.AddChild(sLeaser.sprites[sprite_index]);
            //调整图层顺序
            sLeaser.sprites[sprite_index].MoveBehindOtherNode(sLeaser.sprites[3]);
        }
        //在Player_Update里调用
        public void Update(Player self, bool eu)
        {
            //保存上一步图像
            for (int i = 0; i < ptArr_show.Length; i++)
                ptArr_show_last[i].pos = ptArr_show[i].pos;
            foreach (var pt in ptArr_show) 
                pt.Update();
            for (int i = 0; i < 10; i++)
                foreach (var jt in jtArr)
                    jt.Update();
        }
        //重置布料点的位置
        public void ResetPoint(RoomCamera.SpriteLeaser sLeaser)
        {
            var headPos = sLeaser.sprites[3].GetPosition();
            var headRotation = -sLeaser.sprites[3].rotation;
            //模型坐标到世界坐标的转换
            matrix_rotate = Matrix4x4.Rotate(Quaternion.AngleAxis(headRotation, Vector3.forward));
            matrix_translate = Matrix4x4.Translate(new Vector3(headPos.x, headPos.y, 0));
            matrix_finally = matrix_translate * matrix_rotate;
            for (int y = 0; y < count_h; y++)
            {
                for (int x = 0; x < count_w; x++)
                {
                    //计算模型移动的顶点
                    Vector4 v4_in = ptArr_model[y * count_w + x].pos;
                    v4_in.w = 1;
                    Vector4 v4_out = matrix_finally * v4_in;
                    ptArr_show[y * count_w + x].pos = v4_out;
                }
            }
            for (int i = 0; i < ptArr_show.Length; i++)
            {
                ptArr_show_last[i].pos = ptArr_show[i].pos;
                ptArr_show[i].prePos = ptArr_show[i].pos;
                ptArr_show_last[i].prePos = ptArr_show_last[i].pos;
            }
               
        }
    }
}
