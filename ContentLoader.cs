using BepInEx;
using BepInEx.Configuration;
using LegendAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using UnityEngine;

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

            // This is the just a first little tester code to see if our mod is running on WoL. You'll see it in the BepInEx console
            Debug.Log("Loading Outfits");
            OutfitInfo outfitInfo = new OutfitInfo();
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
            outfitInfo.name = "Shade";
            outfitInfo.outfit = new global::Outfit("Mythical::Shade", 24, new List<global::OutfitModStat>
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
            outfitInfo.outfit = new global::Outfit("Mythical::Hoarder", 23, new List<global::OutfitModStat>
            {
                new global::OutfitModStat(global::OutfitModStat.OutfitModType.Speed, 0f, -0.1f, 0f, false),
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo.customDesc = ((bool b) => "You can carry duplicates of items!");
            outfitInfo.customMod = ((player, b, b2) => {
                if (b)
                {
                    On.Inventory.AddItem_Item_bool_bool += Inventory_AddItem;
                    On.Inventory.ContainsItem += Inventory_Contains;
                } else
                {
                    On.Inventory.AddItem_Item_bool_bool -= Inventory_AddItem;
                    On.Inventory.ContainsItem -= Inventory_Contains;
                }
            });

            Outfits.Register(outfitInfo);


            //SampleSkillLoader.Awake();


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
            itemInfo.itemID = MonsterTooth.staticID;

            spr = ImgHandler.LoadSprite("cannon");

            behemoth.text = itemInfo;
            behemoth.icon = (spr != null ? spr : null);

            Items.Register(behemoth);

        }

        public bool Inventory_Contains(On.Inventory.orig_ContainsItem orig, Inventory self, string id)
        {
            return false;
        }

        public bool Inventory_AddItem(On.Inventory.orig_AddItem_Item_bool_bool orig, Inventory self, Item item, bool show, bool ignoreMax)
        {
            return orig(self, item, show, true);
        }

        // This Update() function will run every frame
        
        // Here, we'll hook on to the CameraController's Awake function, to mess with things when it initializes.
       

        // Here, we're hooking on the Update function. This code runs every frame on that CameraController
        
    }
}
