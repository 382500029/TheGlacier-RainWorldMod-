using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace TheGlacier
{
    public class MyOption : OptionInterface
    {
        MyOption() 
        {
            //设置默认值
            //OpRadioButtonGroup_conf = config.Bind<int>("OpRadioButtonGroup_conf", 1);
            OpCheckBoxUnlockShield_conf = config.Bind<bool>("OpCheckBoxUnlockShield_conf", false);
            OpCheckBoxCancelCyanLizardAngry_conf = config.Bind<bool>("OpCheckBoxCancelCyanLizardAngry_conf", false);
            OpCheckBoxSaveIceData_conf = config.Bind<bool>("OpCheckBoxSaveIceData_conf", false);
            OpCheckBoxUnlockIceShieldNum_conf = config.Bind<bool>("OpCheckBoxUnlockIceShieldNum_conf", false);
        }

        public override void Initialize()
        {
            OpTab opTab = new OpTab(this, "Options");
            InGameTranslator inGameTranslator = Custom.rainWorld.inGameTranslator;
            this.Tabs = new OpTab[]
            {
                opTab
            };
            /*radioButtonGroup = new OpRadioButtonGroup(OpRadioButtonGroup_conf);
            radioButton1 = new OpRadioButton(new Vector2(10, 450));
            radioButton2 = new OpRadioButton(new Vector2(10, 420));
            radioButtonGroup.SetButtons(new OpRadioButton[] {
                radioButton1,
                radioButton2
            });*/
            //标题
            opTab.AddItems(new UIelement[]
            {
                new OpLabel(10f, 540f, inGameTranslator.Translate("The Glacier"), true)
                {
                    alignment = FLabelAlignment.Left
                }
            });
            //选项
            opTab.AddItems(new UIelement[]
            {
                new OpCheckBox(OpCheckBoxUnlockShield_conf, new Vector2(10, 450)),
                new OpLabel(new Vector2(50f, 450f), new Vector2(200f, 24f), inGameTranslator.Translate("Unlock IceShield ability"), FLabelAlignment.Left, false, null),
                new OpCheckBox(OpCheckBoxCancelCyanLizardAngry_conf, new Vector2(10, 420)),
                new OpLabel(new Vector2(50f, 420f), new Vector2(200f, 24f), inGameTranslator.Translate("Cancel CyanLizard hate"), FLabelAlignment.Left, false, null),
                new OpCheckBox(OpCheckBoxSaveIceData_conf, new Vector2(10, 390)),
                new OpLabel(new Vector2(50f, 390f), new Vector2(200f, 24f), inGameTranslator.Translate("Save Ice data to the next cycle(Save bug not fixed yet)"), FLabelAlignment.Left, false, null),
                new OpCheckBox(OpCheckBoxUnlockIceShieldNum_conf, new Vector2(10, 360)),
                new OpLabel(new Vector2(50f, 360f), new Vector2(200f, 24f), inGameTranslator.Translate("Unlock the maximum number of ice shields"), FLabelAlignment.Left, false, null),
            //new OpLabel(new Vector2(50f, 420f), new Vector2(200f, 24f), inGameTranslator.Translate("If scavenger dies, the players continue playing"), FLabelAlignment.Left, false, null),
            /*radioButtonGroup,
            radioButton1,
            radioButton2*/
        });
        }

        public static readonly MyOption Instance = new MyOption();

        /*public readonly Configurable<int> OpRadioButtonGroup_conf;
        private OpRadioButtonGroup radioButtonGroup;
        private OpRadioButton radioButton1;
        private OpRadioButton radioButton2;*/

        public readonly Configurable<bool> OpCheckBoxUnlockShield_conf;
        public readonly Configurable<bool> OpCheckBoxCancelCyanLizardAngry_conf;
        public readonly Configurable<bool> OpCheckBoxSaveIceData_conf;
        public readonly Configurable<bool> OpCheckBoxUnlockIceShieldNum_conf;
    }
}
