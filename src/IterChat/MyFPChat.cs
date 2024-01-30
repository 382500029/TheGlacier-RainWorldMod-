using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGlacier2
{
    public class MyFPChat
    {
        public static void Hook()
        {
            On.SSOracleBehavior.PebblesConversation.AddEvents += PebblesConversation_AddEvents;
        }

        private static void PebblesConversation_AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation pebblesConversation)
        {
            foreach (var self in pebblesConversation.owner.PlayersInRoom)
            {
                if (self.slugcatStats.name == Plugin.YourSlugID)
                {
                    pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate("You're not like them. Who made you?"), 60);
                    pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate("As far as I know, the giant structure of Dawn Prelude has collapsed over a dozen rain cycles."), 60);
                    pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate("...... How did you escape from the hijacking that was like a great escape to here?"), 60);
                    pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate("I'll give you another instinct which will be the best defense."), 60);
                    pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate("……"), 60);
                    pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate("…"), 60);
                    pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate("Just take it and then don't come back and bother me again. "), 60);
                    //解锁冰盾能力
                    GlobalVar.glacier2_iceshield_lock = false;
                    return;
                }
            }
            orig.Invoke(pebblesConversation);
        }
    }
}
