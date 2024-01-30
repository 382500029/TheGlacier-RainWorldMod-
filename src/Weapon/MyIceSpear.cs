using MoreSlugcats;
using System.Diagnostics;
using System;
using UnityEngine;

namespace TheGlacier2
{
    public class MyIceSpear : Spear
    {
        public int energy = 0;
        public int energy_max = 3;
        public LightSource lightSource = null;

        public static int IceSpear_ID = -20230825;
        public static SoundID soundID_freeze;//音效

        public MyIceSpear(AbstractPhysicalObject abstractPhysicalObject, World world) : base(abstractPhysicalObject, world)
        {
#if MYDEBUG
            try
            {
#endif
            energy = energy_max;
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

        public static void HookSound()
        {
#if MYDEBUG
            try
            {
#endif
            //加载音效
            soundID_freeze = new SoundID("SNDfreeze", true);
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

        public static void HookTexture()
        {
#if MYDEBUG
            try
            {
#endif
            //加载冰矛贴图
            Futile.atlasManager.LoadAtlas("atlases/icespear");
            Futile.atlasManager.LoadAtlas("atlases/icespear2");
            Futile.atlasManager.LoadAtlas("atlases/icespear3");
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
            //冰矛存档读取
            On.AbstractPhysicalObject.Realize += AbstractPhysicalObject_Realize;
            //生物冰冻效果
            CreatureEffectHook();
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
            if (energy > 0)
            {
                sLeaser.sprites[0].color = Color.white;
                switch (energy)
                {
                    default:
                    case 3:
                        sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName("icespear");
                        break;
                    case 2:
                        sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName("icespear2");
                        break;
                    case 1:
                        sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName("icespear3");
                        break;
                }
            }
            else
            {
                sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName("SmallSpear");
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
            base.Update(eu);
            if (energy > 0)
            {
                if (lightSource == null)
                {

                    var ls = lightSource = new LightSource(firstChunk.pos, false, new Color(41f / 255f, 69f / 255f, 83f / 255f), null);
                    room.AddObject(ls);
                }
                if (lightSource != null)
                {
                    if (mode == Weapon.Mode.Thrown)
                    {
                        //产生雪花效果
                        Vector2 snowPos = firstChunk.pos;
                        snowPos.x -= rotation.x * 30;
                        snowPos.y -= rotation.y * 30;
                        for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
                            room.AddObject(new MySnow(snowPos, firstChunk.vel));
                    }

                    Vector2 pos = firstChunk.pos;
                    pos.x += rotation.x * 30;
                    pos.y += rotation.y * 30;
                    var ls = lightSource;
                    ls.requireUpKeep = true;
                    ls.setPos = pos;
                    switch (energy)
                    {
                        default:
                        case 3:
                            ls.setRad = 50;
                            break;
                        case 2:
                            ls.setRad = 40;
                            break;
                        case 1:
                            ls.setRad = 25;
                            break;
                    }

                    ls.stayAlive = true;
                    ls.setAlpha = 1f;
                    if (ls.slatedForDeletetion || ls.room != room)
                        lightSource = null;
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

        public static void FreezeCreature(Creature creature)
        {
#if MYDEBUG
            try
            {
#endif
            if (!GlobalVar.freezeCreature.TryGetValue(creature, out var colorRecord))
            {

                if (creature is Lizard)
                    GlobalVar.freezeCreature.Add(creature, new LizardColorRecord());
                else if (creature is Player)
                    GlobalVar.freezeCreature.Add(creature, new SlugcatColorRecord());
                else if (creature is Scavenger)
                    GlobalVar.freezeCreature.Add(creature, new ScavengerColorRecord());
                else if (creature is Vulture)
                    GlobalVar.freezeCreature.Add(creature, new VultureColorRecord());
                else if (creature is Deer)
                    GlobalVar.freezeCreature.Add(creature, new DeerColorRecord());
                else if (creature is Centipede)
                    GlobalVar.freezeCreature.Add(creature, new CentipedeColorRecord());
                else if (creature is MirosBird)
                    GlobalVar.freezeCreature.Add(creature, new MirosBirdColorRecord());
                else if (creature is JetFish)
                    GlobalVar.freezeCreature.Add(creature, new JetFishColorRecord());
                else if (creature is Yeek)
                    GlobalVar.freezeCreature.Add(creature, new YeekColorRecord());
                else if (creature is VultureGrub)
                    GlobalVar.freezeCreature.Add(creature, new VultureGrubColorRecord());
                else if (creature is BigSpider)
                    GlobalVar.freezeCreature.Add(creature, new BigSpiderColorRecord());
                else if (creature is LanternMouse)
                    GlobalVar.freezeCreature.Add(creature, new LanternMouseColorRecord());
                else if (creature is NeedleWorm)
                    GlobalVar.freezeCreature.Add(creature, new NeedleWormColorRecord());
                else if (creature is Snail)
                    GlobalVar.freezeCreature.Add(creature, new SnailColorRecord());
                else if (creature is Cicada)
                    GlobalVar.freezeCreature.Add(creature, new CicadaColorRecord());
                else if (creature is EggBug)
                    GlobalVar.freezeCreature.Add(creature, new EggBugColorRecord());
                else if (creature is Hazer)
                    GlobalVar.freezeCreature.Add(creature, new HazerColorRecord());
                else if (creature is DropBug)
                    GlobalVar.freezeCreature.Add(creature, new DropBugColorRecord());
                else if (creature is DaddyLongLegs)
                    GlobalVar.freezeCreature.Add(creature, new DaddyLongLegsColorRecord());
                else if (creature is BigEel)
                    GlobalVar.freezeCreature.Add(creature, new BigEelColorRecord());
                else if (creature is StowawayBug)
                    GlobalVar.freezeCreature.Add(creature, new StowawayBugColorRecord());
                else if (creature is Inspector)
                    GlobalVar.freezeCreature.Add(creature, new InspectorColorRecord());
                else if (creature is PoleMimic)
                    GlobalVar.freezeCreature.Add(creature, new PoleMimicColorRecord());
                else if (creature is TentaclePlant)
                    GlobalVar.freezeCreature.Add(creature, new TentaclePlantColorRecord());
                else if (creature is TubeWorm)
                    GlobalVar.freezeCreature.Add(creature, new TubeWormColorRecord());
                creature.room.PlaySound(soundID_freeze, creature.mainBodyChunk);
            }
            else
                colorRecord.OneMoreFreeze();
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

        public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            bool res = base.HitSomething(result, eu);
            if(energy > 0)
            {
                if(result.obj is Player player && res == false)
                {
                    //玩家可以防御
                    return false;
                }
                if (result.obj is Creature creature)
                {
                    //生物无法防御
                    FreezeCreature(creature);
                    energy--;
                    if (energy <= 0)
                    {
                        abstractSpear.electricCharge = 0;
                        if (lightSource != null)
                        {
                            lightSource.RemoveFromRoom();
                            lightSource = null;
                        }
                    }
                    else
                    {
                        abstractSpear.electricCharge = energy + IceSpear_ID;
                    }
                }
            }
            return res;
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
                return false;
            }
#endif
        }

        public static void AbstractPhysicalObject_Realize(On.AbstractPhysicalObject.orig_Realize orig, AbstractPhysicalObject self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.realizedObject != null)
                return;
            if (self is AbstractSpear abstractSpear)
            {
                var tempEnergy = abstractSpear.electricCharge - IceSpear_ID;
                if (tempEnergy > 0 && tempEnergy <= 3)
                {
                    MyIceSpear iceSpear;
                    self.realizedObject = iceSpear = new MyIceSpear(self, self.world);
                    iceSpear.energy = tempEnergy;
                }
            }
            orig.Invoke(self);
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

        public static void CreatureEffectHook()
        {
#if MYDEBUG
            try
            {
#endif
            //蜥蜴
            On.LizardGraphics.DrawSprites += LizardColorRecord.LizardGraphics_DrawSprites;
            On.Lizard.Update += LizardColorRecord.Lizard_Update;
            //猫崽
            On.PlayerGraphics.DrawSprites += SlugcatColorRecord.SlugcatGraphics_DrawSprites;
            On.Player.Update += SlugcatColorRecord.Slugcat_Update;
            //拾荒
            On.ScavengerGraphics.DrawSprites += ScavengerColorRecord.ScavengerGraphics_DrawSprites;
            On.Scavenger.Update += ScavengerColorRecord.Scavenger_Update;
            //秃鹫
            On.VultureGraphics.DrawSprites += VultureColorRecord.VultureGraphics_DrawSprites;
            On.Vulture.Update += VultureColorRecord.Vulture_Update;
            //雨鹿
            On.DeerGraphics.DrawSprites += DeerColorRecord.DeerGraphics_DrawSprites;
            On.Deer.Update += DeerColorRecord.Deer_Update;
            //蜈蚣 翅膀图像无法替换
            On.CentipedeGraphics.DrawSprites += CentipedeColorRecord.CentipedeGraphics_DrawSprites;
            On.Centipede.Update += CentipedeColorRecord.Centipede_Update;
            //钢鸟
            On.MirosBirdGraphics.DrawSprites += MirosBirdColorRecord.MirosBirdGraphics_DrawSprites;
            On.MirosBird.Update += MirosBirdColorRecord.MirosBird_Update;
            //蛙鱼
            On.JetFishGraphics.DrawSprites += JetFishColorRecord.JetFishGraphics_DrawSprites;
            On.JetFish.Update += JetFishColorRecord.JetFish_Update;
            //跳跳蛙
            On.MoreSlugcats.YeekGraphics.DrawSprites += YeekColorRecord.YeekGraphics_DrawSprites;
            On.MoreSlugcats.Yeek.Update += YeekColorRecord.Yeek_Update;
            //射线虫
            On.VultureGrubGraphics.DrawSprites += VultureGrubColorRecord.VultureGrubGraphics_DrawSprites;
            On.VultureGrub.Update += VultureGrubColorRecord.VultureGrub_Update;
            //狼蛛
            On.BigSpiderGraphics.DrawSprites += BigSpiderColorRecord.BigSpiderGraphics_DrawSprites;
            On.BigSpider.Update += BigSpiderColorRecord.BigSpider_Update;
            //光鼠
            On.MouseGraphics.DrawSprites += LanternMouseColorRecord.LanternMouseGraphics_DrawSprites;
            On.LanternMouse.Update += LanternMouseColorRecord.LanternMouse_Update;
            //面条蝇 翅膀图像无法替换
            On.NeedleWormGraphics.DrawSprites += NeedleWormColorRecord.NeedleWormGraphics_DrawSprites;
            On.NeedleWorm.Update += NeedleWormColorRecord.NeedleWorm_Update;
            //波动龟
            On.SnailGraphics.DrawSprites += SnailColorRecord.SnailGraphics_DrawSprites;
            On.Snail.Update += SnailColorRecord.Snail_Update;
            //蝉乌贼
            On.CicadaGraphics.DrawSprites += CicadaColorRecord.CicadaGraphics_DrawSprites;
            On.Cicada.Update += CicadaColorRecord.Cicada_Update;
            //蛋虫
            On.EggBugGraphics.DrawSprites += EggBugColorRecord.EggBugGraphics_DrawSprites;
            On.EggBug.Update += EggBugColorRecord.EggBug_Update;
            //墨鱼
            On.HazerGraphics.DrawSprites += HazerColorRecord.HazerGraphics_DrawSprites;
            On.Hazer.Update += HazerColorRecord.Hazer_Update;
            //落网虫
            On.DropBugGraphics.DrawSprites += DropBugColorRecord.DropBugGraphics_DrawSprites;
            On.DropBug.Update += DropBugColorRecord.DropBug_Update;
            //蘑菇
            On.DaddyGraphics.DrawSprites += DaddyLongLegsColorRecord.DaddyGraphics_DrawSprites;
            On.DaddyLongLegs.Update += DaddyLongLegsColorRecord.DaddyLongLegs_Update;
            //利维坦 部分图像无法替换
            On.BigEelGraphics.DrawSprites += BigEelColorRecord.BigEelGraphics_DrawSprites;
            On.BigEel.Update += BigEelColorRecord.BigEel_Update;
            //偷渡虫（藤壶）
            On.MoreSlugcats.StowawayBugGraphics.DrawSprites += StowawayBugColorRecord.StowawayBugGraphics_DrawSprites;
            On.MoreSlugcats.StowawayBug.Update += StowawayBugColorRecord.StowawayBug_Update;
            //三头鸡（监察者）
            On.MoreSlugcats.InspectorGraphics.DrawSprites += InspectorColorRecord.InspectorGraphics_DrawSprites;
            On.MoreSlugcats.Inspector.Update += InspectorColorRecord.Inspector_Update;
            //拟态草
            On.PoleMimicGraphics.DrawSprites += PoleMimicColorRecord.PoleMimicGraphics_DrawSprites;
            On.PoleMimic.Update += PoleMimicColorRecord.PoleMimic_Update;
            //红树 部分图像无法替换
            On.TentaclePlantGraphics.DrawSprites += TentaclePlantColorRecord.TentaclePlantGraphics_DrawSprites;
            On.TentaclePlant.Update += TentaclePlantColorRecord.TentaclePlant_Update;
            //管虫
            On.TubeWormGraphics.DrawSprites += TubeWormColorRecord.TubeWormGraphics_DrawSprites;
            On.TubeWorm.Update += TubeWormColorRecord.TubeWorm_Update;
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
