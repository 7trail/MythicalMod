using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
    class AttackSpeedUpItem : Item
    {
		public AttackSpeedUpItem()
		{
			this.ID = AttackSpeedUpItem.staticID;
			this.category = global::Item.Category.Offense;

			atkSpeed = new NumVarStatMod(this.ID, 0.1f, 11, VarStatModType.Multiplicative, false, false);
			
		}
		public override void Activate()
		{
			this.SetModStatus(true);
		}


		// Token: 0x0600000F RID: 15
		public override void Deactivate()
		{
			this.SetModStatus(false);
		}

		public virtual void SetModStatus(bool givenStatus)
		{
			//global::StatManager.ModifyAllStatData(this.damageMod, this.parentSkillCategory, global::StatData.damageStr, new global::StatManager.ModApplyConditional(base.IgnoreStatusConditional), givenStatus);
			if (this.parentEntity.animSpeedHandler != null && atkSpeed != null)
            {
				this.parentEntity.animSpeedHandler.Modify(atkSpeed,givenStatus);
            }
		}

		public override string ExtraInfo
		{
			get
			{
				return base.PercentToStr(0.1f, "+");
			}
		}

		public static string staticID = "attackSpeedUpItem";
		public static NumVarStatMod atkSpeed;
	}
}
