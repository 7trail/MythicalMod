using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
	class AllOrNothing : Item
	{
		public AllOrNothing()
		{
			this.category = global::Item.Category.Offense;
			this.ID = staticID;
			this.isCursed = true;
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
				On.Health.TakeDamage += AllNothing;
				//On.Health.TakeDamage += this.FrostThing;
				return;
			}
			On.Health.TakeDamage -= AllNothing;

		}

		public bool AllNothing(On.Health.orig_TakeDamage orig, Health self, AttackInfo info, Entity attackEntity, bool crit)
		{
			Entity enemy = self.entityScript;
			//info.isCritical = true;
			if (UnityEngine.Random.value < 0.5f)
            {
				info.damage *= 3;
			} else
            {
				info.freezeChance = 0;
				info.burnChance = 0;
				info.chaosChance = 0;
				info.critHitChance = 0;
				info.poisonChance = 0;
				info.rootChance = 0;
				info.shockChance = 0;
				info.shakeCamera = false;
				info.slowChance = 0;
				info.knockbackMultiplier = 0;
				info.damage *= 0;
			}
			

			return orig(self, info, attackEntity, crit);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000011 RID: 17
		public override string ExtraInfo
		{
			get
			{
				return base.PercentToStr(0.5f, "+-");
			}
		}

		// Token: 0x06000019 RID: 25


		// Token: 0x04000003 RID: 3
		public static string staticID = "allOrNothing";
		

	}
}
