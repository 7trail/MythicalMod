using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
				Health.globalTakeDamageActualHandlers += FrostThing;
				//On.Health.TakeDamage += this.FrostThing;
				return;
			}
			Health.globalTakeDamageActualHandlers -= FrostThing;
		}

		public void FrostThing(AttackInfo info, Entity attackEntity, Entity hurtEntity)
		{
			if (hurtEntity is Enemy)
            {
				Enemy enemy = (Enemy)hurtEntity;
				if (enemy.name.StartsWith("Frost"))
                {
					info.isCritical = true;
                }
            }
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
