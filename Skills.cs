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

        //So far, all this can do is replace spells. But fear not! I will be adding more support soon :)
        public static void Awake()
        {
            On.Player.SkillState.ctor += Skill_ctor;
            On.FSM.AddState += FSM_AddState;
            On.Player.SkillState.InitChargeSkillSettings += SkillState_InitChargeSkillSettings;
            On.Attack.SetAttackInfo_string_string_int_bool += Attack_SetAttackInfo_string_string_int_bool;
        }

        private static void Skill_ctor(On.Player.SkillState.orig_ctor orig, Player.SkillState self, string newName, FSM fsm, Player parentPlayer)
        {
            orig(self, newName, fsm, parentPlayer);
            if (skillsDict.ContainsKey(self.skillID))
            {
                Debug.Log("Constructing modded skill");
                SkillInfo info = skillsDict[self.skillID];
                self.InitChargeSkillSettings(info.startingCharges, info.chargeCooldown, self.skillData, self);
            }
        }

        private static void SkillState_InitChargeSkillSettings(On.Player.SkillState.orig_InitChargeSkillSettings orig, Player.SkillState self, int maxCharges, float delayBetweenCharges, StatData statData, Player.SkillState skillState)
        {
            orig(self, maxCharges, delayBetweenCharges, statData, skillState);

            if (skillsDict.ContainsKey(self.skillID))
            {

                Debug.Log("Modding charges of skill");
                SkillInfo info = skillsDict[self.skillID];
                self.cooldownRef.chargeDelayTime = info.chargeCooldown;
                self.cooldownRef.MaxChargeCount = info.startingCharges;
                self.cooldownRef.chargeCount = info.startingCharges ;

                self.cooldownRef.MaxTime = info.cooldown;
                self.cooldownRef.isChargeSkill = info.isChargeSkill;
                
                //Utils.printAllFields(self.cooldownRef, true);
                //self.cooldownRef.statData.numVarStatDict[StatData.cdStr].BaseValue *= 0.2f;
            }
        }

        private static void FSM_AddState(On.FSM.orig_AddState orig, FSM self, IState newState)
        {
            if (newState is Player.SkillState) {
                string str = ((Player.SkillState)newState).skillID;
                if (skillsDict.ContainsKey(str))
                {
                    Debug.Log("Added state");
                    SkillInfo info = skillsDict[str];

                    //Player.BaseDashState airchanneldashpoopoo = ((Player.BaseDashState)newState);
                    newState = DefaultInitFunction(self, ((Player.SkillState)newState),info);
                }
            }
            orig(self, newState);
        }

        private static AttackInfo Attack_SetAttackInfo_string_string_int_bool(
            On.Attack.orig_SetAttackInfo_string_string_int_bool orig, Attack self, string newSkillCat, string newSkillID, int newSkillLevel, bool newIsUltimate)
        {

            AttackInfo oldAttackInfo = orig(self, newSkillCat, newSkillID, newSkillLevel, newIsUltimate);

            if (skillsDict.ContainsKey(newSkillID))
            {
                Debug.Log("Attack info tweaks");
                AttackInfo newAttackInfo = skillsDict[newSkillID].attackInfo;
                if (newAttackInfo == null)
                {
                    //Utils.loge.LogError("couldn't load attackinfo json");
                    return oldAttackInfo;
                }

                replaceAttackInfo(oldAttackInfo, newAttackInfo);

                //Utils.SaveJson(stinkyAttackInfo, "stink");
                //Utils.SaveJson(sexyAttackInfo, "sexy");

                return newAttackInfo;
            }

            return oldAttackInfo;
        }

        private static void replaceAttackInfo(AttackInfo stinky, AttackInfo sexy)
        {

            sexy.entity = stinky.entity;
            sexy.gameObject = stinky.gameObject;
            sexy.skillCategory = stinky.skillCategory;
            sexy.attackInfoKey = stinky.attackInfoKey;
            sexy.atkObjID = stinky.atkObjID;
            sexy.attacker = stinky.attacker;
        }

        public static void Register(SkillInfo skillInfo)
        {
            if (!skillsDict.ContainsKey(skillInfo.replacementID))
            {
                skillsDict.Add(skillInfo.replacementID, skillInfo);
            } else
            {
                skillsDict[skillInfo.replacementID] = skillInfo;
            }
        }

        public static IState DefaultInitFunction(FSM fsm, Player.SkillState newState, SkillInfo info)
        {
            return (IState)Activator.CreateInstance(info.newState, fsm, newState.parent);
        }

        public struct SkillInfo
        {
            public string displayName;
            public string replacementID;
            public System.Type newState;
            public AttackInfo attackInfo;
            public int startingCharges;
            public float cooldown;
            public float chargeCooldown;
            public bool isChargeSkill;
            public SkillInfo(string name = "Default")
            {
                displayName = name;
                replacementID = "VinePullDash";                
                newState = null;
                startingCharges = 1;
                cooldown = 1;
                chargeCooldown = 0;
                isChargeSkill = true;
                attackInfo = null;
            }

           

        }
    }
}
