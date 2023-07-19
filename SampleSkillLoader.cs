using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using LegendAPI;
using Chaos.ListExtensions;

namespace Mythical
{
    class SampleSkillLoader
    {
        public static void Awake()
        {
            //ElementType element = Elements.Register(newElement);
            //Debug.Log("New element name: " + element.ToString());

            ElementInfo newElement = new ElementInfo();
            newElement.name = "Radiant";
            newElement.color = Color.yellow;
            newElement.weakTo = new List<ElementType>() { ElementType.Chaos };
            newElement.icon = ImgHandler.LoadSprite("radiant");
            newElement.iconInk = newElement.icon;
            newElement.impactAudioID = "ImpactShield";
            ElementType element = Elements.Register(newElement);


            SkillInfo skillInfo = new SkillInfo() {
                displayName = "Zeal's Finisher",
                ID = "Mythical::RadiantStorm",
                description = "Create an explosion where you dash!",
                enhancedDescription = "Create an explosion where you land as well!",
                stateType = typeof(FireBurstDash),
                icon = Extensions.loadSprite("firestorm"),
                skillStats = Utils.LoadFromEmbeddedJson<SkillStats>("StatData1.json"),
            };

            Skills.Register(skillInfo);

            skillInfo = new SkillInfo() {
                displayName = "Poison Breath",
                ID = "Mythical::UseRadiantBlast",
                description = "Shoot forth a burst of poisonous air!",
                enhancedDescription = "Poison is more powerful!",
                stateType = typeof(UsePoisonBlastState),
                icon = Extensions.loadSprite("poisonBlast"),
                skillStats = Utils.LoadFromEmbeddedJson<SkillStats>("StatData2.json"),
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo() {
                displayName = "Poison Beam",
                ID = "Mythical::UsePoisonBeam",
                description = "Fire a deadly beam of poison!",
                enhancedDescription = "Poison is more powerful!",
                stateType = typeof(UsePoisonBeam),
                icon = Extensions.loadSprite("poisonBeam"),
                skillStats = Utils.LoadFromEmbeddedJson<SkillStats>("StatData3.json"),
            };

            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Shock Stream",
                ID = "Mythical::UseShockLaceLine",
                description = "Fire a sequence of powerful lightning bursts!",
                enhancedDescription = "Bursts are larger and punch harder!",
                stateType = typeof(UseShockLaceLine),
                icon = ImgHandler.LoadSprite("shockstream"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::UseShockLaceLine",
                    
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Lightning,
                            subElementType = ElementType.Lightning,
                            damage = 3,
                            cooldown = 8f,
                            knockbackMultiplier = -0.25f,
                            hitStunDurationModifier = 0.4f,
                            sameAttackImmunityTime = 0.15f,
                            overdriveSingleIncrease=false
                        }
                    }
                },
            };

            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Wave Dash",
                ID = "Mythical::UseSlicingBarrageGood",
                description = "Chase your foes with a series of air gusts!",
                enhancedDescription = "You fire off more attacks!",
                stateType = typeof(UseSlicingBarrageGood),
                icon = ImgHandler.LoadSprite("wavedash"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::UseSlicingBarrageGood",
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Air,
                            subElementType = ElementType.Air,
                            damage = 8,
                            cooldown = 8f,
                            knockbackMultiplier = 30f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0f,
                        },
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Air,
                            subElementType = ElementType.Air,
                            damage = 8,
                            cooldown = 7f,
                            knockbackMultiplier = 30f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1f,
                            sameAttackImmunityTime = 0.05f,
                        },
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Air,
                            subElementType = ElementType.Air,
                            damage = 9,
                            cooldown = 7f,
                            knockbackMultiplier = 30f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 2f,
                            sameAttackImmunityTime = 0.05f,
                        }
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Combustion Wave",
                ID = "Mythical::UseCombustionWaveGood",
                description = "Shoot off twin lines of explosions!",
                enhancedDescription = "Final explosion ignites the area on fire!",
                stateType = typeof(UseCombustionWaveGood),
                icon = ImgHandler.LoadSprite("combustion"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::UseCombustionWaveGood",
                    
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Fire,
                            subElementType = ElementType.Fire,
                            damage = 5,
                            cooldown = 6f,
                            knockbackMultiplier = 20f,
                            hitStunDurationModifier = 1f,
                            sameAttackImmunityTime = 0.05f,
                            burnChance = 0,
                            overdriveSingleIncrease=true,
                            knockbackOverwrite = true,
                        },
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Fire,
                            subElementType = ElementType.Fire,
                            damage = 25,
                            cooldown = 6f,
                            knockbackMultiplier = 35f,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.15f,
                            burnChance = 1,
                            overdriveSingleIncrease=true,
                            knockbackOverwrite = true,
                        },
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Fire,
                            subElementType = ElementType.Fire,
                            damage = 13,
                            cooldown = 6f,
                            knockbackMultiplier = 10f,
                            hitStunDurationModifier = 1.75f,
                            sameAttackImmunityTime = 0.15f,
                            burnChance = 1,
                            overdriveSingleIncrease=true,
                        }
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Amber's Gambit",
                ID = "Mythical::UseRandomMinions",
                description = "Creates two random minions!",
                enhancedDescription = "Enhances all of the minions spawned!",
                stateType = typeof(UseRandomMinion),
                icon = ImgHandler.LoadSprite("gambitA"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::UseRandomMinions",

                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = element,
                            subElementType = element,
                            spawnCount = 2,
                            cooldown = 15f,
                            baseHealth = 30,
                            duration = 10,
                            knockbackOverwrite = true
                        }
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Frost Circlet",
                ID = "Mythical::UseFrostRing",
                description = "Throw out a ring of icicles that guard you against your foes!",
                enhancedDescription = "Icicles go further and move faster!",
                stateType = typeof(UseFrostRing),
                icon = ImgHandler.LoadSprite("frostRing"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::UseFrostRing",
                    
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Water,
                            subElementType = ElementType.Ice,
                            damage = 8,
                            cooldown = 7,
                            knockbackMultiplier = 8f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.1f,
                        },
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Water,
                            subElementType = ElementType.Ice,
                            damage = 10,
                            cooldown = 7,
                            knockbackMultiplier = 10f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.1f,
                        },
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Obsidian Dash",
                ID = "Mythical::ObsidianDash",
                description = "Lunge forward with obsidian daggers!",
                enhancedDescription = "Daggers pierce enemies !",
                stateType = typeof(ObsidianDash),
                icon = ImgHandler.LoadSprite("obsidiandash"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::ObsidianDash",
                    
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Earth,
                            subElementType = ElementType.Earth,
                            damage = 8,
                            cooldown = 4,
                            knockbackMultiplier = 12f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.1f,
                        },
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Earth,
                            subElementType = ElementType.Earth,
                            damage = 10,
                            cooldown = 4,
                            knockbackMultiplier = 15f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.1f,
                        },
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Holy Beam",
                ID = "Mythical::UseHolyBeam",
                description = "Channel radiant energy into a powerful beam!",
                enhancedDescription = "Your beam is longer, stronger, and more enduring!",
                stateType = typeof(UseHolyBeam),
                icon = ImgHandler.LoadSprite("rbeam2"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::UseHolyBeam",
                    
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = element,
                            subElementType = element,
                            damage = 10,
                            knockbackMultiplier = 10,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.12f,
                        },
                        new SkillStatsLevel
                        {
                            elementType = element,
                            subElementType = element,
                            damage = 12,
                            knockbackMultiplier = 10,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.12f,
                        },
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Prism Rush",
                ID = "Mythical::UseRadiantDashBeam",
                description = "Twin beams of light surge from within you!",
                enhancedDescription = "Your beams are longer, stronger, and more enduring!",
                stateType = typeof(UseRadiantDashBeam),
                icon = ImgHandler.LoadSprite("rBeamDash"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::UseRadiantDashBeam",

                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = element,
                            subElementType = element,
                            damage = 8,
                            cooldown = 4,
                            knockbackMultiplier = 8f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.15f,
                        },
                        new SkillStatsLevel
                        {
                            damage = 10,
                            knockbackMultiplier = 10f,
                            knockbackOverwrite = true,
                            cooldown = 4,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.13f,
                        },
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Obsidian Flurry",
                ID = "Mythical::UseEarthCascadeChain",
                description = "Send a stream of obsidian daggers at your enemies!",
                enhancedDescription = "Daggers thrown are more powerful!",
                stateType = typeof(UseEarthCascadeChain),
                icon = ImgHandler.LoadSprite("earthFlurry"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::UseEarthCascadeChain",

                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Earth,
                            subElementType = ElementType.Earth,
                            damage = 4,
                            spawnCount = 8,
                            cooldown = 0.5f,
                            knockbackMultiplier = 8f,
                            knockbackOverwrite = true,
                            overdriveProgressMultiplier = 0.8f,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.15f,
                        },
                        new SkillStatsLevel
                        {
                            damage = 4,
                            spawnCount = 8,
                            cooldown = 0.5f,
                            knockbackMultiplier = 10f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1.5f,
                            sameAttackImmunityTime = 0.13f,
                        },
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Dragon Strike",
                ID = "Mythical::DragonCross",
                description = "Create a stream of chaotic dragons to pull your enemies in!",
                enhancedDescription = "Dragons go further and faster!",
                stateType = typeof(DragonCross),
                icon = ImgHandler.LoadSprite("dStrike"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::DragonCross",
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Chaos,
                            subElementType = ElementType.Chaos,
                            damage = 6,
                            knockbackMultiplier = -10f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1f,
                            sameAttackImmunityTime = 0.2f,
                        },
                        new SkillStatsLevel
                        {
                            damage = 7,
                            knockbackMultiplier = -10f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1f,
                            sameAttackImmunityTime = 0.2f,
                        },
                        new SkillStatsLevel
                        {
                            damage = 8,
                            knockbackMultiplier = -15f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 1f,
                            sameAttackImmunityTime = 0.2f,
                        }
                    }
                },
            };
            Skills.Register(skillInfo);
            

            skillInfo = new SkillInfo()
            {
                displayName = "Celestial Surge",
                ID = "Mythical::LightDragonDash",
                description = "Summon forth chaos incarnate with your dash!",
                enhancedDescription = "Dragons go further and faster!",
                stateType = typeof(LightDragonDash),
                icon = ImgHandler.LoadSprite("lDragonDash"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::LightDragonDash",
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = element,
                            subElementType = element,
                            damage = 14,
                            cooldown = 5,
                            knockbackMultiplier = 16f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 2f,
                            sameAttackImmunityTime = 0.2f,
                        },
                        new SkillStatsLevel
                        {
                            damage = 19,
                            cooldown = 5,
                            knockbackMultiplier = 20f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 2f,
                            sameAttackImmunityTime = 0.2f,
                        },
                        new SkillStatsLevel
                        {
                            damage = 22,
                            cooldown = 5,
                            knockbackMultiplier = 22f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 2f,
                            sameAttackImmunityTime = 0.2f,
                        }
                    }
                },
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo()
            {
                displayName = "Darkwing Flicker",
                ID = "Mythical::DarkDragonDash",
                description = "Summon forth chaos incarnate with your dash!",
                enhancedDescription = "Dragons go further and faster!",
                stateType = typeof(DarkDragonDash),
                icon = ImgHandler.LoadSprite("dDragonDash"),
                skillStats = new SkillStatsInfo()
                {
                    ID = "Mythical::DarkDragonDash",
                    
                    targetNames = new string[2] { "EnemyHurtBox", "DestructibleHurtBox" },
                    levelInfos = new SkillStatsLevel[]
                    {
                        new SkillStatsLevel
                        {
                            elementType = ElementType.Chaos,
                            subElementType = ElementType.Chaos,
                            damage = 8,
                            cooldown = 4,
                            knockbackMultiplier = 12f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 2f,
                            sameAttackImmunityTime = 0.2f,
                        },
                        new SkillStatsLevel
                        {
                            damage = 12,
                            cooldown = 4,
                            knockbackMultiplier = 14f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 2f,
                            sameAttackImmunityTime = 0.2f,
                        },
                        new SkillStatsLevel
                        {
                            damage = 14,
                            cooldown = 4,
                            knockbackMultiplier = 16f,
                            knockbackOverwrite = true,
                            hitStunDurationModifier = 2f,
                            sameAttackImmunityTime = 0.2f,
                        }
                    }
                },
            };
            Skills.Register(skillInfo);



            //-----------------

            newArc = ImgHandler.LoadSprite("newArc");
            newArc2 = ImgHandler.LoadSprite("newArc2");

            On.FireArc.OnTargetHit += (On.FireArc.orig_OnTargetHit orig, FireArc self, Entity ent) =>
            {
                DragonCross.darkArcs.RemoveNullValues();
                if (DragonCross.darkArcs.Contains(self))
                {
                    //ChaoticBurst.CreateBurst(self.transform.position, self.attackBox.atkInfo, 1);
                } else
                {
                    orig(self, ent);
                }
            };

            On.FireArc.ResetProjectile += (On.FireArc.orig_ResetProjectile orig, FireArc self) =>
            {
                DragonCross.darkArcs.RemoveNullValues();
                if (DragonCross.darkArcs.Contains(self))
                {
                    self.currentPosition = self.transform.position;
                    //ChaoticBurst.CreateBurst(self.transform.position, self.attackBox.atkInfo, 1);
                    PoolManager.GetPoolItem<ParticleEffect>("SmokeEmitter").Emit(new int?(3), new Vector3?(self.currentPosition + self.moveVector * 0.5f), null, null, 0f, null, null);
                    string audioID = "FireArcEnd";
                    Vector2? soundOrigin = new Vector2?(self.currentPosition);
                    float standardPitchRange = SoundManager.StandardPitchRange;
                    SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, -1f, standardPitchRange, false);
                    DragonCross.darkArcs.Remove(self);
                    ResetProjectileBase(self);
                }
                else
                {
                    orig(self);
                }
            };

            On.FireBeam.Start += (On.FireBeam.orig_Start orig, FireBeam self) =>
            {
                if (self.hitStopwatchID == -1)
                {
                    self.hitStopwatchID = ChaosStopwatch.Begin(self.hitInterval, true, self.hitInterval, self.hitCount, 0);
                }
                self.UpdateRangeVars();
                self.widthScaleVec = new Vector3(self.beamThickness * 0.5f, 1f, 1f);
                self.beamTipTrans.localScale = self.widthScaleVec;
                self.beamStartObj = self.transform.Find("BeamStart").gameObject;
                self.beamStartObj.transform.localScale = self.widthScaleVec;
                self.beamActive = true;
                self.anim.Play("Active", -1, 0f);
                string audioID = "VacuumExplosion";
                Transform transform = self.transform;
                SoundManager.PlayAudioWithDistance(audioID, null, transform, 24f, -1f, UnityEngine.Random.Range(0.85f, 0.95f), false);
                self.ogBeamThickness = self.beamThickness;
                self.beamTipTrans.localScale = new Vector3(self.beamTipTrans.localScale.x * 1.25f, self.beamTipTrans.localScale.x * 1.25f, 1f);
                self.beamStartObj.transform.localScale = new Vector3(self.beamStartObj.transform.localScale.x, self.beamStartObj.transform.localScale.x, 1f);
                self.fireEmitterLeftTrans.localPosition = new Vector3(-0.1f * self.beamThickness, self.fireEmitterLeftTrans.localPosition.y, self.fireEmitterLeftTrans.localPosition.z);
                self.fireEmitterRightTrans.localPosition = new Vector3(0.1f * self.beamThickness, self.fireEmitterLeftTrans.localPosition.y, self.fireEmitterLeftTrans.localPosition.z);
                self.leftShapeMod = self.leftPSys.shape;
                self.rightShapeMod = self.rightPSys.shape;
                self.destroyDelay = 1f;
            };

            On.SBCardInfoPageUI.InitElementalTabs += (On.SBCardInfoPageUI.orig_InitElementalTabs orig, SBCardInfoPageUI self) =>
            {
                orig(self);
                foreach(GameObject obj in self.eleTabDict.Values)
                {
                    obj.GetComponent<RectTransform>().position = obj.GetComponent<RectTransform>().position + new Vector3(0, 15, 0);
                }
                foreach (GameObject obj in self.eleTabSelDict.Values)
                {
                    obj.GetComponent<RectTransform>().position = obj.GetComponent<RectTransform>().position + new Vector3(0, 15, 0);
                }
                GameObject o = GameObject.Instantiate(self.eleTabDict[ElementType.Chaos], self.eleTabDict[ElementType.Chaos].transform.position, Quaternion.identity);
                o.transform.parent = self.eleTabs;
                o.name = newElement.name;
                self.eleTabDict[element] = o;
                self.eleNewTabDict[element] = o.GetComponent<UnityEngine.UI.Image>();
                self.eleNewTabDict[element].enabled = true;
                self.eleNewTabDict[element].sprite = ImgHandler.LoadSprite("radiantBG");
                o.GetComponent<RectTransform>().position = o.GetComponent<RectTransform>().position + new Vector3(0, -30,0);

                o = GameObject.Instantiate(self.eleTabSelDict[ElementType.Chaos], self.eleTabSelDict[ElementType.Chaos].transform.position, Quaternion.identity);
                o.transform.parent = self.eleSelTabs;
                o.name = newElement.name;
                self.eleTabSelDict[element] = o;
                o.GetComponent<UnityEngine.UI.Image>().sprite = ImgHandler.LoadSprite("radiantSel");
                o.GetComponent<RectTransform>().position = o.GetComponent<RectTransform>().position + new Vector3(0, -30, 0);
            };

            On.SBCardInfoPageUI.UpdateTabs += (On.SBCardInfoPageUI.orig_UpdateTabs orig, SBCardInfoPageUI self, ElementType el, SpellBookUI.SkillEquipType playerInfoSelectedType) =>
            {
                foreach (KeyValuePair<ElementType, GameObject> keyValuePair in self.eleTabSelDict)
                {
                    if (keyValuePair.Key == el)
                    {
                        keyValuePair.Value.SetActive(true);
                    }
                    else
                    {
                        keyValuePair.Value.SetActive(false);
                    }

                    if (LegendAPI.Elements.eleDict.ContainsKey(keyValuePair.Key))
                    {
                        int ct = 0;
                        foreach(KeyValuePair<ElementType, List<string>> kvp in Player.elementSkillDict)
                        {
                            if (kvp.Key == element)
                            {
                                foreach(string s in kvp.Value)
                                {
                                    if (!LootManager.lockedSkillList.Contains(s))
                                    {
                                        ct++;
                                    }
                                }
                            }
                        }

                        self.eleNewTabDict[element].enabled = ct > 0;
                    }
                    else
                    {
                        if (Player.NewSpellManager.GetUnlockCount(playerInfoSelectedType, keyValuePair.Key) > 0)
                        {
                            self.eleNewTabDict[keyValuePair.Key].enabled = true;
                        }
                        else
                        {
                            self.eleNewTabDict[keyValuePair.Key].enabled = false;
                        }
                    }
                }
                
            };
        }
        public static Sprite newArc;
        public static Sprite newArc2;

        public static void ResetProjectileBase(Projectile p)
        {
            ChaosStopwatch.Stop(p.flyStopwatchID);
            ChaosStopwatch.Stop(p.lifeStopwatchID);
            if (p.particleEffectOnReset && p.particleEffectName != null && p.particleEffectName != string.Empty)
            {
                PoolManager.GetPoolItem<ParticleEffect>(p.particleEffectName).Emit(new int?(p.particleEmitCount), new Vector3?(p.transform.position + p.particleEmitOffset), null, null, 0f, null, null);
            }
            if (p.projectileSprite != null)
            {
                p.projectileSprite.transform.rotation = Quaternion.identity;
            }
            p.boomerangReturning = false;
            p.flagForReset = false;
            p.forcedReturn = false;
            if (p.gameObject != null)
            {
                if (p.destroyOnDisable)
                {
                    UnityEngine.Object.Destroy(p.gameObject);
                }
                else
                {
                    p.gameObject.SetActive(false);
                }
            }
        }

    }

    

}


public static class Utils
{
    public static T LoadFromEmbeddedJson<T>(string jsn)
    {

        string jsnstring;

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Mythical.{jsn}";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            jsnstring = reader.ReadToEnd();
        }

        return JsonUtility.FromJson<T>(jsnstring);
    }
}
