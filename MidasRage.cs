using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    class MidasRage : Item
    {
		public MidasRage()
		{
			this.ID = staticID;
			this.category = global::Item.Category.Misc;
			//this.damageMod = new global::NumVarStatMod(this.ID, -0.3f, 10, VarStatModType.Multiplicative, false);
		}

		// Token: 0x0600000E RID: 14
		public override void Activate()
		{
			this.SetModStatus(true);
		}

		// Token: 0x0600000F RID: 15
		public override void Deactivate()
		{
			this.SetModStatus(false);
		}

		// Token: 0x06000010 RID: 16
		public virtual void SetModStatus(bool givenStatus)
		{
			//global::StatManager.ModifyAllStatData(this.damageMod, this.parentSkillCategory, global::StatData.damageStr, new global::StatManager.ModApplyConditional(base.IgnoreStatusConditional), givenStatus);
			DropTableEntry entry = LootManager.lootDropTable["Destructible"];
			if (givenStatus)
			{
				startMin = entry.minGold;
				startMax = entry.maxGold;
				startChance = entry.goldDropRate;
				entry.minGold = 6;
				entry.maxGold = 10;
				entry.goldDropRate = 0.5f;
				return;
			}
			entry.minGold = startMin;
			entry.maxGold = startMax;
			entry.goldDropRate = startChance;
		}

		public static bool hasEditedDestructible = false;
		public static int startMin=0;
		public static int startMax=1;
		public static float startChance = 0.5f;
		public static string staticID = "MidasRage";
	}
}
