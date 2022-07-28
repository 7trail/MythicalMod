using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Chaos.AnimatorExtensions;
using System.Collections;

namespace Mythical
{
	public class ContestantAirShieldState : Enemy.SkillState
	{
		// Token: 0x060016FB RID: 5883 RVA: 0x000C7910 File Offset: 0x000C5D10
		public ContestantAirShieldState(string newName, FSM newFSM, Enemy newEnt) : base(newName, newFSM, newEnt, 1, 1, 1, 1, 5, false)
		{
			this.parent = newEnt;
			this.requireLineOfSight = false;
			this.evadeMod = new NumVarStatMod(this.skillID, 1f, 10, VarStatModType.Override, false);
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x000C797A File Offset: 0x000C5D7A
		public override bool IsReady()
		{
			return base.IsReady() && !this.evadeActive;
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x000C7993 File Offset: 0x000C5D93
		public override void OnEnter()
		{
			base.OnEnter();
			this.parent.movement.EndMovement();
			this.parent.anim.Play("ChargeAlt", -1, 0f);
			this.evadeActive = false;
		}

		// Token: 0x060016FE RID: 5886 RVA: 0x000C79CD File Offset: 0x000C5DCD
		public override void WindupUpdate()
		{
			base.WindupUpdate();
			this.parent.anim.Play("ChargeAlt", -1, 0f);
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x000C79F0 File Offset: 0x000C5DF0
		public override void OnExit()
		{
			base.OnExit();
			if (!this.WindupCheck)
			{
				this.BreakShield();
			}
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x000C7A0C File Offset: 0x000C5E0C
		public override void ExecuteSkill()
		{
			base.ExecuteSkill();
			this.parent.anim.Play("ChargeAlt", -1, 0.5f);
			this.DeactivatePowerUpEffect(false);
			PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(5), new Vector3?(this.parent.hurtBoxTransform.position), ParticleEffectOverrides.StartSpeed1p5, null, 0f, null, null);
			DustEmitter poolItem = PoolManager.GetPoolItem<DustEmitter>();
			int particleCount = 100;
			float num = 0.5f;
			Vector3? emitPosition = new Vector3?(this.parent.transform.position);
			poolItem.EmitCircle(particleCount, num, -1f, -1f, emitPosition, null);
			CameraController.ShakeCamera(0.3f, false);
			SoundManager.PlayAudioWithDistance("WindArrowStart", new Vector2?(this.parent.transform.position), null, 24f, -1f, 0.6f, false);
			string audioID = "TornadoEnd";
			Vector2? soundOrigin = new Vector2?(this.parent.transform.position);
			num = UnityEngine.Random.Range(0.75f, 0.85f);
			SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, -1f, num, false);
			this.parent.StartCoroutine(this.EvadeOn());
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x000C7B64 File Offset: 0x000C5F64
		private void OnEvade()
		{
			PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(6), new Vector3?(this.parent.hurtBoxTransform.position), null, null, 0f, null, null);
			string audioID = "TornadoEnd";
			Vector2? soundOrigin = new Vector2?(this.parent.transform.position);
			float overridePitch = UnityEngine.Random.Range(0.75f, 0.85f);
			SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, -1f, overridePitch, false);
			if (this.parent.health != null)
			{
				Health health = this.parent.health;
				health.evadeEventHandlers = (Health.EvadeEventHandler)Delegate.Remove(health.evadeEventHandlers, new Health.EvadeEventHandler(this.OnEvade));
			}
			this.BreakShield();
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x000C7C70 File Offset: 0x000C6070
		public void BreakShield()
		{
			this.evadeActive = false;
			if (this.parent.health != null)
			{
				this.parent.health.evadeStat.Modify(this.evadeMod, false);
			}
			if (this.currentEffect != null && this.currentEffect.gameObject != null)
			{
				this.currentEffect.Stop();
				UnityEngine.Object.Destroy(this.currentEffect.gameObject, 1.5f);
			}
			string audioID = "TornadoEnd";
			Vector2? soundOrigin = new Vector2?(this.parent.transform.position);
			float overridePitch = UnityEngine.Random.Range(0.75f, 0.85f);
			SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, -1f, overridePitch, false);
			base.StartCooldown(-1f);
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x000C7D50 File Offset: 0x000C6150
		public IEnumerator EvadeOn()
		{
			if (this.parent.health == null)
			{
				yield break;
			}
			this.evadeActive = true;
			this.parent.health.evadeStat.Modify(this.evadeMod, true);
			Health health = this.parent.health;
			health.evadeEventHandlers = (Health.EvadeEventHandler)Delegate.Remove(health.evadeEventHandlers, new Health.EvadeEventHandler(this.OnEvade));
			Health health2 = this.parent.health;
			health2.evadeEventHandlers = (Health.EvadeEventHandler)Delegate.Combine(health2.evadeEventHandlers, new Health.EvadeEventHandler(this.OnEvade));
			this.currentEffect = Globals.ChaosInst<ParticleEffect>(Player.UseWindDefense.EffectPrefab, this.parent.hurtBoxTransform, new Vector3?(this.parent.hurtBoxTransform.position), null);
			ParticleEffect particleEffect = this.currentEffect;
			int? particleCount = new int?(30);
			ParticleSystemOverride evadeEffectOverride = Player.UseWindDefense.evadeEffectOverride;
			particleEffect.Emit(particleCount, null, evadeEffectOverride, null, 0f, null, null);
			this.currentEffect.followTransform = this.parent.hurtBoxTransform;
			ParticleEffect particleEffect2 = this.currentEffect;
			evadeEffectOverride = Player.UseWindDefense.evadeEffectOverride;
			particleEffect2.Play(null, null, evadeEffectOverride, null, null, null, 0f, true);
			this.durationStopwatch.IsRunning = true;
			while (this.durationStopwatch.IsRunning && this.evadeActive)
			{
				if (!this.audioStopwatch.IsRunning)
				{
					this.audioStopwatch.IsRunning = true;
					string audioID = "WindCircle";
					Vector2? soundOrigin = new Vector2?(this.parent.transform.position);
					float overridePitch = UnityEngine.Random.Range(0.9f, 1.1f);
					SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, 0.075f, overridePitch, false);
				}
				yield return null;
			}
			this.BreakShield();
			yield break;
		}

		// Token: 0x04001D44 RID: 7492
		private ChaosQuickStopwatch durationStopwatch = new ChaosQuickStopwatch(4f);

		// Token: 0x04001D45 RID: 7493
		private ChaosQuickStopwatch audioStopwatch = new ChaosQuickStopwatch(0.25f);

		// Token: 0x04001D46 RID: 7494
		private NumVarStatMod evadeMod;

		// Token: 0x04001D47 RID: 7495
		private bool evadeActive;

		// Token: 0x04001D48 RID: 7496
		private ParticleEffect currentEffect;

	}
}
