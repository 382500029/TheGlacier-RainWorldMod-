using IL;
using Mono.Cecil;
using MoreSlugcats;
using On;
using RWCustom;
using SlugBase.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ImprovedInput;
using TheGlacier;

namespace TheGlacier2
{
    public class MyIceShield : CosmeticSprite
    {
        Player player;
        private FContainer frontContainer;
        private FContainer backContainer;
        public float counter = 0;
        private float counter_last = 0;
        public const float counter_size = 2.25f;
        private Vector2 selfPos;
        private float selfAngle = 0;
        private float selfAngle_last = 0;
        private bool exploded = false;
        public static SoundID soundID_iceExplode;//音效
        public static SoundID soundID_iceShieldCraft;//音效
        private bool readyExplode = false;
        private int ready_count = 20;
        public const float change_distance = 50f;
        public const int readyCraftMax = 40;//合成预备时间
        public const int craftMax = 120;//合成预备时间
        public const int needFood = 3;//合成要求的食物数量
        public const int MyIceShield_max = 2;
        private bool bfSwitch = true;//前后图层切换
        private float disappear_alpha = 0.73f;//玩家死亡消失
        private float disappear_alpha_max = 0.73f;
        //初始化函数
        public MyIceShield(Player player)
        {
#if MYDEBUG
            try
            {
#endif
            this.player = player;
            frontContainer = new FContainer();
            backContainer = new FContainer();
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
            Futile.atlasManager.LoadAtlas("atlases/iceshield");
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
            soundID_iceExplode = new SoundID("SNDiceExplode", true);
            soundID_iceShieldCraft = new SoundID("SNDiceShieldCraft", true);
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
        /*---------------------------------------------------------------------冰盾能力--------------------------------------------------------------------------------------*/
        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            //注册冰盾按键
            PlayerKeybind drop = GlobalVar.iceshield_skill;
            //咬住挣脱
            On.Creature.Violence += Creature_Violence;
            //挣脱蜥蜴
            On.Lizard.Bite += Lizard_Bite;
            //挣脱蘑菇
            On.DaddyLongLegs.Eat += DaddyLongLegs_Eat;
            //挣脱蜈蚣
            On.Centipede.UpdateGrasp += Centipede_UpdateGrasp;
            //挣脱利维坦
            On.BigEel.JawsSnap += BigEel_JawsSnap;
            //挣脱红树
            On.TentaclePlant.Carry += TentaclePlant_Carry;
            //挣脱拟态草
            On.PoleMimic.Carry += PoleMimic_Carry;
            //挣脱火虫
            On.EggBug.CarryObject += EggBug_CarryObject;
            //冰盾转换
            On.Spear.HitSomething += Spear_HitSomething;
            On.ScavengerBomb.HitSomething += ScavengerBomb_HitSomething;
            //挣脱魔王秃鹫
            On.Vulture.Carry += Vulture_Carry;

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

        private static void Vulture_Carry(On.Vulture.orig_Carry orig, Vulture vulture)
        {
            if (vulture.IsKing == false)
            {
                orig.Invoke(vulture);
                return;
            }
            if(vulture.grasps == null ||
                vulture.grasps[0] == null ||
                 vulture.grasps[0].grabbedChunk == null)
            {
                orig.Invoke(vulture);
                return;
            }
            //如果被伤害的物体不是玩家则运行原程序
            if (vulture.grasps[0].grabbedChunk.owner is not Player self)
            {
                orig.Invoke(vulture);
                return;
            }
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(vulture);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
            {
                orig.Invoke(vulture);
                return;
            }
            //如果至少有一个冰盾 激活一个冰盾
            bool boot = true;
            foreach (var i in pv.iceShieldList)
            {
                //是否有冰盾有激活
                if (i.readyExplode)
                    boot = false;
            }
            if (boot)
                pv.iceShieldList[pv.iceShieldList.Count - 1].readyExplode = true;
            orig.Invoke(vulture);
            self.stun = 0;
        }

        private static void EggBug_CarryObject(On.EggBug.orig_CarryObject orig, EggBug eggBug, bool eu)
        {
            orig.Invoke(eggBug, eu);
            PhysicalObject obj = eggBug.grasps[0].grabbed;
            if(obj == null)
                return;
            
            if(obj is not Player self)
                return;
            
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
                return;
            //如果至少有一个冰盾 激活一个冰盾
            bool boot = true;
            foreach (var i in pv.iceShieldList)
            {
                //是否有冰盾有激活
                if (i.readyExplode)
                    boot = false;
            }
            if (boot)
                pv.iceShieldList[pv.iceShieldList.Count - 1].readyExplode = true;
        }

        private static bool ScavengerBomb_HitSomething(On.ScavengerBomb.orig_HitSomething orig, ScavengerBomb bomb, SharedPhysics.CollisionResult result, bool eu)
        {
            //如果被命中的不是玩家
            if (result.obj is not Player self)
                return orig.Invoke(bomb, result, eu);
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return orig.Invoke(bomb, result, eu);
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
                return orig.Invoke(bomb, result, eu);
            //如果至少有一个冰盾，则立即激活一个冰盾
            if (pv.iceShieldList.Count > 0)
            {
                foreach (var i in pv.iceShieldList)
                {
                    //是否有冰盾有激活
                    if (i.exploded == false)
                    {
                        i.IceExplode();
                        break;
                    }
                }
            }
            else
                return orig.Invoke(bomb, result, eu);
            //拾荒炸弹变石头
            var absRock = new AbstractPhysicalObject(self.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID());
            self.room.abstractRoom.AddEntity(absRock);
            absRock.RealizeInRoom();
            Rock rock = (Rock)absRock.realizedObject;
            for (int i = 0; i < rock.bodyChunks.Length; i++)
            {
                rock.bodyChunks[i].pos = bomb.bodyChunks[i].pos;
                rock.bodyChunks[i].lastPos = bomb.bodyChunks[i].lastPos;
                rock.bodyChunks[i].vel = -bomb.bodyChunks[i].vel / 2;
                rock.bodyChunks[i].rad = bomb.bodyChunks[i].rad;
            }
            bomb.Destroy();
            return false;
        }

        private static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear spear, SharedPhysics.CollisionResult result, bool eu)
        {
            //如果被命中的不是玩家
            if(result.obj is not Player self)
                return orig.Invoke(spear, result, eu);
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return orig.Invoke(spear, result, eu);
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
                return orig.Invoke(spear, result, eu);
            //如果至少有一个冰盾，则立即激活一个冰盾
            if (pv.iceShieldList.Count > 0)
            {
                foreach (var i in pv.iceShieldList)
                {
                    //是否有冰盾有激活
                    if (i.exploded == false)
                    {
                        if (spear is MyIceSpear)
                        {
                            //直接弹开
                            for (int j = 0; j < spear.bodyChunks.Length; j++)
                                spear.bodyChunks[j].vel = -spear.bodyChunks[j].vel / 2;
                            self.room.PlaySound(soundID_iceShieldCraft, self.mainBodyChunk.pos);
                            return false;
                        }
                        i.IceExplode();
                        break;
                    }
                } 
            }
            else
                return orig.Invoke(spear, result, eu);
            //炸矛和电矛转换普通矛
            if (spear is ExplosiveSpear ||
                spear is ElectricSpear)
            {
                //变成普通矛
                AbstractSpear abstractSpear = new AbstractSpear(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), false);
                self.room.abstractRoom.AddEntity(abstractSpear);
                abstractSpear.RealizeInRoom();
                Spear realSpear = (Spear)abstractSpear.realizedObject;
                for (int i = 0; i < realSpear.bodyChunks.Length; i++)
                {
                    realSpear.bodyChunks[i].pos = spear.bodyChunks[i].pos;
                    realSpear.bodyChunks[i].lastPos = spear.bodyChunks[i].lastPos;
                    realSpear.bodyChunks[i].vel = -spear.bodyChunks[i].vel / 2;
                    realSpear.bodyChunks[i].rad = spear.bodyChunks[i].rad;
                }
            }
            else
            {
                //变成冰矛
                AbstractSpear abstractIceSpear = new AbstractSpear(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), false);
                var MyIceSpear = new MyIceSpear(abstractIceSpear, self.room.world);
                MyIceSpear.energy = MyIceSpear.energy_max;
                MyIceSpear.abstractSpear.electricCharge = MyIceSpear.energy + MyIceSpear.IceSpear_ID;
                self.room.abstractRoom.AddEntity(MyIceSpear.abstractSpear);
                MyIceSpear.abstractSpear.RealizeInRoom();
                for (int i = 0; i < MyIceSpear.bodyChunks.Length; i++)
                {
                    MyIceSpear.bodyChunks[i].pos = spear.bodyChunks[i].pos;
                    MyIceSpear.bodyChunks[i].lastPos = spear.bodyChunks[i].lastPos;
                    MyIceSpear.bodyChunks[i].vel = -spear.bodyChunks[i].vel / 2;
                    MyIceSpear.bodyChunks[i].rad = spear.bodyChunks[i].rad;
                }
            }
            spear.Destroy();
            return false;
        }

        private static void Lizard_Bite(On.Lizard.orig_Bite orig, Lizard lizard, BodyChunk chunk)
        {
            orig.Invoke(lizard, chunk);
            if (chunk == null)
                return;
            if (chunk.owner is not Player self)
                return;
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
                return;
            //如果至少有一个冰盾 激活一个冰盾
            bool boot = true;
            foreach (var i in pv.iceShieldList)
            {
                //是否有冰盾有激活
                if (i.readyExplode)
                    boot = false;
            }
            if (boot)
                pv.iceShieldList[pv.iceShieldList.Count - 1].readyExplode = true;
        }

        private static void PoleMimic_Carry(On.PoleMimic.orig_Carry orig, PoleMimic poleMimic, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(poleMimic, eu);
            if (poleMimic.grasps == null ||
                poleMimic.grasps[0] == null ||
                poleMimic.grasps[0].grabbedChunk == null)
                return;
            if (poleMimic.grasps[0].grabbedChunk.owner is not Player self)
                return;
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
                return;
            //如果至少有一个冰盾 激活一个冰盾
            bool boot = true;
            foreach (var i in pv.iceShieldList)
                //是否有冰盾有激活
                if (i.readyExplode)
                    boot = false;
            if (boot)
                pv.iceShieldList[pv.iceShieldList.Count - 1].readyExplode = true;
            for (int i = 0; i < poleMimic.stickChunks.Length; i++)
                poleMimic.stickChunks[i] = null;
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

        private static void TentaclePlant_Carry(On.TentaclePlant.orig_Carry orig, TentaclePlant tentaclePlant, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(tentaclePlant, eu);
            if (tentaclePlant.grasps == null ||
                tentaclePlant.grasps[0] == null ||
                tentaclePlant.grasps[0].grabbedChunk == null)
                return;
            if (tentaclePlant.grasps[0].grabbedChunk.owner is not Player self)
                return;
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
                return;
            //如果至少有一个冰盾 激活一个冰盾
            bool boot = true;
            foreach (var i in pv.iceShieldList)
                //是否有冰盾有激活
                if (i.readyExplode)
                    boot = false;
            if (boot)
                pv.iceShieldList[pv.iceShieldList.Count - 1].readyExplode = true;
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

        private static void BigEel_JawsSnap(On.BigEel.orig_JawsSnap orig, BigEel bigEel)
        {
#if MYDEBUG
            try
            {
#endif
            bigEel.snapFrame = true;
            bigEel.room.PlaySound(SoundID.Leviathan_Bite, bigEel.mainBodyChunk);
            bigEel.room.ScreenMovement(bigEel.mainBodyChunk.pos, new Vector2(0f, 0f), 1.3f);
            for (int i = 1; i < bigEel.bodyChunks.Length; i++)
            {
                bigEel.bodyChunks[i].vel += (Custom.RNV() + Custom.DirVec(bigEel.bodyChunks[i - 1].pos, bigEel.bodyChunks[i].pos)) * Mathf.Sin(Mathf.InverseLerp(1f, 11f, i) * (float)Math.PI) * 8f;
            }

            Vector2 pos = bigEel.mainBodyChunk.pos;
            Vector2 vector = Custom.DirVec(bigEel.bodyChunks[1].pos, bigEel.mainBodyChunk.pos);
            bigEel.beakGap = 0f;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int j = 0; j < bigEel.room.physicalObjects.Length; j++)
            {
                for (int num = bigEel.room.physicalObjects[j].Count - 1; num >= 0; num--)
                {
                    if (!(bigEel.room.physicalObjects[j][num] is BigEel))
                    {
                        for (int k = 0; k < bigEel.room.physicalObjects[j][num].bodyChunks.Length; k++)
                        {
                            if (!bigEel.InBiteArea(bigEel.room.physicalObjects[j][num].bodyChunks[k].pos, bigEel.room.physicalObjects[j][num].bodyChunks[k].rad / 2f))
                            {
                                continue;
                            }

                            Vector2 b = Custom.ClosestPointOnLine(pos, pos + vector, bigEel.room.physicalObjects[j][num].bodyChunks[k].pos);
                            if (!ModManager.MSC || (!(bigEel.room.physicalObjects[j][num] is BigJellyFish) && !(bigEel.room.physicalObjects[j][num] is EnergyCell)))
                            {
                                bigEel.clampedObjects.Add(new BigEel.ClampedObject(bigEel.room.physicalObjects[j][num].bodyChunks[k], Vector2.Distance(pos, b)));
                                UnityEngine.Debug.Log("Caught: " + bigEel.room.physicalObjects[j][num].ToString());
                            }

                            if (ModManager.MSC && bigEel.room.physicalObjects[j][num] is EnergyCell)
                            {
                                (bigEel.room.physicalObjects[j][num] as EnergyCell).Explode();
                            }

                            if (bigEel.room.physicalObjects[j][num].bodyChunks[k].rad > bigEel.beakGap)
                            {
                                bigEel.beakGap = bigEel.room.physicalObjects[j][num].bodyChunks[k].rad;
                            }

                            if (bigEel.room.physicalObjects[j][num] is Creature)
                            {
                                if (bigEel.room.physicalObjects[j][num] is Player self)
                                {
                                    //如果玩家不是Glacier则运行原程序
                                    if (self.slugcatStats.name == Plugin.YourSlugID)
                                    {
                                        //取玩家变量
                                        GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
                                        //如果至少有一个冰盾 激活一个冰盾
                                        if (pv.iceShieldList.Count > 0)
                                        {
                                            foreach (var i in pv.iceShieldList)
                                                //是否有冰盾有激活
                                                if (i.exploded == false)
                                                {
                                                    i.IceExplode(bigEel);
                                                    break;
                                                }
                                            bigEel.clampedObjects.Clear();
                                            return;
                                        }
                                    }
                                    flag3 = true;
                                }
                                else
                                {
                                    flag = true;
                                }
                                 (bigEel.room.physicalObjects[j][num] as Creature).Die();
                            }
                            else
                            {
                                flag2 = true;
                            }

                            if (bigEel.graphicsModule != null)
                            {
                                if (bigEel.room.physicalObjects[j][num] is IDrawable)
                                {
                                    bigEel.graphicsModule.AddObjectToInternalContainer(bigEel.room.physicalObjects[j][num] as IDrawable, 0);
                                }
                                else if (bigEel.room.physicalObjects[j][num].graphicsModule != null)
                                {
                                    bigEel.graphicsModule.AddObjectToInternalContainer(bigEel.room.physicalObjects[j][num].graphicsModule, 0);
                                }
                            }
                        }
                    }
                }
            }

            if (flag)
            {
                bigEel.room.PlaySound(SoundID.Leviathan_Crush_NPC, bigEel.mainBodyChunk);
            }

            if (flag2)
            {
                bigEel.room.PlaySound(SoundID.Leviathan_Crush_Non_Organic_Object, bigEel.mainBodyChunk);
            }

            if (flag3)
            {
                bigEel.room.PlaySound(SoundID.Leviathan_Crush_Player, bigEel.mainBodyChunk);
            }

            for (float num2 = 20f; num2 < 100f; num2 += 1f)
            {
                if (bigEel.room.GetTile(pos + vector * num2).Solid)
                {
                    bigEel.room.PlaySound(SoundID.Leviathan_Clamper_Hit_Terrain, bigEel.mainBodyChunk.pos);
                    break;
                }
            }

            for (int l = 0; l < bigEel.clampedObjects.Count; l++)
            {
                bigEel.clampedObjects[l].chunk.owner.ChangeCollisionLayer(0);
                bigEel.Crush(bigEel.clampedObjects[l].chunk.owner);
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

        private static void Centipede_UpdateGrasp(On.Centipede.orig_UpdateGrasp orig, Centipede centipede, int g)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(centipede, g);
            if (centipede.grasps == null ||
                centipede.grasps[g] == null)
                return;
            if (centipede.grasps[g].grabbed is not Player self)
                return;
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
                return;
            //如果至少有一个冰盾 激活一个冰盾
            bool boot = true;
            foreach (var i in pv.iceShieldList)
                //是否有冰盾有激活
                if (i.readyExplode)
                    boot = false;
            if (boot)
                pv.iceShieldList[pv.iceShieldList.Count - 1].readyExplode = true;
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

        private static void DaddyLongLegs_Eat(On.DaddyLongLegs.orig_Eat orig, DaddyLongLegs daddyLongLegs, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            List<DaddyLongLegs.EatObject> removeList = new List<DaddyLongLegs.EatObject>();
            foreach (var obj in daddyLongLegs.eatObjects)
            {
                if (obj == null || obj.chunk == null)
                    continue;
                if (obj.chunk.owner is Player)
                {
                    Player self = obj.chunk.owner as Player;
                    if (self.slugcatStats.name != Plugin.YourSlugID)
                        continue;
                    //取玩家变量
                    GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
                    //如果没有冰盾
                    if (pv.iceShieldList.Count == 0)
                        continue;
                    //如果至少有一个冰盾 激活一个冰盾
                    bool boot = true;
                    foreach (var i in pv.iceShieldList)
                    {
                        //是否有冰盾有激活
                        if (i.readyExplode)
                            boot = false;
                    }
                    if (boot)
                        pv.iceShieldList[pv.iceShieldList.Count - 1].readyExplode = true;
                    removeList.Add(obj);
                }
            }
            if (removeList.Count > 0)
            {
                daddyLongLegs.eatObjects.Clear();
                foreach (var p in daddyLongLegs.tentacles)
                    p.grabChunk = null;
            }
            orig.Invoke(daddyLongLegs, eu);
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

        private static void Creature_Violence(On.Creature.orig_Violence orig, Creature creature, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
        {
#if MYDEBUG
            try
            {
#endif
            if (hitChunk == null)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //如果被伤害的物体不是玩家则运行原程序
            if (hitChunk.owner is not Player self)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }

            if (creature is Lizard)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //如果伤害类型不是 x 也运行原程序
            if (type != Creature.DamageType.Bite &&
               type != Creature.DamageType.Electric &&
               type != Creature.DamageType.Stab)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //偷渡虫情况特殊处理
            if (source != null && source.owner is StowawayBug)
            {
                //钩子伤害不处理
                if (damage < 1f)
                    orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                else
                {
                    //如果至少有一个冰盾，则立即激活一个冰盾
                    if (pv.iceShieldList.Count > 0)
                    {
                        foreach (var i in pv.iceShieldList)
                        {
                            //是否有冰盾有激活
                            if (i.exploded == false)
                            {
                                i.IceExplode();
                                break;
                            }
                        }
                    }
                    orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, 0, stunBonus);
                    self.stun = 0;
                }
                return;
            }
            //如果至少有一个冰盾 激活一个冰盾
            bool boot = true;
            foreach (var i in pv.iceShieldList)
            {
                //是否有冰盾有激活
                if (i.readyExplode)
                    boot = false;
            }
            if (boot)
                pv.iceShieldList[pv.iceShieldList.Count - 1].readyExplode = true;
            //防止玩家被咬死
            orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, 0, stunBonus);
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
        /*------------------------------------------------------------------------冰盾合成--------------------------------------------------------------------------------*/
        public static void IceShieldCraft(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            //选项设置解锁
            if (MyOption.Instance.OpCheckBoxUnlockShield_conf.Value == false)
                //技能是否锁住
                if (GlobalVar.glacier2_iceshield_lock)
                    return;
            //食物数量条件
            if (self.playerState.foodInStomach < needFood)
                return;
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            /******************************24_2_16 设置选项**********************************/
            if (MyOption.Instance.OpCheckBoxUnlockIceShieldNum_conf.Value == false)
            {
                //最多两个冰盾
                if (pv.iceShieldList.Count >= MyIceShield_max)
                    return;
            }
            //]]

            if (self.input[0].x == 0 && //不能左右移动 同时
                self.input[0].y == 0 && //不能上下移动 同时
                GlobalVar.IsPressedIceShield(self) &&//按住冰盾合成键
                !self.input[0].jmp &&//不能按住跳键 同时
                !self.input[0].pckp &&//不能拾取 同时
                !self.input[0].thrw &&//不能按住投掷键 同时
                !self.craftingObject)//不能正在合成物品
            {
                pv.iceShield_ReadyCraft--;
                //如果到了判定时间
                if (pv.iceShield_ReadyCraft <= 0)
                {
                    //预备合成冰盾
                    self.Blink(5);
                    pv.iceShield_craft--;
                    self.room.AddObject(new MySnow(self.mainBodyChunk.pos, true, GetTracePos_Snow, self));
                    if (pv.iceShield_craft <= 0)
                    {
                        //合成一个冰盾
                        pv.iceShield_craft = craftMax;
                        self.SubtractFood(3);
                        //合成
                        var iceShield = new MyIceShield(self);
                        Color color;
                        color.r = 192.0f / 255.0f;
                        color.g = 237.0f / 255.0f;
                        color.b = 249.0f / 255.0f;
                        color.a = 1.0f;
                        self.room.AddObject(new ExplosionSpikes(self.room, self.bodyChunks[1].pos, 9, 8f, 5f, 5f, 90f, color));
                        self.room.PlaySound(soundID_iceShieldCraft, self.mainBodyChunk.pos);
                        pv.iceShieldList.Add(iceShield);
                        self.room.AddObject(iceShield);
                    }
                }
            }
            else
            {
                //中断准备
                if (pv.iceShield_ReadyCraft < readyCraftMax)
                {
                    pv.iceShield_ReadyCraft += 2;
                    if (pv.iceShield_ReadyCraft > readyCraftMax)
                        pv.iceShield_ReadyCraft = readyCraftMax;
                }
                if (pv.iceShield_craft < craftMax)
                {
                    pv.iceShield_craft += 2;
                    if (pv.iceShield_craft > craftMax)
                        pv.iceShield_craft = craftMax;
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
        /*------------------------------------------------------------------------冰盾图像--------------------------------------------------------------------------------*/
        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
#if MYDEBUG
            try
            {
#endif
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("iceshield", true);
            sLeaser.sprites[0].alpha = disappear_alpha;
            var fcon = rCam.ReturnFContainer("Water");
            var bcon = rCam.ReturnFContainer("Background");
            fcon.AddChild(frontContainer);
            bcon.AddChild(backContainer);
            frontContainer.AddChild(sLeaser.sprites[0]);
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

        //绘制图像
        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
#if MYDEBUG
            try
            {
#endif
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            //如果玩家在管道则不绘制图像
            sLeaser.sprites[0].isVisible = !player.inShortcut;

            var pos = Vector2.Lerp(player.bodyChunks[1].lastPos, player.bodyChunks[1].pos, timeStacker);
            var angle = Mathf.LerpAngle(selfAngle_last, selfAngle, timeStacker);

            var counter_finally = Mathf.Lerp(counter_last, counter, timeStacker);
            float rad = counter_finally * Mathf.Deg2Rad;
            selfPos.x = pos.x + 30f * (Mathf.Sin(rad) * Mathf.Cos(angle * Mathf.Deg2Rad));
            selfPos.y = pos.y + 30f * (Mathf.Sin(rad) * Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 finallyPos = selfPos;
            finallyPos -= Vector2.Lerp(rCam.lastPos, rCam.pos, timeStacker);
            sLeaser.sprites[0].x = finallyPos.x;
            sLeaser.sprites[0].y = finallyPos.y;
            sLeaser.sprites[0].rotation = -angle;

            //切换前后容器
            if (Mathf.Cos(counter_finally * Mathf.Deg2Rad) > 0)
            {
                //前景
                if (!bfSwitch)
                {
                    //如果当前图像在后景，则切换到前进
                    bfSwitch = true;
                    backContainer.RemoveAllChildren();
                    frontContainer.AddChild(sLeaser.sprites[0]);
                }
            }
            else
            {
                //后景
                if (bfSwitch)
                {
                    //如果当前图像在前进，则切换到后景
                    bfSwitch = false;
                    frontContainer.RemoveAllChildren();
                    backContainer.AddChild(sLeaser.sprites[0]);
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

        //更新
        public override void Update(bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            //如果玩家死亡
            //从最后一个冰盾开始融化
            if (player.dead)
            {
                
                //取玩家变量
                GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);
                //如果是最后一个冰盾
                if (pv.iceShieldList.IndexOf(this) == pv.iceShieldList.Count - 1)
                {
                    if (disappear_alpha > 0)
                        disappear_alpha -= 0.01f;
                    else
                    {
                        //消失
                        for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
                            room.AddObject(new MySnow(selfPos));
                        exploded = true;
                    }
                }
            }
            else
            {
                if (disappear_alpha + 0.01f < disappear_alpha_max)
                    disappear_alpha += 0.01f;
            }
            //如果玩家在管道则不绘制图像
            if (player.inShortcut)
                return;
            base.Update(eu);
            if (!player.dead)
            {
                counter_last = counter;
                counter += counter_size;
            }
            selfAngle_last = selfAngle;
            selfAngle = -player.bodyChunks[1].Rotation.GetAngle() + 90;
            //冰盾轨道间距平分
            if (!player.dead)
                AdjustSelfAngle();
            if (readyExplode && ready_count > 0)
            {
                ready_count--;
                if (ready_count == 0)
                    IceExplode();
            }
            if (exploded)
            {
                //取玩家变量
                GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);
                pv.iceShieldList.Remove(this);
                Destroy();
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
        public void AdjustSelfAngle()
        {
#if MYDEBUG
            try
            {
#endif
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);
            float part = pv.iceShieldList.Count;
            float angle = 360 / part;
            //取上一个冰盾的counter
            var index = pv.iceShieldList.IndexOf(this);
            if (index == 0)
                return;
            float prep_counter = pv.iceShieldList[index - 1].counter;
            //当前冰盾的counter值肯定小于上一个冰盾counter
            var angleBase = Mathf.Floor(prep_counter / 360) * 360;
            counter = counter % 360 + angleBase;
            while (counter > prep_counter)
                counter -= 360;
            var dstAngle = prep_counter - angle;
            if (counter + counter_size < dstAngle)
                counter += counter_size;
            else
                counter -= counter_size / 2;
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
        public static Vector2 GetTracePos_Snow(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            return self.mainBodyChunk.pos;
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

        public void SpearChange()
        {
#if MYDEBUG
            try
            {
#endif
            //检测玩家附近投掷状态的矛
            foreach (var objL in player.room.physicalObjects)
            {
                foreach (var obj in objL)
                {
                    if (obj is Spear spear)
                    {
                        //不能是正在被摧毁的对象
                        if (spear.slatedForDeletetion)
                            continue;
                        //必须得是投掷状态的矛
                        if (spear.mode != Weapon.Mode.Thrown)
                            continue;
                        //除了自己投掷的
                        if (spear.thrownBy == player)
                            continue;
                        //矛快击中玩家时
                        List<float> dv = new List<float>();
                        for (int i = 0; i < spear.bodyChunks.Length; i++)
                            for (int j = 0; j < player.bodyChunks.Length; j++)
                                dv.Add((spear.bodyChunks[i].pos - player.bodyChunks[j].pos).sqrMagnitude);
                        var min = float.MaxValue;
                        foreach (var f in dv)
                            if (f < min)
                                min = f;
                        //矛和玩家的最短距离必须在change_distance以内
                        if (min > change_distance * change_distance)
                            continue;
                        //如果obj是矛
                        if (spear is MyIceSpear mis && mis.energy > 0)
                            continue;
                        //炸矛和电矛转换普通矛
                        else if (spear is ExplosiveSpear ||
                            spear is ElectricSpear)
                        {
                            //变成普通矛
                            AbstractSpear abstractSpear = new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), false);
                            player.room.abstractRoom.AddEntity(abstractSpear);
                            abstractSpear.RealizeInRoom();
                            Spear realSpear = (Spear)abstractSpear.realizedObject;
                            for (int i = 0; i < realSpear.bodyChunks.Length; i++)
                            {
                                realSpear.bodyChunks[i].pos = spear.bodyChunks[i].pos;
                                realSpear.bodyChunks[i].lastPos = spear.bodyChunks[i].lastPos;
                                realSpear.bodyChunks[i].vel = spear.bodyChunks[i].vel;
                                realSpear.bodyChunks[i].rad = spear.bodyChunks[i].rad;
                            }
                            IceExplode();
                            //取玩家变量
                            GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);
                            pv.iceShieldList.Remove(this);
                            Destroy();
                            spear.Destroy();
                            return;
                        }
                        else
                        {
                            //变成冰矛
                            AbstractSpear abstractIceSpear = new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), false);
                            var MyIceSpear = new MyIceSpear(abstractIceSpear, player.room.world);
                            MyIceSpear.energy = MyIceSpear.energy_max;
                            MyIceSpear.abstractSpear.electricCharge = MyIceSpear.energy + MyIceSpear.IceSpear_ID;
                            player.room.abstractRoom.AddEntity(MyIceSpear.abstractSpear);
                            MyIceSpear.abstractSpear.RealizeInRoom();
                            for (int i = 0; i < MyIceSpear.bodyChunks.Length; i++)
                            {
                                MyIceSpear.bodyChunks[i].pos = spear.bodyChunks[i].pos;
                                MyIceSpear.bodyChunks[i].lastPos = spear.bodyChunks[i].lastPos;
                                MyIceSpear.bodyChunks[i].vel = spear.bodyChunks[i].vel;
                                MyIceSpear.bodyChunks[i].rad = spear.bodyChunks[i].rad;
                            }
                            IceExplode();
                            //取玩家变量
                            GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);
                            pv.iceShieldList.Remove(this);
                            Destroy();
                            spear.Destroy();
                            return;
                        }
                    }
                    else if (obj is ScavengerBomb bomb)
                    {
                        //不能是正在被摧毁的对象
                        if (bomb.slatedForDeletetion)
                            continue;
                        //必须得是投掷状态的炸弹
                        if (bomb.mode != Weapon.Mode.Thrown)
                            continue;
                        //除了自己投掷的
                        if (bomb.thrownBy == player)
                            continue;
                        //炸弹快击中玩家时
                        List<float> dv = new List<float>();
                        for (int i = 0; i < bomb.bodyChunks.Length; i++)
                            for (int j = 0; j < player.bodyChunks.Length; j++)
                                dv.Add((bomb.bodyChunks[i].pos - player.bodyChunks[j].pos).sqrMagnitude);
                        var min = float.MaxValue;
                        foreach (var f in dv)
                            if (f < min)
                                min = f;
                        //炸弹和玩家的最短距离必须在change_distance以内
                        if (min > change_distance * change_distance)
                            continue;
                        //变成普通石头
                        var absRock = new AbstractPhysicalObject(room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, room.GetWorldCoordinate(player.mainBodyChunk.pos), room.game.GetNewID());
                        player.room.abstractRoom.AddEntity(absRock);
                        absRock.RealizeInRoom();
                        Rock rock = (Rock)absRock.realizedObject;
                        for (int i = 0; i < rock.bodyChunks.Length; i++)
                        {
                            rock.bodyChunks[i].pos = bomb.bodyChunks[i].pos;
                            rock.bodyChunks[i].lastPos = bomb.bodyChunks[i].lastPos;
                            rock.bodyChunks[i].vel = bomb.bodyChunks[i].vel;
                            rock.bodyChunks[i].rad = bomb.bodyChunks[i].rad;
                        }
                        IceExplode();
                        //取玩家变量
                        GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);
                        pv.iceShieldList.Remove(this);
                        Destroy();
                        bomb.Destroy();
                        return;
                    }
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

        //冰爆
        public void IceExplode(float range = 200)
        {
#if MYDEBUG
            try
            {
#endif
            //冻住范围内的所有生物
            if (room == null)
                return;
            foreach (var absc in room.abstractRoom.creatures)
            {
                var creature = absc.realizedCreature;
                //跳过蛞蝓猫和拾荒
                if (creature is Player ||
                    creature is Scavenger)
                    continue;
                var x1 = creature.mainBodyChunk.pos.x;
                var y1 = creature.mainBodyChunk.pos.y;
                var x2 = selfPos.x;
                var y2 = selfPos.y;
                if ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) < range * range)
                {
                    MyIceSpear.FreezeCreature(creature);
                    creature.Violence(null, null, creature.bodyChunks[1], null, Creature.DamageType.Explosion,
                        0.1f, 40);
                    creature.LoseAllGrasps();
                }

            }
            Color color;
            color.r = 192.0f / 255.0f;
            color.g = 237.0f / 255.0f;
            color.b = 249.0f / 255.0f;
            color.a = 1.0f;
            room.AddObject(new Explosion.ExplosionLight(selfPos, 320f, 1f, 3, color));
            room.AddObject(new ExplosionSpikes(room, selfPos, 9, 8f, 5f, 5f, 90f, color));
            room.AddObject(new ShockWave(selfPos, 120f, 0.045f, 4));
            room.PlaySound(soundID_iceExplode, selfPos);
            exploded = true;
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

        public void IceExplode(Creature creature)
        {
#if MYDEBUG
            try
            {
#endif
            IceExplode();
            MyIceSpear.FreezeCreature(creature);
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

        //攻击目标生物
        public void AttackCreature(Creature creature)
        {

        }
    }
}
