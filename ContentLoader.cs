using BepInEx;
using BepInEx.Configuration;
using LegendAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XUnity.ResourceRedirector;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Mythical {

    #region BepInPlugin Notes
    // The BepInPlugin attribute in [brackets] is setting parameters for bepin to load your mod

    // GUID: "TheTimeSweeper.CameraModExample"
    // The the identifier for your mod. This one simply follows the convention of "AuthorName.YourModName"

    // Name: "CameraModExample"
    // The the full human-readable name of your mod.

    // Version: "0.1.0"
    // The Version, as you'd expect, useful for differing between updates.
    //     Customary to follow Semantic Versioning (major.minor.patch). 
    //         You don't have to, but you'll just look silly in front of everyone. It's ok. I won't make fun of you.
    #endregion

    [BepInDependency("TheTimeSweeper.CustomPalettes", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("Amber.TournamentEdition", "Tournament Edition", "2.4.6")]
    public class ContentLoader : BaseUnityPlugin {
        #region BaseUnityPlugin Notes
        // BaseUnityPlugin is the main class that gets loaded by bepin.
        // It inherits from MonoBehaviour, so it gains all the familiar Unity callback functions you can use: 
        //     Awake, Start, Update, FixedUpdate, etc.

        //     Awake is most important. it's basically where we initialize everything we do

        //     For further reading, you can check out https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

        // now, close these two Notes regions so the script looks little nicer to work with 
        #endregion
        public static ConfigEntry<int> configContestantCount;
        public static ConfigEntry<bool> enableTicket;
        public static ConfigEntry<bool> lockGemCount;
        public static ConfigEntry<string> OnlineID;
        public static ConfigEntry<bool> enableSkills;
        public static ConfigEntry<bool> enableItems;
        public static ConfigEntry<bool> enableCursedItems;
        public static ConfigEntry<bool> enableTedRobes;
        public static ConfigEntry<bool> enableCustomRobes;
        public static ConfigEntry<bool> enableDragonFallChanges;
        //----------------
        public static List<string> bannedArcana = new List<string>();
        public static Dictionary<string, Texture2D> particles = new Dictionary<string, Texture2D>();
        public static Dictionary<string, HeadgearDef> headgears = new Dictionary<string, HeadgearDef>();
        public static Dictionary<string, UnityEngine.Color> trails = new Dictionary<string, UnityEngine.Color>();
        public static Dictionary<string, UnityEngine.Color> trails2 = new Dictionary<string, UnityEngine.Color>();

        public static void RegisterTrail(string id, UnityEngine.Color clr1, UnityEngine.Color clr2)
        {
            trails[id] = clr1;
            if (clr2 != null)
            {
                trails2[id] = clr2;
            }
        }


        public List<Sprite> titleScreens = new List<Sprite>();
        public bool hasAddedTitleCards;
        public static bool ChaosDrops = false;
        public static bool UseBanlist = false;
        public static bool SpawnChests = false;
        public static bool ResetGems = false;
        public static bool FreezeStartPositions = false;
        public static bool Malice = false;
        public static int RainbowStartIndex = -1;
        // This Awake() function will run at the very start when the mod is initialized

        public Sprite cherrySprite;
        public Sprite orangeSprite;
        
        void CreateConfig()
        {
            const string tedSection = "Tournament Edition";

            configContestantCount =
                Config.Bind<int>(tedSection,
                                 "Contestants",
                                 6,
                                 "How many Contestants you want to spawn. Defaults to 6.");
            enableCursedItems =
                Config.Bind<bool>(tedSection,
                                 "Enable Cursed Relics",
                                 true,
                                 "Add cursed relics to Tomi and allow them to be equipped/unequipped at will");
            lockGemCount =
                Config.Bind<bool>(tedSection,
                                 "Lock Gem Count",
                                 true,
                                 "Do you want the gem count to reset to 999 on stage load?");
            ResetGems =
                Config.Bind<bool>(tedSection,
                                 "Reset Gems",
                                 false,
                                 "Reset gems to the maximum value?").Value;
            OnlineID =
                Config.Bind<string>(tedSection,
                                 "Online ID",
                                 "DefaultID",
                                 "What your Online ID is for online connections. (currently unused)");

            const string contentSection = "Enable/Disable Content";

            const string qolSection = "Enable/Disable QOL changes";

            enableTedRobes =
                Config.Bind<bool>(contentSection,
                                 "Enable TED Robes",
                                 true,
                                 "Enables built-in robes from TED");
            enableCustomRobes =
                Config.Bind<bool>(contentSection,
                                 "Enable Custom Robes",
                                 true,
                                 "Enables adding custom .robe files from the Custom Robes folder");

            enableSkills =
                Config.Bind<bool>(contentSection,
                                 "Enable Arcana",
                                 true,
                                 "Set false to disable adding custom arcana from TED");

            enableItems =
                Config.Bind<bool>(contentSection,
                                 "Enable Relics",
                                 true,
                                 "Set false to disable adding relics from TED");
            Malice =
                Config.Bind<bool>(contentSection,
                                 "TOKEN OF MALICE",
                                 false,
                                 "SURA'S REPENTANCE IS UPON HIM (requires enable relics)").Value;
            enableTicket =
                Config.Bind<bool>(contentSection,
                                 "???",
                                 false,
                                 "Enable a special item for a special event? (too late!)");
            enableDragonFallChanges =
                Config.Bind<bool>(qolSection,
                                 "Dragon Fall Changes",
                                 true,
                                 "Should Dragon Fall absorb arcana in PVP?");
        }

        public int nextAssignableID = 32;

        public List<string> robeNames = new List<string>();

        void Awake() {

            CreateConfig();

            //Skills.Awake();
            if (enableSkills.Value) {
                SampleSkillLoader.Awake();
            }
            //UnityEngine.Texture2D img = ImgHandler.LoadTex2D("icon");
            //WindowIconTools.SetIcon(img.GetRawTextureData(), img.width, img.height, WindowIconKind.Big);
            //Screen.SetResolution(1200, 675, false);

            //OnlineSupport.Hooks();
            ContestantChanges.Init();
            UltraCouncilChallenge.Init();

            if (enableTedRobes.Value) {
                AddALLTHEROBES();
            }

            if (enableCustomRobes.Value) {
                AddCustomRobes();
            }

            try { 

            string banListPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Banlist.txt");
            bannedArcana = File.ReadAllLines(banListPath).ToList();

            } catch
            {
                bannedArcana = new List<string>();
            }

            // Title screen additions

            titleScreens.Add(ImgHandler.LoadSprite("bg1"));
            /*tleScreens.Add(ImgHandler.LoadSprite("bg2"));
            titleScreens.Add(ImgHandler.LoadSprite("bg3"));
            titleScreens.Add(ImgHandler.LoadSprite("bg4"));
            titleScreens.Add(ImgHandler.LoadSprite("bg5"));
            titleScreens.Add(ImgHandler.LoadSprite("bg6"));*/
            titleScreens.Add(ImgHandler.LoadSprite("bg7"));

            string path3 = "Custom Titles";
            string text21 = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path3);
            string[] fileEntries2 = Directory.GetFiles(text21);

            foreach (string file in fileEntries2) {
                if (!file.EndsWith(".txt")) {
                    titleScreens.Add(ImgHandler.LoadSpriteFull(file));
                }
            }

            /*
            // Item Spawner revisions

            On.ItemSpawner.Reset += (On.ItemSpawner.orig_Reset orig, ItemSpawner self) =>
            {
                foreach (ItemSpawner.ItemPoolEntry itemPoolEntry in self.itemPoolDict.Values)
                {
                    foreach (PickupItem pickupItem in itemPoolEntry.itemArray)
                    {
                        try
                        {
                            pickupItem.Disable();
                        }
                        catch
                        {
                            //Debug.Log("Cleaning up item failed");
                        }
                    }
                }
            };*/


            // Disable Drops

            On.Destructible.Break += (On.Destructible.orig_Break orig, Destructible self) => {
                Debug.Log("Checking if Destructible broke");
                if (announcementPairs.ContainsKey(self.gameObject)) {
                    announcementPairs.Remove(self.gameObject);
                }
                if (self.name.Contains("NoPickups") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Spell Drops Disabled", 3f);
                    Debug.Log("No drops");
                    SpawnPickups = false;
                }
                if (self.name.Contains("NoEffects") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Stage Effects Disabled", 3f);
                    Debug.Log("No effects");
                    StageEffects = false;
                }
                if (self.name.Contains("NoHazards") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Stage Hazards Disabled", 3f);
                    Debug.Log("No hazards");
                    StageHazards = false;
                }
                if (self.name.Contains("MonoDrops") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Mono Element Drops", 3f);
                    Debug.Log("Mono Drops");
                    MonoElementDrops = true;
                }
                if (self.name.Contains("BestTo3") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Match Will Be First To 3", 3f);
                    Debug.Log("Best To 3");
                    BestTo3 = true;
                }
                if (self.name.Contains("Depletion") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Health Will Drain", 3f);
                    Debug.Log("Depletion");
                    Depletion = true;
                }
                if (self.name.Contains("SpawnMB") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Bosses Will Spawn", 3f);
                    Debug.Log("Spawn Bosses");
                    SpawnMiniBoss = true;
                }
                if (self.name.Contains("SaveArcana") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Arcana Will Be Saved", 3f);
                    Debug.Log("Save Arcana");
                    SaveArcana = true;
                }

                if (self.name.Contains("NoBuffs") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Robe Effects Disabled", 3f);
                    Debug.Log("Disable Buffs");

                    foreach (Player p in GameController.activePlayers) {
                        Outfit.GetAvailableOutfit(p.outfitID).SetMods(false, false);
                    }

                    RobeBuffs = false;
                }

                if (self.name.Contains("UseBanList") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Arcana Banlist Enabled", 3f);
                    Debug.Log("Arcana Banlist Enabled");

                    UseBanlist = true;
                }

                if (self.name.Contains("Chest") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Chests spawn instead of Arcana", 3f);
                    Debug.Log("Chest Spawns");

                    SpawnChests = true;
                }

                if (self.name.Contains("Freeze") && inPVPScene) {
                    GameUI.BroadcastNoticeMessage("Freeze Position Enabled", 3f);
                    Debug.Log("Freeze Enabled");

                    UseBanlist = true;
                }

                orig(self);
            };

            On.GameController.TogglePvpMode += (On.GameController.orig_TogglePvpMode orig, bool b) => {
                orig(b);
                if (!b) {
                    // Debug.Log("Killing all Mbosses");
                    foreach (MiniBoss mb in FindObjectsOfType<MiniBoss>()) {
                        mb.health.CurrentHealthValue = -1;
                        mb.fsm.QueueChangeState("Dead", false);
                        mb.health.AnnounceDeathEvent(mb);
                    }
                    //Debug.Log("Killing all Bosses");
                    foreach (Boss mb in FindObjectsOfType<Boss>()) {
                        mb.health.CurrentHealthValue = -1;
                        mb.fsm.QueueChangeState("Dead", false);
                        mb.health.AnnounceDeathEvent(mb);
                        Destroy(mb.gameObject, 5);
                    }
                    GameController.bosses.Clear();
                }

                if (b && SpawnMiniBoss) {
                    List<string> elements = new List<string>() { "Fire", "Earth", "Lightning", "Ice", "Air" };
                    List<Enemy.EName> bosses = new List<Enemy.EName>() { Enemy.EName.SuperKnight, Enemy.EName.SuperMage, Enemy.EName.SuperLancer, Enemy.EName.SuperArcher, Enemy.EName.SuperRogue, Enemy.EName.SuperCoffin };
                    //Debug.Log("Spawning Mbosses");
                    Enemy.Spawn(bosses[UnityEngine.Random.Range(0, bosses.Count)], ChaosArenaChanges.offset + Vector3.up * 6).chestLootTableID = String.Empty;
                    Enemy.Spawn(bosses[UnityEngine.Random.Range(0, bosses.Count)], ChaosArenaChanges.offset - Vector3.up * 6).chestLootTableID = String.Empty;
                    //Debug.Log("Spawning Boss");
                    //string str = elements[UnityEngine.Random.Range(0, 5)];
                    /*string str = "Final";
                    try
                    {

                        Boss boss = UnityEngine.Object.Instantiate<GameObject>(ChaosBundle.Get(bossPrefabFilePaths[str + "Boss"]), ChaosArenaChanges.offset, Quaternion.identity).GetComponent<Boss>();
                        boss.fsm.ChangeState(boss.bossReadyState.name, false);
                        boss.chestLootTableID = String.Empty;
                    }
                    catch { }*/
                }
            };
            //IL.PvpController.ResetStage += PvpController_ResetStage2;


            if (enableDragonFallChanges.Value)
            {
                IL.Player.UseDragonGrade.CheckForDragons += tmp;
            }

            On.PvpController.HandleSkillSpawn += (On.PvpController.orig_HandleSkillSpawn orig, PvpController self) => {
                if (SpawnPickups) {
                    if (!self.resetSkillStopwatch.IsRunning) {
                        self.resetSkillStopwatch.IsRunning = true;
                        LootManager.ResetAvailableSkills();
                    }
                    if (ChaosStopwatch.Check(self.skillSpawnStopwatchID)) {
                        self.skillSpawnStopwatchID = ChaosStopwatch.Begin(self.skillSpawnTime + UnityEngine.Random.Range(-1f, 1f), false, 0f, 0, 0);
                        self.droppedSkillID = LootManager.GetSkillID(false, false);
                        if (self.droppedSkillID == string.Empty) {
                            return;
                        }
                        self.skillSpawnLocIndex = (self.skillSpawnLocIndex + UnityEngine.Random.Range(1, self.skillSpawnLocCount)) % self.skillSpawnLocCount;
                        Vector3 vector = self.skillSpawnLocations[self.skillSpawnLocIndex];
                        Vector3 spawnLocation = vector;
                        foreach (GameObject p in GameController.players) {
                            if (p.GetComponent<Player>().inventory.ContainsItem("Mythical::Horseshoe")) {
                                spawnLocation = Vector3.Lerp(spawnLocation, p.transform.position, 0.4f);
                            }
                        }
                        int amount = 1;
                        string skillID = self.droppedSkillID;
                        bool droppedEmpowered = self.DroppedEmpowered;
                        if (!SpawnChests) {
                            LootManager.DropSkill(spawnLocation, amount, skillID, 0f, 0f, null, true, droppedEmpowered);
                        } else {
                            Instantiate(TreasureChest.Prefab, spawnLocation, Quaternion.identity);
                        }
                        vector.y -= 0.5f;
                        Player.SpawnAllyPortal(vector, 0.5f, true);
                    }
                }
            };

            On.PvpController.Start += (On.PvpController.orig_Start orig, PvpController self) => {
                orig(self);
                if (BestTo3) {
                    self.maxRoundCount += 2;
                }
            };

            On.PvpController.TogglePlayerInvulnerable += (On.PvpController.orig_TogglePlayerInvulnerable orig, PvpController self, bool b) => {
                for (int i = 0; i < GameController.players.Count; i++) {
                    Player p = GameController.players[i].GetComponent<Player>();
                    if (FreezeStartPositions) {
                        if (b) {
                            valueIndex[i] = p.movement.moveSpeedStat.CurrentValue;
                            p.movement.moveSpeedStat.CurrentValue = 0;
                        } else {
                            p.movement.moveSpeedStat.CurrentValue = valueIndex[i];
                        }


                    }
                }
                orig(self, b);
            };


            //List<Sprite> loadingTheListIDontNeedThisToAllocate = IconManager.TSBGSpriteList;
            On.IconManager.GetBGSprite += (On.IconManager.orig_GetBGSprite orig, int index) => {
                if (!hasAddedTitleCards) {
                    hasAddedTitleCards = true;
                    List<Sprite> loadingTheListIDontNeedThisToAllocate = IconManager.TSBGSpriteList;
                    TitleScreen.bgCount += titleScreens.Count;

                    List<Sprite> sprites = new List<Sprite>();

                    foreach (Sprite spr in titleScreens) {
                        sprites.Add(spr);
                    }
                    foreach (Sprite spr in IconManager.TSBGSpriteList) {
                        sprites.Add(spr);
                    }

                    IconManager.tsbgSpriteList = sprites;


                }
                return orig(index);
            };


            On.PvpController.ResetStage += PvpController_ResetStage;
            On.PvpController.ResetPlayers += PvpController_ResetPlayers;
            On.Player.SetPlayerOutfitColor += Us_PlayCredits;

            // Stage effects

            On.PvpController.ApplyAirBuffs += (On.PvpController.orig_ApplyAirBuffs orig, PvpController self) => {
                if (StageEffects) { orig(self); }
            };
            On.PvpController.ApplyFireBuffs += (On.PvpController.orig_ApplyFireBuffs orig, PvpController self) => {
                if (StageEffects) { orig(self); }
            };
            On.PvpController.ApplyEarthBuffs += (On.PvpController.orig_ApplyEarthBuffs orig, PvpController self) => {
                if (StageEffects) { orig(self); }
            };

            // Music

            LoadSong("TitleScreen", "Sprites/Vaporwave.ogg");
            LoadSong("Hub", "Sprites/Trap.ogg");
            LoadSong("Boss", "Sprites/Rock.ogg");

            BGMInfo bgm = new BGMInfo() {
                name = "PvP",
                fallback = BGMTrackType.Original,
                message = "Do you want to hear the Council's sick beats?",
                messageConfirm = "yee",
                messageCancel = "na",
                soundtrack = clipDict
            };

            LegendAPI.Music.Register(bgm);

            On.Player.Start += (On.Player.orig_Start orig, Player self) => {
                orig(self);
                self.gameObject.AddComponent<TrailTEDManager>();
                self.gameObject.AddComponent<TEDLineManager>();
                Headgear gear = Globals.ChaosInst<Headgear>(HorseMaskItem.Prefab,
                    (!self.headPosition) ? self.transform : self.headPosition.transform, null, null);
                /*gear.spriteVariations = new Sprite[] //Up,Right,Down,UR,DR
                {
                    ImgHandler.LoadSprite("headgear/heart1"),
                    ImgHandler.LoadSprite("headgear/heart2"),
                    ImgHandler.LoadSprite("headgear/heart3"),
                    ImgHandler.LoadSprite("headgear/heart4"),
                    ImgHandler.LoadSprite("headgear/heart5")
                };*/
                gear.gameObject.AddComponent<TEDHeadgear>();
                gear.Initialize(self, "TournamentEditionHeadgear", null);

            };

            // Boss, Hub, TitleScreen


            /* No Longer Necessary
             
            On.Player.Start += (On.Player.orig_Start orig, Player self) =>
            {
                orig(self);
                for (int i = 0; i < IconManager.WizardSpriteList.Count; i++)
                {
                    //IconManager.wizardSprites[playerSprites[i].name] = playerSprites[i];
                    string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Exports/");
                    Debug.Log("Saved File Here: " + text);
                    SaveSubSprite(ExtractAndName(IconManager.WizardSpriteList[i]), text);
                }
            };
            */
            //Insignia of Legend Changes

            On.PlayerWinItem.SetEventHandlers += (On.PlayerWinItem.orig_SetEventHandlers orig, PlayerWinItem self, bool b) => {
                orig(self, b);
                ChaosDrops = b;
            };

            On.DialogManager.InitDialogDictionary += (On.DialogManager.orig_InitDialogDictionary orig, DialogManager self, string path) => {
                orig(self, path);

                DialogEntries dialogEntries = JsonUtility.FromJson<DialogEntries>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sprites/newDialogue.txt")));
                foreach (DialogEntry dialogEntry in dialogEntries.entries) {
                    dialogEntry.Initialize();
                    Debug.Log("Adding " + dialogEntry.ID);
                    MaliceAdditions.maliceDialog.Add(dialogEntry.ID, dialogEntry);
                }
            };

            On.DialogManager.InitPortraitSprites += (On.DialogManager.orig_InitPortraitSprites orig, DialogManager self) => {
                orig(self);
                DialogManager.portraitSprites["IceQueenMalice"] = ImgHandler.LoadSprite("Bosses/IceQueenMalice");
                DialogManager.portraitSprites["EarthLordMalice"] = ImgHandler.LoadSprite("Bosses/EarthLordMalice");
                DialogManager.portraitSprites["FireBossMalice"] = ImgHandler.LoadSprite("Bosses/FireBossMalice");
                DialogManager.portraitSprites["AirBossMalice"] = ImgHandler.LoadSprite("Bosses/AirBossMalice");
                DialogManager.portraitSprites["LightningGirlMalice"] = ImgHandler.LoadSprite("Bosses/LightningGirlMalice");
                DialogManager.portraitSprites["LightningBossMalice"] = ImgHandler.LoadSprite("Bosses/LightningBossMalice");
                DialogManager.portraitSprites["FinalBossMalice"] = ImgHandler.LoadSprite("Bosses/FinalBossMalice");
                DialogManager.portraitSprites["FinalBossMalice2"] = ImgHandler.LoadSprite("Bosses/FinalBossMalice2");
            };

            On.DialogManager.InitSpeakerDictionary += (On.DialogManager.orig_InitSpeakerDictionary orig, DialogManager self, string path) => {
                orig(self, path);
                DialogSpeakers dialogSpeakers = JsonUtility.FromJson<DialogSpeakers>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sprites/newSpeakers.txt")));
                for (int i = 0; i < dialogSpeakers.speakers.Length; i++) {
                    Debug.Log("Adding " + dialogSpeakers.speakers[i].speakerID);
                    DialogManager.speakerDict.Add(dialogSpeakers.speakers[i].speakerID, dialogSpeakers.speakers[i]);
                }
            };

            On.GameController.Start += (On.GameController.orig_Start orig, GameController self) => {
                orig(self);
                // Chaos arena changes
                if (!addedGMHooks) {
                    On.SBPlayerPageUI.Load += (On.SBPlayerPageUI.orig_Load orig2, SBPlayerPageUI self2, Player p, Dictionary<SpellBookUI.SkillEquipType, int> playerSkillSlots) => {
                        try {
                            orig2(self2, p, playerSkillSlots);
                        }
                        catch {
                            try {
                                self2.spellIconArray[0].sprite = IconManager.GetSkillIcon(p.assignedSkills[playerSkillSlots[SpellBookUI.SkillEquipType.Basic]].skillID);
                            }
                            catch { }
                            try {
                                self2.spellIconArray[1].sprite = IconManager.GetSkillIcon(p.assignedSkills[playerSkillSlots[SpellBookUI.SkillEquipType.Dash]].skillID);
                            }
                            catch { }
                            try {
                                self2.spellIconArray[2].sprite = IconManager.GetSkillIcon(p.assignedSkills[playerSkillSlots[SpellBookUI.SkillEquipType.Optional]].skillID);
                            }
                            catch { }
                            try {
                                self2.spellIconArray[3].sprite = IconManager.GetSkillIcon(p.assignedSkills[playerSkillSlots[SpellBookUI.SkillEquipType.Signature]].skillID);
                            }
                            catch { }
                            self2.selectPrompt.sprite = GameUI.GetInputSprite(p.inputDevice.inputScheme, "Confirm");
                            self2.cancelPrompt.sprite = GameUI.GetInputSprite(p.inputDevice.inputScheme, "Cancel");
                        }
                    };

                    On.TextManager.GetUIText += (On.TextManager.orig_GetUIText orig2, string t) => {
                        if (MaliceAdditions.MaliceActive) {
                            if (t == "enemyName-bossFire") { return "Malignant Zeal"; }
                            if (t == "enemyName-bossChaos") { return "Malignant Sura"; }
                            if (t == "enemyName-bossIce") { return "Malignant Freiya"; }
                            if (t == "enemyName-bossEarth") { return "Malignant Atlas"; }
                            if (t == "enemyName-bossAir") { return "Malignant Shuu"; }
                            if (t == "enemyName-bossLightning") { return "Twins of the New Moon"; }
                        }

                        return orig2(t);
                    };
                    On.LootManager.GetSkillID += (On.LootManager.orig_GetSkillID orig2, bool l, bool s) => {
                        string finalResult = "";
                        while (true) {
                            if (ChaosDrops && inAPVPScene && UnityEngine.Random.value < 0.25f) {
                                finalResult = LootManager.chaosSkillList[UnityEngine.Random.Range(0, LootManager.chaosSkillList.Count)];
                            } else if (MonoElementDrops && inAPVPScene) {

                                List<ElementType> usableElements = new List<ElementType>();
                                if (monoskills.Count == 0) {
                                    for (int i = 0; i < GameController.players.Count; i++) {
                                        usableElements.Add(GameController.players[i].GetComponent<Player>().assignedSkills[0].element);
                                    }

                                    for (int i = 0; i < LootManager.completeSkillList.Count; i++) {
                                        if (usableElements.Contains(GameController.players[0].GetComponent<Player>().GetSkill(LootManager.completeSkillList[i]).element)) {
                                            // Debug.Log("Added " + LootManager.completeSkillList[i]);
                                            monoskills.Add(LootManager.completeSkillList[i]);
                                        }
                                    }
                                }
                                if (monoskills.Count > 0) {
                                    finalResult = monoskills[UnityEngine.Random.Range(0, monoskills.Count)];
                                } else {
                                    return orig2(l, s);
                                }
                            } else {
                                finalResult = orig2(l, s);
                            }

                            if (!ContentLoader.UseBanlist || !bannedArcana.Contains(finalResult)) {
                                break;
                            }

                        }
                        return finalResult;
                    };

                    On.LootManager.DropSkill += (On.LootManager.orig_DropSkill orig3, Vector3 v, int a, string id, float l, float s, HashSet<ElementType> set, bool life, bool emp) => {
                        if (inAPVPScene && LootManager.chaosSkillList.Contains(id)) {
                            emp = true;
                        }
                        orig3(v, a, id, l, s, set, life, emp);
                    };



                    addedGMHooks = true;
                    ChaosArenaChanges.Init();

                    if (enableCursedItems.Value) {
                        CursedRelics.Init();
                    }
                }
            };

            /*On.TitleScreen.Awake += (orig, self) => {
                orig(self);
                self.extraVersion.text += $" - {BepInEx.Bootstrap.Chainloader.PluginInfos.Count} Mods";
                var newOption = GameObject.Instantiate(self.transform.Find("TitleMenu/Options").gameObject, self.transform.Find("TitleMenu"));
                newOption.name = "Mods";
                var menuopts = self.menuTextOpts.ToList();
                menuopts.Add(newOption.GetComponent<Text>());
                newOption.GetComponent<Text>().text = "Mods";
                var trigger = new UnityEngine.EventSystems.EventTrigger.Entry();
                newOption.GetComponent<UnityEngine.EventSystems.EventTrigger>().triggers[0] = trigger;
                trigger.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
                trigger.callback.AddListener((data) => self.SelectMenuIndex(6));
                var delta = menuopts[0].transform.position.y - menuopts[1].transform.position.y;
                for (int i = 3; i < menuopts.Count - 1; i++) {
                    menuopts[i].rectTransform.anchoredPosition3D = new Vector3(0, -1 * delta, 0);
                }
                self.menuTextOpts = menuopts.ToArray();
                //menuobj.transform.SetParent(GameUI.Instance.transform);
            };
            IL.TitleScreen.HandleNavigation += (il) => {
                var c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After, x => x.MatchLdfld(typeof(TitleScreen).GetField("currentMenuIndex")))) {
                    c.EmitDelegate<Func<int, int>>((orig) => orig == 2 ? 5 : orig == 6 ? 2 : orig == 5 ? -1 : orig);
                }
                if (c.TryGotoNext(x => x.MatchSub()) && c.TryGotoPrev(MoveType.After, x => x.MatchLdfld(out _))) {
                    c.EmitDelegate<Func<int, int>>((orig) => orig == 3 ? 7 : orig == 6 ? 3 : orig == 0 ? 6 : orig);
                }
            };
            On.TitleScreen.ConfirmMenuOption += (orig, self) => {
                if (self.currentState == TitleScreen.TitleScreenState.Menu && self.currentMenuIndex == 6) {
                    //ModMenuUI.Instance?.Toggle();
                }
                orig(self);
            };*/

            On.TitleScreen.AllowMultiplayer += (On.TitleScreen.orig_AllowMultiplayer orig, TitleScreen self) => {
                return true;
            };


            /*On.Outfit.OutfitIsInUse += (On.Outfit.orig_OutfitIsInUse orig, string id, int playerID) =>
            {
                if (GameController.activePlayers.Length > 1)
                {
                    return GameDataManager.gameData.playerData[playerID].outfitName == id;
                } else
                {
                    return orig(id, playerID);
                }
            };*/

            On.Outfit.SetMods += (On.Outfit.orig_SetMods orig, Outfit self, bool b, bool b2) => {
                orig(self, b && (RobeBuffs), b2 && (RobeBuffs));
            };

            On.Health.TakeDamage += (On.Health.orig_TakeDamage orig, Health self, AttackInfo givenAttackInfo, Entity attackEntity, bool critPreCalculated) => {
                bool b = orig(self, givenAttackInfo, attackEntity, critPreCalculated);
                if (self.entityScript is Player) {
                    if (((Player)self.entityScript).inventory.ContainsItem("Mythical::StunDown")) {
                        self.entityScript.hitStunDurationModifier *= 0.5f;
                    }
                }


                return b;
            };

            if (enableItems.Value) {
                AddItems();
            }
            //Adjustments
            /*On.PlatWallet.ctor += (On.PlatWallet.orig_ctor orig, PlatWallet self, int i) =>
            {
                orig(self,i);
                self.maxBalance = 9999;
                self.balance = 9999;
            };*/


        }

        private static void AddItems() {
            /*if (enableTicket.Value) {
                ItemInfo itemInfo2 = new ItemInfo();
                itemInfo2.name = "PrimeTicket";
                itemInfo2.item = new PrimeTicket();
                itemInfo2.tier = 1;
                TextManager.ItemInfo txt = default(TextManager.ItemInfo);
                txt.displayName = "Arcana Ticket";
                txt.description = "With everyone that falls, a message in their wake.";
                txt.itemID = PrimeTicket.staticID;
                Sprite spr = ImgHandler.LoadSprite("ticket");
                itemInfo2.text = txt;
                itemInfo2.icon = ((spr != null) ? spr : null);
                Items.Register(itemInfo2);
            }*/

            ItemInfo itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::SevenFlushChaos";
            itemInfo.item = new SevenFlushChaos();
            itemInfo.tier = 1;
            TextManager.ItemInfo text2 = default(TextManager.ItemInfo);
            text2.displayName = "Neve's Onyx";
            text2.description = "Holding seven chaos arcana greatly increases damage! You slowly take damage when this is active!";
            text2.itemID = SevenFlushChaos.staticID;
            Sprite itemsprite = ImgHandler.LoadSprite("neveschaos");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::TokenCringe";
            itemInfo.item = new TokenCringe();
            itemInfo.tier = 1;
            itemInfo.priceMultiplier = 3;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Token of Cringe";
            text2.description = "Ooooooh the dashing I can't stand the dashing";
            text2.itemID = TokenCringe.constID;
            itemsprite = ImgHandler.LoadSprite("tokenCringe");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::EnhanceFire";
            itemInfo.item = new EnhanceFire();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Malignant Garnet";
            text2.description = "Enhance all Fire arcana! Unenhance and weaken all other arcana!";
            text2.itemID = EnhanceFire.staticID;
            itemsprite = ImgHandler.LoadSprite("enhancefire");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::EnhanceWater";
            itemInfo.item = new EnhanceWater();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Malignant Diamond";
            text2.description = "Enhance all Water arcana! Unenhance and weaken all other arcana!";
            text2.itemID = EnhanceWater.staticID;
            itemsprite = ImgHandler.LoadSprite("enhancewater");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::EnhanceEarth";
            itemInfo.item = new EnhanceEarth();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Malignant Jade";
            text2.description = "Enhance all Earth arcana! Unenhance and weaken all other arcana!";
            text2.itemID = EnhanceEarth.staticID;
            itemsprite = ImgHandler.LoadSprite("enhanceearth");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::EnhanceAir";
            itemInfo.item = new EnhanceAir();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Malignant Opal";
            text2.description = "Enhance all Air arcana! Unenhance and weaken all other arcana!";
            text2.itemID = EnhanceAir.staticID;
            itemsprite = ImgHandler.LoadSprite("enhanceair");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::EnhanceLightning";
            itemInfo.item = new EnhanceLightning();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Malignant Topaz";
            text2.description = "Enhance all Lightning arcana! Unenhance and weaken all other arcana!";
            text2.itemID = EnhanceLightning.staticID;
            itemsprite = ImgHandler.LoadSprite("enhancelightning");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "Mythical::StunDown";
            itemInfo.item = new StunDown();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Amber's Jewelry";
            text2.description = "You are stunned for less time, but you take more damage!";
            text2.itemID = StunDown.staticID;
            itemsprite = ImgHandler.LoadSprite("stunDown");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "SurfsGambit";
            itemInfo.item = new SurfsGambit();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Surf's Gambit";
            text2.description = "You gain 30% more damage, at the cost of losing your standard arcana!";
            text2.itemID = SurfsGambit.staticID;
            itemsprite = ImgHandler.LoadSprite("gambit");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "BlinkModule";
            itemInfo.item = new BlinkModule();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Blink Module";
            text2.description = "Your dashes are longer and have more endlag!";
            text2.itemID = BlinkModule.staticID;
            itemsprite = ImgHandler.LoadSprite("blink");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            itemInfo = new ItemInfo();
            itemInfo.name = "PetSquid";
            itemInfo.item = new PetSquid();
            itemInfo.tier = 1;
            text2 = default(TextManager.ItemInfo);
            text2.displayName = "Amber's Pet Squid";
            text2.description = "Taking damage in rapid succession releases a burst of bubbles!";
            text2.itemID = BlinkModule.staticID;
            itemsprite = ImgHandler.LoadSprite("squid");
            itemInfo.text = text2;
            itemInfo.icon = ((itemsprite != null) ? itemsprite : null);
            Items.Register(itemInfo);

            if (Malice) {
                MaliceAdditions.Init();
            }
        }

        public int AddPalette(string file)
        {
            return CustomPalettes.Palettes.AddPalette(ImgHandler.LoadTex2D(file));
        }

        public int AddPalette(byte[] bytes)
        {
            return CustomPalettes.Palettes.AddPalette(ImgHandler.LoadTex2D("", T2D: ImgHandler.LoadPNGAlt(bytes)));
        }

        private void AddALLTHEROBES() {

            OutfitInfo sev = new OutfitInfo();
            sev.name = "Replica";
            sev.outfit = new Outfit("Mythical::Replica", 0, new List<OutfitModStat>
                {
                    new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, 0.05f, 0f, false),
                    new OutfitModStat(OutfitModStat.OutfitModType.Speed, 0f, 0.1f, 0f, false),
                    new OutfitModStat(OutfitModStat.OutfitModType.Evade, 0.05f, 0f, 0f, false),
                    new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
                }, false, false);
            Outfits.Register(sev);

            OutfitInfo outfitInfo = new OutfitInfo();
            outfitInfo.name = "Walter";
            outfitInfo.outfit = new global::Outfit("Mythical::Walter", AddPalette("walter"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo.customDesc = ((b, outfitModStat) => "The Living Menace.");
            particles.Add("Mythical::Walter", ImgHandler.LoadTex2D("walterParticle"));
            Outfits.Register(outfitInfo);

            OutfitInfo outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Guardian";
            outfitInfo2.outfit = new global::Outfit("Mythical::Guardian", AddPalette("guardian"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "The Defender");
            Outfits.Register(outfitInfo2);

            OutfitInfo outfitInfo7 = new OutfitInfo();
            outfitInfo7.name = "Scholar";
            outfitInfo7.outfit = new global::Outfit("Mythical::Scholar", AddPalette("scholar"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo7.customDesc = ((b, outfitModStat) => "The Keeper Of Knowledge");
            Outfits.Register(outfitInfo7);

            OutfitInfo outfitInfo6 = new OutfitInfo();
            outfitInfo6.name = "Fear";
            outfitInfo6.outfit = new global::Outfit("Mythical::Fear", AddPalette("fear"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo6.customDesc = ((b, outfitModStat) => "The Everlasting Terror");
            Outfits.Register(outfitInfo6);

            OutfitInfo outfitInfo5 = new OutfitInfo();
            outfitInfo5.name = "Conquest";
            outfitInfo5.outfit = new global::Outfit("Mythical::Conquest", AddPalette("conquest"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo5.customDesc = ((b, outfitModStat) => "The Iron Fist");
            Outfits.Register(outfitInfo5);



            OutfitInfo outfitInfo4 = new OutfitInfo();
            outfitInfo4.name = "Tycoon";
            outfitInfo4.outfit = new global::Outfit("Mythical::Tycoon", AddPalette("tycoon"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo4.customDesc = ((b, outfitModStat) => "The Invisible Hand");
            particles.Add("Mythical::Tycoon", ImgHandler.LoadTex2D("tycoonParticle"));
            Outfits.Register(outfitInfo4);

            OutfitInfo outfitInfo3 = new OutfitInfo();
            outfitInfo3.name = "Surf";
            outfitInfo3.outfit = new global::Outfit("Mythical::Surf", AddPalette("surf"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo3.customDesc = ((b, outfitModStat) => "The Challenger");
            Outfits.Register(outfitInfo3);
            RegisterTrail("Mythical::Surf", new UnityEngine.Color(0.75f, 1f, 1, 0.8f), new UnityEngine.Color(0.75f, 1f, 1, 0.3f));

            // New ones
            OutfitInfo outfitInfo9 = new OutfitInfo();
            outfitInfo9.name = "Vision";
            outfitInfo9.outfit = new global::Outfit("Mythical::Vision", AddPalette("vision"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo9.customDesc = ((b, outfitModStat) => "The All Seeing Eye");
            Outfits.Register(outfitInfo9);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Archaic";
            outfitInfo2.outfit = new global::Outfit("Mythical::Archaic", AddPalette("archaic"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "From Days Past - only_going_up_fr0m_here");
            //Outfits.Register(outfitInfo2);

            outfitInfo = new OutfitInfo();
            outfitInfo.name = "Crimson";
            outfitInfo.outfit = new global::Outfit("Mythical::Crimson", AddPalette("crimson"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, true, false);
            outfitInfo.customDesc = ((b, outfitModStat) => "Designed by only_going_up_fr0m_here!");
            // Outfits.Register(outfitInfo);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Nemesis";
            outfitInfo2.outfit = new global::Outfit("Mythical::Nemesis", AddPalette("nemesis"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Wizard's Vestige");
            particles.Add("Mythical::Nemesis", ImgHandler.LoadTex2D("nemParticle"));
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Lotus";
            outfitInfo2.outfit = new global::Outfit("Mythical::Lotus", AddPalette("lotus"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Dying Petals");
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Malachite";
            outfitInfo2.outfit = new global::Outfit("Mythical::Malachite", AddPalette("malachite"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "The Bright");
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Opal";
            outfitInfo2.outfit = new global::Outfit("Mythical::Opal", AddPalette("opal"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Pearlescent Beauty");
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Roar";
            outfitInfo2.outfit = new global::Outfit("Mythical::Roar", AddPalette("roar"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Shouts of Arcanus");
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Patina";
            outfitInfo2.outfit = new global::Outfit("Mythical::Patina", AddPalette("patina"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Aged Like Wine");
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Pigmented";
            outfitInfo2.outfit = new global::Outfit("Mythical::Pigmented", AddPalette("pigmented"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "A Colorful Embrace");
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Cerberus";
            outfitInfo2.outfit = new global::Outfit("Mythical::Cerberus", AddPalette("cerberus"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Trash Taste, Loser");
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Abysmal";
            outfitInfo2.outfit = new global::Outfit("Mythical::Abysmal", AddPalette("abysmal"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "i feel like desperately trying to beg for more matches with covetous");
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Verde";
            outfitInfo2.outfit = new global::Outfit("Mythical::Verde", AddPalette("verde"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Better Call Saul!");
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Lunes";
            outfitInfo2.outfit = new global::Outfit("Mythical::Lunes", AddPalette("lunes"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Feliz Jueves");
            // Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Jaccablu";
            outfitInfo2.outfit = new global::Outfit("Mythical::Jaccablu", AddPalette("jaccablu"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Fire fades wills of steel to temper");
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Rumor";
            outfitInfo2.outfit = new global::Outfit("Mythical::Rumor", AddPalette("rumor"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Pay me no mind -- 1st Prize Halloween 2022");
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Mando";
            outfitInfo2.outfit = new global::Outfit("Mythical::Mando", AddPalette("mando"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "This is the Way -- 2nd Prize Halloween 2022");
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Sandstorm";
            outfitInfo2.outfit = new global::Outfit("Mythical::Sandstorm", AddPalette("sandstorm"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Darude -- 3rd Prize Halloween 2022");
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Genius";
            outfitInfo2.outfit = new global::Outfit("Mythical::Genius", AddPalette("genius"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Get schwifty -- 4th Prize Halloween 2022");
            Outfits.Register(outfitInfo2);
            RegisterTrail("Mythical::Genius", new UnityEngine.Color(1f, 1f, 1, 0.8f), new UnityEngine.Color(0.6f, 0.8f, 0.8f, 0.3f));

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Shade";
            outfitInfo2.outfit = new global::Outfit("Mythical::Shade", AddPalette("shade"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "I was born into this -- 5th Prize Halloween 2022");
            Outfits.Register(outfitInfo2);


            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Lover";
            outfitInfo2.outfit = new global::Outfit("Mythical::Lover", AddPalette("lover"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Happy Valentines Day!");
            Outfits.Register(outfitInfo2);
            headgears.Add("Mythical::Lover", new HeadgearDef() {
                sprites = new Sprite[]
                {
                    ImgHandler.LoadSprite("headgear/heart1"),
                    ImgHandler.LoadSprite("headgear/heart2"),
                    ImgHandler.LoadSprite("headgear/heart3"),
                    ImgHandler.LoadSprite("headgear/heart4"),
                    ImgHandler.LoadSprite("headgear/heart5")
                }
            });
            RegisterTrail("Mythical::Lover", new UnityEngine.Color(1, 0.8f, 0.8f, 0.5f), new UnityEngine.Color(1, 0.8f, 0.8f, 0.2f));

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Genesis";
            outfitInfo2.outfit = new global::Outfit("Mythical::Genesis", AddPalette("genesis"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Hood is off!");
            Outfits.Register(outfitInfo2);
            headgears.Add("Mythical::Genesis", new HeadgearDef() {
                sprites = new Sprite[]
                {
                    ImgHandler.LoadSprite("headgear/girl1"),
                    ImgHandler.LoadSprite("headgear/girl2"),
                    ImgHandler.LoadSprite("headgear/girl3"),
                    ImgHandler.LoadSprite("headgear/girl4"),
                    ImgHandler.LoadSprite("headgear/girl5")
                }
            });

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Icarus";
            outfitInfo2.outfit = new global::Outfit("Mythical::Icarus2", AddPalette("icarus"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false),
                new OutfitModStat(OutfitModStat.OutfitModType.Health,250,0,0,false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Melt'em Down! Reward for completing the Contestant Assault.");
            outfitInfo2.unlockCondition = () => {
                return PlayerPrefs.GetInt("mythical::CA", 0) == 1;
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Gaia";
            outfitInfo2.outfit = new global::Outfit("Mythical::Gaia2", AddPalette("gaia"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false),
                new OutfitModStat(OutfitModStat.OutfitModType.Fall,0,0,0,true)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "The Bearer of Life! Reward for completing the Contestant Assault.");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2, OutfitModStat modStat) {
                player.regenDelay = b2 ? 3 : 8;
            };
            outfitInfo2.unlockCondition = () => {
                return PlayerPrefs.GetInt("mythical::CA", 0) == 1;
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Goddess";
            outfitInfo2.outfit = new global::Outfit("Mythical::Goddess", AddPalette("goddess"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false),
                new OutfitModStat(OutfitModStat.OutfitModType.Health,0f,0.65f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Speed,0f,0.65f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Damage,0f,0.25f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.CritChance,0.2f,0,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Armor,0.25f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Evade,0.2f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Cooldown,-0f,-0.45f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.HealAmount,0.3f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Gold,0.1f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.HealCrit,0.1f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.ODRate,0.2f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.ODDamage,0.25f,0,0,false),
                //new OutfitModStat(OutfitModStat.OutfitModType.Health,-200,0,0,false),
                //new OutfitModStat(OutfitModStat.OutfitModType.Damage,0,0.2f,0,false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Awarded to the Lucky Few who completed the Ultra Council Challenge!");
            outfitInfo2.unlockCondition = () => {
                return PlayerPrefs.GetInt("mythical::UCC", 0) == 1;
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Challenger";
            outfitInfo2.outfit = new global::Outfit("Mythical::Challenger", AddPalette("challenger"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "Awarded to the Lucky Few who completed the Ultra Council Challenge!");
            outfitInfo2.unlockCondition = () => {
                return PlayerPrefs.GetInt("mythical::UCC", 0) == 1;
            };
            Outfits.Register(outfitInfo2);


            NevesChaosPalette = AddPalette("chaosrobe");

            

            for(int i = 0; i < 6; i++)
            {
                int ind = AddPalette("prism" + (i + 1).ToString());
                if ( i == 0)
                {
                    RainbowStartIndex = ind;
                }
            } 

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "P.R.I.S.M";
            outfitInfo2.outfit = new global::Outfit("Mythical::Prism", RainbowStartIndex, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((b, outfitModStat) => "You're gonna love this one!");
            Outfits.Register(outfitInfo2);
            rainbowOutfit = Outfits.OutfitCatalog[outfitInfo2.outfit.outfitID];
        }

        public static int RainbowIndex = 0;
        public static OutfitInfo rainbowOutfit = null;


        private void AddCustomRobes() {
            OutfitInfo outfitInfoCustom;

            string path2 = "Custom Robes";
            string robesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path2);
            string[] fileEntries = Directory.GetFiles(robesPath);

            foreach (string file in fileEntries) {
                if (file.EndsWith(".robe")) {
                    string data = File.ReadAllText(file);
                    string[] split = data.Split(new string[] { "***BREAK***" }, System.StringSplitOptions.RemoveEmptyEntries);
                    outfitInfoCustom = new OutfitInfo();
                    outfitInfoCustom.name = split[0];
                    string id = split[2] + "::" + split[0];
                    outfitInfoCustom.outfit = new global::Outfit(id, AddPalette(Convert.FromBase64String(split[3])), new List<global::OutfitModStat>
                    {
                        new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
                    }, false, false);
                    outfitInfoCustom.customDesc = ((b, outfitModStat) => split[1]);

                    if (split.Length > 4) {
                        UnityEngine.Color color1 = UnityEngine.Color.black;
                        UnityEngine.Color color2 = UnityEngine.Color.black;
                        int clr = 0;
                        for (int i = 4; i < split.Length; i++) {
                            if (split[i].StartsWith("particle::")) {
                                string s = split[i].Replace("particle::", "");
                                Texture2D tex = ImgHandler.LoadTex2D(id + " Particle", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(s)));
                                particles.Add(id, tex);
                                ;
                            }
                            if (split[i].StartsWith("color1::")) {
                                clr = 1;
                                string s = split[i].Replace("color1::", "");
                                string[] spl = s.Split('-');
                                float[] values = new float[] { float.Parse(spl[0]), float.Parse(spl[01]), float.Parse(spl[02]), float.Parse(spl[03]) };
                                color1 = new UnityEngine.Color(values[0], values[1], values[2], values[3]);
                            }
                            if (split[i].StartsWith("color2::")) {
                                clr = 2;
                                string s = split[i].Replace("color2::", "");
                                string[] spl = s.Split('-');
                                float[] values = new float[] { float.Parse(spl[0]), float.Parse(spl[01]), float.Parse(spl[02]), float.Parse(spl[03]) };
                                color2 = new UnityEngine.Color(values[0], values[1], values[2], values[3]);
                            }
                            if (split[i].StartsWith("headgear::")) {
                                string s = split[i].Replace("headgear::", "");
                                string[] spl = s.Split(new string[] { "**HEADGEAR**" }, System.StringSplitOptions.RemoveEmptyEntries);
                                HeadgearDef def = new HeadgearDef() {
                                    sprites = new Sprite[] {
                                        ImgHandler.LoadSprite(id + " Gear 1", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[0]))),
                                        ImgHandler.LoadSprite(id + " Gear 2", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[1]))),
                                        ImgHandler.LoadSprite(id + " Gear 3", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[2]))),
                                        ImgHandler.LoadSprite(id + " Gear 4", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[3]))),
                                        ImgHandler.LoadSprite(id + " Gear 5", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[4])))
                                    }
                                };

                                headgears.Add(id, def);

                            }
                        }
                        if (clr == 1) { RegisterTrail(id, color1, color1); } else if (clr == 2) { RegisterTrail(id, color1, color2); }


                    }

                    Outfits.Register(outfitInfoCustom);
                }
            }
        }

        public static float[] valueIndex = new float[2] {0,0};
        public static bool RobeBuffs = true;
        public static bool SpawnMiniBoss = false;
        public static bool FreezeEnabled = false;
        public static int NevesChaosPalette = 0;
        public static float TimeToRainbowCycle = 0;
        public void Update()
        {

            if (rainbowOutfit != null)
            {
                TimeToRainbowCycle += Time.deltaTime;
                if (TimeToRainbowCycle > 0.5f)
                {
                    Debug.Log("I am cycling");
                    TimeToRainbowCycle = 0;
                    RainbowIndex = (RainbowIndex + 1) % 6;
                    rainbowOutfit.outfit.outfitColorIndex = RainbowStartIndex + RainbowIndex;
                    foreach(Player player in GameController.playerScripts)
                    {
                        if (player != null && player.outfitID == rainbowOutfit.outfit.outfitID)
                        {
                            player.outfitColorIndex = RainbowStartIndex + RainbowIndex;
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                //StartCoroutine(OnlineSupport.UploadData(OnlineSupport.url));
            }
            List<KeyValuePair<GameObject, string>> toRemove = new List<KeyValuePair<GameObject, string>>();
            if (GameController.Instance != null)
            {
                if (GameController.players!=null&&GameController.players.Count > 0)
                {
                    foreach (KeyValuePair<GameObject, string> pair in announcementPairs)
                    {
                        if (pair.Key!=null)
                        {
                            if (Vector3.Distance(GameController.players[0].transform.position, pair.Key.transform.position) < 4)
                            {
                                GameUI.BroadcastNoticeMessage(pair.Value, 1f);
                            }
                        }
                        else
                        {
                            toRemove.Add(pair);
                        }
                    }
                }
            }
            foreach (KeyValuePair<GameObject,string> p in toRemove)
            {
                Destroy(p.Key);
            }

            if (SceneManager.GetActiveScene().name.ToLower()!="pvp")
            {
                if (true)
                {
                    if (Time.time > nextTime)
                    {
                        nextTime = Time.time + 1f;
                        if (GameController.instance != null) {
                            foreach (GameObject player in GameController.players) {
                                if (player.GetComponent<Player>().inventory.ContainsItem("Mythical::SevenFlushChaos")) {
                                    SevenFlush flush = (SevenFlush)player.GetComponent<Player>().inventory.GetItem("Mythical::SevenFlushChaos");
                                    if (flush.isActive) {
                                        SoundManager.PlayAudio("ImpactPhysicalHeavy", 1, false, 0.25f);
                                        player.GetComponent<Player>().health.CurrentHealthValue = (int)Mathf.Clamp(player.GetComponent<Player>().health.CurrentHealthValue - 5, 1, 100000);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        float nextTime = 0.25f;

        public static Sprite[] playerSprites;
        public static AssetBundle playerBundle;

        public bool hasSwappedAudioClips = false;
        public static bool SpawnPickups = true;
        public static bool StageEffects = true;
        public static bool StageHazards = true;
        public static bool BestTo3 = false;
        public static bool Depletion = false;
        public static bool MonoElementDrops = false;
        public static bool addedGMHooks = false;
        public static bool playedCredits=false;
        List<string> monoskills = new List<string>();
        public static Dictionary<GameObject,SpriteRenderer> newPlayerDict = new Dictionary<GameObject, SpriteRenderer>();


        private static Texture2D ExtractAndName(Sprite sprite)
        {
            var output = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var r = sprite.textureRect;
            var pixels = sprite.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
            output.SetPixels(pixels);
            output.Apply();
            output.name = sprite.texture.name + " " + sprite.name;
            return output;
        }
        private static void SaveSubSprite(Texture2D tex, string saveToDirectory)
        {
            if (!System.IO.Directory.Exists(saveToDirectory)) System.IO.Directory.CreateDirectory(saveToDirectory);
            System.IO.File.WriteAllBytes(System.IO.Path.Combine(saveToDirectory, tex.name + ".png"), tex.EncodeToPNG());
        }

        public bool inPVPScene { get
            {
                return SceneManager.GetActiveScene().name.ToLower() == "pvp";
            } }
        public static  bool inAPVPScene
        {
            get
            {
                return SceneManager.GetActiveScene().name.ToLower().Contains("pvp");
            }
        }
        public static GameObject interactableParent;
        Dictionary<GameObject, string> announcementPairs = new Dictionary<GameObject, string>();
        public void OnLevelWasLoaded()
        {
            try
            {
                if (ResetGems)
                {
                    Player.platWallet.balance = Player.platWallet.maxBalance; //Enjoy
                }
                
            }
            catch { }
            try
            {
                if (SceneManager.GetActiveScene().name.ToLower()!="pvparena")
                {
                    if (!RobeBuffs)
                    {
                        foreach (Player p in GameController.activePlayers)
                        {
                            Outfit.GetAvailableOutfit(p.outfitID).SetMods(true, true);
                        }
                    }

                    RobeBuffs = true;
                }
                if (inPVPScene)
                {
                    GameObject area = GameObject.Find("StagingArea");
                    interactableParent = area.transform.Find("Interactables").gameObject;
                    interactableParent.transform.parent = null;
                    interactableParent.transform.Translate(0, 3, 0);
                    GameObject.Find("Npcs").transform.Translate(0, 2, 0);
                    GameObject.Find("ExitPortalDisabled").transform.Translate(0, -3, 0);
                    Destroy(GameObject.Find("WallPillars"));
                    area.transform.localScale = new Vector3(1.4f, 1.2f, 1);
                    area.transform.Translate(-10, 5, 0);



                    ChaosArenaChanges.ResetTileSet();
                    ChaosArenaChanges.AddCustomArenaPortals();

                    GameObject mimi = MimicNpc.Prefab;
                    pvpItems = new Dictionary<int, List<string>>();
                    arcana = new Dictionary<int, List<string>>();
                    Instantiate(mimi, new Vector3(-3, 10, 0), Quaternion.identity);

                    SpawnPickups = true;
                    StageEffects = true;
                    StageHazards = true;
                    MonoElementDrops = false;
                    Depletion = false;
                    BestTo3 = false;
                    SpawnMiniBoss = false;
                    SaveArcana = false;
                    UseBanlist=false;
                    FreezeStartPositions = false;
                    SpawnChests = false;

                    GameObject noPickups = Instantiate(Tree.Prefab, new Vector3(-11, -3, 0), Quaternion.identity);
                    noPickups.name = "NoPickups";
                    announcementPairs[noPickups] = "Disable spell drops!";

                    GameObject noEffects = Instantiate(Tree.Prefab, new Vector3(11, -3, 0), Quaternion.identity);
                    noEffects.name = "NoEffects";
                    announcementPairs[noEffects] = "Disable the arenas' special effects!";

                    GameObject noHazards = Instantiate(MetalBarrelDeco.Prefab, new Vector3(23, 5, 0), Quaternion.identity);
                    noHazards.name = "NoHazards";
                    announcementPairs[noHazards] = "Disable the arenas' dangerous hazards!";

                    GameObject monoDrops = Instantiate(MetalBarrelDeco.Prefab, new Vector3(-23, 5, 0), Quaternion.identity);
                    monoDrops.name = "MonoDrops";
                    announcementPairs[monoDrops] = "Only drops spells of types already held! (Currently basics only for performance sake)";

                    GameObject bestTo3 = Instantiate(MetalBarrelDeco.Prefab, new Vector3(8, 3, 0), Quaternion.identity);
                    bestTo3.name = "BestTo3";
                    announcementPairs[bestTo3] = "Makes matches first to 3 instead of first to 2!";

                    GameObject spawnMB = Instantiate(MetalBarrelDeco.Prefab, new Vector3(-8, 3, 0), Quaternion.identity);
                    spawnMB.name = "SpawnMB";
                    announcementPairs[spawnMB] = "Spawn strong foes at the start of each round!";

                    GameObject noBuffs = Instantiate(MetalBarrelDeco.Prefab, new Vector3(-16, 3, 0), Quaternion.identity);
                    noBuffs.name = "NoBuffs";
                    announcementPairs[noBuffs] = "Disable robe buffs for the match!";

                    GameObject saveArcana = Instantiate(MetalBarrelDeco.Prefab, new Vector3(16, 3, 0), Quaternion.identity);
                    saveArcana.name = "SaveArcana";
                    announcementPairs[saveArcana] = "Save arcana picked up between rounds!";

                    GameObject banBarrel = Instantiate(MetalBarrelDeco.Prefab, new Vector3(23, 15, 0), Quaternion.identity);
                    banBarrel.name = "UseBanList";
                    announcementPairs[banBarrel] = "Use the banlist in the plugins folder!";

                    //GameObject freezeBarrel = Instantiate(MetalBarrelDeco.Prefab, new Vector3(23, 15, 0), Quaternion.identity);
                    //freezeBarrel.name = "Freeze";
                    //announcementPairs[banBarrel] = "Disable movement and attacks until round begins!";

                    GameObject chests = Instantiate(MetalBarrelDeco.Prefab, new Vector3(-23, 15, 0), Quaternion.identity);
                    chests.name = "Chests";
                    announcementPairs[chests] = "Spawn Chests instead of arcana!";

                    //GameObject depletion = Instantiate(Tree.Prefab, new Vector3(-8, 3, 0), Quaternion.identity);
                    //depletion.name = "Depletion";
                    //announcementPairs[depletion] = "Player health will slowly decay!";

                    foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
                    {
                        if (obj.name.ToLower() == "loadoutnpc")
                        {
                            //Destroy(obj);
                            obj.transform.position = new Vector3(3, 10, 0);
                        }

                        if (obj.name.ToLower().Contains("trainingdummy"))
                        {
                            Destroy(obj);
                        }
                    }

                    foreach (GameObject t in announcementPairs.Keys)
                    {
                        if (t != null)
                        {
                            Destructible tree = t.GetComponentInChildren<Destructible>();
                            if (tree != null)
                            {
                                //tree.l
                                tree.destroyOnContact = false;
                                tree.health.healthStat.BaseValue = 100;
                                tree.health.CurrentHealthValue = 100;
                                tree.health.healthStat.ModifiedValue = 100;
                                tree.health.healthStat.CurrentValue = 100;
                            }
                        }
                    }



                    

                }

                List<string> Destroynames = new List<string>() { };
                if (inAPVPScene && SceneManager.GetActiveScene().name.ToLower().Contains("arena"))
                {


                    monoskills = new List<string>();
                    nextTime = Time.time + 5;
                    if (!StageEffects || !StageHazards)
                    {
                        if (!StageEffects) { Destroynames.Add("overdrive"); Destroynames.Add("enemy"); }
                        if (!StageHazards) { Destroynames.Add("hazard"); }
                        foreach (GameObject o in FindObjectsOfType<GameObject>())
                        {
                            if (o.transform.root != null && o.transform.root.name.ToLower() == "pvprooms")
                            {
                                foreach (string s in Destroynames)
                                {
                                    if (o.name.ToLower().Contains(s))
                                    {
                                        Destroy(o);
                                    }
                                }
                            }
                        }
                    }

                    ApplyPvpTokens();


                }
            } catch { }


            

        }

        public Texture2D EXPOSED;

        private void tmp(MonoMod.Cil.ILContext ilContext)
        {
            ILCursor il = new ILCursor(ilContext);
            UInt64 s = 0;
            while (il.Next != null)
            {
                s += (UInt32)il.Next.OpCode.Code;
                il.Index++;
            }
            if (s != 0x140A)
            {
                Debug.LogError("The CheckForDragons is not the same as assumed!\nGot: 0x" + s.ToString("x").ToUpper() + " instead of: 0x140A");
                return;
            }
            il.Index = 2;
            il.RemoveRange(5);
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_S, (sbyte)19);
            il.Prev.OpCode = Mono.Cecil.Cil.OpCodes.Brfalse_S;
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldsfld, typeof(System.String).GetField("Empty", BindingFlags.Static | BindingFlags.Public));
            il.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(Globals).GetMethod("InGameScene", BindingFlags.Static | BindingFlags.Public));
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_S, (sbyte)8);
            il.Prev.OpCode = Mono.Cecil.Cil.OpCodes.Brtrue_S;
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldsfld, typeof(GameController).GetField("pvp", BindingFlags.Static | BindingFlags.Public));
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_S, (sbyte)1);
            il.Prev.OpCode = Mono.Cecil.Cil.OpCodes.Brtrue_S;
            il.Emit(Mono.Cecil.Cil.OpCodes.Ret);
        }
        public void ApplyPvpTokens()
        {
            for (int i = 0; i < GameController.players.Count; i++)
            {
                Player p = GameController.players[i].GetComponent<Player>();
                if (p.inventory != null)
                {
                    if (p.inventory.itemDict.Count > 0)
                    {
                        string relic = p.inventory.itemDict.ElementAt(0).Key;
                        if (relic == "TokenShuffler")
                        {
                            p.RandomizeBuild(true, false, true);
                        }
                        else if (relic == "TokenCursed")
                        {
                            for (int k = 0; k < 6; k++)
                            {
                                if (p.assignedSkills[k] != null)
                                {
                                    p.RemoveSkill(p.assignedSkills[k]);
                                }
                            }

                            p.AssignSkillSlot(1, "Dash", false, false);
                        }
                        else if (relic == "Mythical::TokenCringe")
                        {
                            for (int k = 0; k < 6; k++)
                            {
                                if (p.assignedSkills[k] != null)
                                {
                                    p.RemoveSkill(p.assignedSkills[k]);
                                }
                            }

                            //p.AssignSkillSlot(1, "Dash", false, false);
                        }
                        else if (relic == "TokenBanker")
                        {
                            string id = LootManager.GetSkillID(false, false);
                            p.AssignSkillSlot(4, id, false, false);
                            id = LootManager.GetSkillID(false, false);
                            p.AssignSkillSlot(5, id, false, false);
                            //Player.goldWallet.balance = 9999;
                        }
                        else if (relic == "TokenTailor")
                        {
                            UpgradePlayer.Upgrade(p);
                        }
                        else if (relic == "PrimeTicket")
                        {
                            //UpgradePlayer.Upgrade(p); //Chaos reaper, chaotic rift, twin turbines, distortion beam, lightning dragons, mark of discord.
                            p.AssignSkillSlot(0, "UseChaosScytheBasic", false, false);
                            p.AssignSkillSlot(1, "ChaosDash", false, false);
                            p.AssignSkillSlot(2, "UseShockBoomerang", true, true);
                            p.AssignSkillSlot(3, "UseIceBoomerang", true, true);
                            p.AssignSkillSlot(4, "UseChaosBeam", false, false);
                            p.AssignSkillSlot(5, "UseShockDragon", true, true);
                            //p.AssignSkillSlot(5, "UseChaosSwordSummon", false, false);
                        }
                        else if (relic == "SurfsGambit")
                        {
                            //p.RemoveSkill(p.GetAnyStandardSkill());
                        }
                        p.lowerHUD.cooldownUI.RefreshEntries();
                    }
                }
            }
        }

        public void Us_PlayCredits(On.Player.orig_SetPlayerOutfitColor orig, Player self, NumVarStatMod mod, bool givenStatus)
        {
            orig(self, mod, givenStatus);

            if (!playedCredits)
            {
                playedCredits = true;
                StartCoroutine(BootUpCredits());
            }
            
        }
        public static bool SaveArcana = false;
        public static Texture2D FillColorAlpha(Texture2D tex2D, Color32? fillColor = null)
        {
            if (fillColor == null)
            {
                fillColor = UnityEngine.Color.clear;
            }
            Color32[] fillPixels = new Color32[tex2D.width * tex2D.height];
            for (int i = 0; i < fillPixels.Length; i++)
            {
                fillPixels[i] = (Color32)fillColor;
            }
            tex2D.SetPixels32(fillPixels);
            return tex2D;
        }

        public static Dictionary<int, List<string>> pvpItems = new Dictionary<int, List<string>>();
        public static Dictionary<int, List<string>> arcana = new Dictionary<int, List<string>>();
        public static Dictionary<int, List<bool>> arcanaEnhance = new Dictionary<int, List<bool>>();
        public void PvpController_ResetPlayers(On.PvpController.orig_ResetPlayers orig, PvpController self, bool b)
        {
            orig(self, b);
            int i = 0;
            foreach(Player p in GameController.activePlayers)
            {
                if (p == null)
                {
                    continue;
                }
                if (pvpItems.ContainsKey(i))
                {

                    //p.inventory.AddItem(p.inventory.GetItem(pvpItems[i]));
                    foreach (string s in pvpItems[i])
                    {
                        p.GiveDesignatedItem(s);
                    }
                    
                }

                if (SaveArcana && arcana.ContainsKey(i))
                {
                    int a = 0;
                    foreach(string s in arcana[i])
                    {
                        p.AssignSkillSlot(a+4, s, false,false);
                        p.assignedSkills[a + 4].SetEmpowered(arcanaEnhance[i][a], EmpowerStatMods.DefaultEmpowerMod);
                        a++;
                    }
                    p.lowerHUD.cooldownUI.RefreshEntries();
                }

                p.health.RestoreHealth(500,false,false,true,false);

                i++;
            }

            ApplyPvpTokens();
        }

        public void PvpController_ResetStage(On.PvpController.orig_ResetStage orig, PvpController self, bool b)
        {
            pvpItems = new Dictionary<int, List<string>>();
            arcana = new Dictionary<int, List<string>>();
            arcanaEnhance = new Dictionary<int, List<bool>>();
            int i = 0;
            try
            {
                if (GameController.activePlayers != null)
                {
                    foreach (Player p in GameController.activePlayers)
                    {
                        if (p.inventory != null && p.inventory.itemDict != null)
                        {
                            if (p.inventory.itemDict.Count > 0)
                            {
                                List<string> str = new List<string>();
                                foreach(string s in p.inventory.itemDict.Keys)
                                {
                                    str.Add(s);
                                }
                                pvpItems[i] = str;
                            }
                        }

                        List<string> a = new List<string>();
                        List<bool> bo = new List<bool>();
                        if (p.assignedSkills[4] != null)
                        {
                            a.Add(p.assignedSkills[4].skillID);
                            bo.Add(p.assignedSkills[4].IsEmpowered);
                        }
                        if (p.assignedSkills[5] != null)
                        {
                            a.Add(p.assignedSkills[5].skillID);
                            bo.Add(p.assignedSkills[5].IsEmpowered);
                        }
                        if (a.Count>0 && !b)
                        {
                            arcana[i] = a;
                            arcanaEnhance[i] = bo;
                        }

                        
                        

                        i++;
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Log("AVERTED ERROR: " + e.Message);
            }
            orig(self,b);
        }

        public bool Inventory_AddItem(On.Inventory.orig_AddItem_Item_bool_bool orig, Inventory self, Item givenItem, bool showNotice, bool ignoreMax)
        {
            self.currentItemID = givenItem.ID;
            Player.newItems[givenItem.category].Remove(self.currentItemID);
            if (self.ContainsItem(self.currentItemID))
            {
                return false;
            }
            if (!self.CheckForMutallyExclusiveItems(self.currentItemID))
            {
                return false;
            }
            if (self.itemDict.Count >= 18 &&!self.CheckItemCombine(givenItem) && !ignoreMax && !self.DropItem(string.Empty))
            {
                return false;
            }
            givenItem.parentEntity = self.parentEntity;
            givenItem.SetParentSkillCategory();
            self.itemDict[self.currentItemID] = givenItem;
            givenItem.Activate();
            if (showNotice && self.parentEntity is Player)
            {
                ((Player)self.parentEntity).newItemNoticeUI.Display(TextManager.GetItemName(self.currentItemID), IconManager.GetItemIcon(self.currentItemID), null, false, false, false, true);
                if (givenItem.isCursed)
                {
                    SoundManager.PlayAudio("BuyCursedRelic", 1f, false, -1f, -1f);
                }
                else
                {
                    SoundManager.PlayAudio("Powerup", 1f, false, -1f, -1f);
                }
                if (givenItem.isCursed)
                {
                    PoolManager.GetPoolItem<ParticleEffect>("CursedAuraBurstEffect").Play(null, new Vector3?(self.parentEntity.transform.position), null, null, null, null, 0f, false);
                }
            }
            self.count++;
            self.AnnounceItemEvent(givenItem, true);
            if (givenItem.isCursed)
            {
                AchievementManager.UnlockAchievement(Achievement.HoldFourCursed, false);
            }
            return true;

            //return orig(self, item, show, true);
        }

        private void PvpController_ResetStage2(MonoMod.Cil.ILContext ilContext)
        {
            ILCursor il = new ILCursor(ilContext);
            il.Index = 76;
            for (int i = 0; i < 25; i++)
            {
                if (il.Next == null) break;
                il.Remove();
            }
            il.Index = 75;
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_1);
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_S, (sbyte)13);
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_0);
            il.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(GameController).GetMethod("PauseAllPlayers", BindingFlags.Static | BindingFlags.Public));
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_1);
            il.Emit(Mono.Cecil.Cil.OpCodes.Stfld, typeof(PvpController).GetField("matchInProgress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            il.Index = 76;
            il.Next.OpCode = Mono.Cecil.Cil.OpCodes.Brfalse_S;
        }
        public void LoadSong(string title, string path)
        {
            path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
            StartCoroutine(LoadSongCoroutine(title,path));
            /*
            string url = string.Format("file://{0}", path);
            WWW www = new WWW(url);
            AudioClip song = www.GetAudioClipCompressed(false, AudioType.OGGVORBIS);
            song.name = title + " (CUSTOM)";
            Debug.Log("Got Clip: " + song.name);
            if (song != null)
            {
                Debug.Log("WOOHOO IT LOADED");
            }
            clipDict[title] = song;*/
        }

        IEnumerator LoadSongCoroutine(string title, string path)
        {
            Debug.Log("Starting song routine");
            string url = string.Format("file:///{0}", path);
            using (WWW www = new WWW(url))
            {
                yield return www;
                AudioClip song = www.GetAudioClipCompressed(false, AudioType.OGGVORBIS);
                song.name = title + " (CUSTOM)";
                clipDict[title] = song;
            }
           
            
        }

        IEnumerator BootUpCredits()
        {
            GameUI.BroadcastNoticeMessage("Wizard of Legend: Tournament Edition by only_going_up_fr0m_here", 3f);
            yield return new WaitForSeconds(3);
            GameUI.BroadcastNoticeMessage("Code Assistance provided by RandomlyAwesome and TheTimesweeper", 3f);
            yield return new WaitForSeconds(3);
            GameUI.BroadcastNoticeMessage("Special Thanks to Holy Grind, Aries13, and Cerberus", 3f);
        }

        public static Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();
        public static Dictionary<string, string> bossPrefabFilePaths = new Dictionary<string, string>
        {
            {
                "IceBoss",
                "Assets/Prefabs/Bosses/IceBoss.prefab"
            },
            {
                "FireBoss",
                "Assets/Prefabs/Bosses/FireBoss.prefab"
            },
            {
                "EarthBoss",
                "Assets/Prefabs/Bosses/EarthBoss.prefab"
            },
            {
                "AirBoss",
                "Assets/Prefabs/Bosses/AirBoss.prefab"
            },
            {
                "LightningBoss",
                "Assets/Prefabs/Bosses/LightningBoss.prefab"
            },
            {
                "FinalBoss",
                "Assets/Prefabs/Bosses/FinalBoss.prefab"
            }
        };
    }
}

public static class Extensions
{
    public static ItemInfo loadSprite(this ItemInfo info, string name)
    {
        Sprite spr = Mythical.ImgHandler.LoadSprite(name);
        info.icon = (spr != null ? spr : null);
        return info;
    }
    public static Sprite loadSprite(string name)
    {
        Sprite spr = Mythical.ImgHandler.LoadSprite(name);
        return (spr != null ? spr : null);
    }
}

public class HeadgearDef
{
    public Sprite[] sprites = new Sprite[0];
}