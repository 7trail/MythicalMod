using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
	public class RadiantDashState : Player.BaseDashState
	{
		// Token: 0x060030FB RID: 12539 RVA: 0x00166518 File Offset: 0x00164918
		public RadiantDashState(FSM fsm, Player parentPlayer) : base(RadiantDashState.staticID, fsm, parentPlayer)
		{
			this.showEffects = false;
			this.playSFX = false;
			//this.empoweredStat.SetInitialBaseValue(true);
		}

		// Token: 0x060030FC RID: 12540 RVA: 0x00166551 File Offset: 0x00164951
		public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod)
		{
		}

		// Token: 0x060030FD RID: 12541 RVA: 0x00166554 File Offset: 0x00164954
		public override void OnEnter()
		{
			base.OnEnter();
			this.startPosition = this.parent.transform.position;
			this.endPosition = this.startPosition + this.inputVector * RadiantDashState.teleportRange * (IsEmpowered ? 1.5f:1) *(UnityEngine.Random.value<0.07f?0.1f:1f);
			if (!Globals.CheckCircle(this.endPosition, 0.25f, ChaosCollisions.layerAllWallAndObst) || Pathfinder.GetPath(Pathfinder.connectedNodeMap, this.startPosition, this.endPosition, ChaosCollisions.layerAllWallAndObst) == null)
			{
				this.endPosition = Globals.GetLinecastVector(this.startPosition, this.endPosition, ChaosCollisions.layerAllWallAndObst) - this.inputVector * 0.3f;
				if (Vector2.SqrMagnitude(this.endPosition - this.startPosition) < 1f)
				{
					this.endPosition = this.startPosition;
				}
			}
			this.SpawnTeleportEffect(true);
			string audioID = "ChaosBeamStart";
			Vector2? soundOrigin = new Vector2?(this.startPosition);
			float standardPitchRange = SoundManager.StandardPitchRange;
			SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, 0.2f, standardPitchRange, false);
			this.teleportStopwatch.IsRunning = true;
			this.ToggleColliders(false);
		}

		// Token: 0x060030FE RID: 12542 RVA: 0x00166690 File Offset: 0x00164A90
		public override void FixedUpdate()
		{
			if (!this.finishedDashing)
			{
				this.teleLerpValue = this.teleportStopwatch.TimePercentage;
				if (this.teleLerpValue >= 1f)
				{
					this.finishedDashing = true;
					this.parent.rigidbody2D.MovePosition(this.endPosition);
				}
				else
				{
					this.parent.rigidbody2D.MovePosition(Vector2.Lerp(this.startPosition, this.endPosition, this.teleLerpValue));
				}
			}
		}

		// Token: 0x060030FF RID: 12543 RVA: 0x00166714 File Offset: 0x00164B14
		private void SpawnTeleportEffect(bool givenStart = true)
		{
			this.effectPosition = this.parent.attackOriginTrans.position;
			if (givenStart)
			{
				AnimEffect poolItem = PoolManager.GetPoolItem<AnimEffect>("ChaosPortal");
				Vector3 position = this.effectPosition;
				string animName = "Depart";
				Vector3? localEulerAngles = new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
				poolItem.Play(position, animName, 0f, default(Vector2), 1f, 1f, localEulerAngles);
			}
			else
			{
				PoolManager.GetPoolItem<AnimEffect>("ChaosPortal").Play(this.effectPosition, "Spawn", 0f, default(Vector2), 1f, 1f, null);
				ChaosBurst.Spawn(this.effectPosition, 3);
			}
			SoundManager.PlayAudioWithDistance("Teleport", new Vector2?(this.effectPosition), null, 24f, 0.075f, 1f, false);
		}

		// Token: 0x06003100 RID: 12544 RVA: 0x0016681C File Offset: 0x00164C1C
		public override void DashFinished()
		{
			base.DashFinished();
			this.SpawnTeleportEffect(false);
			string audioID = "EnemyDead";
			Vector2? soundOrigin = new Vector2?(this.parent.transform.position);
			float overridePitch = UnityEngine.Random.Range(1.1f, 1.2f);
			SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, 0.35f, overridePitch, false);
			this.ToggleColliders(true);
		}

		// Token: 0x06003101 RID: 12545 RVA: 0x00166884 File Offset: 0x00164C84
		private void ToggleColliders(bool status)
		{
			this.parent.hurtBoxObj.SetActive(status);
			this.parent.floorContact.SetActive(status);
			this.parent.fall.disableFallTransition = !status;
			if (status)
			{
				this.parent.Show();
			}
			else
			{
				this.parent.Hide();
			}
		}

		// Token: 0x040033DC RID: 13276
		public new static string staticID = "RadiantDash";

		// Token: 0x040033DD RID: 13277
		private static float teleportRange = 9f;

		// Token: 0x040033DE RID: 13278
		private ChaosQuickStopwatch teleportStopwatch = new ChaosQuickStopwatch(0.175f);

		// Token: 0x040033DF RID: 13279
		private float teleLerpValue;

		// Token: 0x040033E0 RID: 13280
		private Vector2 startPosition;

		// Token: 0x040033E1 RID: 13281
		private Vector2 endPosition;

		// Token: 0x040033E2 RID: 13282
		private Vector2 effectPosition;
	}
}
