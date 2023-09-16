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
    public class BossModule : CourierModule
    {

        public const string BOSS_OPTIONS_LOC_ID = "TRAINER_REBORN_BOSS_OPTIONS_TEXT";
        public const string RUXXTIN_PATTERN_BUTTON_LOC_ID = "TRAINER_REBORN_RUXXTIN_PATTERN_TEXT";

        SubMenuButtonInfo bossOptionsButton;
        MultipleOptionButtonInfo ruxxtinPatternButton;

        private List<int> lowRuxxPattern = new List<int>() { 1, 2, 1, 1, 0, 2 };
        private List<int> highRuxxPattern = new List<int>() { 0, 2, 1, 1, 2, 0 };
        private RuxxtinPattern activeRuxxPattern = RuxxtinPattern.ORIGINAL;

        private QueenOfQuillsRing origLeftRing;
        private QueenOfQuillsRing origRightRing;
        private QueenOfQuillsRing origLeftRingUp;
        private QueenOfQuillsRing origRightRingUp;

        private enum RuxxtinPattern
        {
            ORIGINAL = 0,
            HIGH = 1, 
            LOW = 2,
        } 

        private enum QueenLowRingPattern
        {
            ORIGINAL = 0,
            LEFT = 1, 
            RIGHT = 2,
        }

        private enum QueenHighRingPattern
        {
            ORIGINAL = 0,
            LEFT = 1,
            RIGHT = 2,
        }

        public override void Load()
        {
            base.Load();

            activeRuxxPattern = RuxxtinPattern.ORIGINAL;

            bossOptionsButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(BOSS_OPTIONS_LOC_ID), null);
            ruxxtinPatternButton = Courier.UI.RegisterMultipleModOptionButton(() => Manager<LocalizationManager>.Instance.GetText(RUXXTIN_PATTERN_BUTTON_LOC_ID), null, ChangeRuxxtinPattern, GetRuxxtinPatternIndex, GetRuxxtinPatternForIndex);

            On.NecromancerBoss.Awake += AwakeRuxxtin;
            On.QueenBoss.BeginFight += QueenBeginFight;
        }

        void ChangeRuxxtinPattern(int index)
        {
            if (index > (int)RuxxtinPattern.LOW) index = 0;
            if (index < 0) index = (int)RuxxtinPattern.LOW;
            activeRuxxPattern = (RuxxtinPattern) index;
        }

        int GetRuxxtinPatternIndex(MultipleOptionButtonInfo buttonInfo)
        {
            return (int)activeRuxxPattern;
        }

        string GetRuxxtinPatternForIndex(int index)
        {
            switch ((RuxxtinPattern)index)
            {
                case RuxxtinPattern.ORIGINAL:
                    return "Original";
                case RuxxtinPattern.HIGH:
                    return "High";
                case RuxxtinPattern.LOW:
                    return "Low";
            }
            return "???";
        }

        private void AwakeRuxxtin(On.NecromancerBoss.orig_Awake orig, NecromancerBoss self)
        {
            switch(activeRuxxPattern)
            {
                default:
                case RuxxtinPattern.ORIGINAL:
                    self.bossPattern1 = highRuxxPattern;
                    self.bossPattern2 = lowRuxxPattern;
                    break;
                case RuxxtinPattern.HIGH:
                    self.bossPattern1 = highRuxxPattern;
                    self.bossPattern2 = highRuxxPattern;
                    break;
                case RuxxtinPattern.LOW:
                    self.bossPattern1 = lowRuxxPattern;
                    self.bossPattern2 = lowRuxxPattern;
                    break;
            }
            orig(self);
        }

        private void setBackupRings(QueenBoss self)
        {
            if (!origLeftRing) origLeftRing = self.leftRing;
            if (!origLeftRingUp) origLeftRingUp = self.leftRingUp;
            if (!origRightRing) origRightRing = self.rightRing;
            if (!origRightRingUp) origRightRingUp = self.rightRingUp;
        }

        private void QueenBeginFight(On.QueenBoss.orig_BeginFight orig, QueenBoss self)
        {
            setBackupRings(self);
            orig(self);
        }
    }
}
