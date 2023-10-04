using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Mod.Courier;
using Mod.Courier.Module;
using Mod.Courier.Save;
using Mod.Courier.UI;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static FightingBossHitData;
using static Mod.Courier.UI.TextEntryButtonInfo;
// using static UnityEngine.EventSystems.EventTrigger;

namespace TrainerReborn
{
    public class TrainerRebornModule : CourierModule
    {
        public const string INF_HEALTH_BUTTON_LOC_ID = "TRAINER_REBORN_INF_HEALTH_BUTTON";
        public const string INF_SHURIKEN_BUTTON_LOC_ID = "TRAINER_REBORN_INF_SHURIKEN_BUTTON";
        public const string INF_JUMP_BUTTON_LOC_ID = "TRAINER_REBORN_INF_JUMP_BUTTON";
        public const string NO_KNOCKBACK_BUTTON_LOC_ID = "TRAINER_REBORN_NO_KNOCKBACK_BUTTON";
        public const string NO_BOUNDS_BUTTON_LOC_ID = "TRAINER_REBORN_NO_BOUNDS_BUTTON";
        public const string DEBUG_POS_BUTTON_LOC_ID = "TRAINER_REBORN_DEBUG_POS_BUTTON";
        public const string DEBUG_BOSS_BUTTON_LOC_ID = "TRAINER_REBORN_DEBUG_BOSS_BUTTON";
        public const string TOGGLE_COLLISIONS_BUTTON_LOC_ID = "TRAINER_REBORN_TOGGLE_COLLISIONS_BUTTON";
        public const string SECOND_QUEST_BUTTON_LOC_ID = "TRAINER_REBORN_SECOND_QUEST_BUTTON";
        public const string REFILL_HEALTH_LOC_ID = "TRAINER_REBORN_REFILL_HEALTH_BUTTON";
        public const string REFILL_SHURIKEN_LOC_ID = "TRAINER_REBORN_REFILL_SHURIKEN_BUTTON";
        public const string RELOAD_BUTTON_LOC_ID = "TRAINER_REBORN_RELOAD_BUTTON";
        public const string FULL_RELOAD_BUTTON_LOC_ID = "TRAINER_REBORN_FULL_RELOAD_BUTTON";
        public const string SAVE_BUTTON_LOC_ID = "TRAINER_REBORN_SAVE_BUTTON";
        public const string SPEED_MULT_BUTTON_LOC_ID = "TRAINER_REBORN_SPEED_MULT_BUTTON";
        public const string DEBUG_TEXT_COLOR_BUTTON_LOC_ID = "TRAINER_REBORN_DEBUG_TEXT_COLOR_BUTTON";
        public const string SWITCH_DIMENSION_TO_8_LOC_ID = "TRAINER_REBORN_SWITCH_DIMENSION_TO_8";
        public const string SWITCH_DIMENSION_TO_16_LOC_ID = "TRAINER_REBORN_SWITCH_DIMENSION_TO_16";
        public const string TP_BUTTON_LOC_ID = "TRAINER_REBORN_TP_BUTTON";
        public const string GET_ITEM_BUTTON_LOC_ID = "TRAINER_REBORN_GET_ITEM_BUTTON";
        public const string SHOW_HITBOXES_BUTTON_LOC_ID = "TRAINER_REBORN_SHOW_HITBOXES_BUTTON";

        public const string TP_LEVEL_ENTRY_LOC_ID = "TRAINER_REBORN_TP_LEVEL_ENTRY";
        public const string TP_LOCATION_ENTRY_LOC_ID = "TRAINER_REBORN_TP_LOCATION_ENTRY";
        public const string ITEM_NAME_ENTRY_LOC_ID = "TRAINER_REBORN_ITEM_NAME_ENTRY";
        public const string ITEM_NUMBER_ENTRY_LOC_ID = "TRAINER_REBORN_ITEM_NUMBER_ENTRY";

        public const string POS_DEBUG_TEXT_LOC_ID = "TRAINER_REBORN_POS_DEBUG_TEXT";
        public const string NO_KNOCKBACK_DEBUG_TEXT_LOC_ID = "TRAINER_REBORN_NO_KNOCKBACK_DEBUG_TEXT";
        public const string CAMERA_UNLOCKED_DEBUG_TEXT_LOC_ID = "TRAINER_REBORN_CAMERA_UNLOCKED_DEBUG_TEXT";
        public const string NO_COLLISIONS_DEBUG_TEXT_LOC_ID = "TRAINER_REBORN_NO_COLLISIONS_DEBUG_TEXT";
        public const string INF_SHURIKEN_DEBUG_TEXT_LOC_ID = "TRAINER_REBORN_INF_SHURIKEN_DEBUG_TEXT";
        public const string INF_HEALTH_DEBUG_TEXT_LOC_ID = "TRAINER_REBORN_INF_HEALTH_DEBUG_TEXT";
        public const string INF_JUMP_DEBUG_TEXT_LOC_ID = "TRAINER_REBORN_INF_JUMP_DEBUG_TEXT";
        public const string SPEED_DEBUG_TEXT_LOC_ID = "TRAINER_REBORN_SPEED_DEBUG_TEXT";

        public const string RELOAD_BEHAVIOUR_LOC_ID = "TRAINER_REBORN_RELOAD_BEHAVIOUR_TEXT";
        public const string RESET_GAUNTLET_RELOAD_BUTTON_LOC_ID = "TRAINER_REBORN_RESET_GAUNTLET_RELOAD_TEXT";
        public const string RESET_BADONK_RELOAD_BUTTON_LOC_ID = "TRAINER_REBORN_RESET_BADONK_RELOAD_TEXT";

        public const string DEBUG_ROOM_TIMER_BUTTON_LOC_ID = "TRAINER_REBORN_DEBUG_ROOM_TIMER_TEXT";
        public const string DISABLE_CHECKPOINTS_BUTTON_LOC_ID = "TRAINER_REBORN_DISABLE_CHECKPOINTS_BUTTON";

        public const string DISABLE_CHECKPOINTS_TEXT_LOC_ID = "TRAINER_REBORN_DISABLE_CHECKPOINTS_TEXT";

        public const string TOGGLE_RESTORE_BLOCKS_BUTTON_LOC_ID = "TRAINER_REBORN_RESTORE_BLOCKS_BUTTON";

        public const string RESTORE_BLOCKS_TEXT_LOC_ID = "TRAINER_REBORN_RESTORE_BLOCKS_TEXT";

        public override Type ModuleSaveType => typeof(TrainerRebornSave);

        public TrainerRebornSave Save => (TrainerRebornSave)ModuleSave;

        public bool noKnockback;

        public bool noBounds;

        public bool debugPos;

        public bool debugBoss;

        public bool infJump;

        public bool infHealth;

        public bool infShuriken;

        public bool collisionsDisabled;

        public bool hitboxesShown;

        public float speedMult = 1;

        public Color debugTextColor = Color.white;

        private TextMeshProUGUI debugText8;

        private TextMeshProUGUI debugText16;

        private TextMeshProUGUI roomTimerText8;
        private TextMeshProUGUI roomTimerText16;

        private static MethodInfo get_PlayerShurikensInfo = typeof(PlayerManager).GetProperty("PlayerShurikens", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).GetGetMethod();
        private static MethodInfo get_PlayerShurikensHookInfo = typeof(TrainerRebornModule).GetMethod(nameof(PlayerManager_get_PlayerShurikens), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);


        private const int RoomTimerPrecisionCurrent = 2;
        private const int RoomTimerPrecisionPrevious = 3;
        private const string RoomTimerSeparator = " / ";
        private static readonly string DefaultLastRoomTime = "0." + new string('0', RoomTimerPrecisionPrevious);

        private string lastRoomKey;
        private string sectionStartRoomKey;
        private EBits sectionStartDimension;
        private string lastRoomTime = DefaultLastRoomTime;
        private readonly Stopwatch roomWatch = new Stopwatch();
        private readonly Stopwatch sectionWatch = new Stopwatch();

        ToggleButtonInfo infHealthButton;
        ToggleButtonInfo infShurikenButton;
        ToggleButtonInfo infJumpButton;
        ToggleButtonInfo noBoundsButton;
        ToggleButtonInfo noKnockbackButton;
        ToggleButtonInfo debugPosButton;
        ToggleButtonInfo debugBossButton;
        ToggleButtonInfo toggleCollisionsButton;
        ToggleButtonInfo secondQuestButton;
        ToggleButtonInfo showHitboxesButton;
        ToggleButtonInfo resetBadonkOnReloadButton;
        ToggleButtonInfo resetGauntletOnReloadButton;
        SubMenuButtonInfo refillHealthButton;
        SubMenuButtonInfo refillShurikenButton;
        SubMenuButtonInfo reloadButton;
        SubMenuButtonInfo saveButton;
        SubMenuButtonInfo fullReloadButton;
        SubMenuButtonInfo switchDimensionButton;
        SubMenuButtonInfo reloadBehaviourButton;
        TextEntryButtonInfo speedMultButton;
        TextEntryButtonInfo debugTextColorButton;
        TextEntryButtonInfo tpButton;
        TextEntryButtonInfo getItemButton;
        ToggleButtonInfo debugRoomTimerButton;
        ToggleButtonInfo disableCheckpointsButton;
        ToggleButtonInfo toggleRestoreBlocksButton;
        ToggleButtonInfo toggleSectionTimerButton;
        TextEntryButtonInfo sectionLimitButton;
        TextEntryButtonInfo tpRoomButton;

        private bool receivingHit;

        public bool resetBadonkOnReload = false;
        public bool resetGauntledOnReload = false;
        public bool debugRoomTimer = false;
        public bool disableCheckpoints = false;
        public bool debugSectionTimer = false;
        public int sectionCounter = 0;
        public int sectionLimit = 1;

        public string hitGameObject;
        public string hurtZoneName;
        public float hitDirection;
        public float gameObjectX;
        public float hurtZoneX;
        public HitData receiveHitHitData;

        private string lastSectionTime = DefaultLastRoomTime;

        public bool restoreBlocks = false;

        public bool startSkylandsOnManfred = true;

        public override void Load()
        {
            On.InGameHud.OnGUI += InGameHud_OnGUI;
            On.PlayerController.CanJump += PlayerController_CanJump;
            On.PlayerController.Awake += PlayerController_Awake;
            On.PlayerController.ReceiveHit += PlayerController_ReceiveHit;
            On.Hittable.Awake += Hittable_Awake;
            On.PlayerController.Update += PlayerController_Update;
#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
            new Hook(get_PlayerShurikensInfo, get_PlayerShurikensHookInfo, this);
            // Stuff that doesn't always call orig(self)
            using (new DetourContext("TrainerRebornLast")
            {
                Before = { "*" }
            })
            {
                On.PlayerController.Damage += PlayerController_Damage;
                On.RetroCamera.SnapPositionToCameraBounds += RetroCamera_SnapPositionToCameraBounds;
            }

            // Stuff that should always go first
            using (new DetourContext("TrainerRebornFirst")
            {
                After = { "*" }
            })
            {
                On.PlayerController.CancelGraplou += PlayerController_CancelGraplou;
                On.PlayerController.CancelJumpCoroutine += PlayerController_CancelJumpCoroutine;
                On.PlayerKnockbackState.StateEnter += PlayerKnockbackState_StateEnter;
                On.PlayerKnockbackState.StateExecute += PlayerKnockbackState_StateExecute;
                On.PlayerKnockbackState.StateExit += PlayerKnockbackState_StateExit;
            }

            tpButton = Courier.UI.RegisterTextEntryModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(TP_BUTTON_LOC_ID), OnEnterTeleportLevel, 17, () => Manager<LocalizationManager>.Instance.GetText(TP_LEVEL_ENTRY_LOC_ID), () => GetLevelNameFromEnum(Manager<LevelManager>.Instance?.GetCurrentLevelEnum() ?? ELevel.NONE), CharsetFlags.Letter);
            infHealthButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(INF_HEALTH_BUTTON_LOC_ID), OnInfHealth, (b) => infHealth);
            infShurikenButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(INF_SHURIKEN_BUTTON_LOC_ID), OnInfShuriken, (b) => infShuriken);
            infJumpButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(INF_JUMP_BUTTON_LOC_ID), OnInfJump, (b) => infJump);
            noKnockbackButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(NO_KNOCKBACK_BUTTON_LOC_ID), OnNoKnockback, (b) => noKnockback);
            noBoundsButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(NO_BOUNDS_BUTTON_LOC_ID), OnNoBounds, (b) => noBounds);
            toggleCollisionsButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(TOGGLE_COLLISIONS_BUTTON_LOC_ID), OnToggleCollisions, (b) => !collisionsDisabled);
            switchDimensionButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(Manager<DimensionManager>.Instance?.CurrentDimension == EBits.BITS_8 ? SWITCH_DIMENSION_TO_16_LOC_ID : SWITCH_DIMENSION_TO_8_LOC_ID), OnSwitchDimensionButton);
            getItemButton = Courier.UI.RegisterTextEntryModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(GET_ITEM_BUTTON_LOC_ID), OnEnterItemToGive, 16, () => Manager<LocalizationManager>.Instance.GetText(ITEM_NAME_ENTRY_LOC_ID), () => "", CharsetFlags.Letter);
            saveButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(SAVE_BUTTON_LOC_ID), OnSaveButton);
            reloadButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(RELOAD_BUTTON_LOC_ID), OnReloadButton);
            fullReloadButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(FULL_RELOAD_BUTTON_LOC_ID), OnFullReloadButton);
            refillHealthButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(REFILL_HEALTH_LOC_ID), OnRefillHealthButton);
            refillShurikenButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(REFILL_SHURIKEN_LOC_ID), OnRefillShurikenButton);

            showHitboxesButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(SHOW_HITBOXES_BUTTON_LOC_ID), OnShowHitboxes, (b) => hitboxesShown);
            debugPosButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(DEBUG_POS_BUTTON_LOC_ID), OnDebugPos, (b) => debugPos);
            debugBossButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(DEBUG_BOSS_BUTTON_LOC_ID), OnDebugBoss, (b) => debugBoss);
            speedMultButton = Courier.UI.RegisterTextEntryModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(SPEED_MULT_BUTTON_LOC_ID), OnEnterSpeed, 4, null, () => Manager<PlayerManager>.Instance?.Player?.RunSpeedMultiplier.ToString() ?? "" + speedMult, CharsetFlags.Number | CharsetFlags.Dot);
            secondQuestButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(SECOND_QUEST_BUTTON_LOC_ID), OnSecondQuest, (b) => Manager<ProgressionManager>.Instance.secondQuest);
            debugTextColorButton = Courier.UI.RegisterTextEntryModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(DEBUG_TEXT_COLOR_BUTTON_LOC_ID), OnEnterDebugTextColor, 7, null, () => "", CharsetFlags.Letter);

            // Disable certain features until we enter the level
            secondQuestButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;
            tpButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;
            getItemButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;
            refillHealthButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;
            refillShurikenButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;
            reloadButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;
            saveButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;
            switchDimensionButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;

            reloadBehaviourButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(RELOAD_BEHAVIOUR_LOC_ID), null);
            resetGauntletOnReloadButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(RESET_GAUNTLET_RELOAD_BUTTON_LOC_ID), OnResetGauntletOnReload, (b) => resetGauntledOnReload);
            resetBadonkOnReloadButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(RESET_BADONK_RELOAD_BUTTON_LOC_ID), OnResetBadonkOnReload, (b) => resetBadonkOnReload);

            debugRoomTimerButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(DEBUG_ROOM_TIMER_BUTTON_LOC_ID), OnDebugRoomTimer, (b) => debugRoomTimer);
            disableCheckpointsButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(DISABLE_CHECKPOINTS_BUTTON_LOC_ID), OnDisableCheckpoints, (b) => disableCheckpoints);

            toggleRestoreBlocksButton = Courier.UI.RegisterToggleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(TOGGLE_RESTORE_BLOCKS_BUTTON_LOC_ID), OnToggleRestoreBlocks, (b) => restoreBlocks);

            toggleSectionTimerButton = Courier.UI.RegisterToggleModOptionButton(() => "Show Section Debug Info", OnToggleSectionTimer, (b) => debugSectionTimer);
            sectionLimitButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Section Limit", OnSectionLimitButton,2,() => "Set Section Limit", () => "1", CharsetFlags.Number);
            tpRoomButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Teleport to Room", OnEnterTeleportRoom, 17, () => "Teleport to Room",  () => "",CharsetFlags.Letter | CharsetFlags.Dash | CharsetFlags.Number);

            On.BreakableCollision.OnEnterRoom += BreakableCollision_OnEnterRoom;
            On.LevelRoom.EnterRoom += LevelRoom_EnterRoom_RoomTimer;
            On.LevelRoom.EnterRoom += LevelRoom_EnterRoom_SectionTimer;

            On.HurtZone.HitPlayer += HurtZone_HitPlayer;
            On.PlayerController.ReceiveHit += PlayerController_ReceiveHit1;


            if (Dicts.tpDict == null)
            {
                Dicts.InitTpDict();
            }
            if (Dicts.itemDict == null)
            {
                Dicts.InitItemDict();
            }
            if (Dicts.levelDict == null)
            {
                Dicts.InitLevelDict();
            }

            // Allow player to do beginning Skylands shmup if they travel by talking to Manfred in Glacial
            On.GoToSkylandsCutscene.OnChoiceDone += StartSkylandsOnManfred;
            // Any time Skylands is initialized, decide if player should start in the shmup section
            On.ElementalSkylandsLevelInitializer.OnBeforeInitDone += RideManfredOnInit;

        }        

        private void PlayerController_ReceiveHit1(On.PlayerController.orig_ReceiveHit orig, PlayerController self, HitData hitData)
        {
            receiveHitHitData=hitData;
            orig(self, hitData);
        }

        private void HurtZone_HitPlayer(On.HurtZone.orig_HitPlayer orig, HurtZone self, GameObject collidedWith)
        {
            hitGameObject = collidedWith.name;
            gameObjectX = collidedWith.transform.position.x;
            hurtZoneName = self.name;
            hurtZoneX = self.transform.position.x;
            hitDirection = collidedWith.transform.position.x - self.transform.position.x;

            orig(self, collidedWith);
        }

        private void LevelRoom_EnterRoom_RoomTimer(On.LevelRoom.orig_EnterRoom orig, LevelRoom self, bool teleportedInRoom)
        {
            if (debugRoomTimer)
            {
                var roomKey = self.roomKey;
                if (String.IsNullOrEmpty(roomKey))
                {
                    roomWatch.Stop();
                }
                else
                {
                    if (String.IsNullOrEmpty(lastRoomKey))
                    {
                        if (lastRoomKey == roomKey)
                        {
                            roomWatch.Start(); //Resume after loading
                        }
                        else
                        {
                            lastRoomTime = FormatRoomTimer(RoomTimerPrecisionPrevious, roomWatch);
                            roomWatch.Reset(); //Restart after loading
                            roomWatch.Start();
                        }
                    }
                    else
                    {
                        lastRoomTime = FormatRoomTimer(RoomTimerPrecisionPrevious, roomWatch);
                        roomWatch.Reset();
                        roomWatch.Start();
                    }
                    lastRoomKey = roomKey;
                }

            }
            orig(self, teleportedInRoom);
        }

        static string GetLevelNameFromEnum(ELevel levelEnum)
        {
            if (Dicts.levelDict == null)
            {
                Dicts.InitLevelDict();
            }
            foreach (KeyValuePair<string, string> kvp in Dicts.levelDict)
            {
                if (levelEnum.ToString().Equals(kvp.Value))
                    return kvp.Key;
            }
            return "";
        }

        void PlayerController_Awake(On.PlayerController.orig_Awake orig, PlayerController self)
        {
            orig(self);
            self.SetRunSpeedMultiplier(speedMult);
            List<LayerMask> collisionMaskList = Manager<PlayerManager>.Instance.Player.Controller.collisionMaskList;
            // Remove collisions if they were disabled earlier
            if (collisionsDisabled)
            {
                collisionMaskList[0] = 0;
                collisionMaskList[1] = 0;
            }
        }

        void OnInfHealth()
        {
            infHealth = !infHealth;
            infHealthButton.UpdateStateText();
            Console.WriteLine("Infinite Health: " + infHealth);
        }

        void OnInfShuriken()
        {
            infShuriken = !infShuriken;
            infShurikenButton.UpdateStateText();
            Console.WriteLine("Infinite Shurikens: " + infShuriken);
        }

        void OnInfJump()
        {
            infJump = !infJump;
            infJumpButton.UpdateStateText();
            Console.WriteLine("Infinite Jumps: " + infJump);
        }

        void OnNoKnockback()
        {
            noKnockback = !noKnockback;
            noKnockbackButton.UpdateStateText();
            Console.WriteLine("No Knockback: " + noKnockback);
        }

        void OnNoBounds()
        {
            noBounds = !noBounds;
            noBoundsButton.UpdateStateText();
            Console.WriteLine("No Camera Bounds: " + noBounds);
        }

        void OnDebugPos()
        {
            debugPos = !debugPos;
            debugPosButton.UpdateStateText();
            Console.WriteLine("Position Debug Display: " + debugPos);
        }

        void OnDebugBoss()
        {
            debugBoss = !debugBoss;
            debugBossButton.UpdateStateText();
            Console.WriteLine("Boss Debug Display: " + debugBoss);
        }

        void OnShowHitboxes()
        {
            hitboxesShown = !hitboxesShown;
            showHitboxesButton.UpdateStateText();
            Console.WriteLine("Hitboxes Shown: " + hitboxesShown);
        }

        void OnReloadButton()
        {
            Console.WriteLine("Reloading to last checkpoint");

            Manager<LevelManager>.Instance.LoadToLastCheckpoint(false, false);
            if (resetGauntledOnReload)
            {
                Manager<ProgressionManager>.Instance.UnsetFlag("RuxxtinEncounter_1");
            }
            if (resetBadonkOnReload)
            {
                Manager<ProgressionManager>.Instance.UnsetFlag("RuxxtinEncounter_2");
            }
        }

        void OnResetGauntletOnReload()
        {
            resetGauntledOnReload = !resetGauntledOnReload;
            resetGauntletOnReloadButton.UpdateStateText();
            Console.WriteLine("Reset Gauntlet On Reload: " + resetGauntledOnReload);
        }

        void OnResetBadonkOnReload()
        {
            resetBadonkOnReload = !resetBadonkOnReload;
            resetBadonkOnReloadButton.UpdateStateText();
            Console.WriteLine("Reset Badonk On Reload: " + resetBadonkOnReload);
        }

        void OnRefillHealthButton()
        {
            Console.WriteLine("Refilling health");
            Manager<PlayerManager>.Instance?.Player?.Heal(int.MaxValue / 2); // A bit hacky, but how can it possibly go wrong
        }

        void OnRefillShurikenButton()
        {
            Console.WriteLine("Refilling shurikens");
            Manager<PlayerManager>.Instance.PlayerShurikens = Manager<PlayerManager>.Instance.GetMaxShuriken();
        }

        void OnToggleCollisions()
        {
            collisionsDisabled = !collisionsDisabled;
            List<LayerMask> collisionMaskList = Manager<PlayerManager>.Instance?.Player?.Controller?.collisionMaskList;
            if (collisionMaskList != null)
            {
                // Only add collisions if they are turned off
                if (!collisionsDisabled)
                {
                    collisionMaskList[0] = 4608;
                    collisionMaskList[1] = 25165824;
                }
                else if (collisionsDisabled)
                {
                    collisionMaskList[0] = 0;
                    collisionMaskList[1] = 0;
                }
            }
            toggleCollisionsButton.UpdateStateText();
            Console.WriteLine("Collisions: " + !collisionsDisabled);
        }

        void OnDebugRoomTimer()
        {
            debugRoomTimer = !debugRoomTimer;
            debugRoomTimerButton.UpdateStateText();

            if(!debugRoomTimer && roomWatch.IsRunning)
            {
                roomWatch.Stop();
            }
            if (!debugRoomTimer && sectionWatch.IsRunning)
            {
                sectionWatch.Stop();
            }

            Console.WriteLine("Room Timer Display: " + debugRoomTimer);
        }

        void OnDisableCheckpoints()
        {
            disableCheckpoints = !disableCheckpoints;
            disableCheckpointsButton.UpdateStateText();
            if(disableCheckpoints)
            {
                Manager<SaveManager>.Instance.DisableSave("TrainerReborn");
            }else
            {
                Manager<SaveManager>.Instance.EnableSave("TrainerReborn");
            }
            
            Console.WriteLine("Disable Checkpoints: " + disableCheckpoints);
        }

        void OnSaveButton()
        {
            Console.WriteLine("Instant Saving");
            if(debugRoomTimer)
            {
                sectionStartRoomKey = null;
                sectionStartDimension = EBits.NONE;
            }
            Vector3 loadedLevelPlayerPosition = new Vector2(Manager<PlayerManager>.Instance.Player.transform.position.x, Manager<PlayerManager>.Instance.Player.transform.position.y);
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.mana = Manager<PlayerManager>.Instance.PlayerShurikens; ;
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelPlayerPosition = loadedLevelPlayerPosition;
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelName = Manager<LevelManager>.Instance.CurrentSceneName;
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelDimension = Manager<DimensionManager>.Instance.CurrentDimension;
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.playerLocationSceneName = Manager<LevelManager>.Instance.CurrentSceneName;
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.playerFacingDirection = Manager<PlayerManager>.Instance.Player.LookDirection;
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.playerLocationDimension = Manager<DimensionManager>.Instance.CurrentDimension;
            Manager<SaveManager>.Instance.Save();
        }

        void OnSecondQuest()
        {
            Manager<ProgressionManager>.Instance.secondQuest = !Manager<ProgressionManager>.Instance.secondQuest;
            secondQuestButton.UpdateStateText();
            Console.WriteLine("Second Quest: " + Manager<ProgressionManager>.Instance.secondQuest);
        }

        bool OnEnterSpeed(string entry)
        {
            if (float.TryParse(entry, out speedMult))
            {
                Manager<PlayerManager>.Instance?.Player?.SetRunSpeedMultiplier(speedMult);
                Console.WriteLine("Speed Multiplier: " + speedMult);
            }
            else
            {
                Console.WriteLine("Speed Multiplier set to invalid value");
            }
            return true;
        }

        bool OnEnterDebugTextColor(string entry)
        {
            if (entry.Equals("White", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.white;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            else if (entry.Equals("Black", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.black;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            else if (entry.Equals("Red", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.red;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            else if (entry.Equals("Green", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.green;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            else if (entry.Equals("Blue", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.blue;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            else if (entry.Equals("Cyan", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.cyan;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            else if (entry.Equals("Gray", StringComparison.InvariantCultureIgnoreCase) || entry.Equals("Grey", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.gray;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            else if (entry.Equals("Magenta", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.magenta;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            else if (entry.Equals("Yellow", StringComparison.InvariantCultureIgnoreCase))
            {
                debugTextColor = Color.yellow;
                Console.WriteLine("Debug text color set to: " + debugTextColor);
            }
            if (debugText8 != null)
                debugText8.color = debugTextColor;
            if (debugText16 != null)
                debugText16.color = debugTextColor;
            return true;
        }

        // When they enter the level to load into
        bool OnEnterTeleportLevel(string level)
        {
            if (Dicts.tpDict == null)
            {
                Dicts.InitTpDict();
            }
            level = level.Replace(" ", "");
            if (Dicts.tpDict.ContainsKey(level))
            {
                TextEntryPopup locationPopup = InitTextEntryPopup(tpButton.addedTo, Manager<LocalizationManager>.Instance.GetText(TP_LOCATION_ENTRY_LOC_ID), (entry) => OnEnterTeleportLocation(level, entry), 2, null, CharsetFlags.Number | CharsetFlags.Dash);
                locationPopup.onBack += () =>
                {
                    locationPopup.gameObject.SetActive(false);
                    tpButton.textEntryPopup.gameObject.SetActive(true);
                    tpButton.textEntryPopup.StartCoroutine(tpButton.textEntryPopup.BackWhenBackButtonReleased());
                };
                tpButton.textEntryPopup.gameObject.SetActive(false);
                locationPopup.Init(string.Empty);
                locationPopup.gameObject.SetActive(true);
                locationPopup.transform.SetParent(tpButton.addedTo.transform.parent);
                tpButton.addedTo.gameObject.SetActive(false);
                Canvas.ForceUpdateCanvases();
                locationPopup.initialSelection.GetComponent<UIObjectAudioHandler>().playAudio = false;
                EventSystem.current.SetSelectedGameObject(locationPopup.initialSelection);
                locationPopup.initialSelection.GetComponent<UIObjectAudioHandler>().playAudio = true;
                return false;
            }
            Console.WriteLine("Teleport Level set to an invalid value");
            return false;
        }

        bool OnEnterTeleportLocation(string level, string location)
        {
            if (int.TryParse(location, out int tpLoc) && Dicts.tpDict[level].TryGetValue(tpLoc, out float[] loadPos))
            {
                EBits dimension = Manager<DimensionManager>.Instance.currentDimension;
                Manager<PauseManager>.Instance.Resume();
                Manager<UIManager>.Instance.GetView<OptionScreen>().Close(false);
                string levelName = level.Equals("Surf", StringComparison.InvariantCultureIgnoreCase) ? Dicts.levelDict[level] : (Dicts.levelDict[level] + "_Build");                
                Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelPlayerPosition = new Vector2(loadPos[0], loadPos[1]);
                LevelLoadingInfo levelLoadingInfo = new LevelLoadingInfo(levelName, false, true, LoadSceneMode.Single, ELevelEntranceID.NONE, dimension);              
                Console.WriteLine("Teleporting to location " + tpLoc + " in " + level);
                // Close mod options menu before TPing out
                Courier.UI.ModOptionScreen?.Close(false);
                Manager<AudioManager>.Instance.StopMusic();                            

                // If Skylands is destination, don't start on Manfred and TP normally                
                if (level.Equals("elementalskylands", StringComparison.InvariantCultureIgnoreCase))
                {
                    startSkylandsOnManfred = false;                    
                }                 

                Manager<LevelManager>.Instance.LoadLevel(levelLoadingInfo);                             
                return true;
            }
            Console.WriteLine("Teleport Location set to an invalid value");
            return false;
        }

        private void RideManfredOnInit(On.ElementalSkylandsLevelInitializer.orig_OnBeforeInitDone orig, ElementalSkylandsLevelInitializer self)
        {
            self.startOnManfred = startSkylandsOnManfred;         
            orig(self);
        }

        private void StartSkylandsOnManfred(On.GoToSkylandsCutscene.orig_OnChoiceDone orig, GoToSkylandsCutscene self, DialogChoice choice)
        {
            startSkylandsOnManfred = true;
            orig(self, choice);
        }

        void OnFullReloadButton()
        {
            EBits dimension = Manager<DimensionManager>.Instance.currentDimension;
            Manager<PauseManager>.Instance.Resume();
            Manager<UIManager>.Instance.GetView<OptionScreen>()?.Close(false);
            //Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelPlayerPosition = Managernew Vector2(loadPos[0], loadPos[1]);
            LevelLoadingInfo levelLoadingInfo = new LevelLoadingInfo(Manager<LevelManager>.Instance.CurrentSceneName, false, true, LoadSceneMode.Single, ELevelEntranceID.NONE, dimension);
            //Console.WriteLine("Teleporting to location " + tpLoc + " in " + level);
            // Close mod options menu before TPing out
            if (Courier.UI.ModOptionScreenLoaded)
            {
                Courier.UI.ModOptionScreen?.Close(false);
            }

            Manager<AudioManager>.Instance.StopMusic();
            Manager<LevelManager>.Instance.LoadLevel(levelLoadingInfo);
            if (resetGauntledOnReload)
            {
                Manager<ProgressionManager>.Instance.UnsetFlag("RuxxtinEncounter_1");
            }
            if (resetBadonkOnReload)
            {
                Manager<ProgressionManager>.Instance.UnsetFlag("RuxxtinEncounter_2");
            }
        }

        void OnSwitchDimensionButton()
        {
            Manager<DimensionManager>.Instance.SetDimension(Manager<DimensionManager>.Instance.CurrentDimension == EBits.BITS_8 ? EBits.BITS_16 : EBits.BITS_8);
            switchDimensionButton.nameTextMesh.text = switchDimensionButton.GetText();
            Console.WriteLine("Switched to " + Manager<DimensionManager>.Instance.CurrentDimension);
        }

        // When they enter the name of the item to give
        bool OnEnterItemToGive(string item)
        {
            if (Dicts.itemDict == null)
            {
                Dicts.InitItemDict();
            }
            item = item.Replace(" ", "");
            if (Dicts.itemDict.ContainsKey(item))
            {
                TextEntryPopup quantityPopup = InitTextEntryPopup(getItemButton.addedTo, Manager<LocalizationManager>.Instance.GetText(ITEM_NUMBER_ENTRY_LOC_ID), (entry) => OnEnterItemQuantity(item, entry), 4, null, CharsetFlags.Number | CharsetFlags.Dash);
                quantityPopup.onBack += () =>
                {
                    quantityPopup.gameObject.SetActive(false);
                    getItemButton.textEntryPopup.gameObject.SetActive(true);
                    getItemButton.textEntryPopup.StartCoroutine(getItemButton.textEntryPopup.BackWhenBackButtonReleased());
                };
                getItemButton.textEntryPopup.gameObject.SetActive(false);
                quantityPopup.Init(string.Empty);
                quantityPopup.gameObject.SetActive(true);
                quantityPopup.transform.SetParent(getItemButton.addedTo.transform.parent);
                getItemButton.addedTo.gameObject.SetActive(false);
                Canvas.ForceUpdateCanvases();
                quantityPopup.initialSelection.GetComponent<UIObjectAudioHandler>().playAudio = false;
                EventSystem.current.SetSelectedGameObject(quantityPopup.initialSelection);
                quantityPopup.initialSelection.GetComponent<UIObjectAudioHandler>().playAudio = true;
                return false;
            }
            Console.WriteLine("Item Name To Give set to an invalid value");
            return false;
        }

        bool OnEnterItemQuantity(string item, string number)
        {
            if (int.TryParse(number, out int quantity))
            {
                if (item.Equals("TimeShard", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (quantity >= 0)
                    {
                        Manager<InventoryManager>.Instance.CollectTimeShard(quantity);
                    }
                    else
                    {
                        Manager<InventoryManager>.Instance.SpendTimeShard(-quantity);
                    }
                }
                else
                {
                    string[] itemIDs = Dicts.itemDict[item].Split('-');
                    if (quantity >= 1)
                    {
                        if (itemIDs.Length == 1)
                        {
                            Manager<InventoryManager>.Instance.AddItem((EItems)int.Parse(itemIDs[0]), 1);
                        }
                        else
                        {
                            for (int i = int.Parse(itemIDs[0]); i <= int.Parse(itemIDs[1]); i++)
                            {
                                Manager<InventoryManager>.Instance.AddItem((EItems)i, 1);
                            }
                        }
                    }
                    if (quantity <= 0)
                    {
                        if (itemIDs.Length == 1)
                        {
                            Manager<InventoryManager>.Instance.RemoveItem((EItems)int.Parse(itemIDs[0]), 1);
                        }
                        else
                        {
                            for (int i = int.Parse(itemIDs[0]); i <= int.Parse(itemIDs[1]); i++)
                            {
                                Manager<InventoryManager>.Instance.RemoveItem((EItems)i, 1);
                            }
                        }
                    }
                }
                Console.WriteLine("Giving " + quantity + "x " + item);
                return true;
            }
            Console.WriteLine("Item Quantity set to an invalid value");
            return false;
        }

        Vector3 RetroCamera_SnapPositionToCameraBounds(On.RetroCamera.orig_SnapPositionToCameraBounds orig, RetroCamera self, Vector3 pos)
        {
            if (!noBounds)
            {
                return orig(self, pos);
            }
            return pos;
        }


        int PlayerManager_get_PlayerShurikens(Func<PlayerManager, int> orig, PlayerManager self)
        {
            if (!infShuriken)
            {
                return orig(self);
            }
            return self.GetMaxShuriken();
        }

        void PlayerController_Damage(On.PlayerController.orig_Damage orig, PlayerController self, int amount)
        {
            if (!infHealth)
            {
                orig(self, amount);
            }
        }

        void PlayerController_ReceiveHit(On.PlayerController.orig_ReceiveHit orig, PlayerController self, HitData hitData)
        {
            if (noKnockback)
            {
                int cloudstepCount = self.CloudStepCount;
                bool airJumpAvailable = self.AirJumpAvailable;
                int lookDir = self.LookDirection;
                receivingHit = true;
                orig(self, hitData);
                self.CloudStepCount = cloudstepCount;
                self.AirJumpAvailable = airJumpAvailable;
                self.SetLookDirection(lookDir);
                receivingHit = false;
                if (self.StateMachine.CurrentState is PlayerKnockbackState)
                {
                    if (self.IsCompletelyInWater())
                    {
                        self.StateMachine.SetState<PlayerInWaterState>();
                    }
                    else
                    {
                        self.StateMachine.SetState<PlayerDefaultState>();
                    }
                }
            }
            else
            {
                orig(self, hitData);
            }
        }

        void PlayerController_CancelGraplou(On.PlayerController.orig_CancelGraplou orig, PlayerController self, bool stopCharging)
        {
            if (!noKnockback || !receivingHit)
            {
                orig(self, stopCharging);
            }
        }

        void PlayerController_CancelJumpCoroutine(On.PlayerController.orig_CancelJumpCoroutine orig, PlayerController self)
        {
            if (!noKnockback || !receivingHit)
            {
                orig(self);
            }
        }

        void PlayerKnockbackState_StateEnter(On.PlayerKnockbackState.orig_StateEnter orig, PlayerKnockbackState self, StateMachine stateMachine)
        {
            if (!noKnockback || !receivingHit)
            {
                orig(self, stateMachine);
            }
        }

        void PlayerKnockbackState_StateExecute(On.PlayerKnockbackState.orig_StateExecute orig, PlayerKnockbackState self)
        {
            if (!noKnockback || !receivingHit)
            {
                orig(self);
            }
        }

        void PlayerKnockbackState_StateExit(On.PlayerKnockbackState.orig_StateExit orig, PlayerKnockbackState self)
        {
            if (!noKnockback)
            {
                orig(self);
            }
        }

        bool PlayerController_CanJump(On.PlayerController.orig_CanJump orig, PlayerController self)
        {
            if (self.IsDucking && !self.CanUnduck())
            {
                return orig(self);
            }
            if (infJump)
            {
                return true;
            }
            return orig(self);
        }

        List<Hittable> hitboxList = new List<Hittable>();

        private bool onPauseScreen()
        {
            return (Manager<UIManager>.Instance.GetView<PauseScreen>()?.gameObject.activeInHierarchy ?? false) &&
                !(Manager<UIManager>.Instance.GetView<OptionScreen>()?.gameObject.activeInHierarchy ?? false);
        }

        void PlayerController_Update(On.PlayerController.orig_Update orig, PlayerController self)
        {
            orig(self);
            // Add hacky hotkeys for reloading and saving
            if (!self.Paused || (Manager<UIManager>.Instance.GetView<PauseScreen>()?.gameObject.activeInHierarchy ?? false))
            {
                string input = Input.inputString.ToLower();
                if (input.Contains(Save.reloadKeyBinding.ToString()))
                {
                    OnReloadButton();
                }
                else if (input.Contains(Save.saveKeyBinding.ToString()))
                {
                    OnSaveButton();
                }
                else if (input.Contains(Save.fullReloadKeyBinding.ToString()))
                {
                    OnFullReloadButton();
                }
                if (input.Contains(Save.refillHealthKeyBinding.ToString()))
                {
                    OnRefillHealthButton();
                }
                if (input.Contains(Save.refillShurikenKeyBinding.ToString()))
                {
                    OnRefillShurikenButton();
                }
                if (input.Contains(Save.disableCheckpoints.ToString()))
                {
                    OnDisableCheckpoints();
                }
                if (input.Contains(Save.toggleRestoreBlocks.ToString()))
                {
                    OnToggleRestoreBlocks();
                }
                if (input.Contains(Save.toggleRoomTimer.ToString()))
                {
                    OnDebugRoomTimer();
                }
            }

            foreach (Hittable hittable in hitboxList)
            {
                try
                {
                    self.StartCoroutine(ShowHitboxesRoutine(hittable, hittable.GetComponent<BoxCollider2D>()));
                }
                catch (Exception) { }
            }
            hitboxList.Clear();
        }

        void Hittable_Awake(On.Hittable.orig_Awake orig, Hittable self)
        {
            orig(self);
            hitboxList.Add(self);
        }

        IEnumerator ShowHitboxesRoutine(Hittable hittable, BoxCollider2D collider)
        {
            Rect rect = collider.GetRect();
            LineRenderer[] lines = new LineRenderer[4];
            for (int i = 0; i < 4; i++)
            {
                GameObject lineGo = new GameObject();
                lines[i] = lineGo.AddComponent<LineRenderer>();
                lines[i].material = new Material(Shader.Find("Sprites/Default"));
                lines[i].startColor = Color.red;
                lines[i].endColor = Color.red;
                lines[i].startWidth = .1f;
                lines[i].endWidth = .1f;
            }
            // This affects the z-value
            // I tried making it larger since it's still behind certain things, but that just made it invisible
            Vector3 offset = new Vector3(0, 0, -39.7f);
            lines[0].SetPosition(0, (Vector3)rect.position + offset);
            lines[0].SetPosition(1, (Vector3)(rect.position + new Vector2(rect.width, 0)) + offset);
            lines[1].SetPosition(0, (Vector3)(rect.position + new Vector2(rect.width, 0)) + offset);
            lines[1].SetPosition(1, (Vector3)(rect.position + rect.size) + offset);
            lines[2].SetPosition(0, (Vector3)(rect.position + rect.size) + offset);
            lines[2].SetPosition(1, (Vector3)(rect.position + new Vector2(0, rect.height)) + offset);
            lines[3].SetPosition(0, (Vector3)(rect.position) + offset);
            lines[3].SetPosition(1, (Vector3)(rect.position + new Vector2(0, rect.height)) + offset);
            while (true)
            {
                if (collider.isActiveAndEnabled)
                {
                    rect = collider.GetRect();
                    lines[0].SetPosition(0, (Vector3)rect.position + offset);
                    lines[0].SetPosition(1, (Vector3)(rect.position + new Vector2(rect.width, 0)) + offset);
                    lines[1].SetPosition(0, (Vector3)(rect.position + new Vector2(rect.width, 0)) + offset);
                    lines[1].SetPosition(1, (Vector3)(rect.position + rect.size) + offset);
                    lines[2].SetPosition(0, (Vector3)(rect.position + rect.size) + offset);
                    lines[2].SetPosition(1, (Vector3)(rect.position + new Vector2(0, rect.height)) + offset);
                    lines[3].SetPosition(0, (Vector3)(rect.position) + offset);
                    lines[3].SetPosition(1, (Vector3)(rect.position + new Vector2(0, rect.height)) + offset);
                }
                foreach (LineRenderer lr in lines)
                {
                    lr.gameObject.SetActive(false);
                }
                if (hitboxesShown)
                {
                    foreach (LineRenderer lr in lines)
                    {
                        lr.gameObject.SetActive(hittable.isActiveAndEnabled);
                    }
                }

                yield return null;
            }
        }

        public void InGameHud_OnGUI(On.InGameHud.orig_OnGUI orig, InGameHud self)
        {
            orig(self);
            if (debugText8 == null)
            {
                debugText8 = UnityEngine.Object.Instantiate(self.hud_8.coinCount, self.hud_8.gameObject.transform);
                debugText16 = UnityEngine.Object.Instantiate(self.hud_16.coinCount, self.hud_16.gameObject.transform);
                debugText8.transform.Translate(0f, -110f, 0f);
                debugText16.transform.Translate(0f, -110f, 0f);
                debugText8.fontSize = 7f;
                debugText16.fontSize = 7f;
                debugText8.alignment = TextAlignmentOptions.TopRight;
                debugText16.alignment = TextAlignmentOptions.TopRight;
                debugText8.enableWordWrapping = false;
                debugText16.enableWordWrapping = false;
                debugText8.color = debugTextColor;
                debugText16.color = debugTextColor;
            }
            if(roomTimerText8 == null)
            {
                roomTimerText8 = UnityEngine.Object.Instantiate(self.hud_8.coinCount, self.hud_8.gameObject.transform);
                roomTimerText16 = UnityEngine.Object.Instantiate(self.hud_16.coinCount, self.hud_16.gameObject.transform);
                roomTimerText8.transform.Translate(-1000f, 12.5f, 0f);
                roomTimerText16.transform.Translate(-1000f, 12.5f, 0f);
                roomTimerText8.alignment = TextAlignmentOptions.Left;
                roomTimerText16.alignment = TextAlignmentOptions.Left;
                roomTimerText8.fontSize = 7f;
                roomTimerText16.fontSize = 7f;
                roomTimerText8.enableWordWrapping = false;
                roomTimerText16.enableWordWrapping = false;
                roomTimerText8.color = Color.white;
                roomTimerText16.color = Color.white;
            }
            roomTimerText8.text = roomTimerText16.text = string.Empty;
            debugText8.text = debugText16.text = string.Empty;
            UpdateDebugText();
        }

        private void AddNextToPlayerName(string debug)
        {
            roomTimerText8.text += debug;
            roomTimerText16.text += debug;
        }

        private void AddToDebug(string debug)
        {
            debugText8.text += debug;
            debugText16.text += debug;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateDebugText()
        {
            if (debugPos)
            {
                Vector2 playerPos = Manager<PlayerManager>.Instance.Player.transform.position;
                string posText = Manager<LocalizationManager>.Instance.GetText(POS_DEBUG_TEXT_LOC_ID);
                posText = posText.Replace("[posX]", playerPos.x.ToString("F1"));
                posText = posText.Replace("[posY]", playerPos.y.ToString("F1"));
                AddToDebug("\r\n" + posText);
            }
            if (noKnockback)
            {
                AddToDebug("\r\n" + Manager<LocalizationManager>.Instance.GetText(NO_KNOCKBACK_DEBUG_TEXT_LOC_ID));
            }
            if (noBounds)
            {
                AddToDebug("\r\n" + Manager<LocalizationManager>.Instance.GetText(CAMERA_UNLOCKED_DEBUG_TEXT_LOC_ID));
            }
            if (collisionsDisabled)
            {
                AddToDebug("\r\n" + Manager<LocalizationManager>.Instance.GetText(NO_COLLISIONS_DEBUG_TEXT_LOC_ID));
            }
            if (infShuriken)
            {
                AddToDebug("\r\n" + Manager<LocalizationManager>.Instance.GetText(INF_SHURIKEN_DEBUG_TEXT_LOC_ID));
            }
            if (infHealth)
            {
                AddToDebug("\r\n" + Manager<LocalizationManager>.Instance.GetText(INF_HEALTH_DEBUG_TEXT_LOC_ID));
            }
            if (infJump)
            {
                AddToDebug("\r\n" + Manager<LocalizationManager>.Instance.GetText(INF_JUMP_DEBUG_TEXT_LOC_ID));
            }
            if (Manager<PlayerManager>.Instance.Player.RunSpeedMultiplier > 1f)
            {
                string speedText = Manager<LocalizationManager>.Instance.GetText(SPEED_DEBUG_TEXT_LOC_ID);
                speedText = speedText.Replace("[Speed]", Manager<PlayerManager>.Instance.Player.RunSpeedMultiplier.ToString());
                AddToDebug("\r\n" + speedText);
            }
            if (debugBoss)
            {
                AddToDebug(GetDebugBossString());
            }
            if (disableCheckpoints)
            {
                AddToDebug("\r\n" + Manager<LocalizationManager>.Instance.GetText(DISABLE_CHECKPOINTS_TEXT_LOC_ID));
            }
            if (restoreBlocks)
            {
                AddToDebug("\r\n" + Manager<LocalizationManager>.Instance.GetText(RESTORE_BLOCKS_TEXT_LOC_ID));
            }
            if (debugSectionTimer)
            {
                AddToDebug("\r\n");
                AddToDebug("Start Room: " + SceneManager.GetActiveScene().name);
                AddToDebug("\r\n");
                AddToDebug("Start Dimension: " + sectionStartDimension);
                AddToDebug("\r\n");
                AddToDebug("Limit: " + sectionLimit);
                AddToDebug("\r\n");
                AddToDebug("Count: " + sectionCounter);
            }
            if (debugRoomTimer)
            {
                AddNextToPlayerName("\r\n");
                AddNextToPlayerName("Section (" + sectionCounter + "/" + sectionLimit + ")");
                AddNextToPlayerName("\r\n");
                AddNextToPlayerName(FormatRoomTimer(RoomTimerPrecisionCurrent, sectionWatch) + RoomTimerSeparator + lastSectionTime);
                AddNextToPlayerName("\r\n");
                AddNextToPlayerName("\r\n");
                AddNextToPlayerName("Room");
                AddNextToPlayerName("\r\n");
                AddNextToPlayerName(FormatRoomTimer(RoomTimerPrecisionCurrent, roomWatch) + RoomTimerSeparator + lastRoomTime);
                
            }
            //AddToDebug("\r\n");
            //AddToDebug("Gameobject Name: " + hitGameObject);
            //AddToDebug("\r\n");
            //AddToDebug("Gameobject Postion X: " + gameObjectX);
            //AddToDebug("\r\n");
            //AddToDebug("Hurtzone: " + hurtZoneName);
            //AddToDebug("\r\n");
            //AddToDebug("Hurtzone X: " + hurtZoneX);
            //AddToDebug("\r\n");
            //AddToDebug("Hit Direction: " + hitDirection);
            //AddToDebug("\r\n");
            //AddToDebug("Receive Hit Direction: " + receiveHitHitData.hitDirection);
            //AddToDebug("\r\n");
            //AddToDebug("Receive Hit Zone: " +receiveHitHitData.hitZone);
            //AddToDebug("Knockback State:" + Mathf.Sign(hitDirection));

        }

        private string GetDebugBossString()
        { // TODO
            string bossName = "";
            string bossState = "";
            string bossHealth = "";
            string currentSceneName = Manager<LevelManager>.Instance.CurrentSceneName;
            if (currentSceneName == "Level_02_AutumnHills_Build")
            {
                bossName = "Leaf Golem";
                if (Manager<LeafGolemFightManager>.Instance != null)
                {
                    DynData<LeafGolemBoss> bossData = new DynData<LeafGolemBoss>(Manager<LeafGolemFightManager>.Instance.bossInstance);
                    bossState = bossData.Get<StateMachine>("stateMachine").CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<LeafGolemFightManager>.Instance.bossInstance.CurrentHP.ToString() + "/" + Manager<LeafGolemFightManager>.Instance.bossInstance.maxHP;
                }
            }
            if (currentSceneName == "Level_03_ForlornTemple_Build")
            {
                bossName = "Demon King";
                if (Manager<DemonKingFightManager>.Instance != null)
                {
                    bossState = Manager<DemonKingFightManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<DemonKingFightManager>.Instance.bossInstance.CurrentHP.ToString() + "/" + Manager<DemonKingFightManager>.Instance.bossInstance.maxHP;
                }
            }
            if (currentSceneName == "Level_04_Catacombs_Build")
            {
                bossName = "Ruxxtin";
                if (Manager<NecromancerFightManager>.Instance != null)
                {
                    DynData<NecromancerBoss> bossData = new DynData<NecromancerBoss>(Manager<NecromancerFightManager>.Instance.bossInstance);
                    bossState = bossData.Get<StateMachine>("stateMachine").CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<NecromancerFightManager>.Instance.bossInstance.CurrentHP.ToString() + "/" + Manager<NecromancerFightManager>.Instance.bossInstance.maxHP;
                }
            }
            if (currentSceneName == "Level_05_A_HowlingGrotto_Build")
            {
                bossName = "Emerald Golem";
                if (Manager<EmeraldGolemFightManager>.Instance != null)
                {
                    if (Manager<EmeraldGolemFightManager>.Instance.EssenceComponent == null)
                    {
                        DynData<EmeraldGolemBoss> bossData = new DynData<EmeraldGolemBoss>(Manager<EmeraldGolemFightManager>.Instance.bossComponent);
                        bossState = Manager<EmeraldGolemFightManager>.Instance.bossComponent.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                        bossHealth = "B: " + Manager<EmeraldGolemFightManager>.Instance.bossComponent.CurrentHP + "/" + Manager<EmeraldGolemFightManager>.Instance.bossComponent.maxHP + " G: " + bossData.Get<int>("gemHP") + "/" + Manager<EmeraldGolemFightManager>.Instance.bossComponent.gemMaxHP;
                    }
                    else
                    {
                        bossState = "MovementCoroutine";
                        bossHealth = "E: " + Manager<EmeraldGolemFightManager>.Instance.EssenceComponent.CurrentHP.ToString() + "/" + Manager<EmeraldGolemFightManager>.Instance.EssenceComponent.maxHP;
                    }
                }
            }
            if (currentSceneName == "Level_07_QuillshroomMarsh_Build")
            {
                bossName = "Queen Of Quills";
                if (Manager<QueenOfQuillsFightManager>.Instance != null)
                {
                    bossState = Manager<QueenOfQuillsFightManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<QueenOfQuillsFightManager>.Instance.bossInstance.CurrentHP.ToString() + "/" + Manager<QueenOfQuillsFightManager>.Instance.bossInstance.maxHP;
                }
            }
            if (currentSceneName == "Level_08_SearingCrags_Build")
            {
                bossName = "Colos & Suses";
                if (Manager<SearingCragsBossFightManager>.Instance != null)
                {
                    DynData<ColossusesBoss> bossData = new DynData<ColossusesBoss>(Manager<SearingCragsBossFightManager>.Instance.colossusesInstance);
                    if (bossData.Get<StateMachine>("stateMachine") != null)
                    {
                        bossState = string.Concat(new object[]
                        {
                        bossState,
                        bossData.Get<StateMachine>("stateMachine").CurrentState.ToString().Split(' ', '(')[0],
                        " C: ",
                        Manager<SearingCragsBossFightManager>.Instance.colosInstance.GetCurrentState().ToString().Split(' ', '(')[0],
                        " S: ",
                        Manager<SearingCragsBossFightManager>.Instance.susesInstance.GetCurrentState().ToString().Split(' ', '(')[0]
                        });
                        if (Manager<SearingCragsBossFightManager>.Instance.colosInstance != null && Manager<SearingCragsBossFightManager>.Instance.susesInstance != null)
                        {
                            bossHealth = bossHealth + "C: " + Manager<SearingCragsBossFightManager>.Instance.colosInstance.CurrentHP + "/" + Manager<SearingCragsBossFightManager>.Instance.colosInstance.maxHP + " S: " + Manager<SearingCragsBossFightManager>.Instance.susesInstance.CurrentHP + "/" + Manager<SearingCragsBossFightManager>.Instance.susesInstance.maxHP;
                        }
                    }
                }
            }
            if (currentSceneName == "Level_10_A_TowerOfTime_Build")
            {
                bossName = "Arcane Golem";
                if (Manager<ArcaneGolemBossFightManager>.Instance != null)
                {
                    bossState = Manager<ArcaneGolemBossFightManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<ArcaneGolemBossFightManager>.Instance.bossInstance.head.CurrentHP.ToString() + "/" + Manager<ArcaneGolemBossFightManager>.Instance.bossInstance.head.maxHP;
                    bossHealth = bossHealth + "  P2: " + Manager<ArcaneGolemBossFightManager>.Instance.bossInstance.secondPhaseStartHP;
                }
            }
            if (currentSceneName == "Level_11_A_CloudRuins_Build")
            {
                bossName = "Manfred";
                if (Manager<ManfredBossfightManager>.Instance != null)
                {
                    bossState = Manager<ManfredBossfightManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<ManfredBossfightManager>.Instance.bossInstance.head.hittable.CurrentHP.ToString() + "/" + Manager<ManfredBossfightManager>.Instance.bossInstance.head.hittable.maxHP;
                }
            }
            if (currentSceneName == "Level_12_UnderWorld_Build")
            {
                bossName = "Demon General";
                if (Manager<DemonGeneralFightManager>.Instance != null)
                {
                    bossState = Manager<DemonGeneralFightManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<DemonGeneralFightManager>.Instance.bossInstance.CurrentHP.ToString() + "/" + Manager<DemonGeneralFightManager>.Instance.bossInstance.maxHP;
                }
            }
            if (currentSceneName == "Level_04_C_RiviereTurquoise_Build")
            {
                bossName = "Butterfly Matriarch";
                if (Manager<ButterflyMatriarchFightManager>.Instance != null)
                {
                    bossState = Manager<ButterflyMatriarchFightManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<ButterflyMatriarchFightManager>.Instance.bossInstance.CurrentHP.ToString() + "/" + Manager<ButterflyMatriarchFightManager>.Instance.bossInstance.maxHP;
                    bossHealth = bossHealth + "  P2: " + Manager<ButterflyMatriarchFightManager>.Instance.bossInstance.phase1MaxHP;
                    bossHealth = bossHealth + ", P3: " + Manager<ButterflyMatriarchFightManager>.Instance.bossInstance.phase2MaxHP;
                }
            }
            if (currentSceneName == "Level_09_B_ElementalSkylands_Build")
            {
                bossName = "Clockwork Concierge";
                if (Manager<ConciergeFightManager>.Instance != null)
                {
                    bossState = string.Concat(new object[]
                    {
                    bossState,
                    Manager<ConciergeFightManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0],
                    " B: ",
                    Manager<ConciergeFightManager>.Instance.bossInstance.bodyStateMachine.CurrentState.ToString().Split(' ', '(')[0],
                    " H: ",
                    Manager<ConciergeFightManager>.Instance.bossInstance.headStateMachine.CurrentState.ToString().Split(' ', '(')[0]
                    });
                    bossHealth = ((!Manager<ConciergeFightManager>.Instance.bossInstance.opened) ? (bossHealth + "H: " + Manager<ConciergeFightManager>.Instance.bossInstance.head.CurrentHP + " C: " + Manager<ConciergeFightManager>.Instance.bossInstance.bodyCanon_1.CurrentHP + "|" + Manager<ConciergeFightManager>.Instance.bossInstance.bodyCanon_2.CurrentHP + "|" + Manager<ConciergeFightManager>.Instance.bossInstance.bodyCanon_3.CurrentHP + " T: " + Manager<ConciergeFightManager>.Instance.bossInstance.sideTrap.CurrentHP) : ("H: " + Manager<ConciergeFightManager>.Instance.bossInstance.heart.CurrentHP + "/" + Manager<ConciergeFightManager>.Instance.bossInstance.heart.maxHP));
                }
            }
            if (currentSceneName == "Level_11_B_MusicBox_Build")
            {
                bossName = "Phantom";
                if (Manager<PhantomFightManager>.Instance != null)
                {
                    bossState = Manager<PhantomFightManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<PhantomFightManager>.Instance.bossInstance.hittable.CurrentHP.ToString() + "/" + Manager<PhantomFightManager>.Instance.bossInstance.hittable.maxHP;
                    bossHealth = bossHealth + "  P2: " + Manager<PhantomFightManager>.Instance.bossInstance.moveSequence_2_Threshold * Manager<PhantomFightManager>.Instance.bossInstance.hittable.maxHP;
                    bossHealth = bossHealth + ", P3: " + Manager<PhantomFightManager>.Instance.bossInstance.moveSequence_3_Threshold * Manager<PhantomFightManager>.Instance.bossInstance.hittable.maxHP;
                }
            }
            if (currentSceneName == "Level_15_Surf")
            {
                bossName = "Octo";
                if (Manager<SurfBossManager>.Instance != null)
                {
                    bossState = Manager<SurfBossManager>.Instance.bossInstance.stateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<SurfBossManager>.Instance.bossInstance.hittable.CurrentHP + "/" + Manager<PhantomFightManager>.Instance.bossInstance.hittable.maxHP;
                    bossHealth = bossHealth + "  P2: " + Manager<SurfBossManager>.Instance.bossInstance.moveSequence_2_Threshold * Manager<SurfBossManager>.Instance.bossInstance.hittable.maxHP;
                    bossHealth = bossHealth + ", P3: " + Manager<SurfBossManager>.Instance.bossInstance.moveSequence_3_Threshold * Manager<SurfBossManager>.Instance.bossInstance.hittable.maxHP;
                }
            }
            if (currentSceneName == "Level_16_Beach_Build")
            {
                bossName = "Totem";
                if (Manager<TotemBossFightManager>.Instance != null)
                {
                    bossState = Manager<TotemBossFightManager>.Instance.bossInstance.StateMachine.CurrentState.ToString().Split(' ', '(')[0];
                    bossHealth = Manager<TotemBossFightManager>.Instance.bossInstance.CurrentHp + "/" + Manager<TotemBossFightManager>.Instance.bossInstance.maxHP;
                }
            }
            if (currentSceneName == "Level_18_Volcano_Chase_Build")
            {
                bossName = "Unable to debug Punch Out Boss";
            }
            if (bossHealth == "")
            {
                return "\r\nNo Boss Found";
            }
            return "\r\n" + bossName + " HP: " + bossHealth + "\r\nState: " + bossState;
        }
        private string FormatRoomTimer(int msPrecision, Stopwatch roomWatch)
        {
            TimeSpan elapsed = roomWatch.Elapsed;

            StringBuilder sb = new StringBuilder();

            if (elapsed.TotalMinutes >= 1d)
            {
                sb.Append((int)elapsed.TotalMinutes);
                sb.Append(":");
            }

            sb.Append(elapsed.Seconds.ToString(elapsed.TotalSeconds >= 10d ? "D2" : "D"));

            if (msPrecision > 0 && msPrecision < 4)
            {
                sb.Append(".");
                if (msPrecision == 3)
                {
                    sb.Append(elapsed.Milliseconds.ToString("D3"));
                }
                else if (msPrecision == 2)
                {
                    sb.Append((elapsed.Milliseconds / 10).ToString("D2"));
                }
                else
                {
                    sb.Append(elapsed.Milliseconds / 100);
                }
            }

            return sb.ToString();
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

        void OnToggleSectionTimer()
        {
            debugSectionTimer = !debugSectionTimer;
            toggleSectionTimerButton.UpdateStateText();
            Console.WriteLine("Debug Section Timer: " + debugSectionTimer);
        }

        bool OnSectionLimitButton(string limit)
        {
            sectionLimit = int.Parse(limit);
            sectionCounter = 0;
            sectionLimitButton.UpdateStateText();

            if (sectionWatch.IsRunning)
            {
                sectionWatch.Stop();
                sectionWatch.Reset();
            }

            Console.WriteLine("Section Limit: " + sectionLimit);

            return true;
        }


        private void LevelRoom_EnterRoom_SectionTimer(On.LevelRoom.orig_EnterRoom orig, LevelRoom self, bool teleportedInRoom)
        {
            if (debugRoomTimer)
            {
                var roomKey = self.roomKey;
                var currentDimension = Manager<DimensionManager>.Instance.currentDimension;
                if (String.IsNullOrEmpty(roomKey))
                {
                    sectionWatch.Stop();
                }
                else
                {
                    if (String.IsNullOrEmpty(sectionStartRoomKey))
                    {
                        sectionStartRoomKey = roomKey;
                        sectionStartDimension = Manager<DimensionManager>.Instance.currentDimension;
                    }
                    if (String.IsNullOrEmpty(lastRoomKey))
                    {
                        if (lastRoomKey == roomKey)
                        {
                            sectionWatch.Start(); //Resume after loading
                            sectionCounter = 0;
                        }
                        else
                        {
                            lastSectionTime = FormatRoomTimer(RoomTimerPrecisionPrevious, sectionWatch);
                            sectionWatch.Reset(); //Restart after loading
                            sectionWatch.Start();
                            sectionCounter = 0;
                        }
                    }
                    else
                    {
                        if (sectionStartRoomKey == roomKey && sectionStartDimension == currentDimension)
                        {
                            lastSectionTime = FormatRoomTimer(RoomTimerPrecisionPrevious, sectionWatch);
                            sectionWatch.Reset();
                            sectionWatch.Start();
                            sectionCounter = 0;
                        } else
                        {
                            sectionCounter += 1;
                        }
                           
                        if (sectionCounter == sectionLimit)
                        {
                            sectionWatch.Stop();
                            lastSectionTime = FormatRoomTimer(RoomTimerPrecisionPrevious, sectionWatch);
                        }
                    }
                    lastRoomKey = roomKey;
                }

            }
            orig(self, teleportedInRoom);
        }

        bool OnEnterTeleportRoom(string room)
        {
            if (Dicts.tpRoomDict == null)
            {
                Dicts.InitTpRoomDict();
            }
            if (!String.IsNullOrEmpty(room))
            {
                var tpData = Dicts.tpRoomDict[room];
                var level = tpData.First;
                var loadPos = tpData.Second;
                EBits dimension = Manager<DimensionManager>.Instance.currentDimension;
                Manager<PauseManager>.Instance.Resume();
                Manager<UIManager>.Instance.GetView<OptionScreen>().Close(false);
                string levelName = level.ToString() + "_Build";
                Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelPlayerPosition = new Vector2(loadPos[0], loadPos[1]);
                LevelLoadingInfo levelLoadingInfo = new LevelLoadingInfo(levelName, false, true, LoadSceneMode.Single, ELevelEntranceID.NONE, dimension);
                // Close mod options menu before TPing out
                Courier.UI.ModOptionScreen?.Close(false);

                Manager<AudioManager>.Instance.StopMusic();
                Manager<LevelManager>.Instance.LoadLevel(levelLoadingInfo);
                return true;
            }
            Console.WriteLine("Teleport Location set to an invalid value");
            return false;
        }
    }
}
