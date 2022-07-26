using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
    public static class CursedRelics
    {
        public static void Init()
        {
			On.Item.hook_IsUnlocked hook_IsUnlocked = (On.Item.orig_IsUnlocked orig, string s, bool b) => orig(s, b) || global::LootManager.completeItemDict[s].isCursed;
			On.Inventory.hook_RemoveItem hook_RemoveItem = delegate (On.Inventory.orig_RemoveItem orig, global::Inventory self, string s, bool b3, bool b4)
			{
				//bool flag2 = !b3 && global::LootManager.completeItemDict[s].isCursed && self.parentEntity is global::Player;
				//if (flag2)
				{
					b3 = true;
				}
				return orig(self, s, b3, b4);
			};
			On.Player.hook_GiveDesignatedItem hook_GiveDesignatedItem = delegate (On.Player.orig_GiveDesignatedItem orig, global::Player self, string s)
			{
				bool flag2 = s != null && s != string.Empty && global::LootManager.completeItemDict[s].isCursed;
				if (flag2)
				{
					self.inventory.AddItem(s, false, false);
				}
				else
				{
					orig(self, s);
				}
			};

			On.RelicChestUI.LoadPlayerRelics += NoxHook;
            On.Item.IsUnlocked += hook_IsUnlocked;
            On.Player.GiveDesignatedItem += hook_GiveDesignatedItem;
            On.Inventory.RemoveItem += hook_RemoveItem;
        }

		static internal void NoxHook(On.RelicChestUI.orig_LoadPlayerRelics orig, global::RelicChestUI self)
		{
			orig(self);
			foreach (string text in global::LootManager.cursedItemIDList)
			{
				global::Item.Category category = global::LootManager.completeItemDict[text].category;
				self.categoryInfoDict[category].idList.Add(text);
				self.categoryInfoDict[category].unlockedCount++;
			}
			foreach (string text2 in global::LootManager.cursedHubOnlyItemIDList)
			{
				global::Item.Category category2 = global::LootManager.completeItemDict[text2].category;
				self.categoryInfoDict[category2].idList.Add(text2);
				self.categoryInfoDict[category2].unlockedCount++;
			}
		}
	}
}
