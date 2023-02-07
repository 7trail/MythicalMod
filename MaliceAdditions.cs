using LegendAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    public static class MaliceAdditions
    {
        public static Dictionary<String, Texture2D> originalBossTextures = new Dictionary<string, Texture2D>();
        public static Dictionary<String, Texture2D> newBossTextures = new Dictionary<string, Texture2D>();
        public static NumVarStatMod healthMod;

        public static void Init()
        {
            healthMod = new NumVarStatMod("Malice", 0.5f, 10, VarStatModType.Multiplicative, false);
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::TokenMalice";
            itemInfo.item = new TokenMalice();
            itemInfo.tier = 1;
            itemInfo.priceMultiplier = 3;
            TextManager.ItemInfo text2 = default(TextManager.ItemInfo);
            text2.displayName = "Token of Malice";
            text2.description = "Sura's Repentance is upon him.";
            text2.itemID = TokenMalice.constID;
            Sprite itemsprite = ImgHandler.LoadSprite("tokenMalice");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            //Sprites
            newBossTextures["FireBoss"] = ImgHandler.LoadTex2D("Bosses/altFire",true);
<<<<<<< HEAD
            newBossTextures["FinalBoss"] = ImgHandler.LoadTex2D("Bosses/altFinal", true);
            newBossTextures["EarthBoss"] = ImgHandler.LoadTex2D("Bosses/altEarth", true);
            newBossTextures["IceBoss"] = ImgHandler.LoadTex2D("Bosses/altIce", true);
            newBossTextures["AirBoss"] = ImgHandler.LoadTex2D("Bosses/altAir", true);
=======
            newBossTextures["FinalBoss"] = ImgHandler.LoadTex2D("Bosses/altFinal",true);
            newBossTextures["EarthBoss"] = ImgHandler.LoadTex2D("Bosses/altEarth",true);
            newBossTextures["IceBoss"] = ImgHandler.LoadTex2D("Bosses/altIce",true);
            newBossTextures["AirBoss"] = ImgHandler.LoadTex2D("Bosses/altAir",true);
            newBossTextures["LightningBoss"] = ImgHandler.LoadTex2D("Bosses/altLightning", true);
            newBossTextures["LightningBoss2"] = ImgHandler.LoadTex2D("Bosses/altLightning2", true);
>>>>>>> 6ec5b060cbdcbf7d5f04a7b52370fdfeb32ae504

            originalBossTextures["FireBoss"] = ImgHandler.LoadTex2D("Bosses/FireBoss");
            originalBossTextures["FinalBoss"] = ImgHandler.LoadTex2D("Bosses/FinalBoss");
            originalBossTextures["EarthBoss"] = ImgHandler.LoadTex2D("Bosses/EarthBoss");
            originalBossTextures["IceBoss"] = ImgHandler.LoadTex2D("Bosses/IceBoss");
            originalBossTextures["AirBoss"] = ImgHandler.LoadTex2D("Bosses/AirBoss");
            originalBossTextures["LightningBoss"] = ImgHandler.LoadTex2D("Bosses/LightningBoss");
            originalBossTextures["LightningBoss2"] = ImgHandler.LoadTex2D("Bosses/LightningBoss2");



            On.Boss.Start += (On.Boss.orig_Start orig, Boss self) =>
            {
                orig(self);
                self.gameObject.AddComponent<MaliceReskin>();
            };




            On.FireBoss.InitAttackPatterns += (On.FireBoss.orig_InitAttackPatterns orig, FireBoss self) =>
            {
                orig(self);

                if (MaliceActive)
                {
                    //Debug.Log("2");
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).maxTime /= 2f;
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).minTime /= 2f;
                    //Debug.Log("2.25");
                    
                    self.health.healthStat.Modify(healthMod, true);
                    //Debug.Log("2.5");
                    
                    //Debug.Log("2.75");
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.lineState,
                        self.barrageState,
                        self.flareState,
                        self.comboState,
                        self.vacuumState
                    });
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.barrageState,
                        self.lineState,
                        self.comboState,
                        self.flareState,
                        self.heelState
                    });
                    //Debug.Log("3");

                    //Debug.Log("3.25");
                    
                }
            };

            On.IceBoss.InitAttackPatterns += (On.IceBoss.orig_InitAttackPatterns orig, IceBoss self) =>
            {
                orig(self);

                if (MaliceActive)
                {
                    //Debug.Log("2");
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).maxTime/=2f;
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).minTime /= 2f;
                    //Debug.Log("2.25");
                    self.element = ElementType.Fire;
                    self.health.healthStat.Modify(healthMod, true);
                    //Debug.Log("2.5");

                    //Debug.Log("2.75");
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.barrageState,
                        self.swordState,
                        self.beamState,
                        self.bombState,
                        self.lanceState
                    });
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.barrageState,
                        self.lanceState,
                        self.blizzardState,
                        self.beamState,
                        self.dashState
                    });

                    
                }
            };
            
            On.EarthBoss.InitAttackPatterns += (On.EarthBoss.orig_InitAttackPatterns orig, EarthBoss self) =>
            {
                orig(self);

                if (MaliceActive)
                {
                    //Debug.Log("2");
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).maxTime /= 2f;
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).minTime /= 2f;
                    //Debug.Log("2.25");
                    self.element = ElementType.Water;
                    self.health.healthStat.Modify(healthMod, true);
                    //Debug.Log("2.5");

                    //Debug.Log("2.75");
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.drillState,
                        self.pillarState,
                        self.leapState,
                        self.drillState,
                        self.rockState
                    });
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.leapState,
                        self.poisonState,
                        self.drillState,
                        self.pillarState,
                        self.wallState
                    });
                }
            };

            On.AirBoss.InitAttackPatterns += (On.AirBoss.orig_InitAttackPatterns orig, AirBoss self) =>
            {
                orig(self);

                if (MaliceActive)
                {
                    //Debug.Log("2");
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).maxTime /= 2f;
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).minTime /= 2f;
                    //Debug.Log("2.25");
                    self.element = ElementType.Earth;
                    self.health.healthStat.Modify(healthMod, true);
                    //Debug.Log("2.5");

                    //Debug.Log("2.75");
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.pressureState,
                        self.railState,
                        self.vortexState,
                        self.starState,
                        self.tornadoState
                    });
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.twisterState,
                        self.railState,
                        self.starState,
                        self.vortexState,
                        self.pressureState
                    });
                }
            };

            On.LightningBoss.InitAttackPatterns += (On.LightningBoss.orig_InitAttackPatterns orig, LightningBoss self) =>
            {
                orig(self);

                if (MaliceActive)
                {
                    //Debug.Log("2");
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).maxTime /= 2f;
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).minTime /= 2f;
                    //Debug.Log("2.25");
                    self.element = ElementType.Air;
                    self.health.healthStat.Modify(healthMod, true);
                    //Debug.Log("2.5");

                    //Debug.Log("2.75");
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.blenderState,
                        self.boxState,
                        self.chainState,
                        self.javelinState,
                        self.dashState
                    });
                    self.attackPatternList.Add(new List<Boss.BossSkillState>
                    {
                        self.teleState,
                        self.netState,
                        self.javelinState,
                        self.dashState,
                        self.drumState
                    });
                }
            };

            On.FinalBoss.InitAttackPatterns += (On.FinalBoss.orig_InitAttackPatterns orig, FinalBoss self) =>
            {
                orig(self);

                if (MaliceActive)
                {
                    //Debug.Log("2");
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).maxTime /= 2f;
                    ((Boss.BossIdleState)self.fsm.states["Idle"]).minTime /= 2f;
                    //Debug.Log("2.25");
                    //self.element = ElementType.Air;
                    self.health.healthStat.Modify(healthMod, true);
                    
                    //Debug.Log("2.75");
                }
            };

        }

        public static bool MaliceActive
        {
            get
            {
                bool b = false;
                foreach(GameObject player in GameController.players)
                {
                    if (player.GetComponent<Player>().inventory.ContainsItem("Mythical::TokenMalice"))
                    {
                        b = true;
                    }
                }
                return b;
            }
        }
        public static Dictionary<string, DialogEntry> maliceDialog = new Dictionary<string, DialogEntry>();
    }



    public class TokenMalice : NpcTokenItem
    {
        // Token: 0x06002687 RID: 9863 RVA: 0x00114E9C File Offset: 0x0011329C
        public TokenMalice() : base("Mythical::TokenMalice")
        {
        }
        // Token: 0x04002AD1 RID: 10961
        public const string constID = "Mythical::TokenMalice";
    }

    public class MaliceReskin : MonoBehaviour
    {
        public Boss b;
        public void Start()
        {
            b = GetComponent<Boss>();
        }

        public void Update()
        {
            if (b is AirBoss && MaliceAdditions.MaliceActive)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetTexture("_MainTex", MaliceAdditions.newBossTextures["AirBoss"]);
                b.spriteRenderer.SetPropertyBlock(block);
                b.element = ElementType.Earth;
            }
            if (b is FireBoss && MaliceAdditions.MaliceActive)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetTexture("_MainTex", MaliceAdditions.newBossTextures["FireBoss"]);
                b.spriteRenderer.SetPropertyBlock(block);
                b.element = ElementType.Lightning;
            }
            if (b is EarthBoss && MaliceAdditions.MaliceActive)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetTexture("_MainTex", MaliceAdditions.newBossTextures["EarthBoss"]);
                b.spriteRenderer.SetPropertyBlock(block);
                b.element = ElementType.Water;
            }
            if (b is IceBoss && MaliceAdditions.MaliceActive)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetTexture("_MainTex", MaliceAdditions.newBossTextures["IceBoss"]);
                b.spriteRenderer.SetPropertyBlock(block);
                b.element = ElementType.Fire;

            }
            if (b is FinalBoss && MaliceAdditions.MaliceActive)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetTexture("_MainTex", MaliceAdditions.newBossTextures["FinalBoss"]);
                b.spriteRenderer.SetPropertyBlock(block);

            }

            if (b is LightningBoss && MaliceAdditions.MaliceActive)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                if (b.spriteRenderer.sprite.texture.name.Contains("2")) {
                    block.SetTexture("_MainTex", MaliceAdditions.newBossTextures["LightningBoss2"]);
                }
                else {
                    block.SetTexture("_MainTex", MaliceAdditions.newBossTextures["LightningBoss"]);
                } 
                b.spriteRenderer.SetPropertyBlock(block);
                b.element = ElementType.Air;
            }

        }
    }

}
