﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
	class IceCrit : FrostCrit
    {
		public IceCrit()
        {
			this.category = global::Item.Category.Offense;
			this.element = "frost";
			this.ID = "frostCrit";
			
		}
    }
	class FlameCrit : FrostCrit
	{
		public FlameCrit()
		{
			this.category = global::Item.Category.Offense;
			this.element = "flame";
			this.ID = "flameCrit";
		}
	}
	class EarthCrit : FrostCrit
	{
		public EarthCrit()
		{
			this.category = global::Item.Category.Offense;
			this.element = "earth";
			this.ID = "earthCrit";
		}
	}
	class WindCrit : FrostCrit
	{
		public WindCrit()
		{
			this.category = global::Item.Category.Offense;
			this.element = "wind";
			this.ID = "windCrit";
		}
	}
	class ThunderCrit : FrostCrit
	{
		public ThunderCrit()
		{
			this.category = global::Item.Category.Offense;
			this.element = "thunder";
			this.ID = "thunderCrit";
		}
	}
	class FrostCrit : Item
	{
		public FrostCrit()
		{
			this.category = global::Item.Category.Offense;
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
			if (givenStatus)
			{
				On.Health.TakeDamage += FrostThing;
				//On.Health.TakeDamage += this.FrostThing;
				return;
			}
			On.Health.TakeDamage -= FrostThing;

		}

		public bool FrostThing(On.Health.orig_TakeDamage orig, Health self, AttackInfo info, Entity attackEntity, bool crit)
		{
			Entity enemy = self.entityScript;
			if (enemy.gameObject.name.ToLower().Contains(element))
			{
				//info.isCritical = true;
				if (!crit && !info.isCritical)
				{
					info.critHitChance = 100f;
					return orig(self, info, attackEntity, false);
				} else
				{
					info.critHitChance = 100f;
					return orig(self, info, attackEntity, false);
				}
			}
			return orig(self, info, attackEntity, crit);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000011 RID: 17
		public override string ExtraInfo
		{
			get
			{
				return base.PercentToStr(1.0f, "+");
			}
		}

		// Token: 0x06000019 RID: 25


		// Token: 0x04000003 RID: 3
		public static string staticID = "frostCrit";
		public static int count = 0;
		public static string[] elements = new string[]{"frost","flame","earth","wind","thunder"};
		public string element = "Frost";

	}
}
