using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
	public class InvisibleOnLowHealth : Item
	{
		// Token: 0x06002321 RID: 8993 RVA: 0x0011F0E0 File Offset: 0x0011D2E0
		public InvisibleOnLowHealth()
		{
			this.ID = InvisibleOnLowHealth.staticID;
			this.category = Item.Category.Misc;
			this.healthThreshold = PlayerStatusBar.healthBlinkThreshold;
			//this.cdMod = new NumVarStatMod(this.ID, 0f, 10, VarStatModType.Multiplicative, false, false);
		}

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06002322 RID: 8994 RVA: 0x0001DBE1 File Offset: 0x0001BDE1
		private bool CDReady
		{
			get
			{
				return InvisibleOnLowHealth.idCDDict.ContainsKey(this.parentID) && ChaosStopwatch.Check(InvisibleOnLowHealth.idCDDict[this.parentID]);
			}
		}

		// Token: 0x06002323 RID: 8995 RVA: 0x0001DC10 File Offset: 0x0001BE10
		public override void Activate()
		{
			this.SetModStatus(true);
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x0001DC19 File Offset: 0x0001BE19
		public override void Deactivate()
		{
			this.SetModStatus(false);
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x0011F144 File Offset: 0x0011D344
		private void SetModStatus(bool givenStatus)
		{
			if (!this.SetParentAsPlayer())
			{
				return;
			}
			this.parentID = this.parentPlayer.playerID;
			if (!InvisibleOnLowHealth.idCDDict.ContainsKey(this.parentID))
			{
				InvisibleOnLowHealth.idCDDict[this.parentID] = -1;
			}
			this.SetItemBarStatus();
			Health health = this.parentPlayer.health;
			health.takeDamageEventHandlers = (Health.OnTakeDamageHandler)Delegate.Remove(health.takeDamageEventHandlers, new Health.OnTakeDamageHandler(this.OnTakeDamage));
			NumVarStat healthStat = this.parentPlayer.health.healthStat;
			healthStat.statChangedEventHandlers = (VarStat<float>.StatChangedEventHandler)Delegate.Remove(healthStat.statChangedEventHandlers, new VarStat<float>.StatChangedEventHandler(this.OnHealthChange));
			if (givenStatus)
			{
				Health health2 = this.parentPlayer.health;
				health2.takeDamageEventHandlers = (Health.OnTakeDamageHandler)Delegate.Combine(health2.takeDamageEventHandlers, new Health.OnTakeDamageHandler(this.OnTakeDamage));
				NumVarStat healthStat2 = this.parentPlayer.health.healthStat;
				healthStat2.statChangedEventHandlers = (VarStat<float>.StatChangedEventHandler)Delegate.Combine(healthStat2.statChangedEventHandlers, new VarStat<float>.StatChangedEventHandler(this.OnHealthChange));
			}
			else
			{
				this.RemoveFromItemBar();
			}
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x0011F288 File Offset: 0x0011D488
		private void SetItemBarStatus()
		{
			if (this.CDReady && this.parentPlayer != null && this.parentPlayer.health.HealthPercentage <= this.healthThreshold)
			{
				this.UpdateItemBar(ItemStatusBar.ItemState.Ready);
			}
			else
			{
				this.UpdateItemBar(ItemStatusBar.ItemState.Disabled);
			}
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x0001DC22 File Offset: 0x0001BE22
		private void OnTakeDamage(AttackInfo givenInfo, Entity givenEnt = null)
		{
			this.damageTakenOnFrame = true;
		}

		// Token: 0x06002328 RID: 9000 RVA: 0x0011F2E0 File Offset: 0x0011D4E0
		private void OnHealthChange()
		{
			if (!this.damageTakenOnFrame)
			{
				return;
			}
			this.damageTakenOnFrame = false;
			if (!this.CDReady || this.parentPlayer == null || this.parentPlayer.health.HealthPercentage > this.healthThreshold)
			{
				return;
			}
			this.parentPlayer.overdriveEffects.Burst(40, false, true, true);
			this.parentPlayer.health.SetInvulnerabilityDuration(this.durationStopwatch.Delay, true);
			this.parentPlayer.StartCoroutine(this.StartTimer());
		}

		// Token: 0x06002329 RID: 9001 RVA: 0x0011F36C File Offset: 0x0011D56C
		private IEnumerator StartTimer()
		{
			InvisibleOnLowHealth.idCDDict[this.parentID] = ChaosStopwatch.Begin(InvisibleOnLowHealth.cdTime, false, 0f, 0, 0);
			this.durationStopwatch.IsRunning = true;
			this.UpdateItemBar(ItemStatusBar.ItemState.Active);
			while (this.durationStopwatch.IsRunning)
			{
				yield return null;
			}
			this.UpdateItemBar(ItemStatusBar.ItemState.Disabled);
			while (!this.CDReady)
			{
				yield return null;
			}
			this.SetItemBarStatus();
			yield break;
		}

		// Token: 0x04002950 RID: 10576
		public static string staticID = "InvisibleOnLowHealth";

		// Token: 0x04002951 RID: 10577
		private static Dictionary<int, int> idCDDict = new Dictionary<int, int>();

		// Token: 0x04002952 RID: 10578
		private static float cdTime = 20f;

		// Token: 0x04002953 RID: 10579
		private ChaosQuickStopwatch durationStopwatch = new ChaosQuickStopwatch(5f);

		// Token: 0x04002954 RID: 10580
		private bool damageTakenOnFrame;

		// Token: 0x04002955 RID: 10581
		private float healthThreshold;

		// Token: 0x04002956 RID: 10582

		// Token: 0x04002957 RID: 10583
		private int parentID = -1;
	}
}
