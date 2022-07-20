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
using UnityEngine;
using UnityEngine.SceneManagement;
using XUnity.ResourceRedirector;
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
    [BepInPlugin("Amber.Mythical", "Mythical", "0.1.0")]
    public class ContentLoader : BaseUnityPlugin { 
        #region BaseUnityPlugin Notes
        // BaseUnityPlugin is the main class that gets loaded by bepin.
        // It inherits from MonoBehaviour, so it gains all the familiar Unity callback functions you can use: 
        //     Awake, Start, Update, FixedUpdate, etc.

        //     Awake is most important. it's basically where we initialize everything we do

        //     For further reading, you can check out https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

        // now, close these two Notes regions so the script looks little nicer to work with 
        #endregion
        public List<Texture2D> palettes = new List<Texture2D>();
        public List<Sprite> titleScreens = new List<Sprite>();
        public bool hasAddedTitleCards;
        public static bool ChaosDrops = false;
        // This Awake() function will run at the very start when the mod is initialized

        public Sprite cherrySprite;
        public Sprite basePalette;
        public Sprite orangeSprite;
        void Awake() {

            //Skills.Awake();
            //SampleSkillLoader.Awake();
            //UnityEngine.Texture2D img = ImgHandler.LoadTex2D("icon");
            //WindowIconTools.SetIcon(img.GetRawTextureData(), img.width, img.height, WindowIconKind.Big);
            //Screen.SetResolution(1200, 675, false);

            // LETS FUCKING GO

            
            basePalette = ImgHandler.LoadSprite("Base");
            newPlayerSprite = ImgHandler.LoadTex2D("Walter2");
            cherrySprite = ImgHandler.LoadSprite("tree1", new Vector2(0.5f,0.2f));
            orangeSprite = ImgHandler.LoadSprite("tree2", new Vector2(0.5f, 0.2f));
            Debug.Log("Cherry Blossom Tree sprite from https://opengameart.org/content/lpc-plant-repack. Cropped to one singular tree.");
            Debug.Log("Cherry Orange Tree sprite from https://opengameart.org/content/lpc-orange-trees. Cropped to one singular tree.");


            OutfitInfo outfitInfo = new OutfitInfo();
            outfitInfo.name = "Walter";
            outfitInfo.outfit = new global::Outfit("Mythical::Walter", 41, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo.customDesc = ((bool b) => "The Living Menace.");
            outfitInfo.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo);

            OutfitInfo outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Guardian";
            outfitInfo2.outfit = new global::Outfit("Mythical::Guardian", 42, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "The Defender");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            OutfitInfo outfitInfo7 = new OutfitInfo();
            outfitInfo7.name = "Scholar";
            outfitInfo7.outfit = new global::Outfit("Mythical::Scholar", 36, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo7.customDesc = ((bool b) => "The Keeper Of Knowledge");
            outfitInfo7.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo7);

            OutfitInfo outfitInfo6 = new OutfitInfo();
            outfitInfo6.name = "Fear";
            outfitInfo6.outfit = new global::Outfit("Mythical::Fear", 37, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo6.customDesc = ((bool b) => "The Everlasting Terror");
            outfitInfo6.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo6);

            OutfitInfo outfitInfo5 = new OutfitInfo();
            outfitInfo5.name = "Conquest";
            outfitInfo5.outfit = new global::Outfit("Mythical::Conquest", 38, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo5.customDesc = ((bool b) => "The Iron Fist");
            outfitInfo5.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo5);

            

            OutfitInfo outfitInfo4 = new OutfitInfo();
            outfitInfo4.name = "Tycoon";  
            outfitInfo4.outfit = new global::Outfit("Mythical::Tycoon", 39, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo4.customDesc = ((bool b) => "The Invisible Hand");
            outfitInfo4.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo4);

            OutfitInfo outfitInfo3 = new OutfitInfo();
            outfitInfo3.name = "Surf";
            outfitInfo3.outfit = new global::Outfit("Mythical::Surf", 40, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo3.customDesc = ((bool b) => "The Challenger");
            outfitInfo3.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo3);

            OutfitInfo outfitInfo8 = new OutfitInfo();
            outfitInfo8.name = "Terror";
            outfitInfo8.outfit = new global::Outfit("Mythical::Terror", 35, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo8.customDesc = ((bool b) => "The Worst Of The Best");
            outfitInfo8.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo8);

            // New ones
            OutfitInfo outfitInfo9 = new OutfitInfo();
            outfitInfo9.name = "Vision";
            outfitInfo9.outfit = new global::Outfit("Mythical::Vision", 34, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo9.customDesc = ((bool b) => "The All Seeing Eye");
            outfitInfo9.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo9);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Archaic";
            outfitInfo2.outfit = new global::Outfit("Mythical::Archaic", 43, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "From Days Past - only_going_up_fr0m_here");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo = new OutfitInfo();
            outfitInfo.name = "Crimson";
            outfitInfo.outfit = new global::Outfit("Mythical::Crimson", 33, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, true, false);
            outfitInfo.customDesc = ((bool b) => "Designed by only_going_up_fr0m_here!");
            outfitInfo.customMod = ((player, b, b2) => { });
            Outfits.Register(outfitInfo);

            


            outfitInfo = new OutfitInfo();
            outfitInfo.name = "Sovereign";
            outfitInfo.outfit = new global::Outfit("Mythical::Sovereign", 32, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, true, false);
            outfitInfo.customDesc = ((bool b) => "Designed by Cerberus!");
            outfitInfo.customMod = ((player, b, b2) => {});
            Outfits.Register(outfitInfo);
            

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Empress";
            outfitInfo2.outfit = new global::Outfit("Mythical::Empress", 44, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Designed by Cerberus!");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Despair";
            outfitInfo2.outfit = new global::Outfit("Mythical::Despair", 45, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "ABANDON ALL HOPE");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            List<string> robeNames = new List<string>() { "sovereign", "crimson", "vision","terror","scholar","fear","conquest","tycoon","surf","walter","guardian","relic","empress","Despair" };
            foreach(string robeName in robeNames)
            {
                palettes.Add(ImgHandler.LoadTex2D(robeName));
            }

            // Title screen additions

            titleScreens.Add(ImgHandler.LoadSprite("bg1"));
            titleScreens.Add(ImgHandler.LoadSprite("bg2"));

            // Disable Drops

            On.Destructible.Break += (On.Destructible.orig_Break orig, Destructible self) =>
            {
                if (self.name.Contains("NoPickups") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Spell Drops Disabled",3f);
                    Debug.Log("No drops");
                    SpawnPickups = false;
                }
                if (self.name.Contains("NoEffects") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Stage Effects Disabled", 3f);
                    Debug.Log("No effects");
                    StageEffects = false;
                }
                orig(self);
            };

            On.PvpController.HandleSkillSpawn += (On.PvpController.orig_HandleSkillSpawn orig, PvpController self) =>
            {
                if (SpawnPickups)
                {
                    orig(self);
                }
            };

            


            //List<Sprite> loadingTheListIDontNeedThisToAllocate = IconManager.TSBGSpriteList;
            On.IconManager.GetBGSprite += (On.IconManager.orig_GetBGSprite orig, int index) =>
            {
                if (!hasAddedTitleCards)
                {
                    hasAddedTitleCards = true;
                    List<Sprite> loadingTheListIDontNeedThisToAllocate = IconManager.TSBGSpriteList;
                    TitleScreen.bgCount += titleScreens.Count;

                    List<Sprite> sprites = new List<Sprite>();

                    foreach (Sprite spr in titleScreens)
                    {
                        sprites.Add(spr);
                    }
                    foreach(Sprite spr in IconManager.TSBGSpriteList)
                    {
                        sprites.Add(spr);
                    }

                    IconManager.tsbgSpriteList = sprites;


                }
                return orig(index);
            };
            

            //Palettes

            On.PvpController.ResetStage += PvpController_ResetStage;
            On.PvpController.ResetPlayers += PvpController_ResetPlayers;
            On.Player.SetPlayerOutfitColor += Us_AddOutfit;

            On.GameProgressBoard.SetPlayerColors += (On.GameProgressBoard.orig_SetPlayerColors orig, GameProgressBoard self) =>
             {
                 if (newPalette != null)
                 {
                     self.p1PieceImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                     self.p1PieceImage.material.SetTexture("_Palette", newPalette);
                 }
                 orig(self);
                 
             };

            On.OutfitMenu.LoadMenu += (On.OutfitMenu.orig_LoadMenu orig, OutfitMenu self, Player p) =>
            {
                self.outfitImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                self.outfitImage.material.SetTexture("_Palette", newPalette);
                orig(self, p);
            };
            
            On.DeathSummaryUI.Activate += (On.DeathSummaryUI.orig_Activate orig, DeathSummaryUI self, float f) => {
                orig(self,f);
                if (newPalette != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        self.playerRefs[i].outfitImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                        self.playerRefs[i].outfitImage.material.SetTexture("_Palette", newPalette);
                    }
                }
            };

            On.WardrobeUI.LoadOutfits += (On.WardrobeUI.orig_LoadOutfits orig, WardrobeUI self) =>
            {
                orig(self);
                if (newPalette != null)
                {
                    for (int j = 0; j < self.totalOutfitCount; j++)
                    {
                        
                        self.wrRef.outfitImageArray[j].material.SetFloat("_PaletteCount", 32+palettes.Count);
                        self.wrRef.outfitImageArray[j].material.SetTexture("_Palette", newPalette);
                    }
                }
            };
            On.DialogManager.Activate += (On.DialogManager.orig_Activate orig, DialogManager self, DialogMessage m, bool b, bool s) =>
            {
                orig(self, m, b, s);
                if (m.rightActive && m.RightSpeaker != null && newPalette != null)
                {
                    self.rightPlayerImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                    self.rightPlayerImage.material.SetTexture("_Palette", newPalette);
                }
            };
            On.UnlockNotifier.SetNotice += (On.UnlockNotifier.orig_SetNotice orig, UnlockNotifier self, UnlockNotifier.NoticeVars vars) =>
            {
                orig(self, vars);
                if (newPalette != null)
                {
                    self.outfitIconImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                    self.outfitIconImage.material.SetTexture("_Palette", newPalette);
                }
            };
            On.WardrobeUI.UpdateOutfits += (On.WardrobeUI.orig_UpdateOutfits orig, WardrobeUI self, int givenIndex) =>
            {
                orig(self, givenIndex);
                if (newPalette != null)
                {
                    self.wrRef.playerImages[self.currentPlayerImageIndex].material.SetFloat("_PaletteCount", 32 + palettes.Count);
                    self.wrRef.playerImages[self.currentPlayerImageIndex].material.SetTexture("_Palette", newPalette);
                }
            };
            On.OutfitStoreItem.Start += (On.OutfitStoreItem.orig_Start orig, OutfitStoreItem self) =>
            {
                orig(self);
                self.itemSpriteRenderer.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                self.itemSpriteRenderer.material.SetTexture("_Palette", newPalette);
            };
            On.PlayerStatusBar.Update += (On.PlayerStatusBar.orig_Update orig, PlayerStatusBar self) =>
            {
                orig(self);
                if (self.playerPortrait != null && newPalette != null)
                {
                    Material material = UnityEngine.Object.Instantiate<Material>(self.playerPortrait.material);
                    material.SetFloat("_PaletteCount", 32 + palettes.Count);
                    material.SetTexture("_Palette", newPalette);
                    self.playerPortrait.material = material;
                }
            };
            On.WardrobeUI.AssignOutfit += (On.WardrobeUI.orig_AssignOutfit orig, WardrobeUI self, Outfit o, int i) =>
            {
                self.wrRef.outfitImageArray[i].material.SetFloat("_PaletteCount", 32 + palettes.Count);
                self.wrRef.outfitImageArray[i].material.SetTexture("_Palette", newPalette);
                orig(self, o, i);
            };
            
            On.OutfitMenu.LoadMenu += (On.OutfitMenu.orig_LoadMenu orig , OutfitMenu self, Player p) => { orig(self, p); if (hasAddedPalettes) { self.outfitImage.material.SetTexture("_Palette", newPalette); } };
            On.OutfitMenu.SwapFocus += (On.OutfitMenu.orig_SwapFocus orig, OutfitMenu self, bool n) => { orig(self, n); if (hasAddedPalettes) { self.outfitImage.material.SetTexture("_Palette", newPalette); } };

            /*On.Player.InitComponents += (On.Player.orig_InitComponents orig, Player self) =>
            {
                orig(self);
                Debug.Log("Old Sprite Name: " + self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite.name);
                self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite = newPlayerSprite;
                Debug.Log("Set new palette: "+ self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite.name);
            };*/

            // Stage effects

            On.PvpController.ApplyAirBuffs += (On.PvpController.orig_ApplyAirBuffs orig, PvpController self) =>
            {
                if (StageEffects) { orig(self); }
            };
            On.PvpController.ApplyFireBuffs += (On.PvpController.orig_ApplyFireBuffs orig, PvpController self) =>
            {
                if (StageEffects) { orig(self); }
            };
            On.PvpController.ApplyEarthBuffs += (On.PvpController.orig_ApplyEarthBuffs orig, PvpController self) =>
            {
                if (StageEffects) { orig(self); }
            };

            // Music
            /*
            LoadSong("Title","Sprites/Vaporwave.ogg");
            LoadSong("PVPHub", "Sprites/Trap.ogg");
            LoadSong("PVP", "Sprites/Rock.ogg");

            On.SoundManager.PlayBGM += (On.SoundManager.orig_PlayBGM orig, string str) =>
            {
                if (!hasSwappedAudioClips)
                {
                    hasSwappedAudioClips = true;
                    SoundManager.bgmDict["Boss"].clip = clipDict["PVP"];
                    SoundManager.bgmDict["Hub"].clip = clipDict["PVPHub"];
                    SoundManager.bgmDict["TitleScreen"].clip = clipDict["Title"];
                }
                orig(str);
            };*/


            
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

            On.PlayerWinItem.SetEventHandlers += (On.PlayerWinItem.orig_SetEventHandlers orig, PlayerWinItem self, bool b) =>
            {
                orig(self,b);
                ChaosDrops = b;
            };

            On.GameController.Start += (On.GameController.orig_Start orig, GameController self) =>
            {
                orig(self);
                On.LootManager.GetSkillID += (On.LootManager.orig_GetSkillID orig2, bool l, bool s) =>
                {
                    if (ChaosDrops && inAPVPScene && UnityEngine.Random.value<0.25f)
                    {
                        return LootManager.chaosSkillList[UnityEngine.Random.Range(0, LootManager.chaosSkillList.Count)];
                    }
                    else
                    {
                        return orig2(l, s);
                    }
                };

                On.LootManager.DropSkill += (On.LootManager.orig_DropSkill orig3, Vector3 v, int a, string id, float l, float s, HashSet<ElementType> set, bool life, bool emp) =>
                {
                    if (inAPVPScene && LootManager.chaosSkillList.Contains(id))
                    {
                        emp = true;
                    }
                    orig3(v, a, id, l, s, set, life, emp);
                };
            };

            //Adjustments
            /*On.PlatWallet.ctor += (On.PlatWallet.orig_ctor orig, PlatWallet self, int i) =>
            {
                orig(self,i);
                self.maxBalance = 9999;
                self.balance = 9999;
            };*/
            
        }

        public static Sprite[] playerSprites;
        public static AssetBundle playerBundle;
        public static Texture2D newPlayerSprite;

        public static Dictionary<int, string> pvpItems = new Dictionary<int, string>();
        public bool hasSwappedAudioClips = false;
        public bool hasAddedPalettes = false;
        public static bool SpawnPickups = true;
        public static bool StageEffects = true;
        public static bool loadedWizSprites=false;
        public Texture2D newPalette = null;

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
        public bool inAPVPScene
        {
            get
            {
                return SceneManager.GetActiveScene().name.ToLower().Contains("pvp");
            }
        }
        public void OnLevelWasLoaded()
        {
            try
            {
                Player.platWallet.balance = Player.platWallet.maxBalance; //Enjoy
            }
            catch { }
            if (inPVPScene)
            {
                GameObject mimi = MimicNpc.Prefab;
                pvpItems = new Dictionary<int, string>();
                Instantiate(mimi, new Vector3(0, 5, 0), Quaternion.identity);

                SpawnPickups = true;
                StageEffects = true;
                GameObject noPickups = Instantiate(MetalBarrelDeco.Prefab, new Vector3(-9, -1, 0), Quaternion.identity);
                noPickups.name = "NoPickups";
                noPickups.transform.localScale = Vector3.one * 0.75f;
                noPickups.GetComponentInChildren<SpriteRenderer>().sprite = cherrySprite;
                noPickups.GetComponent<Health>().healthStat.SetInitialBaseValue(100);
                noPickups.GetComponent<Health>().healthStat.CurrentValue = 100;

                GameObject noEffects = Instantiate(MetalBarrelDeco.Prefab, new Vector3(9, -1, 0), Quaternion.identity);
                noEffects.name = "NoEffects";
                noEffects.transform.localScale = Vector3.one * 0.75f;
                noEffects.GetComponentInChildren<SpriteRenderer>().sprite = orangeSprite;
                noEffects.GetComponent<Health>().healthStat.SetInitialBaseValue(100);
                noEffects.GetComponent<Health>().healthStat.CurrentValue = 100;

                foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
                {
                    if (obj.name.ToLower() == "loadoutnpc" || obj.name.ToLower().Contains("trainingdummy"))
                    {
                        Destroy(obj);
                    }
                }

            }

            List<string> Destroynames = new List<string>() { "overdrive", "hazard","spawner" };
            if (inAPVPScene && SceneManager.GetActiveScene().name.ToLower().Contains("arena")) {
                if (!StageEffects)
                {
                    foreach (GameObject o in FindObjectsOfType<GameObject>())
                    {
                        if (o.transform.root.name.ToLower() == "pvprooms")
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

                for (int i = 0; i < 2; i++)
                {
                    if (GameController.playerScripts[i].inventory.itemDict.Count > 0)
                    {
                        if (GameController.playerScripts[i].inventory.itemDict.ElementAt(0).Key == "TokenShuffler")
                        {
                            GameController.playerScripts[i].RandomizeBuild(true, true, true);
                        }
                    }
                }

            }

        }

        public Texture2D EXPOSED;

        public void Us_AddOutfit(On.Player.orig_SetPlayerOutfitColor orig, Player self, NumVarStatMod mod, bool givenStatus)
        {
            orig(self, mod, givenStatus);

            if (!loadedWizSprites)
            {
                loadedWizSprites = true;
                
                Texture2D text = new Texture2D(1, 1); text.LoadImage(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sprites/Walter2.png")));
                Texture2D texture2 = self.spriteRenderer.sprite.texture;
                texture2.SetPixels32(text.GetPixels32());
                texture2.Apply();
                EXPOSED = texture2;
                GameUI.BroadcastNoticeMessage("Special Thanks to Rayman, Holy Grind, and Cerberus", 3f);
            }

            Texture2D tex = basePalette.texture;// (Texture2D) self.spriteMaterial.GetTexture("_Palette");
            if (newPalette == null)
            {
                //Debug.Log("1");
                newPalette = tex;
                if (!hasAddedPalettes)
                {
                    //Debug.Log("2");
                    hasAddedPalettes = true;
                    Texture2D t = newPalette;
                    Texture2D newT;
                    int h = t.height;
                    //Debug.Log("3");
                    foreach (Texture2D te in palettes)
                    {
                        //Debug.Log("Iterating over " + te.name);
                        newT = new Texture2D(newPalette.width, newPalette.height + 2,TextureFormat.RGBA32,false);
                        newT = FillColorAlpha(newT);
                        for(int x = 1; x < newT.width; x++)
                        {
                            for (int y = 0; y < newPalette.height; y++)
                            {
                                newT.SetPixel(x, y, newPalette.GetPixel(x, y));
                            }
                        }
                       // Debug.Log("Out of loop for " + te.name);
                        for (int x = 1; x < newT.width; x++)
                        {
                            newT.SetPixel(x, h, te.GetPixel(x, h));
                        }
                        for (int x = 1; x < newT.width; x++)
                        {
                            newT.SetPixel(x, h+1, te.GetPixel(x, h+1));
                        }

                        //Debug.Log("Out of loop 2 for " + te.name);
                        newT.filterMode = FilterMode.Point;
                        newT.Apply();
                        newPalette = newT;
                        h+=2;
                    }
                }
            }

            if (hasAddedPalettes)
            {
                self.spriteMaterial.SetFloat("_PaletteCount", 32 + palettes.Count);
                self.spriteMaterial.SetTexture("_Palette", newPalette);
                


            }

            //orig(self, mod, givenStatus);
            
        }
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
        public void PvpController_ResetPlayers(On.PvpController.orig_ResetPlayers orig, PvpController self, bool b)
        {
            orig(self, b);
            int i = 0;
            foreach(Player p in GameController.activePlayers)
            {
                if (pvpItems.ContainsKey(i))
                {

                    //p.inventory.AddItem(p.inventory.GetItem(pvpItems[i]));
                    p.GiveDesignatedItem(pvpItems[i]);
                    
                }
                i++;
            }
        }

        public void PvpController_ResetStage(On.PvpController.orig_ResetStage orig, PvpController self, bool b)
        {
            pvpItems.Clear();
            int i = 0;
            foreach(Player p in GameController.activePlayers)
            {
                if (p.inventory.itemDict.Count > 0)
                {
                    pvpItems[i] = p.inventory.itemDict.ElementAt(0).Key;
                }
                i++;
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

        public static Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();

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