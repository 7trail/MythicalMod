using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Chaos.AnimatorExtensions;
using System.Collections;
using Chaos.VectorExtensions;

namespace Mythical
{
	
	public class ContestantAirShieldState : Enemy.SkillState
	{
		// Token: 0x060016FB RID: 5883 RVA: 0x000C7910 File Offset: 0x000C5D10
		public ContestantAirShieldState(string newName, FSM newFSM, Contestant newEnt) : base(newName, newFSM, newEnt, 1, 1, 1, 1, 5, false)
		{
			this.parent = newEnt;
			this.requireLineOfSight = false;
			this.evadeMod = new NumVarStatMod(this.skillID, 1f, 10, VarStatModType.Override, false);
			c = newEnt;
		}
		Contestant c;


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
			c.tornadoReady = false;
			c.tornadoStopwatchID = ChaosStopwatch.Begin(c.tornadoCooldown, false, 0f, 0, 0);
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
	public class ContestantIonSpreadState : Enemy.SkillState
	{
		// Token: 0x06001694 RID: 5780 RVA: 0x000C4838 File Offset: 0x000C2C38
		public ContestantIonSpreadState(string newName, FSM newFSM, Contestant newEnt) : base(newName, newFSM, newEnt, 0, 0, 0f, 0f, 5f, false)
		{
			c = newEnt;
		}

		Contestant c;

		// Token: 0x06001695 RID: 5781 RVA: 0x000C486C File Offset: 0x000C2C6C
		public override void OnEnter()
		{
			base.OnEnter();
			this.parent.movement.EndMovement();
			this.parent.anim.Play("CastAlt");
			this.windupTime = 0.5f;
			this.windupStopwatchID = ChaosStopwatch.Begin(this.windupTime, false, 0f, 0, 0);
			this.spawnPosition = this.parent.hurtBoxTransform.position;
			this.targetVector = (this.parent.closestTargetInfo.hurtBoxPosition - this.parent.hurtBoxTransform.position).normalized;
			this.targetOffset = Globals.GetPerpendicular(this.targetVector, true) * 0.5f;
			this.ionArray[0] = this.CreateIonSpike(this.spawnPosition, this.targetVector);
			this.ionArray[1] = this.CreateIonSpike(this.spawnPosition + this.targetOffset, this.targetVector);
			this.ionArray[2] = this.CreateIonSpike(this.spawnPosition - this.targetOffset, this.targetVector);
			this.ionReleased = false;
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x000C4971 File Offset: 0x000C2D71
		public override void ExecuteSkill()
		{
			base.ExecuteSkill();
			this.DeactivatePowerUpEffect(false);
			this.parent.anim.Play("Cast");
			this.ionReleased = true;
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x000C499C File Offset: 0x000C2D9C
		public override void OnExit()
		{
			base.OnExit();
			if (!this.ionReleased)
			{
				for (int i = this.ionArray.Length - 1; i >= 0; i--)
				{
					if (this.ionArray[i] != null)
					{
						this.ionArray[i].AbortSpike();
					}
				}
			}
			c.fireballReady = false;
			c.fireballStopwatchID = ChaosStopwatch.Begin(c.fireballCooldown, false, 0f, 0, 0);
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x000C49F8 File Offset: 0x000C2DF8
		private IonSpike CreateIonSpike(Vector2 givenPosition, Vector2 givenVector)
		{
			this.currentIS = this.ChaosInst<IonSpike>(IonSpike.Prefab, new Vector2?(givenPosition + givenVector * 0.75f), null, null);
			this.currentIS.SetSkillInfo("Enemy", "ThunderMageIonSpread", false);
			this.currentIS.attackVector = givenVector;
			this.currentIS.delayStopwatchID = ChaosStopwatch.Begin(this.windupTime, false, 0f, 0, 0);
			this.currentIS.maxRange = this.parent.attackRadius + 5f;
			this.currentIS.intervalCount = 5;
			return this.currentIS;
		}

		// Token: 0x04001CFD RID: 7421
		private Vector2 spawnPosition;

		// Token: 0x04001CFE RID: 7422
		private Vector2 targetOffset;

		// Token: 0x04001CFF RID: 7423
		private IonSpike[] ionArray = new IonSpike[3];

		// Token: 0x04001D00 RID: 7424
		private IonSpike currentIS;

		// Token: 0x04001D01 RID: 7425
		private bool ionReleased;
	}

	public class ContestantEarthSummonerAttackState : Enemy.SkillState
	{
		// Token: 0x060011F1 RID: 4593 RVA: 0x000A605C File Offset: 0x000A445C
		public ContestantEarthSummonerAttackState(string newName, FSM newFSM, Enemy newEnt) : base(newName, newFSM, newEnt, 0, 0, 0f, 0f, 5f, false)
		{
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x000A6096 File Offset: 0x000A4496
		public override void OnEnter()
		{
			base.OnEnter();
			this.parent.anim.Play("Charging");
			this.windupStopwatchID = ChaosStopwatch.Begin(this.StdWindupTime, false, 0f, 0, 0);
			this.atkStarted = false;
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x000A60D4 File Offset: 0x000A44D4
		public override void ExecuteSkill()
		{
			if (!this.atkStarted)
			{
				this.atkStarted = true;
				this.atkStopwatchID = ChaosStopwatch.Begin(0f, true, this.atkInterval, this.atkCount, 0);
				this.parent.anim.Play("Cast", -1, 0f);
			}
			StopwatchState stopwatchState = ChaosStopwatch.CheckInterval(this.atkStopwatchID, true);
			if (stopwatchState != StopwatchState.Ready)
			{
				if (stopwatchState == StopwatchState.Done)
				{
					base.ExecuteSkill();
				}
			}
			else
			{
				this.CreateEarthShot(this.parent.transform.position, this.targetVector);
				CameraController.ShakeCamera(0.15f, false);
			}
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x000A6188 File Offset: 0x000A4588
		private void CreateEarthShot(Vector2 givenPosition, Vector2 givenVector)
		{
			this.spawnPosition = givenPosition + UnityEngine.Random.insideUnitCircle.normalized * 0.75f;
			this.spawnPosition = Globals.GetLinecastVector(givenPosition, this.spawnPosition, ChaosCollisions.layerAll);
			this.currentES = (EarthShot)Projectile.CreateProjectile(this.parent, EarthShot.Prefab, new Vector3?(this.spawnPosition), null, null);
			this.currentES.attackBox.SetAttackInfo("Enemy", "EarthSummonerAttack", 1, false);
			this.currentES.targetVector = givenVector;
			this.currentES.moveSpeed = 24f;
			this.currentES.spawnOffset = Vector2.up;
			this.currentES.riseTime = 0.25f;
			this.currentES.delayTime = 0.4f;
			SoundManager.PlayWithDistAndSPR("EarthPebble", this.spawnPosition, 1f);
			PoolManager.GetPoolItem<RockBurstDirectionalEffect>().Emit(new int?(2), new Vector3?(this.spawnPosition - Vector2.up), null, null, 0f, null, null);
			PoolManager.GetPoolItem<FloorCrackEffect>().Crack(this.spawnPosition, 0f, 0.5f, 0.5f, FloorCrackEffectType.Small);
		}

		// Token: 0x04001826 RID: 6182
		private EarthShot currentES;

		// Token: 0x04001827 RID: 6183
		private Vector2 spawnPosition;

		// Token: 0x04001828 RID: 6184
		private int atkStopwatchID;

		// Token: 0x04001829 RID: 6185
		private int atkCount = 5;

		// Token: 0x0400182A RID: 6186
		private float atkInterval = 0.1f;

		// Token: 0x0400182B RID: 6187
		private bool atkStarted;
	}

	public class ContestantFlameMageFirePillarState : Enemy.SkillState
	{
		// Token: 0x06001371 RID: 4977 RVA: 0x000AC4E8 File Offset: 0x000AA8E8
		public ContestantFlameMageFirePillarState(string newName, FSM newFSM, Contestant newEnt, int newReq, int newPriority, float newMin, float newMax, float newCD) : base(newName, newFSM, newEnt, newReq, newPriority, newMin, newMax, newCD, true)
		{
			this.castingAnimName = "Raise";
			c = newEnt;
		}
		Contestant c;
		// Token: 0x06001372 RID: 4978 RVA: 0x000AC514 File Offset: 0x000AA914
		public override void OnEnter()
		{
			base.OnEnter();
			this.parent.movement.EndMovement();
			this.parent.anim.Play(this.castingAnimName, -1, 0f);
			this.windupTime = this.StdWindupTime * 2f;
			this.windupStopwatchID = ChaosStopwatch.Begin(0.25f, false, 0f, 0, 0);
			this.exitTime = this.StdExitTime * 2f;
			for (int i = 0; i < 4; i++)
			{
				this.CreateFirePillar((i+1)*2);
			}
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x000AC596 File Offset: 0x000AA996
		public override void WindupUpdate()
		{
			this.parent.anim.Play(this.castingAnimName, -1, 0f);
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x000AC5B4 File Offset: 0x000AA9B4
		public override void ExitTransition()
		{
			this.fsm.ChangeState("GoBack", false);
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x000AC5C7 File Offset: 0x000AA9C7
		public override void ExecuteSkill()
		{
			base.ExecuteSkill();
			this.DeactivatePowerUpEffect(false);
			this.parent.anim.Play(this.castingAnimName, -1, 1f);
		}

		// Token: 0x06001376 RID: 4982 RVA: 0x000AC5F4 File Offset: 0x000AA9F4
		public override void OnExit()
		{
			base.OnExit();
			if (this.currentFP != null && (this.fsm.nextStateName == "Hurt" || this.fsm.nextStateName == "Dead"))
			{
				this.currentFP.Reset();
			}
			c.fireballReady = false;
			c.fireballStopwatchID = ChaosStopwatch.Begin(c.fireballCooldown, false, 0f, 0, 0);
		}

		// Token: 0x06001377 RID: 4983 RVA: 0x000AC658 File Offset: 0x000AAA58
		private void CreateFirePillar(float radius)
		{
			this.currentFP = this.ChaosInst<FirePillar>(FirePillar.Prefab, new Vector2?(this.parent.closestTargetInfo.position + Vector3.zero.Randomize(1f, 0.5f, 1f)*radius), null, null);
			this.currentFP.attack.SetAttackInfo(this.parent.skillCategory, this.skillID, 1, false);
			this.currentFP.delay = this.windupTime;
			this.currentFP.duration = 2.5f;
		}

		// Token: 0x04001A40 RID: 6720
		private FirePillar currentFP;
	}

	public class ContestantAquaBeamState : Enemy.SkillState
	{
		// Token: 0x06000C58 RID: 3160 RVA: 0x0006ED0C File Offset: 0x0006D10C
		public ContestantAquaBeamState(string newName, FSM newFSM, Enemy newEnt) : base(newName, newFSM, newEnt,1,1,1,1,3,false)
		{
			this.waterFormEffects = new ParticleEffect[2];
			this.waterBounceProjs = new WaterBounceProjectile[2];
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x0006ED70 File Offset: 0x0006D170
		public override void OnEnter()
		{
			base.OnEnter();
			this.PlayAnim(0f);
			this.windupTime = this.StdWindupTime * 1.25f;
			this.windupStopwatchID = ChaosStopwatch.Begin(0.25f, false, 0f, 0, 0);
			this.beamsCreated = false;
			this.lerpStarted = false;
			this.lerpComplete = false;
			this.formEffectStarted = false;
			this.lerpValue = 0f;
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x0006EDE0 File Offset: 0x0006D1E0
		public override void WindupUpdate()
		{
			if (!this.formEffectFrameSkipped)
			{
				this.formEffectFrameSkipped = true;
			}
			if (!this.formEffectStarted)
			{
				this.formEffectStarted = true;
			}
			this.PlayAnim(0f);
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x0006EE18 File Offset: 0x0006D218
		public void StopWaterFormEffect()
		{
			for (int i = 0; i < 2; i++)
			{
				if (this.waterBounceProjs[i] != null)
				{
					UnityEngine.Object.Destroy(this.waterBounceProjs[i].gameObject);
				}
				if (this.waterFormEffects[i] != null)
				{
					this.waterFormEffects[i].Stop();
				}
			}
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x0006F024 File Offset: 0x0006D424
		public override void ExecuteSkill()
		{
			if (!this.beamsCreated)
			{
				this.targetVector = (this.parent.closestTargetInfo.hurtBoxPosition - this.parent.hurtBoxTransform.position).normalized;
				this.beamsCreated = true;
				this.delayStopwatch.IsRunning = true;
				this.atkTransPos = this.parent.transform.position;
				this.spawnPos = this.atkTransPos + this.spawnRange * this.targetVector;
				this.leftBeam = this.CreateAquaBeam(this.spawnPos, this.targetVector);
				//this.lerpVector = Globals.GetPerpendicular(this.targetVector, true);
				this.leftEndVector =this.targetVector.normalized;
				CameraController.ShakeCamera(1f, false);
				this.StopWaterFormEffect();
				this.PlayAnim(0.5f);
				return;
			}
			if (this.lerpComplete)
			{
				base.ExecuteSkill();
			}
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x0006F188 File Offset: 0x0006D588


		// Token: 0x06000C5F RID: 3167 RVA: 0x0006F328 File Offset: 0x0006D728
		public override void FixedUpdate()
		{
			if (!this.beamsCreated || this.delayStopwatch.IsRunning || this.lerpComplete)
			{
				return;
			}
			if (!this.lerpStarted)
			{
				this.lerpStarted = true;
				this.lerpStopwatch.IsRunning = true;
			}
			this.lerpComplete = (this.lerpValue >= 1f);
			this.lerpValue = this.lerpStopwatch.TimePercentage;
			if (this.lerpComplete)
			{
				this.leftBeam.StopBeam();
				return;
			}
			
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x0006F4B7 File Offset: 0x0006D8B7
		public override void OnExit()
		{
			base.OnExit();
			this.StopWaterFormEffect();
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x0006F4C5 File Offset: 0x0006D8C5
		public virtual void PlayAnim(float givenTime = 0f)
		{
			this.parent.anim.PlayDirectional("TwoHandSplitCast" + this.parent.facingDirection.ToString(), -1, givenTime);
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x0006F4FC File Offset: 0x0006D8FC
		private AquaBeam CreateAquaBeam(Vector2 givenPosition, Vector2 givenVector)
		{
			AquaBeam aquaBeam = this.ChaosInst<AquaBeam>(AquaBeam.Prefab, new Vector2?(givenPosition), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null);
			aquaBeam.attack.SetAttackInfo("Enemy", "IceBossAquaBeam", 1, false);
			aquaBeam.hitCount = 40;
			aquaBeam.maxRange = 32f;
			aquaBeam.beamThickness = 2f;
			return aquaBeam;
		}

		// Token: 0x04001130 RID: 4400
		private AquaBeam leftBeam;


		// Token: 0x04001132 RID: 4402
		private Vector2 atkTransPos;

		// Token: 0x04001133 RID: 4403
		private Vector2 spawnPos;

		// Token: 0x04001134 RID: 4404
		private Vector2 leftEndVector;

		// Token: 0x04001135 RID: 4405
		private Vector2 rightEndVector;

		// Token: 0x04001136 RID: 4406
		private Vector2 lerpVector;

		// Token: 0x04001137 RID: 4407
		private ChaosQuickStopwatch delayStopwatch = new ChaosQuickStopwatch(0.5f);

		// Token: 0x04001138 RID: 4408
		private ChaosQuickStopwatch lerpStopwatch = new ChaosQuickStopwatch(0.5f);

		// Token: 0x04001139 RID: 4409
		protected float lerpValue;

		// Token: 0x0400113A RID: 4410
		private bool lerpStarted;

		// Token: 0x0400113B RID: 4411
		private bool lerpComplete;

		// Token: 0x0400113C RID: 4412
		private bool beamsCreated;

		// Token: 0x0400113D RID: 4413
		private float overShootValue = 0.2f;

		// Token: 0x0400113E RID: 4414
		private float spawnRange = 1.375f;

		// Token: 0x0400113F RID: 4415
		private ParticleEffect[] waterFormEffects;

		// Token: 0x04001140 RID: 4416
		private bool formEffectFrameSkipped;

		// Token: 0x04001141 RID: 4417
		private bool formEffectStarted;

		// Token: 0x04001142 RID: 4418
		private WaterBounceProjectile[] waterBounceProjs;
	}

	public class ContestantDrillAttackState : Enemy.SkillState
	{
		// Token: 0x06000AC0 RID: 2752 RVA: 0x00069B16 File Offset: 0x00067F16
		public ContestantDrillAttackState(string newName, FSM newFSM, Enemy newEnt) : base(newName, newFSM, newEnt,1,1,1,1,3,false)
		{
			this.parent = newEnt;
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x00069B28 File Offset: 0x00067F28
		private void PlayAnim(float givenTime = 0f)
		{
			this.parent.anim.PlayDirectional(this.punchAnimStr, -1, givenTime);
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x00069B44 File Offset: 0x00067F44
		public override void OnEnter()
		{
			base.OnEnter();
			this.punchAnimStr = "Punch" + this.parent.facingDirection;
			this.PlayAnim(0f);
			this.windupStopwatchID = ChaosStopwatch.Begin(Mathf.Lerp(0.75f, 0.4f, this.currentDiffValue), false, 0f, 0, 0);
			this.exitTime = this.StdExitTime;
			this.parent.movement.dashVector = this.targetVector;
			this.CreateDrill();
			this.dustEmitAngle = Globals.GetRotationVector(this.targetVector).z + 180f;
			this.chargeUpComplete = false;
			this.dashInitiated = false;
			this.isDashing = false;
			this.targetHit = false;
			this.drillStopped = false;
			this.finalHit = false;
			this.dustEmitterTimerID = ChaosStopwatch.Begin(0.1f, true, 0.1f, 100, 0);
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x00069C3C File Offset: 0x0006803C
		public override void Update()
		{
			StopwatchState stopwatchState = ChaosStopwatch.CheckInterval(this.dustEmitterTimerID, true);
			if (stopwatchState == StopwatchState.Ready)
			{
				float targetAngle = this.dustEmitAngle;
			}
			if (!this.chargeUpComplete)
			{
				this.PlayAnim(0f);
				if (ChaosStopwatch.Check(this.windupStopwatchID))
				{
					this.chargeUpComplete = true;
					this.PlayAnim(1f);
				}
				return;
			}
			if (!this.dashInitiated)
			{
				this.dashInitiated = true;
				this.isDashing = true;
				this.parent.ToggleFloorContact(false);
				this.currentED.Activate();
				this.PlayAnim(0.33f);
				return;
			}
			if (this.isDashing && !this.targetHit)
			{
				this.PlayAnim(0.33f);
				return;
			}
			if (this.targetHit && !this.drillStopped)
			{
				ChaosStopwatch.Stop(this.dustEmitterTimerID);
				if (this.finalHit)
				{
					if (this.parent.anim.AnimPlayed(0.35f))
					{
						this.currentED.attack.ChangeAttackInfoLevel(2);
						this.currentED.Activate();
						this.drillStopped = true;
						this.exitStopwatchID = ChaosStopwatch.Begin(this.exitTime, false, 0f, 0, 0);
					}
					return;
				}
				if (!this.currentED.hitIntervalRunning && !this.finalHit)
				{
					this.finalHit = true;
					this.PlayAnim(0f);
				}
				return;
			}
			else
			{
				if (ChaosStopwatch.Check(this.exitStopwatchID))
				{
					this.ExitTransition();
					return;
				}
				return;
			}
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x00069E1C File Offset: 0x0006821C
		public override void FixedUpdate()
		{
			if (this.isDashing && !this.targetHit)
			{
				this.isDashing = !this.parent.movement.DashToTarget(null, false, false, true);
				if (!this.isDashing)
				{
					ChaosStopwatch.Stop(this.dustEmitterTimerID);
					this.drillStopped = true;
					this.DashStopped();
				}
			}
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x00069E87 File Offset: 0x00068287
		public override void OnExit()
		{
			base.OnExit();
			this.parent.ToggleFloorContact(true);
			if (this.currentED != null)
			{
				this.currentED.Reset();
			}
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x00069EC8 File Offset: 0x000682C8
		private void CreateDrill()
		{
			this.currentED = this.ChaosInst<EarthDrill>(EarthDrill.Prefab, null, null, null);
			this.currentED.followTransform = this.parent.rightHandTrans;
			this.currentED.attack.SetAttackInfo("Enemy", "EarthBossDrillAttackState", 1, false);
			this.currentED.attack.knockbackOverwriteVector = this.targetVector;
			Attack attack = this.currentED.attack;
			attack.entityCollisionEventHandlers = (Attack.EntityCollisionEventHandler)Delegate.Remove(attack.entityCollisionEventHandlers, new Attack.EntityCollisionEventHandler(this.OnTargetHit));
			Attack attack2 = this.currentED.attack;
			attack2.entityCollisionEventHandlers = (Attack.EntityCollisionEventHandler)Delegate.Combine(attack2.entityCollisionEventHandlers, new Attack.EntityCollisionEventHandler(this.OnTargetHit));
			float yValueOffset = -2.25f;
			FacingDirection facingDirection = this.parent.facingDirection;
			if (facingDirection != FacingDirection.Up)
			{
				if (facingDirection == FacingDirection.Down)
				{
					yValueOffset = -2.75f;
				}
			}
			else
			{
				yValueOffset = -3f;
			}
			this.currentED.transform.localEulerAngles = Globals.GetRotationVector((this.targetVector + Entity.GetFacingDirectionVector(this.parent.facingDirection)).normalized);
			this.currentED.spriteSorter.yValueOffset = yValueOffset;
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0006A02C File Offset: 0x0006842C
		private void DashStopped()
		{
			this.parent.movement.EndMovement();
			this.parent.ToggleFloorContact(true);
			CameraController.ShakeCamera(1f, false);
			this.exitStopwatchID = ChaosStopwatch.Begin(this.exitTime, false, 0f, 0, 0);
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0006A088 File Offset: 0x00068488
		private void OnTargetHit(Entity givenEntity)
		{
			if (this.targetHit || givenEntity == null || !(givenEntity is Player))
			{
				return;
			}
			this.isDashing = false;
			this.targetHit = true;
			this.DashStopped();
			this.currentED.StartInterval();
		}

		// Token: 0x04000F2B RID: 3883
		private EarthDrill currentED;

		// Token: 0x04000F2C RID: 3884
		private int dustEmitterTimerID;

		// Token: 0x04000F2D RID: 3885
		private bool chargeUpComplete;

		// Token: 0x04000F2E RID: 3886
		private bool dashInitiated;

		// Token: 0x04000F2F RID: 3887
		private bool isDashing;

		// Token: 0x04000F30 RID: 3888
		private bool targetHit;

		// Token: 0x04000F31 RID: 3889
		private bool drillStopped;

		// Token: 0x04000F32 RID: 3890
		private bool finalHit;

		// Token: 0x04000F33 RID: 3891
		private float dustEmitAngle;

		// Token: 0x04000F34 RID: 3892
		private string punchAnimStr;

	}

}
