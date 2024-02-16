using MoreSlugcats;
using RWCustom;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TheGlacier;

namespace TheGlacier2
{
    public class MyPlayer
    {
        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
            //增强矛的伤害
            On.Spear.HitSomething += Spear_HitSomething;
            //冰矛合成
            IcsSpearCraft.Hook();
            //激怒小青
            On.ArtificialIntelligence.Update += ArtificialIntelligence_Update;
            //吃拾荒眩晕
            On.Player.EatMeatUpdate += Player_EatMeatUpdate;
            //靠近生物减速
            On.Creature.Update += Creature_Update;
            //保存数据
            On.SaveState.SaveToString += SaveState_SaveToString;
            //读取数据
            On.SaveState.LoadGame += SaveState_LoadGame;
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

        private static void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState saveState, string str, RainWorldGame game)
        {
            /******************************24_2_16 保存bug**********************************/
            if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value == false)
            {
                orig.Invoke(saveState, str, game);
                return;
            }
            //]]
            orig.Invoke(saveState, str, game);
            string[] array = Regex.Split(str, "<svA>");
            foreach (var p in array)
            {
                string[] array2 = Regex.Split(p, "<svB>");
                if (array2.Length != 0 && array2[0].Length > 0)
                {
                    if(array2[0] == GlobalVar.glacier2_iceshield_lock_savefield)
                    {
                        GlobalVar.glacier2_iceshield_lock = bool.Parse(array2[1]);
                    }
                    else if (array2[0] == GlobalVar.glacier2_iceshield_count_savefield)
                    {
                        GlobalVar.savedata_glacier2_iceshield_count = array2[1];
                        GlobalVar.enableLoadData = true;
 
                    }
                }
            }
        }

        private static string SaveState_SaveToString(On.SaveState.orig_SaveToString orig, SaveState saveState)
        {
            /******************************24_2_16 保存bug**********************************/
            if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value == false)
            {
                return orig.Invoke(saveState);
            }
            //]]
            string RemoveField(string dataText,string fieldName)
            {
                int index_start = dataText.IndexOf(fieldName);
                //直到清除字段
                while (index_start != -1)
                {
                    //清除字段数据
                    int index_end = dataText.IndexOf("<svA>", index_start) + 5;
                    dataText = dataText.Remove(index_start, index_end - index_start);
                    index_start = dataText.IndexOf(fieldName);
                }
                return dataText;
            }
            var text = orig.Invoke(saveState);
            //--------------------------------------冰盾能力解锁---------------------------------------------
            //清除原来字段
            text = RemoveField(text, GlobalVar.glacier2_iceshield_lock_savefield);
            //写入能力启用数据
            text += string.Format(CultureInfo.InvariantCulture, GlobalVar.glacier2_iceshield_lock_savefield + "<svB>{0}<svA>", GlobalVar.glacier2_iceshield_lock);
            //---------------------------------------冰盾计数------------------------------------------------
            //检查玩家队伍里是否有glacier
            List<Player> glacierList = new List<Player>();

            /******************************24_1_30 修复雨眠bug**********************************/
            if(GlobalVar.game == null || 
                GlobalVar.game.Players == null)
            {
                return orig.Invoke(saveState);
            }
            //]]

            foreach (var absc in GlobalVar.game.Players)
            {
                //24_1_30 修复雨眠bug
                if (absc == null ||
                    absc.realizedCreature == null)
                    continue;
                //]]
                Player self = absc.realizedCreature as Player;
                //如果不是glacier
                if (self.slugcatStats.name != Plugin.YourSlugID)
                    continue;
                glacierList.Add(self);
            }
            //如果没有glacier则不保存数据
            if (glacierList.Count == 0)
                return orig.Invoke(saveState);

            //保存所有glacier的冰盾数据
            string numArr = "";
            foreach (var self in glacierList) 
            {
                //取每个glaicer玩家变量
                GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
                //保存所有的冰盾数据
                numArr += string.Format("{0},", pv.iceShieldList.Count);
            }
            //去除最后一个逗号
            numArr = numArr.Substring(0, numArr.Length - 1);
            //写入glacier们的冰盾数据
            //清除原来字段
            text = RemoveField(text, GlobalVar.glacier2_iceshield_count_savefield);
            //写入冰盾数据
            text += string.Format(CultureInfo.InvariantCulture, GlobalVar.glacier2_iceshield_count_savefield + "<svB>{0}<svA>", numArr);
            return text;
        }

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(self, abstractCreature, world);
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            /*------------------------------------------------------------------剧情模式游戏变量设置------------------------------------------------------------------------------*/
            if (self.room.world.game.manager.menuSetup.startGameCondition == ProcessManager.MenuSetup.StoryGameInitCondition.New)
            {
                GlobalVar.NewGameGlobalVarSet();
            }
            if(self.room.world.game.rainWorld.ExpeditionMode)//在探险模式里开启冰盾能力
            {
                GlobalVar.glacier2_iceshield_lock = false;
            }
            if (self.room.world.game.session is ArenaGameSession)//在竞技场模式里也开启冰盾能力
            {
                GlobalVar.glacier2_iceshield_lock = false;
            }
            //赋值给全局变量供其他函数使用
            GlobalVar.game = self.room.world.game;
            /*------------------------------------------------------------------玩家变量初始化------------------------------------------------------------------------------*/
            var pv = new PlayerVar();
            GlobalVar.playerVar.Add(self, pv);
            /*------------------------------------------------------------------调试图像------------------------------------------------------------------------------*/
            pv.myDebug = new MyDebug(self);

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

        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(self, eu);
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            /******************************24_2_16 保存bug**********************************/
            if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value)
            {
                LoadMyData();
            }
            //]]
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
#if DEBUG
            //启用投掷键输出调试信息
            PutDebugMsgOnThrow(self);
#endif
            //吞炸弹爆炸
            SelfExplode(self);
            //飞行能力
            pv.flyAbility.Glacier2_Fly(self);
            /******************************24_2_16 保存bug**********************************/
            if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value)
            {
                //靠近生物减速
                SlowDownCreature(self);
            }
            //]]  
            //冰盾合成
            MyIceShield.IceShieldCraft(self);
            //披风
            if (pv.cloak != null)
                pv.cloak.Update(self, eu);
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

        private static void PutDebugMsgOnThrow(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.input[0].thrw)
            {
                UnityEngine.Debug.Log(GlobalVar.dbgstr);
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

        //增强矛的伤害
        private static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.thrownBy is Player player)
            {
                if (player.slugcatStats.name == Plugin.YourSlugID)
                {
                    if (self is MyIceSpear iceSpear)
                    {
                        var tempDmg = self.spearDamageBonus;
                        if (iceSpear.energy > 0)
                            self.spearDamageBonus *= 1.5f;
                        else
                            self.spearDamageBonus *= 1.3f;
                        var ret = orig.Invoke(self, result, eu);
                        self.spearDamageBonus = tempDmg;
                        return ret;
                    }
                    else
                    {
                        var tempDmg = self.spearDamageBonus;
                        self.spearDamageBonus *= 1.3f;
                        var ret = orig.Invoke(self, result, eu);
                        self.spearDamageBonus = tempDmg;
                        return ret;
                    }
                }
            }
            return orig.Invoke(self, result, eu);
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

        //激怒小青
        private static void ArtificialIntelligence_Update(On.ArtificialIntelligence.orig_Update orig, ArtificialIntelligence self)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(self);
            //如果设置里取消了青蜥蜴激怒
            if (MyOption.Instance.OpCheckBoxCancelCyanLizardAngry_conf.Value)
                return;
            //设置青蜥蜴负好感
            Player player;
            foreach (AbstractCreature ac in self.creature.world.game.Players)
            {
                player = ac.realizedCreature as Player;
                if (player != null && player.slugcatStats.name == Plugin.YourSlugID)
                {
                    if (self is LizardAI ai &&
                        ai.lizard.Template.type == CreatureTemplate.Type.CyanLizard &&
                        player.room == self.creature.Room.realizedRoom)
                    {
                        Lizard cyanLizard = ai.lizard;
                        cyanLizard.abstractCreature.world.game.session.creatureCommunities.
                            SetLikeOfPlayer(cyanLizard.abstractCreature.creatureTemplate.communityID,
                            cyanLizard.abstractCreature.world.RegionNumber,
                            (player.State as PlayerState).playerNumber,
                            -1.0f);
                        //self.tracker.SeeCreature(player.abstractCreature);
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

        //吞炸弹爆炸
        private static void SelfExplode(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.objectInStomach != null &&
                self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb)
            {
                self.objectInStomach.RealizeInRoom();
                (self.objectInStomach.realizedObject as ScavengerBomb).Explode(null);
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

        //吃拾荒眩晕
        private static void Player_EatMeatUpdate(On.Player.orig_EatMeatUpdate orig, Player self, int graspIndex)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(self, graspIndex);
                return;
            }
            MyEatMeatUpdate(self, graspIndex);
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
        private static void MyEatMeatUpdate(Player self, int graspIndex)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.grasps[graspIndex] == null || !(self.grasps[graspIndex].grabbed is Creature) || self.eatMeat <= 20)
            {
                return;
            }

            if (ModManager.MSC)
            {
                if ((self.grasps[graspIndex].grabbed as Creature).abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Scavenger)
                {
                    self.grasps[graspIndex].grabbed.bodyChunks[0].mass = 0.5f;
                    self.grasps[graspIndex].grabbed.bodyChunks[1].mass = 0.3f;
                    self.grasps[graspIndex].grabbed.bodyChunks[2].mass = 0.05f;
                }

                if (SlugcatStats.SlugcatCanMaul(self.SlugCatClass) && self.grasps[graspIndex].grabbed is Vulture && self.grasps[graspIndex].grabbedChunk.index == 4 && ((self.grasps[graspIndex].grabbed as Vulture).abstractCreature.state as Vulture.VultureState).mask)
                {
                    if (RainWorld.ShowLogs)
                    {
                        UnityEngine.Debug.Log("Vulture mask forced off by artificer eating head");
                    }

                    (self.grasps[graspIndex].grabbed as Vulture).DropMask(Custom.RNV());
                    self.room.PlaySound(SoundID.Slugcat_Eat_Meat_B, self.mainBodyChunk);
                    self.room.PlaySound(SoundID.Drop_Bug_Grab_Creature, self.mainBodyChunk, loop: false, 1f, 0.76f);
                    for (int num = UnityEngine.Random.Range(8, 14); num >= 0; num--)
                    {
                        self.room.AddObject(new WaterDrip(Vector2.Lerp(self.grasps[graspIndex].grabbedChunk.pos, self.mainBodyChunk.pos, UnityEngine.Random.value) + self.grasps[graspIndex].grabbedChunk.rad * Custom.RNV() * UnityEngine.Random.value, Custom.RNV() * 6f * UnityEngine.Random.value + Custom.DirVec(self.grasps[graspIndex].grabbed.firstChunk.pos, (self.mainBodyChunk.pos + (self.graphicsModule as PlayerGraphics).head.pos) / 2f) * 7f * UnityEngine.Random.value + Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * UnityEngine.Random.value * self.EffectiveRoomGravity * 7f, waterColor: false));
                    }
                }
            }

            self.standing = false;
            self.Blink(5);
            if (self.eatMeat % 5 == 0)
            {
                Vector2 vector = Custom.RNV() * 3f;
                self.mainBodyChunk.pos += vector;
                self.mainBodyChunk.vel += vector;
            }

            Vector2 vector2 = self.grasps[graspIndex].grabbedChunk.pos * self.grasps[graspIndex].grabbedChunk.mass;
            float num2 = self.grasps[graspIndex].grabbedChunk.mass;
            for (int i = 0; i < self.grasps[graspIndex].grabbed.bodyChunkConnections.Length; i++)
            {
                if (self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk1 == self.grasps[graspIndex].grabbedChunk)
                {
                    vector2 += self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk2.pos * self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk2.mass;
                    num2 += self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk2.mass;
                }
                else if (self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk2 == self.grasps[graspIndex].grabbedChunk)
                {
                    vector2 += self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk1.pos * self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk1.mass;
                    num2 += self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk1.mass;
                }
            }

            vector2 /= num2;
            self.mainBodyChunk.vel += Custom.DirVec(self.mainBodyChunk.pos, vector2) * 0.5f;
            self.bodyChunks[1].vel -= Custom.DirVec(self.mainBodyChunk.pos, vector2) * 0.6f;
            if (self.graphicsModule == null || (self.grasps[graspIndex].grabbed as Creature).State.meatLeft <= 0 || self.FoodInStomach >= self.MaxFoodInStomach)
            {
                return;
            }

            if (!Custom.DistLess(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos, self.grasps[graspIndex].grabbedChunk.rad))
            {
                (self.graphicsModule as PlayerGraphics).head.vel += Custom.DirVec(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos) * (self.grasps[graspIndex].grabbedChunk.rad - Vector2.Distance(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos));
            }
            else if (self.eatMeat % 5 == 3)
            {
                (self.graphicsModule as PlayerGraphics).head.vel += Custom.RNV() * 4f;
            }

            if (self.eatMeat > 40 && self.eatMeat % 15 == 3)
            {
                self.mainBodyChunk.pos += Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * 4f;
                self.grasps[graspIndex].grabbedChunk.vel += Custom.DirVec(vector2, self.mainBodyChunk.pos) * 0.9f / self.grasps[graspIndex].grabbedChunk.mass;
                for (int num3 = UnityEngine.Random.Range(0, 3); num3 >= 0; num3--)
                {
                    self.room.AddObject(new WaterDrip(Vector2.Lerp(self.grasps[graspIndex].grabbedChunk.pos, self.mainBodyChunk.pos, UnityEngine.Random.value) + self.grasps[graspIndex].grabbedChunk.rad * Custom.RNV() * UnityEngine.Random.value, Custom.RNV() * 6f * UnityEngine.Random.value + Custom.DirVec(vector2, (self.mainBodyChunk.pos + (self.graphicsModule as PlayerGraphics).head.pos) / 2f) * 7f * UnityEngine.Random.value + Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * UnityEngine.Random.value * self.EffectiveRoomGravity * 7f, waterColor: false));
                }

                if (self.SessionRecord != null)
                {
                    self.SessionRecord.AddEat(self.grasps[graspIndex].grabbed);
                }

                (self.grasps[graspIndex].grabbed as Creature).State.meatLeft--;
                if (ModManager.MSC && (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel || self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand) && !(self.grasps[graspIndex].grabbed is Centipede))
                {
                    if (self.grasps[graspIndex].grabbed is not Scavenger)
                    {
                        self.AddQuarterFood();
                        self.AddQuarterFood();
                    }
                    else
                        self.Stun(40);
                }
                else
                {
                    if (self.grasps[graspIndex].grabbed is not Scavenger)
                        self.AddFood(1);
                    else
                        self.Stun(40);

                }
                self.room.PlaySound(SoundID.Slugcat_Eat_Meat_B, self.mainBodyChunk);
            }
            else if (self.eatMeat % 15 == 3)
            {
                self.room.PlaySound(SoundID.Slugcat_Eat_Meat_A, self.mainBodyChunk);
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

        //靠近生物减速
        private static void SlowDownCreature(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            foreach (var absc in self.room.abstractRoom.creatures)
            {
                var creature = absc.realizedCreature;
                //跳过玩家自身
                if (creature is Player && (creature as Player).slugcatStats.name == Plugin.YourSlugID)
                    continue;
                var x1 = creature.abstractPhysicalObject.pos.x;
                var y1 = creature.abstractPhysicalObject.pos.y;
                var x2 = self.abstractPhysicalObject.pos.x;
                var y2 = self.abstractPhysicalObject.pos.y;
                if ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) < 8 * 8)
                {
                    bool slowAdd = true;
                    //玩家抓取该生物则该生物不受影响
                    for (int i = 0; i < self.grasps.Length; i++)
                    {
                        if (self.grasps[i] == null)
                            continue;
                        if (self.grasps[i].grabbed == creature)
                        {
                            //被抓住的生物不受效果影响
                            if (GlobalVar.slowdownCreature.TryGetValue(creature, out _))
                                GlobalVar.slowdownCreature.Remove(creature);
                            slowAdd = false;
                        }

                    }
                    //在效果范围内添加
                    if (slowAdd && !GlobalVar.slowdownCreature.TryGetValue(creature, out _))
                        GlobalVar.slowdownCreature.Add(creature, new SlowDownAbility());
                }
                else
                {
                    //在效果范围外移除
                    if (GlobalVar.slowdownCreature.TryGetValue(creature, out _))
                        GlobalVar.slowdownCreature.Remove(creature);
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
        private static void Creature_Update(On.Creature.orig_Update orig, Creature self, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            /******************************24_2_16 保存bug**********************************/
            if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value == false)
            {
                orig.Invoke(self, eu);
                return;
            }
            //]]
            //如果在效果列表
            if (GlobalVar.slowdownCreature.TryGetValue(self, out SlowDownAbility counter))
            {
                //减速运行
                if (counter.num < 5)
                {
                    counter.num++;
                    orig.Invoke(self, eu);
                }
                else
                {
                    counter.num = 0;
                }
            }
            else
                orig.Invoke(self, eu);
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

        public static void LoadMyData()
        {
            if (GlobalVar.enableLoadData == false)
                return;
            GlobalVar.enableLoadData = false;
            //检查玩家队伍里是否有glacier
            List<Player> glacierList = new List<Player>();
            foreach (var absc in  GlobalVar.game.Players)
            {
                Player self = absc.realizedCreature as Player;
                //如果不是glacier
                if (self.slugcatStats.name != Plugin.YourSlugID)
                    continue;
                glacierList.Add(self);
            }
            
            //如果没有glacier则不读取数据
            if (glacierList.Count == 0)
                return;
            //找不到任何冰盾数据
            if (GlobalVar.savedata_glacier2_iceshield_count.Length == 0)
                return;
            string[] myData = GlobalVar.savedata_glacier2_iceshield_count.Split(',');
            //准备冰盾数量数组
            int sum = 0;//冰盾数量总数
            List<int> saveDataCountList = new List<int>();//存档中分配的数量
            List<int> readyList = new List<int>();//实际分配的数量
            foreach (var p in myData)
            {
                int n = int.Parse(p);
                sum += n;
                saveDataCountList.Add(n);
            }
            //给每只glacier分配冰盾数据
            for (int i = 0; i < glacierList.Count; i++)
            {
                //如果存档中有的冰盾数据小于glacier玩家数量，则新glacier分配的是0
                if(i < saveDataCountList.Count)
                {
                    sum -= saveDataCountList[i];
                    readyList.Add(saveDataCountList[i]);
                }
                else
                {
                    readyList.Add(0);
                }
            }
            //剩余的冰盾如果有多，则平均分给每个glacier玩家
            int average = sum / glacierList.Count;
            int overflow = sum % glacierList.Count;
            for (int i = 0; i < readyList.Count; i++)
                readyList[i] += average;
            //多余的冰盾分给第一个glacier玩家
            readyList[0] += overflow;
            //给所有glacier实例化冰盾
            for (int i = 0; i < glacierList.Count; i++)
            {
                Player self = glacierList[i];
                //取每个glaicer玩家变量
                GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
                for (int j = 0; j < readyList[i]; j++)
                {
                    var iceShield = new MyIceShield(self);
                    pv.iceShieldList.Add(iceShield);
                    self.room.AddObject(iceShield);
                }
            }
        }
    }
}
