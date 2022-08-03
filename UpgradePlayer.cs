using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Mythical
{
    public static class UpgradePlayer
    {
        public static void Upgrade(Player p)
        {
			player = p;
			if (Outfit.GetAvailableOutfit(player.outfitID) == null)
			{
				return;
			}
			if (player.equippedOutfit.useLeveling)
			{
				//player.equippedOutfit.levelModStat.AddMod(new NumVarStatMod("TailorNPC", TailorNpc.upgradeValue - 1f, 10, VarStatModType.Multiplicative, false));
				//RunData.prdDict[player.playerID].outFitEnhanced = true;
				player.outfitEnhanced = true;
				Globals.ChaosInst<Transform>(TailorNpc.SparklePrefab, player.attackOriginTrans, new Vector3?(player.attackOriginTrans.position), null);
				return;
			}
			Outfit currentOutfit = new Outfit(Outfit.GetAvailableOutfit(player.outfitID));
			Outfit.GetAvailableOutfit(player.outfitID).SetEquipStatus(player, false);
			if (currentOutfit.outfitID == "Hope" && currentOutfit.modList.Count <= 1)
			{
				currentOutfit = Outfit.enhancedHope;
			}
			else if (currentOutfit.outfitID == "Patience" && currentOutfit.modList.Count <= 1)
			{
				currentOutfit = Outfit.enhancedPatience;
			}
			else
			{
				OutfitModStat currentMod;
				foreach (OutfitModStat outfitModStat in currentOutfit.modList)
				{
					currentMod = outfitModStat;
					if (currentMod != null)
					{
						if (currentMod.modType == OutfitModStat.OutfitModType.Health)
						{
							if (currentMod.addModifier.modValue > 0f)
							{
								currentMod.addModifier.modValue *= TailorNpc.upgradeValue;
							}
							if (currentMod.multiModifier.modValue > 0f)
							{
								currentMod.multiModifier.modValue *= TailorNpc.upgradeValue;
							}
						}
						else
						{
							currentMod.addModifier.modValue *= TailorNpc.upgradeValue;
							currentMod.multiModifier.modValue *= TailorNpc.upgradeValue;
							currentMod.overrideModifier.modValue *= TailorNpc.upgradeValue;
						}
					}
				}
			}
			currentOutfit.SetEquipStatus(player, true);
			player.equippedOutfit = currentOutfit;
			player.outfitEnhanced = true;
			Globals.ChaosInst<Transform>(TailorNpc.SparklePrefab, player.attackOriginTrans, new Vector3?(player.attackOriginTrans.position), null);
		}

        static Player player;

    }
}
