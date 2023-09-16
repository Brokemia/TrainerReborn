using Mod.Courier;
using Mod.Courier.Module;
using Mod.Courier.Save;
using Mod.Courier.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TrainerReborn.options
{
    public class BreakablesModule : CourierModule
    {
        public const string BREAKABLE_OPTIONS_LOC_ID = "TRAINER_REBORN_BREAKABLE_OPTIONS_TEXT";
        public const string TOGGLE_RESTORE_BLOCKS_BUTTON_LOC_ID = "TRAINER_REBORN_RESTORE_BLOCKS_TEXT";


        public bool restoreBlocks = false;

        SubMenuButtonInfo breakableOptionsButton;
        ToggleButtonInfo toggleRestoreBlocksButton;

        public override void Load()
        {
            base.Load();

            breakableOptionsButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(BREAKABLE_OPTIONS_LOC_ID), null);
            toggleRestoreBlocksButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(TOGGLE_RESTORE_BLOCKS_BUTTON_LOC_ID), OnToggleRestoreBlocks, (b) => restoreBlocks);

            On.BreakableCollision.OnEnterRoom += BreakableCollision_OnEnterRoom;
        }

        void OnToggleRestoreBlocks()
        {
            restoreBlocks = !restoreBlocks;
            toggleRestoreBlocksButton.UpdateStateText();
            Console.WriteLine("Restore Blocks: " + restoreBlocks);
        }

        private void BreakableCollision_OnEnterRoom(On.BreakableCollision.orig_OnEnterRoom orig, BreakableCollision self, bool teleportedInRoom)
        {
            self.repairOnEnterRoom = restoreBlocks;
            orig(self, teleportedInRoom);
        }
    }
}
