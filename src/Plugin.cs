using System;
using BepInEx;
using TheGlacier;
using UnityEngine;

namespace TheGlacier2
{
    [BepInPlugin(MOD_ID, "The Glacier2", "0.2.1")]
    class Plugin : BaseUnityPlugin
    {
        //设置ModID
        private const string MOD_ID = "hwic.theglacier2";
        //用于检查角色id
        public static readonly SlugcatStats.Name YourSlugID = new SlugcatStats.Name("TheGlacier2", false);
        /*-----------------------------------------------------挂钩-----------------------------------------------------*/
        public void OnEnable()
        {
#if MYDEBUG
            try
            {
#endif
            //mod初始化
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            //玩家能力
            MyPlayer.Hook();
            //玩家图像
            MyPlayerGraphics.Hook();
            //游戏内容设置
            MyGame.Hook();
            //冰矛
            MyIceSpear.Hook();
            //冰盾
            MyIceShield.Hook();
            //幻灯片
            //MySlideshows.Hook();
            //迭代器对话
            MyFPChat.Hook();
            MyMoonChat.Hook();
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

        /*----------------------------------------------------资源------------------------------------------------------*/
        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(self);
            //加载设置菜单
            MachineConnector.SetRegisteredOI(MOD_ID, MyOption.Instance);
            //加载玩家皮肤贴图
            MyPlayerGraphics.HookTexture();
            //加载冰矛贴图
            MyIceSpear.HookTexture();
            MyIceSpear.HookSound();//音效
            //加载冰盾贴图
            MyIceShield.HookTexture();
            MyIceShield.HookSound();//音效
            //加载雪花贴图
            MySnow.HookTexture();
            //加载披风贴图
            MyCloak.HookTexture();
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