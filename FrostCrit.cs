using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    class FrostCrit : Item
    {
        public FrostCrit()
		{
			this.ID = FrostCrit.staticID;
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
			GameObject obj = self.parentObject;

			if (obj != null)
            {
				Entity enemy = obj.GetComponent<Entity>();
				if (enemy.gameObject.name.ToLower().StartsWith("frost"))
                {
					//info.isCritical = true;
					info.critHitChance = 100f;
                }
            }
			return orig(self, info, attackEntity,crit);
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
		public static string staticID = "FrostCrit";

	}
}
