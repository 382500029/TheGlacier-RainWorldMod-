using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWCustom;
using static Player;
using UnityEngine;
using System.Diagnostics;

namespace TheGlacier2
{
    public class IcsSpearCraft
    {
        unsafe public static bool* tempBoolPtr = null;
        unsafe public static int* tempIntPtr = null;
        unsafe public static int* tempIntPtr2 = null;
        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            On.Player.SpitUpCraftedObject += Player_SpitUpCraftedObject;
            On.Player.FreeHand += Player_GrabUpdate_Edit;
            On.Player.GrabUpdate += Player_GrabUpdate;
            IL.Player.GrabUpdate += Player_GrabUpdateIL;
            On.Player.GraspsCanBeCrafted += Player_GraspsCanBeCrafted;
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

        private static void Player_SpitUpCraftedObject(On.Player.orig_SpitUpCraftedObject orig, Player self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(self);
                return;
            }
            //合成代码
            self.room.PlaySound(SoundID.Slugcat_Swallow_Item, self.mainBodyChunk);
            for (int i = 0; i < self.grasps.Length; i++)
            {
                if (self.grasps[i] == null)
                {
                    continue;
                }

                AbstractPhysicalObject abstractPhysicalObject = self.grasps[i].grabbed.abstractPhysicalObject;
                //如果不是矛 或者 是炸矛则跳过
                if (!(abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.Spear) || (abstractPhysicalObject as AbstractSpear).explosive)
                    continue;
                //跳过电矛
                if ((abstractPhysicalObject as AbstractSpear).electric)
                    continue;
                //跳过冰矛
                if (abstractPhysicalObject.realizedObject is MyIceSpear && (abstractPhysicalObject.realizedObject as MyIceSpear).energy > 0)
                    continue;
                self.ReleaseGrasp(i);
                abstractPhysicalObject.realizedObject.RemoveFromRoom();
                self.room.abstractRoom.RemoveEntity(abstractPhysicalObject);
                self.SubtractFood(1);
                AbstractSpear abstractIceSpear = new AbstractSpear(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), false);
                var MyIceSpear = new MyIceSpear(abstractIceSpear, self.room.world);
                MyIceSpear.energy = MyIceSpear.energy_max;
                MyIceSpear.abstractSpear.electricCharge = MyIceSpear.energy + MyIceSpear.IceSpear_ID;
                self.room.abstractRoom.AddEntity(MyIceSpear.abstractSpear);
                MyIceSpear.abstractSpear.RealizeInRoom();
                if (self.FreeHand() != -1)
                {
                    self.SlugcatGrab(MyIceSpear.abstractSpear.realizedObject, self.FreeHand());
                }
                return;
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

        unsafe private static int Player_GrabUpdate_Edit(On.Player.orig_FreeHand orig, Player self)
        {
            var ret = orig.Invoke(self);
            PlayerVar pv;
            if (self.slugcatStats.name == Plugin.YourSlugID)
            {
                GlobalVar.playerVar.TryGetValue(self, out pv);
                if (pv.InGrabUpdateProc)//为了修改Player_GrabUpdate里的部分
                {
                    pv.InGrabUpdateProc = false;
                    if (ModManager.MSC && (self.FreeHand() == -1 ||
                        self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Artificer || self.slugcatStats.name == Plugin.YourSlugID) &&
                        self.GraspsCanBeCrafted())
                    {
                        self.craftingObject = true;
                        *tempBoolPtr = true;
                        *tempIntPtr = -1;
                    }
                }
            }
            return ret;
        }

        private static void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            PlayerVar pv;
            if (self.slugcatStats.name == Plugin.YourSlugID)
            {
                GlobalVar.playerVar.TryGetValue(self, out pv);
                pv.InGrabUpdateProc = true;
            }
            orig.Invoke(self, eu);
        }

        private static void Player_GrabUpdateIL(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            //先定位要跳转的标签
            c.TryGotoNext(MoveType.After, i => i.MatchLdcI4(-1),
                                       i => i.MatchStloc(5),
                                       i => i.MatchLdcI4(-1),
                                       i => i.MatchStloc(6),
                                       i => i.MatchLdcI4(-1),
                                       i => i.MatchStloc(7)
                                       );
            c.Emit(OpCodes.Ldloca_S, (byte)1);
            c.Emit(OpCodes.Conv_U);
            c.Emit(OpCodes.Stsfld, typeof(IcsSpearCraft).GetField("tempBoolPtr"));
            c.Emit(OpCodes.Ldloca_S, (byte)6);
            c.Emit(OpCodes.Conv_U);
            c.Emit(OpCodes.Stsfld, typeof(IcsSpearCraft).GetField("tempIntPtr"));
            //寻找第二个位置
            c.TryGotoNext(MoveType.After, i => i.MatchLdarg(1),
                                       i => i.MatchLdcI4(0),
                                       i => i.MatchStfld(typeof(Player).GetField("eatMeat")),
                                       i => i.MatchLdarg(1),
                                       i => i.MatchLdcI4(0),
                                       i => i.MatchStfld(typeof(Player).GetField("wantToPickUp"))
                                       );
        }

        /*------------------------------------------普通矛变冰矛能力---------------------------------------------*/
        private static bool Player_GraspsCanBeCrafted(On.Player.orig_GraspsCanBeCrafted orig, Player self)
        {
            if (self.slugcatStats.name == Plugin.YourSlugID)
            {
                var array = self.grasps;
                //无饱食度不能合成
                if (self.playerState.foodInStomach == 0)
                    return false;
                //左手
                if (array[0] != null && array[0].grabbed is Spear)
                {
                    var absspear = (array[0].grabbed as Spear).abstractSpear;
                    if ((absspear.realizedObject is not MyIceSpear || (absspear.realizedObject is MyIceSpear && (absspear.realizedObject as MyIceSpear).energy == 0)) &&//不是冰矛 或者 冰矛能量为0
                        !absspear.explosive &&//不是炸矛
                        !absspear.electric &&//不是电矛
                        !(absspear.realizedObject as Spear).bugSpear)//不是虫矛
                    {
                        return true;
                    }
                }
                //右手
                if (array[0] == null && array[1] != null && array[1].grabbed is Spear)
                {
                    var absspear = (array[1].grabbed as Spear).abstractSpear;
                    if ((absspear.realizedObject is not MyIceSpear || (absspear.realizedObject is MyIceSpear && (absspear.realizedObject as MyIceSpear).energy == 0)) &&//不是冰矛 或者 冰矛能量为0
                        !absspear.explosive &&//不是炸矛
                        !absspear.electric &&//不是电矛
                        !(absspear.realizedObject as Spear).bugSpear)//不是虫矛
                    {
                        return true;
                    }
                }
            }
            else
                return orig.Invoke(self);
            return false;
        }
    }
}