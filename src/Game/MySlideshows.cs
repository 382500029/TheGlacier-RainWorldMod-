using Menu;
using Nutils.hook;
using UnityEngine;

namespace TheGlacier2
{
    public class MySlideshows
    {
        //游戏开头幻灯片
        public static SlideShow.SlideShowID Intro_TheGlacier2 = new SlideShow.SlideShowID("Intro_TheGlacier2", true);
        public static MenuScene.SceneID Intro_TheGlacier2_cut1 = new MenuScene.SceneID("Intro_TheGlacier2_cut1", true);
        //游戏飞升结尾幻灯片
        public static SlideShow.SlideShowID Outro_TheGlacier2 = new SlideShow.SlideShowID("Outro_TheGlacier2", true);
        public static MenuScene.SceneID Outro_TheGlacier2_cut1 = new MenuScene.SceneID("Outro_TheGlacier2_cut1", true);
        public static MenuScene.SceneID Outro_TheGlacier2_cut2 = new MenuScene.SceneID("Outro_TheGlacier2_cut2", true);

        public static void Hook()
        {
            //开头幻灯片
            SceneHook.AddScene(Intro_TheGlacier2_cut1, BuildIntro_TheGlacier2SceneCut1);
            SceneHook.AddIntroSlideShow("TheGlacier2", "RW_Intro_Theme", Intro_TheGlacier2, BuildIntro);
            //飞升幻灯片
            SceneHook.AddScene(Outro_TheGlacier2_cut1, BuildOutro_TheGlacier2SceneCut1);
            SceneHook.AddScene(Outro_TheGlacier2_cut2, BuildOutro_TheGlacier2SceneCut2);
            SceneHook.AddOutroSlideShow("TheGlacier2", "RW_Outro_Theme", Outro_TheGlacier2, BuildOutro);

        }

        //开头幻灯片
        public static void BuildIntro_TheGlacier2SceneCut1(MenuScene self)
        {
            self.sceneFolder = "slideshows/intro - theglacier2";
            self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder,
                "cut1", new Vector2(0f, 0f), 2f, MenuDepthIllustration.MenuShader.Basic));
        }
        private static void BuildIntro(SlideShow self)
        {
            self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, 0f, 0f, 0f));
            //cut1
            self.playList.Add(new SlideShow.Scene(Intro_TheGlacier2_cut1, self.ConvertTime(0, 0, 20), self.ConvertTime(0, 3, 26), self.ConvertTime(0, 8, 6)));
            for (int i = 1; i < self.playList.Count; i++)
            {
                self.playList[i].startAt += 0.6f;
                self.playList[i].fadeInDoneAt += 0.6f;
                self.playList[i].fadeOutStartAt += 0.6f;
            }
            self.processAfterSlideShow = ProcessManager.ProcessID.Game;
        }

        //飞升幻灯片
        private static void BuildOutro(SlideShow self)
        {
            self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, 0f, 0f, 0f));
            self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Outro_1_Left_Swim, self.ConvertTime(0, 1, 20), self.ConvertTime(0, 5, 0), self.ConvertTime(0, 17, 0)));
            self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Outro_2_Up_Swim, self.ConvertTime(0, 21, 0), self.ConvertTime(0, 25, 0), self.ConvertTime(0, 37, 0)));
            //cut1
            self.playList.Add(new SlideShow.Scene(Outro_TheGlacier2_cut1, self.ConvertTime(0, 41, 10), self.ConvertTime(0, 45, 20), self.ConvertTime(0, 46, 60)));
            //cut2
            self.playList.Add(new SlideShow.Scene(Outro_TheGlacier2_cut2, self.ConvertTime(0, 48, 20), self.ConvertTime(0, 51, 0), self.ConvertTime(0, 55, 0)));
            for (int i = 1; i < self.playList.Count; i++)
            {
                self.playList[i].startAt -= 1.1f;
                self.playList[i].fadeInDoneAt -= 1.1f;
                self.playList[i].fadeOutStartAt -= 1.1f;
            }
            self.processAfterSlideShow = ProcessManager.ProcessID.Credits;
            self.manager.statsAfterCredits = true;
        }
        public static void BuildOutro_TheGlacier2SceneCut1(MenuScene self)
        {
            self.sceneFolder = "slideshows/outro - theglacier2";
            self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder,
                "cut1", new Vector2(0f, 0f), 2f, MenuDepthIllustration.MenuShader.Basic));
        }
        public static void BuildOutro_TheGlacier2SceneCut2(MenuScene self)
        {
            self.sceneFolder = "slideshows/outro - theglacier2";
            self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder,
                "cut2", new Vector2(0f, 0f), 2f, MenuDepthIllustration.MenuShader.Basic));
        }
    }
}
