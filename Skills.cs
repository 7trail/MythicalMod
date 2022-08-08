using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using LegendAPI;
using On;
using UnityEngine;

namespace Mythical
{
    public static class Skills
    {

        public static Dictionary<string, SkillInfo> skillsDict = new Dictionary<string, SkillInfo>();

        public static bool hasLoadedNewSpells = false;

        //So far, all this can do is replace spells. But fear not! I will be adding more support soon :)



        // Did you remember to set the skill base constructor to the new ID?
        public static void Awake()
        {
            On.Player.SkillState.ctor += Skill_ctor;
            On.FSM.AddState += FSM_AddState;
            On.Player.SkillState.InitChargeSkillSettings += SkillState_InitChargeSkillSettings;
            //On.Attack.SetAttackInfo_string_string_int_bool += Attack_SetAttackInfo_string_string_int_bool;

            On.Attack.ExecuteAttack += (On.Attack.orig_ExecuteAttack orig, Attack self, Collider2D col) =>
            {
                orig(self,col);
                Debug.Log(self.atkInfo.skillID + " has dealt " + self.atkInfo.damage + ", data says to deal " + self.statData.GetValue<int>(StatData.damageStr,self.atkInfo.skillLevel) + " at level " + self.atkInfo.skillLevel);
            };
            

            On.GameController.Awake += delegate (On.GameController.orig_Awake orig, GameController self)
            {
                orig.Invoke(self);
                On.LootManager.ResetAvailableSkills += CatalogSkills;
                On.StatManager.LoadData += AddSkills;
                On.StatManager.GetSkillData += (On.StatManager.orig_GetSkillData orig2, string cat, string id) =>
                {
                    if (skillsDict.ContainsKey(id))
                    {
                        Debug.Log(skillsDict[id].finalData.GetValue<int>(StatData.damageStr,0));
                        return skillsDict[id].finalData;
                    }
                    return orig2(cat, id);
                };
            };
            On.CooldownManager.Add += CooldownManager_Add;

            On.CooldownManager.StartCooldownTimer += (On.CooldownManager.orig_StartCooldownTimer orig,CooldownManager self, string id, float ov) => {
                Debug.Log("Using Skill, caught from timer");
                if (skillsDict.ContainsKey(id))
                {
                    Debug.Log("Using Custom Skill, caught from timer");
                    self.cooldowns[id].remainingTimeStopwatchID = ChaosStopwatch.Begin((ov <= 0f) ? skillsDict[id].cooldown : ov, false, 0f, 0, 0);
                    self.cooldowns[id].IsCharging = true;
                    self.AnnounceStartCDEvent(id);
                    return;
                }
                orig(self, id, ov);
            };

            On.SBElementPageUI.SetElementCounts += (On.SBElementPageUI.orig_SetElementCounts orig, SBElementPageUI self, Player p) => {
                try
                {
                    orig(self, p);
                } catch
                {
                    Debug.Log("Went into error loading mode, trying again");
                    foreach(SkillInfo info in skillsDict.Values)
                    {
                        if (!Player.elementSkillDict[info.elementType].Contains(info.ID))
                        {
                            Player.elementSkillDict[info.elementType].Add(info.ID);
                        }
                        p.skillsDict[info.ID] = info.finalState;

                    }
                    orig(self, p);
                }
            };

        }



        public static void AddSkills(On.StatManager.orig_LoadData orig, string str, string statID, string category, string modifier)
        {
            orig(str,statID,category,modifier);
            if (str == StatManager.playerSkillAssetPath)
            {
                string text = category + modifier;
                Dictionary<string, StatData> dictionary = StatManager.data[statID][text];

                foreach (SkillInfo info in skillsDict.Values)
                {
                    if (info.data == null)
                    {
                        Debug.Log("Uh oh! SkillStats is null!");
                    }
                    else
                    {
                        info.data.Initialize();
                        StatData data = new StatData(info.data, text);
                        customDataDict[info.ID] = data;
                        info.finalData = data;
                        if (data == null) { Debug.Log("Uh oh, it's null!"); }
                        Debug.Log("1");
                        dictionary[data.GetValue<string>("ID", -1)] = data;
                        Debug.Log("2");
                        StatManager.globalSkillData[data.GetValue<string>("ID", -1)] = data;
                        Debug.Log("3");
                    }
                }
            }
        }

        static Dictionary<string, StatData> customDataDict = new Dictionary<string, StatData>();

        private static void CatalogSkills(On.LootManager.orig_ResetAvailableSkills orig)
        {
            bool flag = badentrypointsignal.Contains("Loot");
            if (flag)
            {
                orig.Invoke();
            }
            else
            {
                foreach (SkillInfo info in skillsDict.Values)
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
                //Items.LateInit();
                badentrypointsignal.Add("Loot");
                orig.Invoke();
            }
        }

        private static void Skill_ctor(On.Player.SkillState.orig_ctor orig, Player.SkillState self, string newName, FSM fsm, Player parentPlayer)
        {
            orig(self, newName, fsm, parentPlayer);
            if (skillsDict.ContainsKey(self.skillID))
            {
                Debug.Log("Constructing modded skill");
                SkillInfo info = skillsDict[self.skillID];
                self.skillData = info.finalData;

                //self.name = info.displayName;
                if (info.startingCharges > 1)
                {
                    self.InitChargeSkillSettings(info.startingCharges, info.chargeCooldown, self.skillData, self);
                }
            }
        }

        private static void SkillState_InitChargeSkillSettings(On.Player.SkillState.orig_InitChargeSkillSettings orig, Player.SkillState self, int maxCharges, float delayBetweenCharges, StatData statData, Player.SkillState skillState)
        {
            orig(self, maxCharges, delayBetweenCharges, statData, skillState);

            if (skillsDict.ContainsKey(self.skillID))
            {

                Debug.Log("Modding charges of skill " + self.skillID);
                SkillInfo info = skillsDict[self.skillID];
                if (info.cooldown >= 0)
                {
                    self.cooldownRef.MaxTime = info.cooldown;
                    self.cooldownRef.maxTime = info.cooldown;
                }
                if (info.chargeCooldown >= -0.01) {
                    self.cooldownRef.chargeDelayTime = info.chargeCooldown;
                }
                if (info.startingCharges >= 0)
                {
                    
                    self.cooldownRef.MaxChargeCount = info.startingCharges;
                    self.cooldownRef.chargeCount = info.startingCharges;
                }
                self.cooldownRef.isChargeSkill = info.isChargeSkill;
                //Utils.printAllFields(self.cooldownRef, true);
                //self.cooldownRef.statData.numVarStatDict[StatData.cdStr].BaseValue *= 0.2f;
            }
        }

        private static void CooldownManager_Add(On.CooldownManager.orig_Add orig, CooldownManager self, string id, float time, StatData data, Player.SkillState state )
        {
            
            if (skillsDict.ContainsKey(id) )
            {
                SkillInfo info = skillsDict[id];
                if (info.cooldown != -1)
                {
                    Debug.Log("-----------------------------------------------------ADDING CUSTOM CD");
                    orig(self, id, info.cooldown, data, state);
                    return;
                } else
                {
                    orig(self, id, time, data, state);
                    return;
                }
            }
            orig(self, id, time, data, state);
        }

        static List<Player> loadedPlayers = new List<Player>();
        


        private static void FSM_AddState(On.FSM.orig_AddState orig, FSM self, IState newState)
        {

            bool b = true;
            if (newState is Player.SkillState)
            {
                foreach (Player p in loadedPlayers)
                {
                    if(p== ((Player.SkillState)newState).parent)
                    {
                        b = false;
                    }
                }
                if (b)
                {
                    loadedPlayers.Add(((Player.SkillState)newState).parent);
                    if (IconManager.skillIcons == null)
                    {
                        IconManager.skillIcons = IconManager.SkillIcons;
                    }
                    if (TextManager.skillInfoDict == null)
                    {
                        TextManager.skillInfoDict = new Dictionary<string, TextManager.SkillInfo>();
                    }
                
                        hasLoadedNewSpells = true;
                        foreach (SkillInfo skill in skillsDict.Values)
                        {
                            if (skill.isNewSkill)
                            {

                                Debug.Log("Pre State2 thing");
                                Player.SkillState state = DefaultInitFunction(self, ((Player.SkillState)newState), skill);
                                Debug.Log("State 2 thing 1");
                                state.parent.skillsDict[state.skillID] = state;
                                //state.isUnlocked = true;
                                Debug.Log("State 2 thing 2");
                                IState newState2 = (IState)state;
                                Debug.Log("Post State2 thing");
                                if (self.states.ContainsKey(newState2.name))
                                {

                                }
                                else
                                {
                                    self.AddState((IState)newState2);
                                }
                                SetInfo(skill);
                                Debug.Log("Post Add State");
                                if (!((Player.SkillState)newState).parent.cooldownManager.cooldowns.ContainsKey(skill.ID))
                                {
                                    Debug.Log("Adding Cooldown State for " + skill.ID);
                                    Player.SkillState state3 = (Player.SkillState)newState;
                                    ((Player.SkillState)newState).parent.cooldownManager.Add(skill.ID, skill.cooldown, null, state3);
                                    ((Player.SkillState)newState).parent.cooldownManager.cooldowns[skill.ID].maxTime = skill.cooldown;
                                    ((Player.SkillState)newState).parent.cooldownManager.cooldowns[skill.ID].maxChargeStat = new NumVarStat((float)skill.startingCharges, true);
                                }
                                Debug.Log("Post Add State 2");
                            }
                        }

                        foreach (SpellBookUI ui in UnityEngine.MonoBehaviour.FindObjectsOfType<SpellBookUI>())
                        {
                            ui.LoadEleSkillDict(((Player.SkillState)newState).parent);
                        }
                        GameDataManager.gameData.PushSkillData();

                }
                string str = ((Player.SkillState)newState).skillID;
                if (skillsDict.ContainsKey(str) && !skillsDict[str].isNewSkill)
                {

                    Debug.Log("Added state for " + skillsDict[str].displayName);
                    SkillInfo info = skillsDict[str];
                    SetInfo(info);
                    //Player.BaseDashState airchanneldashpoopoo = ((Player.BaseDashState)newState);
                    newState = (IState)DefaultInitFunction(self, ((Player.SkillState)newState), info);

                }
            }
            if (!self.states.ContainsKey(newState.name))
            {
                orig(self, newState);
            }
            
        }

        private static AttackInfo Attack_SetAttackInfo_string_string_int_bool(
            On.Attack.orig_SetAttackInfo_string_string_int_bool orig, Attack self, string newSkillCat, string newSkillID, int newSkillLevel, bool newIsUltimate)
        {
            if (skillsDict.ContainsKey(newSkillID) && skillsDict[newSkillID].atkChanges)
            {
                
                AttackInfo newAttackInfo = self.GenerateAttackInfo(customDataDict[newSkillID],newSkillLevel,newIsUltimate);//skillsDict[newSkillID].attackInfo;
                if (newAttackInfo == null)
                {
                    //Utils.loge.LogError("couldn't load attackinfo json");
                    AttackInfo oldAttackInfo2 = orig(self, newSkillCat, newSkillID, newSkillLevel, newIsUltimate);
                    return oldAttackInfo2;
                }
                Debug.Log("Attack info tweaks");
                //replaceAttackInfo(oldAttackInfo, newAttackInfo);

                //Utils.SaveJson(stinkyAttackInfo, "stink");
                //Utils.SaveJson(sexyAttackInfo, "sexy");
                self.statData = customDataDict[newSkillID];
                self.atkInfo = newAttackInfo;
                return newAttackInfo;
            } else
            {

            }
            AttackInfo oldAttackInfo = orig(self, newSkillCat, newSkillID, newSkillLevel, newIsUltimate);

            return oldAttackInfo;
        }

        private static void replaceAttackInfo(AttackInfo stinky, AttackInfo sexy)
        {
            if (stinky != null && sexy != null)
            {
                sexy.entity = stinky.entity;
                sexy.gameObject = stinky.gameObject;
                sexy.skillCategory = stinky.skillCategory;
                sexy.attackInfoKey = stinky.attackInfoKey;
                sexy.atkObjID = stinky.atkObjID;
                sexy.attacker = stinky.attacker;
            }
        }

        public static void Register(SkillInfo skillInfo)
        {
            if (!skillsDict.ContainsKey(skillInfo.ID))
            {
                skillsDict.Add(skillInfo.ID, skillInfo);
            } else
            {
                skillsDict[skillInfo.ID] = skillInfo;
            }
        }

        public static void SetInfo(SkillInfo info)
        {
            TextManager.SkillInfo skillText = new TextManager.SkillInfo();
            skillText.skillID = info.ID;
            skillText.displayName = info.displayName;
            skillText.description = info.description;
            skillText.empowered = info.empowered;
            Debug.Log("1");

            if (!TextManager.skillInfoDict.ContainsKey(info.ID))
            {
                Debug.Log("2");
                TextManager.skillInfoDict.Add(info.ID, skillText);
                Debug.Log("2.5");
            }
            else
            {
                Debug.Log("3");
                TextManager.skillInfoDict[info.ID] = skillText;
                Debug.Log("3.5");
            }
            if (info.isNewSkill)
            {
                SetIcon(info);
            }
        }

        public static void SetIcon(SkillInfo info)
        {
            if (!IconManager.skillIcons.ContainsKey(info.ID))
            {
                Debug.Log("2");
                IconManager.skillIcons.Add(info.ID, info.skillIcon);
                Debug.Log("2.5");
            }
            else
            {
                Debug.Log("3");
                IconManager.skillIcons[info.ID] = info.skillIcon;
                Debug.Log("3.5");
            }
        }

        public static Player.SkillState DefaultInitFunction(FSM fsm, Player.SkillState newState, SkillInfo info)
        {
            Debug.Log("DefInit 1");
            Player.SkillState state = (Player.SkillState)Activator.CreateInstance(info.newState, fsm, newState.parent);
            info.finalState = state;
            Debug.Log("DefInit 2");
            state.element = info.elementType;
            state.skillID = info.ID;
            state.name = info.ID;
            Debug.Log("DefInit 3");
            if (info.isNewSkill)
            {
                Debug.Log("DefInit 4");

                SetIcon(info);

                

                Debug.Log("DefInit 5");
            } 
            return state;
        }

        public class SkillInfo
        {
            public string displayName;
            public string description;
            public string empowered;
            public string ID;
            public System.Type newState;
            public Player.SkillState finalState;
            public AttackInfo attackInfo;
            public SkillStats data;
            public StatData finalData;
            public int startingCharges;
            public float cooldown;
            public float chargeCooldown;
            public bool isChargeSkill;
            public int tier;

            public ElementType elementType;
            public Sprite skillIcon;
            public bool isNewSkill;
            public bool atkChanges;
            public bool generateAtkInfo = false;

            public SkillInfo(string name = "Default")
            {
                displayName = name;
                ID = "VinePullDash";
                description = "Default Description!";
                empowered = "Default Empowered!";
                newState = null;
                startingCharges = 1;
                cooldown = -1;
                chargeCooldown = 0;
                isChargeSkill = true;
                attackInfo = null;
                tier = 1;
                elementType = ElementType.Fire;
                skillIcon = new Sprite();
                isNewSkill = false;
                data = null;
                atkChanges = true;
            }

           

        }
        internal static List<string> badentrypointsignal = new List<string>();
    }
}
