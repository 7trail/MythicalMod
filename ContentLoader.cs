using BepInEx;
using BepInEx.Configuration;
using LegendAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        // This Awake() function will run at the very start when the mod is initialized
        void Awake() {

            Skills.Awake();
            SampleSkillLoader.Awake();
            // This is the just a first little tester code to see if our mod is running on WoL. You'll see it in the BepInEx console
            /*
            Debug.Log("Loading Outfits");

            OutfitInfo outfitInfo = new OutfitInfo();
            outfitInfo.name = "Vagrant";
            outfitInfo.outfit = new global::Outfit("Mythical::Vagrant", 15, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(global::OutfitModStat.OutfitModType.HealAmount, 0f, 0.2f, 0f, false),
                new global::OutfitModStat(global::OutfitModStat.OutfitModType.CritChance, 0f, 0.2f, 0f, false)
            }, true, false);
            Outfits.Register(outfitInfo);

            outfitInfo = new OutfitInfo();
            outfitInfo.name = "Showman";
            outfitInfo.outfit = new global::Outfit("Mythical::Showman", 30, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(global::OutfitModStat.OutfitModType.Damage, 0f, 0.1f, 0f, false),
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo.customDesc = ((bool b) => "Start with more money, rich boy ;)");
            outfitInfo.customMod = ((player, b, b2) => {
                if (b)
                {
                    Player.goldWallet.balance += 100;
                } else
                {
                    Player.goldWallet.balance -= 100;
                }
            });

            Outfits.Register(outfitInfo);

            outfitInfo = new OutfitInfo();
            outfitInfo.name = "Handyman";
            outfitInfo.outfit = new global::Outfit("Mythical::Handyman", 27, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(global::OutfitModStat.OutfitModType.Speed, 0f, 0.1f, 0f, false),
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo.customDesc = ((bool b) => "Pits will no longer spawn in rooms!");
            outfitInfo.customMod = ((player, b, b2) => {
                Level.removeAllPits = b;
            });

            Outfits.Register(outfitInfo);

            outfitInfo = new OutfitInfo();
            outfitInfo.name = "Hoarder";
            outfitInfo.outfit = new global::Outfit("Mythical::Hoarder", 22, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(global::OutfitModStat.OutfitModType.Speed, 0f, -0.1f, 0f, false),
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo.customDesc = ((bool b) => "You can carry six more items!");
            outfitInfo.customMod = ((player, b, b2) => {
                if (b)
                {
                    On.Inventory.AddItem_Item_bool_bool += Inventory_AddItem;
                } else
                {
                    On.Inventory.AddItem_Item_bool_bool -= Inventory_AddItem;
                }
            });

            Outfits.Register(outfitInfo);


            ItemInfo monsterTooth = new ItemInfo();
            monsterTooth.name = "MonsterTooth";
            monsterTooth.item = new MonsterTooth();
            monsterTooth.tier = 1;

            TextManager.ItemInfo itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Monster Tooth";
            itemInfo.description = "Gain health when killing an enemy!";
            itemInfo.itemID = MonsterTooth.staticID;

            Sprite spr = ImgHandler.LoadSprite("tooth");

            monsterTooth.text = itemInfo;
            monsterTooth.icon = (spr != null ? spr : null);

            Items.Register(monsterTooth);

            ItemInfo behemoth = new ItemInfo();
            behemoth.name = "BrilliantBehemoth";
            behemoth.item = new Behemoth();
            behemoth.tier = 1;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Brilliant Behemoth";
            itemInfo.description = "Chance to create explosions on hit, BUT deal less damage!";
            itemInfo.itemID = Behemoth.staticID;

            spr = ImgHandler.LoadSprite("cannon");

            behemoth.text = itemInfo;
            behemoth.icon = (spr != null ? spr : null);

            Items.Register(behemoth);

            ItemInfo sage = new ItemInfo();
            sage.name = "InvisibleOnLowHealth";
            sage.item = new InvisibleOnLowHealth();
            sage.tier = 2;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Sage's Armor";
            itemInfo.description = "Become invulnerable at low health!";
            itemInfo.itemID = InvisibleOnLowHealth.staticID;

            spr = ImgHandler.LoadSprite("sage");

            sage.text = itemInfo;
            sage.icon = (spr != null ? spr : null);

            Items.Register(sage);

            ItemInfo midas = new ItemInfo();
            midas.name = "MidasRage";
            midas.item = new MidasRage();
            midas.tier = 2;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Rage of Midas";
            itemInfo.description = "Destructibles drop more gold more frequently!";
            itemInfo.itemID = MidasRage.staticID;

            midas.text = itemInfo;
            midas = midas.loadSprite("midas");

            Items.Register(midas); //Here is where all the funny anti element relics come in ------------

            ItemInfo gemChest = new ItemInfo();
            gemChest.name = "GemChestRelic";
            gemChest.item = new GemChestRelic();
            gemChest.tier = 1;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Locked Gem Chest";
            itemInfo.description = "Gets heavier as you progress through the trials! Drop from inventory to open.";
            itemInfo.itemID = GemChestRelic.staticID;

            gemChest.text = itemInfo;
            gemChest = gemChest.loadSprite("gemchest");
            Items.Register(gemChest);

            ItemInfo atkSpeedUp = new ItemInfo();
            atkSpeedUp.name = "atkSpeedUpRelic";
            atkSpeedUp.item = new AttackSpeedUpItem();
            atkSpeedUp.tier = 1;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Energy Drink";
            itemInfo.description = "Increases spell activation speed!";
            itemInfo.itemID = AttackSpeedUpItem.staticID;

            atkSpeedUp.text = itemInfo;
            atkSpeedUp = atkSpeedUp.loadSprite("atkSpeedUp");
            Items.Register(atkSpeedUp);

            ItemInfo unEnhance = new ItemInfo();
            unEnhance.name = UnEnhanceRelic.staticID;
            unEnhance.item = new UnEnhanceRelic();
            unEnhance.tier = 1;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Power Drain";
            itemInfo.description = "Unenhance all current arcana, but greatly reduce cooldowns!";
            itemInfo.itemID = UnEnhanceRelic.staticID;

            unEnhance.text = itemInfo;
            unEnhance = unEnhance.loadSprite("unEnhance");
            Items.Register(unEnhance);

            ItemInfo allOrNothing = new ItemInfo();
            allOrNothing.name = AllOrNothing.staticID;
            allOrNothing.item = new AllOrNothing();
            allOrNothing.tier = 1;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "All Or Nothing";
            itemInfo.description = "Idea by BurnVolcano! You have a 50 percent chance to deal triple damage, but deal no damage otherwise!";
            itemInfo.itemID = AllOrNothing.staticID;

            allOrNothing.text = itemInfo;
            allOrNothing = allOrNothing.loadSprite("allOrNothing");
            Items.Register(allOrNothing);

            ItemInfo rootChanceUp = new ItemInfo();
            rootChanceUp.name = RootChanceUp.staticID;
            rootChanceUp.item = new RootChanceUp();
            rootChanceUp.tier = 1;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Petrified Root";
            itemInfo.description = "Adds a chance to root foes!";
            itemInfo.itemID = RootChanceUp.staticID;

            rootChanceUp.text = itemInfo;
            rootChanceUp = rootChanceUp.loadSprite("rootChanceUp");
            Items.Register(rootChanceUp);


            LoadAntiRelics();
            //DialogueCreator.Init();
            //MakeNewDialogueTest();
            AddBlobBoss();
            */
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

        public static void LoadAntiRelics()
        {
            ItemInfo frost = new ItemInfo();
            frost.name = "frostCrit";
            frost.item = new IceCrit();
            frost.tier = 3;

            TextManager.ItemInfo itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Sanctum of Antifrost";
            itemInfo.description = "Attacks against Frost enemies are guaranteed to be critical!";
            itemInfo.itemID = FrostCrit.staticID;

            frost.text = itemInfo;
            frost = frost.loadSprite("antifrost");

            Items.Register(frost);

            ItemInfo flame = new ItemInfo();
            flame.name = "flameCrit";
            flame.item = new FlameCrit();
            flame.tier = 3;


            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Sanctum of Anti-Flame";
            itemInfo.description = "Attacks against flame enemies are guaranteed to be critical!";
            itemInfo.itemID = "flameCrit";

            flame.text = itemInfo;
            flame = flame.loadSprite("antiflame");

            Items.Register(flame);

            ItemInfo earth = new ItemInfo();
            earth.name = "earthCrit";
            earth.item = new EarthCrit();
            earth.tier = 2;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Sanctum of Anti-Earth";
            itemInfo.description = "Attacks against earth enemies are guaranteed to be critical!";
            itemInfo.itemID = "earthCrit";

            earth.text = itemInfo;
            earth = earth.loadSprite("antiearth");

            Items.Register(earth);

            ItemInfo wind = new ItemInfo();
            wind.name = "windCrit";
            wind.item = new WindCrit();
            wind.tier = 2;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Sanctum of Anti-Wind";
            itemInfo.description = "Attacks against wind enemies are guaranteed to be critical!";
            itemInfo.itemID = "windCrit";

            wind.text = itemInfo;
            wind = wind.loadSprite("antiwind");

            Items.Register(wind);

            ItemInfo thunder = new ItemInfo();
            thunder.name = "thunderCrit";
            thunder.item = new ThunderCrit();
            thunder.tier = 4;

            itemInfo = new TextManager.ItemInfo();
            itemInfo.displayName = "Sanctum of Anti-Thunder";
            itemInfo.description = "Attacks against thunder enemies are guaranteed to be critical!";
            itemInfo.itemID = "thunderCrit";

            thunder.text = itemInfo;
            thunder = thunder.loadSprite("antithunder");

            Items.Register(thunder);
        }

        public static void AddBlobBoss()
        {
            On.ExitRoomEventHandler.Start += addToPool;
        }
        public static void addToPool(On.ExitRoomEventHandler.orig_Start orig, ExitRoomEventHandler self)
        {
            self.miniBossGroupList.Add(
                new List<Enemy.EName>
                {
                    Enemy.EName.SuperBlob,
                    Enemy.EName.Blob,
                    Enemy.EName.BlobRoller,
                    Enemy.EName.BlobSpitter
                }
            );
            self.miniBossGroupList.Add(
                new List<Enemy.EName>
                {
                    Enemy.EName.SuperMovingStatue,
                    Enemy.EName.MovingStatue,
                    Enemy.EName.EnemyTurret,
                    Enemy.EName.MovingStatue
                }
            );
            ExitRoomEventHandler.miniBossGroupCount = 9;
            orig(self);
        }
        public static EnemyHealthBar bar;
        public static void MakeNewDialogueTest()
        {
            DialogueCreator.RegisterDialogue("mod",DialogueCreator.GenerateDialog(new List<string>() {"Among us sus", "Among us sus?", "Among us sus!" }));
        }

        public void OnLevelWasLoaded()
        {
            if (SceneManager.GetActiveScene().name.ToLower()=="pvp")
            {
                GameObject mimi = MimicNpc.Prefab;
                Instantiate(mimi, new Vector3(0, 9, 0), Quaternion.identity);
            }
        }

        public static void MakeStatMod(string id, float value, int priority=10, VarStatModType scaling = VarStatModType.Additive, bool thing = false)
        {

        }

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
}