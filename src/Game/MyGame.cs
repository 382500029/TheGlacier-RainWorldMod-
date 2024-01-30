using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGlacier2
{
    public class MyGame
    {
        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            //打开外层空间大门
            On.RegionGate.customOEGateRequirements += RegionGate_customOEGateRequirements;
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

        //解锁归乡门
        private static bool RegionGate_customOEGateRequirements(On.RegionGate.orig_customOEGateRequirements orig, RegionGate self)
        {
#if MYDEBUG
            try
            {
#endif
            Player firstRealizedPlayer = self.room.game.FirstRealizedPlayer;
            if (firstRealizedPlayer.slugcatStats.name == Plugin.YourSlugID)
                return true;
            else
                return orig.Invoke(self);
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
    }
}
