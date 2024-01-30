using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGlacier2
{
    public class MyPlayerGraphics
    {
        public static void HookTexture()
        {
#if MYDEBUG
            try
            {
#endif
            Futile.atlasManager.LoadAtlas("atlases/Glacier2arm");
            Futile.atlasManager.LoadAtlas("atlases/Glacier2body");
            Futile.atlasManager.LoadAtlas("atlases/Glacier2face");
            Futile.atlasManager.LoadAtlas("atlases/Glacier2head");
            Futile.atlasManager.LoadAtlas("atlases/Glacier2headC");
            Futile.atlasManager.LoadAtlas("atlases/Glacier2hips");
            Futile.atlasManager.LoadAtlas("atlases/Glacier2legs");
            Futile.atlasManager.LoadAtlas("atlases/Glacier2tail");
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

        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
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

        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(self, sLeaser, rCam);
            if (self.player.slugcatStats.name != Plugin.YourSlugID)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self.player, out PlayerVar pv);
            /*------------------------------------------------------------------调试图像------------------------------------------------------------------------------*/
            //pv.myDebug.InitiateSprites(sLeaser, rCam);
            /*------------------------------------------------------------------飞行能力图像------------------------------------------------------------------------------*/
            if (pv.flyAbility.myFlyBar != null)
                pv.flyAbility.myFlyBar.Destroy();
            pv.flyAbility.myFlyBar = new MyFlyBar(self.player);
            self.player.room.AddObject(pv.flyAbility.myFlyBar);
            /*-------------------------------------------------------------------冰盾-------------------------------------------------------------------------------------*/
            List<float> countList = new List<float>();
            int iceShieldCount = pv.iceShieldList.Count;
            if (iceShieldCount > 0)
                foreach (var iceShield in pv.iceShieldList)
                {
                    countList.Add(iceShield.counter);
                    iceShield.Destroy();
                }

            pv.iceShieldList.Clear();
            /*if (iceShieldCount == 0)
            {
                iceShieldCount = 20;
                for (int i = 0; i < iceShieldCount; i++)
                    countList.Add(0);
            }*/
            for (int i = 0; i < iceShieldCount; i++)
            {
                var iceShield = new MyIceShield(self.player);
                iceShield.counter = countList[i];
                pv.iceShieldList.Add(iceShield);
                self.player.room.AddObject(iceShield);
            }
            /*-------------------------------------------------------------------披风-------------------------------------------------------------------------------------*/
            pv.cloak = new MyCloak(self.player);
            pv.cloak.InitiateSprites(self, sLeaser, rCam);
            /*-------------------------------------------------------------------尾巴-------------------------------------------------------------------------------------*/
            //定义尾巴三角网格
            TriangleMesh.Triangle[] tailtris = new TriangleMesh.Triangle[]
            {
                    new TriangleMesh.Triangle(0, 1, 2),
                    new TriangleMesh.Triangle(1, 2, 3),
                    new TriangleMesh.Triangle(2, 3, 4),
                    new TriangleMesh.Triangle(3, 4, 5),
                    new TriangleMesh.Triangle(4, 5, 6),
                    new TriangleMesh.Triangle(5, 6, 7),
                    new TriangleMesh.Triangle(6, 7, 8),
                    new TriangleMesh.Triangle(7, 8, 9),
                    new TriangleMesh.Triangle(8, 9, 10),
                    new TriangleMesh.Triangle(9, 10, 11),
                    new TriangleMesh.Triangle(10, 11, 12),
                    new TriangleMesh.Triangle(11, 12, 13),
                    new TriangleMesh.Triangle(12,13,14),
            };
            pv.tailMesh = new TriangleMesh("Glacier2tail", tailtris, false, true);
            sLeaser.sprites[2] = pv.tailMesh;
            //重新添加自身的所有图像
            self.AddToContainer(sLeaser, rCam, null);
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

        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, UnityEngine.Vector2 camPos)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            if (self.player.slugcatStats.name != Plugin.YourSlugID)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self.player, out PlayerVar pv);
            /*--------------------------------------------------调试图像--------------------------------------------------------*/
            //pv.myDebug.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            /*----------------------------------------------------披风----------------------------------------------------------*/
            pv.cloak.DrawSprites(self,sLeaser,rCam,timeStacker, camPos);
            /*----------------------------------------------------尾巴----------------------------------------------------------*/
            MapTailUV(self, pv.tailMesh);//3D尾巴
            /*----------------------------------------------------身体----------------------------------------------------------*/
            //替换角色身体部件的贴图
            const string Name = "Glacier2";
            for (int i = 0; i < 10; i++)
            {
                if (i == 2)
                    continue;
                //找到名字开头不是Glacier2的element,替换为有这个前缀的版本
                if (!sLeaser.sprites[i].element.name.StartsWith(Name))
                {
                    sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName(Name + sLeaser.sprites[i].element.name);
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
        //3D尾巴
        private static void MapTailUV(PlayerGraphics self, TriangleMesh tail)
        {
#if MYDEBUG
            try
            {
#endif
            float scaleFac = 3f;
            Vector2 legsPos = self.legs.pos;
            Vector2 tailPos = self.tail[0].pos;
            float difference = tailPos.x - legsPos.x;
            float leftRightRatio = Mathf.InverseLerp(-15f, 15f, difference);
            float uvYOffset = Mathf.Lerp(0f, tail.element.uvTopRight.y - tail.element.uvTopRight.y / scaleFac, leftRightRatio);
            for (int vertex = tail.vertices.Length - 1; vertex >= 0; vertex--)
            {
                float interpolation = (float)vertex / 2f / ((float)tail.vertices.Length / 2f);
                bool flag = vertex % 2 == 0;
                Vector2 uvInterpolation;
                if (flag)
                {
                    uvInterpolation = new Vector2(interpolation, 0f);
                }
                else
                {
                    bool flag2 = vertex == tail.vertices.Length - 1;
                    if (flag2)
                    {
                        uvInterpolation = new Vector2(1f, 0f);
                    }
                    else
                    {
                        uvInterpolation = new Vector2(interpolation, 1f);
                    }
                }
                Vector2 uv;
                uv.x = Mathf.Lerp(tail.element.uvBottomLeft.x, tail.element.uvTopRight.x, uvInterpolation.x);
                uv.y = Mathf.Lerp(tail.element.uvBottomLeft.y + uvYOffset, tail.element.uvTopRight.y / scaleFac + uvYOffset, uvInterpolation.y);
                tail.UVvertices[vertex] = uv;
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

        private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig.Invoke(self, sLeaser, rCam, newContatiner);
            if (self.player.slugcatStats.name != Plugin.YourSlugID)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self.player, out PlayerVar pv);
            /*----------------------------------------------------披风----------------------------------------------------------*/
            if (pv.cloak != null)
                pv.cloak.AddToContainer(self, sLeaser, rCam, newContatiner);
        }
    }
}
