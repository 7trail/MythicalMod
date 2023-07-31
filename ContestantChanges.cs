using Chaos.AnimatorExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Mythical
{
    public static class ContestantChanges
    {
        public static StatData tornadoData;
        public static bool hasLoadedChanges=false;
        public static int skill = 0;
        public static void Init()
        {
            On.StatManager.LoadEnemySkills += (On.StatManager.orig_LoadEnemySkills orig, string s) =>
            {
                orig(s);

                CreateFSM("ContestantSummonTornado", s);
                CreateFSM("ConFireball", s);
                CreateFSM("ConThrow", s);

                if (!hasLoadedChanges)
                {
                    hasLoadedChanges = true;

                    On.Contestant.Start += (On.Contestant.orig_Start orig2, Contestant self) =>
                    {
                        orig2(self);
                        self.skillCategory = "Enemy";
                        skill++;
                        skill = skill % 3;
                        //idk which palettes these were supposed to be. you'll have to store them in contentloader and add them here
                        //for now it's a roll of fate!
                        int[] paletteIDs = new int[3] {24,49,65};
                        Material m = self.spriteRenderer.material;
                        //m.SetTexture("_Palette", ContentLoader.newPalette);
                        //m.SetFloat("_PaletteCount", 32 + ContentLoader.palettes.Count);
                        m.SetFloat("_PaletteIndex", paletteIDs[skill]);
                        
                        
                        switch (skill)
                        {
                            case 0:
                                break;
                            case 1:
                                self.fsm.ReplaceState("Tornado", new ContestantAirShieldState("Tornado", self.fsm, self));
                                self.fsm.ReplaceState("Fireball", new ContestantIonSpreadState("Fireball", self.fsm, self));
                                break;
                            case 2:
                                /*self.fsm.ReplaceState("SwordThrow", new ThunderMage.ThunderMageThunderWaveState("SwordThrow", self.fsm, self, 1, 1, 1, 1, 3)
                                {
                                    windupTime = 0.25f,
                                    skillID="ThunderMageThunderWave"
                                });*/ 
                                self.fsm.ReplaceState("Fireball", new ContestantAquaBeamState("Fireball", self.fsm, self));
                                //self.fsm.ReplaceState("Tornado", new ContestantDrillAttackState("Tornado", self.fsm, self));
                                break;
                        }

                    };


                    On.Contestant.TornadoState.OnEnter += (On.Contestant.TornadoState.orig_OnEnter orig2, Contestant.TornadoState self) =>
                    {
                        self.parent.movement.EndMovement();
                        self.parent.anim.PlayDirectional("Cast" + self.parent.facingDirection, -1, 0f);
                        self.tornadoScript = UnityEngine.Object.Instantiate<GameObject>(self.tornadoPrefab, self.parent.hurtBoxTransform.position, Quaternion.identity).GetComponent<Tornado>();
                        self.tornadoScript.attackBox.SetAttackInfo("EnemyScaled", "ContestantSummonTornado", 1);


                    };
                    On.Contestant.FireballState.Update += (On.Contestant.FireballState.orig_Update orig2, Contestant.FireballState self) =>
                    {
                        if (self.parent.anim.AnimPlayed(0.2f) && !self.fireballCreated)
                        {
                            self.fireballScript = UnityEngine.Object.Instantiate<GameObject>(self.fireballPrefab, self.parent.hurtBoxTransform.position, Globals.GetRotationQuaternion(self.targetVector)).GetComponent<Fireball>();
                            self.fireballScript.gameObject.name = self.parent.skillCategory + "Fireball";
                            self.fireballScript.parentObject = self.parent.gameObject;
                            self.fireballScript.moveVector = self.targetVector;
                            self.fireballScript.attackBox.SetAttackInfo("EnemyScaled", "ConFireball", 1, false);
                            self.fireballCreated = true;
                        }
                        else if (self.parent.anim.AnimPlayed(0.9f))
                        {
                            self.parent.HandleExitTransitions(false);
                            return;
                        }
                    };
                    On.Contestant.SwordThrowState.Update += (On.Contestant.SwordThrowState.orig_Update orig2, Contestant.SwordThrowState self) =>
                    {
                        if (self.parent.anim.AnimPlayed(0.2f) && !self.swordThrown)
                        {
                            self.swordThrown = true;
                            self.swordThrowScript = UnityEngine.Object.Instantiate<GameObject>(self.swordThrowPrefab, self.parent.hurtBoxTransform.position, Quaternion.identity).GetComponent<SwordThrowProjectile>();
                            self.swordThrowScript.parentObject = self.parent.gameObject;
                            self.swordThrowScript.moveVector = self.targetVector;
                            self.swordThrowScript.destroyOnDisable = true;
                            self.swordThrowScript.attackBox.SetAttackInfo("EnemyScaled", "ConThrow", 1, false);
                            SoundManager.PlayAudioWithDistance("WindBoomerang", new Vector2?(self.parent.hurtBoxTransform.position), null, 24f, -1f, 1f, false);
                        }
                        else if (self.parent.anim.AnimPlayed(0.9f))
                        {
                            self.parent.HandleExitTransitions(false);
                            return;
                        }
                    };
                    On.Contestant.AttackState.OnEnter += (On.Contestant.AttackState.orig_OnEnter orig2, Contestant.AttackState self) =>
                    {
                        self.parent.closestTargetInfo = Entity.GetClosestTarget(self.parent.targetGroup, self.parent.transform.position, self.parent.escapeRadius, self.parent.onlyTargetPlayers, null, null, false, 0);
                        if (self.parent.closestTargetInfo == null)
                        {
                            self.fsm.ChangeState("GoBack", false);
                            return;
                        }
                        self.slash1 = false;
                        self.parent.FaceTarget(self.parent.closestTargetInfo.position, 4, false);
                        self.parent.anim.PlayDirectional("Slash1" + self.parent.facingDirection, -1, 0f);
                        SoundManager.PlayAudioWithDistance("LightSlash", new Vector2?(self.parent.transform.position), null, 24f, -1f, 1f, false);
                        self.parent.attack.SetAttackInfo("EnemyScaled", "NeutralKnightAttack", 2, false);
                        self.parent.movement.EndMovement();
                    };
                }
            };
        }
        public static void CreateFSM(string id, string s)
        {
            SkillStats stats = Utils.LoadFromEmbeddedJson<SkillStats>(id+".json");
            stats.Initialize();

            tornadoData = new StatData(stats, "Enemy" + s);
            FSMUtils.ApplyStateModifiers(s, tornadoData);
            StatManager.data["Skills"]["Enemy" + s][id] = tornadoData;
        }
    }

    public static class FSMUtils
    {
        public static void ReplaceState(this FSM fsm, string stateName, IState newState)
        {
            fsm.states.Remove(stateName);
            fsm.AddState(newState);
        }

        public static void ApplyStateModifiers(string categoryModifier, StatData statData)
        {
            List<string> value = statData.GetValue<List<string>>("targetNames", -1);
            if (value.Contains(Globals.allyHBStr) || value.Contains(Globals.enemyHBStr))
            {
                value.Add(Globals.ffaHBStr);
            }
            if (value.Contains(Globals.allyFCStr) || value.Contains(Globals.enemyFCStr))
            {
                value.Add(Globals.ffaFCStr);
            }
            if (true)
            {
                if (categoryModifier == "Only")
                {
                    if (value.Contains(Globals.allyHBStr))
                    {
                        value.Remove(Globals.allyHBStr);
                        value.Add(Globals.enemyHBStr);
                    }
                    if (value.Contains(Globals.allyFCStr))
                    {
                        value.Remove(Globals.allyFCStr);
                        value.Add(Globals.enemyFCStr);
                    }
                }
                else if (categoryModifier == "FFA")
                {
                    if (value.Contains(Globals.allyHBStr))
                    {
                        value.Add(Globals.enemyHBStr);
                    }
                    if (value.Contains(Globals.allyFCStr))
                    {
                        value.Add(Globals.enemyFCStr);
                    }
                }
                else if (categoryModifier == "AsAlly")
                {
                    value.Remove(Globals.ffaHBStr);
                    value.Remove(Globals.ffaFCStr);
                }
                statData.currentLevel = 1;
            }
        }

    }

}
