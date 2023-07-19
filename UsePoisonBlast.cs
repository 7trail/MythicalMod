using Chaos.AnimatorExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
	public class UsePoisonBlastState : Player.SkillState
	{
		// Token: 0x060031D0 RID: 12752 RVA: 0x00189078 File Offset: 0x00187478
		public UsePoisonBlastState(FSM parentFSM, Player parentEntity) : base(UsePoisonBlastState.staticID, parentFSM, parentEntity)
		{
			this.hasSignatureVariant = true;
			this.isMeleeSkill = true;
			//this.startCooldownOnEnter = false;
			this.useSmartCD = true;
			//base.InitChargeSkillSettings(1, 0f, null, this);
			this.SetAnimTimes(0.1f, 0.6f, 0.2f, 0.8f, 0.9f, 1f);
		}
        
        public override string OnEnterAnimStr
		{
			get
			{
				return this.parent.PBAoEAnimStr;
			}
		}

		public override void ExecuteSkill()
		{
			base.ExecuteSkill();

			//this.StartCooldownTimer(-1f, true);

			GameObject prefab = FireBlast.Prefab;
			Quaternion? givenQuaternion = new Quaternion?(Globals.GetRotationQuaternion(this.inputVector));
			this.currentFB = base.ChaosInst<FireBlast>(prefab, null, givenQuaternion, null);
			foreach(ParticleSystem system in currentFB.GetComponentsInChildren<ParticleSystem>())
            {
				ParticleSystem.MainModule main = system.main;
				main.startColor = Color.green;
				ParticleSystem.ColorOverLifetimeModule coL = system.colorOverLifetime;
				coL.color = Color.green;
            }
			this.currentFB.attack.SetAttackInfo(this.parent.skillCategory, this.skillID, (!this.isUltimate) ? ((!this.IsEmpowered) ? 1 : 2) : 3, this.isUltimate);
			this.currentFB.attack.knockbackOverwriteVector = this.inputVector;
			if (this.isUltimate)
			{
				this.currentFB.CircleBlast();
				this.ultStopwatchID = ChaosStopwatch.Begin(1.05f, false, 0f, 0, 0);
				this.parent.hud.castBar.StartCast(this.ultStopwatchID, false);
				base.ApplyStopElement();
				this.PlayAudio();
			}
			else
			{
				this.currentFB.Blast();
				
			}
			CameraController.ShakeCamera(0.25f, false);
		}

		public override void ConcludeSkill()
		{
			if (!this.isUltimate || ChaosStopwatch.Check(this.ultStopwatchID))
			{
				this.StopAudio();
				this.ExitToSkillOrRunOrIdle();
				return;
			}
			if (base.CancelToDash(false) || this.currentFB == null)
			{
				return;
			}
			this.PlayAudio();
			this.currentFB.transform.position = this.parent.attackOriginTrans.position;
			this.inputVector = base.GetInputVector(true, true, false);
			this.parent.anim.PlayDirectional(this.parent.PBAoEAnimStr, -1, this.animHoldTime);
			if (this.inputVector == Vector2.zero)
			{
				return;
			}
			this.currentFB.attack.knockbackOverwriteVector = this.inputVector;
			this.currentFB.transform.localEulerAngles = Globals.GetRotationVector(this.inputVector);
		}

		public override void OnExit()
		{
			base.OnExit();
			this.parent.hud.castBar.EndCast();
			if (this.currentFB != null)
			{
				this.currentFB.EndAllAttacks();
			}
			this.StopAudio();
		}

		// Token: 0x060031D5 RID: 12757 RVA: 0x00189342 File Offset: 0x00187742
		private void PlayAudio()
		{
			this.odLoopAudio = SoundManager.PlayIfNotPlaying("FireBlastODLoop", this.odLoopAudio, this.parent.transform, true, -1f, -1f);
		}

		// Token: 0x060031D6 RID: 12758 RVA: 0x00189370 File Offset: 0x00187770
		private void StopAudio()
		{
			if (this.odLoopAudio != null)
			{
				this.odLoopAudio.Stop();
				this.odLoopAudio = null;
				SoundManager.PlayAudioWithDistance("FireBlastODEnd", new Vector2?(this.parent.transform.position), null, 24f, -1f, 1f, false);
			}
		}

		// Token: 0x0400357A RID: 13690
		public new static string staticID = "Mythical::UseRadiantBlast";

		// Token: 0x0400357B RID: 13691
		private FireBlast currentFB;

		// Token: 0x0400357C RID: 13692
		private int ultStopwatchID;

		// Token: 0x0400357D RID: 13693
		private AudioSource odLoopAudio;
	}
}
