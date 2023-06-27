using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using IL;
using LegendAPI;
using On;
using Rewired;
using UnityEngine;

namespace Mythical
{
    public static class Skills
    {
        public static Dictionary<string, SkillInfo> SkillsDict = new Dictionary<string, SkillInfo>();

        public static void Register(SkillInfo skill)
        {
            SkillsDict[skill.ID] = skill;
        }


        public static void Awake() {

            On.StatManager.LoadData += StatManager_LoadData_AddSpellsToLoadedData;
            On.Player.InitData += Player_InitData_AddSpellsToLoadedData;

            On.Player.InitFSM += Player_InitFSM_AddSpells;
            On.GameController.Awake += delegate (On.GameController.orig_Awake orig, GameController self)
            {
                orig.Invoke(self);
                On.LootManager.ResetAvailableSkills += CatalogSkills;
            };

            On.SBCardInfoPageUI.UpdateInfoCards += (On.SBCardInfoPageUI.orig_UpdateInfoCards orig, SBCardInfoPageUI self, List<Player.SkillState> infoSkillList, SpellBookUI.SkillEquipType playerInfoSelectedType) =>

            {
                for (int i = 0; i < SpellBookUI.maxInfoCardCount; i++)
                {
                    int num = self.currentSpellIndex + i - 2;
                    if (num < 0 || num >= infoSkillList.Count)
                    {
                        self.cardArray[i].SetActive(false);
                        self.newCardMarkerArray[i].enabled = false;
                    }
                    else
                    {
                        Player.SkillState skillState = infoSkillList[num];
                        self.cardArray[i].SetActive(true);
                        self.iconArray[i].sprite = IconManager.GetSkillIcon(skillState.skillID);
                        if (Player.NewSpellManager.HasRecord(skillState.skillID, playerInfoSelectedType))
                        {
                            self.newCardMarkerArray[i].enabled = true;
                        }
                        else
                        {
                            self.newCardMarkerArray[i].enabled = false;
                        }
                    }
                }
                self.titleText.text = TextManager.GetSkillName(infoSkillList[self.currentSpellIndex].skillID);
                self.descText.text = TextManager.GetSkillDescription(infoSkillList[self.currentSpellIndex].skillID, self.isSignatureType, infoSkillList[self.currentSpellIndex].element == ElementType.Chaos);
                self.SetSkillFlagImages(infoSkillList[self.currentSpellIndex]);
            };

            On.SBElementPageUI.SetElementCounts += (On.SBElementPageUI.orig_SetElementCounts orig, SBElementPageUI self, Player p) => {
                try
                {
                    orig(self, p);
                }
                catch
                {
                    Debug.Log("Went into error loading mode, trying again");
                    foreach (SkillInfo info in SkillsDict.Values)
                    {
                        if (!Player.elementSkillDict[info.Element].Contains(info.ID))
                        {
                            Player.elementSkillDict[info.Element].Add(info.ID);
                        }

                    }
                    orig(self, p);
                }
            };

        }

        private static void StatManager_LoadData_AddSpellsToLoadedData(On.StatManager.orig_LoadData orig, string assetPath, string statID, string category, string categoryModifier) {
            orig(assetPath, statID, category, categoryModifier);
            AddSpellsToLoadedData();
        }

        private static void Player_InitData_AddSpellsToLoadedData(On.Player.orig_InitData orig, Player self) {
            orig(self);
            Debug.Log("Adding spells for Player " + self.playerID);
            AddSpellsToLoadedData(self.playerID.ToString());
        }

        private static void AddSpellsToLoadedData(string categoryModifier = "") {

            string statID = StatManager.statFieldStr;
            string category = StatManager.playerBaseCategory;

            //Null checks to make sure errors don't occur
            if (IconManager.skillIcons == null)
            {
                IconManager.skillIcons = IconManager.SkillIcons;
            }
            if (TextManager.skillInfoDict == null)
            {
                TextManager.skillInfoDict = new Dictionary<string, TextManager.SkillInfo>();
            }
            string fullCategory = category + categoryModifier;
            Dictionary<string, StatData> dictionary = StatManager.data[statID][fullCategory];
            foreach (SkillInfo skillInfo in SkillsDict.Values)
            {
                if (skillInfo.Sprite != null)
                {
                    IconManager.skillIcons[skillInfo.ID] = skillInfo.Sprite;
                }

                //Putting the skill text stuff here since idk where else it should go
                TextManager.SkillInfo skillText = new TextManager.SkillInfo();
                skillText.skillID = skillInfo.ID;
                skillText.displayName = skillInfo.DisplayName;
                skillText.description = skillInfo.Description;
                skillText.empowered = skillInfo.EnhancedDescription;
                TextManager.skillInfoDict[skillInfo.ID] = skillText;

                SkillStats stats = skillInfo.SkillStats;
                stats.Initialize();
                
                StatData statData = new StatData(stats, fullCategory);
                List<string> targetNames = statData.GetValue<List<string>>("targetNames", -1);

                if (targetNames.Contains(Globals.allyHBStr) || targetNames.Contains(Globals.enemyHBStr))
                {
                    targetNames.Add(Globals.ffaHBStr);
                }
                if (targetNames.Contains(Globals.allyFCStr) || targetNames.Contains(Globals.enemyFCStr))
                {
                    targetNames.Add(Globals.ffaFCStr);
                }
                
                dictionary[statData.GetValue<string>("ID", -1)] = statData;
                StatManager.globalSkillData[statData.GetValue<string>("ID", -1)] = statData;
            }
        }

        private static void Player_InitFSM_AddSpells(On.Player.orig_InitFSM orig, Player self) {
            orig(self);
            foreach (SkillInfo info in SkillsDict.Values)
            {
                Player.SkillState state = (Player.SkillState)Activator.CreateInstance(info.StateType, self.fsm, self);
                info.StateDict[self.playerID] = state;
                self.fsm.AddState(state);
            }
        }

        private static void CatalogSkills(On.LootManager.orig_ResetAvailableSkills orig)
        {
            bool flag = badentrypointsignal.Contains("Loot");
            if (flag)
            {
                orig.Invoke();
            }
            else
            {
                foreach (SkillInfo info in SkillsDict.Values)
                {
                    bool flag2 = !LootManager.completeSkillList.Contains(info.ID);
                    if (flag2)
                    {
                        LootManager.completeSkillList.Add(info.ID);
                    }

                    bool flag3 = info.tier >= LootManager.maxSkillTier;
                    if (flag3)
                    {
                        for (int i = LootManager.maxSkillTier; i <= info.tier; i++)
                        {
                            LootManager.skillTierDict.Add(i, new List<string>());
                        }
                        LootManager.maxSkillTier = info.tier + 1;
                    }
                    foreach (List<string> list in LootManager.skillTierDict.Values)
                    {
                        bool flag4 = list.Contains(info.ID);
                        if (flag4)
                        {
                            list.Remove(info.ID);
                        }
                    }
                    LootManager.skillTierDict[info.tier].Add(info.ID);
                }
                badentrypointsignal.Add("Loot");
                orig.Invoke();
            }
        }
        static List<string> badentrypointsignal = new List<string>();
    }

    public class SkillInfo
    {
        public string ID = "NewArcanaIDDefaultChangeThisPls<3";
        public string DisplayName = "Spell Display Name";
        public string Description = "Description Goes Here";
        public string EnhancedDescription = "Enhanced Description Goes Here";
        public int tier = 0;
        public SkillStats SkillStats;
        public Type StateType;
        public ElementType Element = ElementType.Fire;
        public Sprite Sprite = null;

        public Dictionary<int, Player.SkillState> StateDict = new Dictionary<int, Player.SkillState>();

    }
}
