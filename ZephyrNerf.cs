using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    class ZephyrNerf : Player.UseBuffSkill
    {
		public ZephyrNerf(FSM parentFSM, Player parentEntity) : base(Player.UseWindDefense.staticID, parentFSM, parentEntity)
		{
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x060037A6 RID: 14246 RVA: 0x0002CA89 File Offset: 0x0002AC89
		public static GameObject EffectPrefab
		{
			get
			{
				if (Player.UseWindDefense._effectPrefab == null)
				{
					Player.UseWindDefense._effectPrefab = ChaosBundle.Get("Assets/Prefabs/Effects/Air/WindBallSmallEffect.prefab");
				}
				return Player.UseWindDefense._effectPrefab;
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x060037A7 RID: 14247 RVA: 0x001A2E74 File Offset: 0x001A1074
		private bool ParentIsMoving
		{
			get
			{
				this.checkState = (this.fsm.currentState as Player.SkillState);
				return (this.checkState != null && (this.checkState.isBasic || this.checkState.isMovementSkill)) || (this.fsm != null && (this.fsm.currentStateName == base.name || this.fsm.currentStateName == "Run" || this.fsm.currentStateName == Player.SlideState.staticID));
			}
		}
		public IEnumerator delayZephyr(bool givenStatus)
        {
			yield return new WaitForSeconds(0.1f);
			base.SetBuffStatus(givenStatus);
			this.empStatus = this.IsEmpowered;
			this.parent.overheadPrompt.PlayShakeAnalog(2f);
			SoundManager.PlayAudioWithDistance("WindArrowStart", new Vector2?(this.currentPosition), null, 24f, -1f, 0.6f, false);
			string audioID = "TornadoEnd";
			Vector2? soundOrigin = new Vector2?(this.currentPosition);
			float num = UnityEngine.Random.Range(0.75f, 0.85f);
			SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, -1f, num, false);
			DustEmitter poolItem = PoolManager.GetPoolItem<DustEmitter>();
			int particleCount = 100;
			num = 0.5f;
			Vector3? emitPosition = new Vector3?(this.currentPosition);
			poolItem.EmitCircle(particleCount, num, -1f, -1f, emitPosition, null);
			PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(5), new Vector3?(this.attackPosition), ParticleEffectOverrides.StartSpeed1p5, null, 0f, null, null);
			yield return null;
		}
		// Token: 0x060037A8 RID: 14248 RVA: 0x001A2F20 File Offset: 0x001A1120
		public override void SetBuffStatus(bool givenStatus)
		{
			
			this.evadeStatus = false;
			if (givenStatus)
			{
				this.parent.StartCoroutine(delayZephyr(givenStatus));
			}
			else
			{
				base.SetBuffStatus(givenStatus);
				this.ToggleMods(false, true);
				this.parent.overheadPrompt.HidePrompt(string.Empty);
				if (this.currentEffect != null)
				{
					this.currentEffect.Stop();
					if (this.currentEffect.gameObject != null)
					{
						UnityEngine.Object.Destroy(this.currentEffect.gameObject, 1.5f);
					}
				}
				PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(6), new Vector3?(this.attackPosition), null, null, 0f, null, null);
				string audioID = "TornadoEnd";
				Vector2? soundOrigin = new Vector2?(this.currentPosition);
				float num = UnityEngine.Random.Range(0.75f, 0.85f);
				SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, -1f, num, false);
			}
		}

		// Token: 0x060037A9 RID: 14249 RVA: 0x001A3124 File Offset: 0x001A1324
		public override void WhileBuffActive()
		{
			base.WhileBuffActive();
			if (this.evadeStatus)
			{
				string audioID = "WindCircle";
				Vector2? soundOrigin = new Vector2?(this.parent.transform.position);
				float overridePitch = UnityEngine.Random.Range(0.9f, 1.1f);
				SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, -1f, overridePitch, false);
				if (!this.ParentIsMoving)
				{
					this.evadeStatus = false;
					this.ToggleMods(false, this.empStatus);
				}
			}
			else if (this.ParentIsMoving)
			{
				this.evadeStatus = true;
				this.ToggleMods(true, this.empStatus);
			}
		}

		// Token: 0x060037AA RID: 14250 RVA: 0x001A31CC File Offset: 0x001A13CC
		private void ToggleMods(bool givenStatus, bool givenEmpStatus)
		{
			if (this.parent.health != null)
			{
				this.parent.health.evadeStat.Modify(Player.UseWindDefense.evadeMod, givenStatus);
			}
			if (givenEmpStatus)
			{
				if (this.parent.movement != null)
				{
					this.parent.movement.moveSpeedStat.Modify(Player.UseWindDefense.moveMod, givenStatus);
				}
				this.parent.fall.ignoreFall.Modify(Player.UseWindDefense.fallMod, givenStatus);
				this.parent.TogglePreventFallCollider(!givenStatus);
			}
			else
			{
				if (this.parent.movement != null)
				{
					this.parent.movement.moveSpeedStat.Modify(Player.UseWindDefense.moveMod, false);
				}
				this.parent.fall.ignoreFall.Modify(Player.UseWindDefense.fallMod, false);
				this.parent.TogglePreventFallCollider(true);
			}
			this.parent.ToggleEnemyFloorCollisions(!givenStatus);
			if (this.currentEffect == null)
			{
				this.currentEffect = Globals.ChaosInst<ParticleEffect>(Player.UseWindDefense.EffectPrefab, null, new Vector3?(this.parent.hurtBoxTransform.position), null);
			}
			if (givenStatus)
			{
				ParticleEffect particleEffect = this.currentEffect;
				int? particleCount = new int?(30);
				ParticleSystemOverride overrides = Player.UseWindDefense.evadeEffectOverride;
				particleEffect.Emit(particleCount, null, overrides, null, 0f, null, null);
				this.currentEffect.followTransform = this.parent.hurtBoxTransform;
				ParticleEffect particleEffect2 = this.currentEffect;
				overrides = Player.UseWindDefense.evadeEffectOverride;
				particleEffect2.Play(null, null, overrides, null, null, null, 0f, true);
			}
			else
			{
				this.currentEffect.Stop();
			}
		}

		// Token: 0x04003BB6 RID: 15286
		public new static string staticID = "UseWindDefense";

		// Token: 0x04003BB7 RID: 15287
		public static ParticleSystemOverride evadeEffectOverride = new ParticleSystemOverride
		{
			radius = new float?(0.75f),
			startColor = new Color32?(new Color(1f, 1f, 1f, 0.33f))
		};

		// Token: 0x04003BB8 RID: 15288
		private static GameObject _effectPrefab;

		// Token: 0x04003BB9 RID: 15289
		private static BoolVarStatMod fallMod = new BoolVarStatMod(Player.UseWindDefense.staticID, true, 10);

		// Token: 0x04003BBA RID: 15290
		private static NumVarStatMod evadeMod = new NumVarStatMod(Player.UseWindDefense.staticID, 1f, 0, VarStatModType.Override, false, false);

		// Token: 0x04003BBB RID: 15291
		private static NumVarStatMod moveMod = new NumVarStatMod(Player.UseWindDefense.staticID, 0.4f, 10, VarStatModType.Multiplicative, false, false);

		// Token: 0x04003BBC RID: 15292
		private ParticleEffect currentEffect;

		// Token: 0x04003BBD RID: 15293
		private bool evadeStatus;

		// Token: 0x04003BBE RID: 15294
		private bool empStatus;

		// Token: 0x04003BBF RID: 15295
		private Player.SkillState checkState;
	}
}
