using Mod.Courier;
using Mod.Courier.Module;
using Mod.Courier.Save;
using Mod.Courier.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainerReborn.ui;
using UnityEngine;
using UnityEngine.UI;

namespace TrainerReborn.modules
{
    public class EightBitModule : CourierModule
    {

        public const string BOSS_OPTIONS_LOC_ID = "TRAINER_REBORN_8BIT_OPTIONS_TEXT";
        public const string BAMBOO_CREEK_BUTTON_LOC_ID = "TRAINER_REBORN_8BIT_BAMBOO_CREEK_TEXT";

        public static bool ModuleOptionScreenLoaded;
        public static SubMenuOptionScreen ModuleOptionScreen;

        SubMenuButtonInfo eightBitButton;

        public override void Load()
        {
            base.Load();

            eightBitButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(BOSS_OPTIONS_LOC_ID), OnEightBitOpen);


            void OnEightBitOpen()
            {
                Manager<UIManager>.Instance.GetView<OptionScreen>().gameObject.SetActive(false);
                OptionScreen optionScreen = Manager<UIManager>.Instance.GetView<OptionScreen>();
                if (!ModuleOptionScreenLoaded)
                {
                    ModuleOptionScreen = SubMenuOptionScreen.BuildSubMenuOptionScreen(Manager<UIManager>.Instance.GetView<OptionScreen>());
                }
                Courier.UI.ShowView(ModuleOptionScreen, EScreenLayers.PROMPT, null, false);
                optionScreen.transform.SetParent(eightBitButton.addedTo.transform.parent);
                eightBitButton.addedTo.gameObject.SetActive(false);
                Canvas.ForceUpdateCanvases();
            }
        }
    }
}
