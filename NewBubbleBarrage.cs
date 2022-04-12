using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
	public class NewBubbleBarrage : Player.ProjectileChainAttackState
	{
		// Token: 0x06003520 RID: 13600 RVA: 0x0002A9A6 File Offset: 0x00028BA6
		public NewBubbleBarrage(FSM parentFSM, Player parentEntity) : base(NewBubbleBarrage.staticID, parentFSM, parentEntity)
		{
			this.SetAnimTimes(0.15f, 0.2f, 0.35f, 0.6f, 0.8f, 1f);
			this.shotDelay = 0.0625f;
		}

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x06003521 RID: 13601 RVA: 0x00029C8D File Offset: 0x00027E8D
		public override string OnEnterAnimStr
		{
			get
			{
				return this.parent.ForehandAnimStr;
			}
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x06003522 RID: 13602 RVA: 0x00029C8D File Offset: 0x00027E8D
		public override string OnHoldAnimStr
		{
			get
			{
				return this.parent.ForehandAnimStr;
			}
		}

		// Token: 0x06003523 RID: 13603 RVA: 0x0002981D File Offset: 0x00027A1D
		public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod)
		{
			base.SetEmpowered(givenStatus, givenMod);
			base.SetSkillLevel((!this.IsEmpowered) ? 1 : 2);
		}

		// Token: 0x06003524 RID: 13604 RVA: 0x0002A9E4 File Offset: 0x00028BE4
		public override void OnEnter()
		{
			base.OnEnter();
			CameraController.ShakeCamera(0.2f, false);
		}

		// Token: 0x06003525 RID: 13605 RVA: 0x00172D1C File Offset: 0x00170F1C
		public override void FireChainProjectile()
		{
			this.spawnPos = (Vector2)this.parent.attackOriginTrans.position + this.inputVector * 0.5f;
			this.currentBP = (BubbleProjectile)this.CreateProjectile(AquaPrisonProjectile.Prefab, new Vector3?(this.spawnPos), null, null, true);
			PoolManager.GetPoolItem<WaterDropEmitter>().Emit(new int?(2), new Vector3?(this.spawnPos + this.inputVector), WaterDropEmitter.MedWaterExplosionLightOverride, null, 0f, null, null);
			if (this.IsEmpowered)
			{
				this.currentBP.isLarge = true;
				WaterHitEffect poolItem = PoolManager.GetPoolItem<WaterHitEffect>();
				int? particleCount = new int?(1);
				Vector3? vector = new Vector3?(this.spawnPos);
				Vector3? vector2 = new Vector3?(Globals.GetRotationVector(this.inputVector));
				poolItem.EmitSingle(particleCount, vector, ParticleEffectOverrides.StartColorAlpha0p25, vector2, 0f, null);
			}
			else
			{
				ParticleEffect poolItem2 = PoolManager.GetPoolItem<ParticleEffect>("WaterHitSmallEffect");
				int? particleCount = new int?(1);
				Vector3? vector2 = new Vector3?(this.spawnPos);
				Vector3? vector = new Vector3?(Globals.GetRotationVector(this.inputVector));
				poolItem2.Emit(particleCount, vector2, ParticleEffectOverrides.StartColorAlpha0p25, vector, 0f, null, null);
			}
			this.FireProjectile(this.currentBP, (this.inputVector + UnityEngine.Random.insideUnitCircle * 0.35f).normalized, false, UnityEngine.Random.Range(8f, 12f), 0f, "BubblePop", false);
		}

		// Token: 0x0400380A RID: 14346
		public new static string staticID = "Mythical::NewBubbleBarrage";

		// Token: 0x0400380B RID: 14347
		private BubbleProjectile currentBP;

		// Token: 0x0400380C RID: 14348
		private Vector2 spawnPos;
	}
}
