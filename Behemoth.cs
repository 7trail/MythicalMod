using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using On;


namespace Mythical
{
	public class Behemoth : global::Item
	{
		// Token: 0x0600000D RID: 13
		public Behemoth()
		{
			this.ID = Behemoth.staticID;
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
				On.Health.TakeDamage += this.ExplosionDamage;
				return;
			}
			On.Health.TakeDamage -= this.ExplosionDamage;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000011 RID: 17
		public override string ExtraInfo
		{

			get
			{
				return base.PercentToStr(this.damageMod, "-");
			}
		}

		// Token: 0x06000012 RID: 18
		public void EnemyKillHeal(On.Player.orig_OnEnemyKill orig, global::Player self, global::Entity givenEntity)
		{
			orig(self, givenEntity);
			self.health.RestoreHealth(3, true, true, false, true);
		}

		// Token: 0x06000019 RID: 25
		public bool ExplosionDamage(On.Health.orig_TakeDamage orig, global::Health self, global::AttackInfo attackInfo, global::Entity attackEntity,bool crit)
		{
			if (UnityEngine.Random.value < 0.25f)
			{
				this.CreateExplosion(global::FlameBurst.burnSkillID, self.transform.position);
			}
			return orig(self, attackInfo, attackEntity,crit);
		}

		// Token: 0x06000058 RID: 88
		public void CreateExplosion(string skillID, Vector2 givenPosition)
		{
			global::FlameBurst.CreateBurst(givenPosition, this.parentSkillCategory, skillID, 1, this.explosionRadius, true);
			global::SoundManager.PlayWithDistAndSPR("BlazingBlitzEnd", givenPosition, 1f);
			global::PoolManager.GetPoolItem<global::ParticleEffect>("SmokeEmitter").Emit(new int?(6), new Vector3?(givenPosition), null, null, 0f, null, null);
			global::CameraController.ShakeCamera(0.25f, false);
		}

		// Token: 0x04000003 RID: 3
		public static string staticID = "BrilliantBehemoth";

		// Token: 0x04000004 RID: 4
		protected global::NumVarStatMod damageMod;

		// Token: 0x04000013 RID: 19
		public float explosionRadius = 1.5f;
	}
}
