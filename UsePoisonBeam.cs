using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
	public class UsePoisonBeam : Player.UseBaseBeam
	{
		// Token: 0x06003039 RID: 12345 RVA: 0x0017C5E0 File Offset: 0x0017A9E0
		public UsePoisonBeam(FSM parentFSM, Player parentEntity) : base(UsePoisonBeam.staticID, parentFSM, parentEntity)
		{
			this.hasSignatureVariant = true;
			this.beamPrefab = AquaBeam.Prefab;
			this.beamRange = 8f;
			this.beamThickness = 1.5f;
			//base.InitChargeSkillSettings(2, 0f, this.skillData, this);
		}

		// Token: 0x0600303A RID: 12346 RVA: 0x0017C634 File Offset: 0x0017AA34
		public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod)
		{
			base.SetEmpowered(givenStatus, givenMod);
			this.beamThickness = ((!this.IsEmpowered) ? 1.5f : 2.25f);
		}

		// Token: 0x0600303B RID: 12347 RVA: 0x0017C65E File Offset: 0x0017AA5E
		public override void OnEnter()
		{
			base.OnEnter();
			this.shotDelay = ((!this.isUltimate) ? 0.125f : 0.08f);
			this.ultLerpValue = 0f;
		}

		// Token: 0x0600303C RID: 12348 RVA: 0x0017C694 File Offset: 0x0017AA94
		public override bool StartAttack()
		{
			if (this.attackStarted)
			{
				return true;
			}
			base.BaseStartAttack();
			if (this.attackStarted)
			{
				this.currentPosition = this.parent.attackOriginTrans.position;
				CameraController.ShakeCamera(0.25f, false);
				if (this.isUltimate)
				{
					for (int i = 0; i < UsePoisonBeam.ultBeamCount; i++)
					{
						this.ultIndex = i - UsePoisonBeam.ultHalfCount;
						this.ultVecArray[i] = Globals.GetRotatedCircleVector(10, this.ultIndex, this.targetVector);
						this.ultBeamArray[i] = this.CreateBeam(this.currentPosition, this.ultVecArray[i] * 1.25f, Player.UseAquaBeam.ultShotCount);
						foreach (SpriteRenderer sr in this.currentBeam.GetComponentsInChildren<SpriteRenderer>())
						{
							sr.color = Color.yellow;
						}
						this.ultBeamArray[i].setKnockbackOverwrite = (this.ultIndex == 0);
					}
					this.ultStopwatch.IsRunning = true;
					this.shotStopwatchID = ChaosStopwatch.Begin(0f, true, this.shotDelay, Player.UseAquaBeam.ultShotCount, 0);
				}
				else
				{
					this.CreateBeam(this.currentPosition, this.targetVector, this.cooldownRef.chargeCount);
					foreach (SpriteRenderer sr in this.currentBeam.GetComponentsInChildren<SpriteRenderer>())
					{
						sr.color = Color.yellow;
					}
				}
			}
			return this.attackStarted;
		}
		
		// Token: 0x0600303D RID: 12349 RVA: 0x0017C7D4 File Offset: 0x0017ABD4
		public override void UpdateBeamVector()
		{
			if (this.isUltimate)
			{
				this.currentPosition = this.parent.attackOriginTrans.position;
				this.ultLerpValue = this.ultStopwatch.TimePercentage;
				for (int i = 0; i < UsePoisonBeam.ultBeamCount; i++)
				{
					this.currentBeam = this.ultBeamArray[i];
					if (!(this.currentBeam == null))
					{
						this.ultIndex = i - UsePoisonBeam.ultHalfCount;
						this.ultVecArray[i] = Vector2.Lerp(this.ultVecArray[i], this.targetVector, this.ultLerpValue);
						this.PointBeam(this.currentBeam, this.currentPosition + Globals.GetRotatedCircleVector(10, this.ultIndex, this.targetVector), this.ultVecArray[i]);
						if (this.ultIndex == 0)
						{
							this.currentBeam.attack.knockbackOverwriteVector = this.targetVector;
						}
						else
						{
							this.currentBeam.attack.knockbackOverwriteVector = Vector2.Lerp(Globals.GetPerpendicular(this.ultVecArray[i], this.ultIndex < 0), this.targetVector, this.ultLerpValue);
						}
					}
				}
				this.currentBeam = null;
			}
			else
			{
				base.UpdateBeamVector();
			}
		}

		// Token: 0x0600303E RID: 12350 RVA: 0x0017C948 File Offset: 0x0017AD48
		public override void OnExit()
		{
			base.OnExit();
			if (this.currentBeam != null)
			{
				this.currentBeam.StopBeam();
			}
			for (int i = 0; i < Player.UseAquaBeam.ultBeamCount; i++)
			{
				if (this.ultBeamArray[i] != null)
				{
					this.ultBeamArray[i].StopBeam();
				}
			}
		}

		// Token: 0x040033EA RID: 13290
		public new static string staticID = "Mythical::UseRadiantBeam";

		// Token: 0x040033EB RID: 13291
		private Beam[] ultBeamArray = new Beam[5];

		// Token: 0x040033EC RID: 13292
		private Vector2[] ultVecArray = new Vector2[5];

		// Token: 0x040033ED RID: 13293
		private static int ultShotCount = 15;

		// Token: 0x040033EE RID: 13294
		private static int ultBeamCount = 5;

		// Token: 0x040033EF RID: 13295
		private static int ultHalfCount = 2;

		// Token: 0x040033F0 RID: 13296
		private int ultIndex;

		// Token: 0x040033F1 RID: 13297
		private ChaosQuickStopwatch ultStopwatch = new ChaosQuickStopwatch(4f);

		// Token: 0x040033F2 RID: 13298
		private float ultLerpValue;
	}
}
