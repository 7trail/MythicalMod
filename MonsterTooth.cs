using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
	public class MonsterTooth : global::Item
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002091 File Offset: 0x00000291
		public MonsterTooth()
		{
			this.ID = MonsterTooth.staticID;
			this.category = global::Item.Category.Offense;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020AB File Offset: 0x000002AB
		public override void Activate()
		{
			this.SetModStatus(true);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020B4 File Offset: 0x000002B4
		public override void Deactivate()
		{
			this.SetModStatus(false);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020BD File Offset: 0x000002BD
		public virtual void SetModStatus(bool givenStatus)
		{
			if (givenStatus)
			{
				On.Player.OnEnemyKill += this.EnemyKillHeal;
				return;
			}
			On.Player.OnEnemyKill -= this.EnemyKillHeal;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020F1 File Offset: 0x000002F1
		public override string ExtraInfo
		{
			get
			{
				return base.PercentToStr(this.damageMod, "+");
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002104 File Offset: 0x00000304
		public void EnemyKillHeal(On.Player.orig_OnEnemyKill orig, global::Player self, global::Entity givenEntity)
		{
			orig(self, givenEntity);
			self.health.RestoreHealth(3, true, true, false, true);
		}

		// Token: 0x04000001 RID: 1
		public static string staticID = "MonsterTooth";

		// Token: 0x04000002 RID: 2
		protected global::NumVarStatMod damageMod;
	}
}
