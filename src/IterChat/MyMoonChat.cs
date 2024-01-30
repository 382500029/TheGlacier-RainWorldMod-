

namespace TheGlacier2
{
    public class MyMoonChat
    {
        public static void Hook()
        {
            On.SLOracleBehaviorHasMark.InitateConversation += SLOracleBehaviorHasMark_InitateConversation;
        }

        private static void SLOracleBehaviorHasMark_InitateConversation(On.SLOracleBehaviorHasMark.orig_InitateConversation orig, SLOracleBehaviorHasMark sLOracleBehaviorHasMark)
        {
            foreach(var self in sLOracleBehaviorHasMark.PlayersInRoom)
            {
                if (self.slugcatStats.name == Plugin.YourSlugID)
                {
                    sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate("..."), 60);
                    sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate("Ah! Hello, little creature."), 60);
                    sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate("Where did you get your communication stamp? This may not be done by the iterator I am familiar with…"), 60);
                    sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate("or it may have lost contact with us."), 60);
                    sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate("I am no longer able to obtain information about the pearls on your cloak - they seem to have begun to decay."), 60);
                    sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate("Hurry up, little creature! There's nothing else I can give you here."), 60);
                    return;
                }
            }
            orig.Invoke(sLOracleBehaviorHasMark);
        }
    }
}
