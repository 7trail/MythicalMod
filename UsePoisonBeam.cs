using Chaos.AnimatorExtensions;
using System;
using System.Collections;
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
			this.beamRange = 6f;
			this.beamThickness = 2f;
            
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
		public new static string staticID = "Mythical::UsePoisonBeam";

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

    public class UseHolyBeam : Player.MeleeAttackState
    {
        // Token: 0x0600347A RID: 13434 RVA: 0x00181D38 File Offset: 0x00180138
        public UseHolyBeam(FSM parentFSM, Player parentEntity) : base(UseHolyBeam.staticID, parentFSM, parentEntity)
        {
            this.maxComboCount = 1;
            this.isBasic = true;
            this.animStartTime = 0f;
            this.animExecTime = 0f;
            this.baseCancelThreshold = 1f;
            this.finalCancelThreshold = 1f;
            this.baseRunThreshold = 1f;
            this.finalRunThreshold = 1f;
            this.baseExitThreshold = 1f;
            this.finalExitThreshold = 1f;
            this.SetAnimTimes(0.1f, 0.2f, 0.25f, 0.9f, 0.95f, 1f);
            this.attackMoveSpeedMod = new NumVarStatMod(this.atkMoveModStr, 0.25f, 10, VarStatModType.OverrideWithMods, false);
            this.finalAttackMoveSpeedMod = this.attackMoveSpeedMod;
        }

        public override void ExecuteAttack()
        {
            if (this.beamFired)
            {
                if (ChaosStopwatch.Check(this.delayStopwatchID))
                {
                    base.ExecuteSkill();
                    this.beamStopped = true;
                }
                else if (this.parent.anim.AnimPlayed(0.45f))
                {
                    this.parent.anim.PlayDirectional(this.parent.PBAoEAnimStr, -1, 0.3f);
                }
            }
            else
            {
                this.beamFired = true;
                this.delayStopwatchID = ChaosStopwatch.Begin(this.beamArcTime * 0.75f, false, 0f, 0, 0);
                this.CreateBeamArc(true);
            }
            
        }

        // Token: 0x17000750 RID: 1872
        // (get) Token: 0x0600347B RID: 13435 RVA: 0x00181D9E File Offset: 0x0018019E
        public override string OnEnterAnimStr
        {
            get
            {
                return (!this.isUltimate) ? this.parent.PBAoEAnimStr : this.parent.ForehandAnimStr;
            }
        }


        public static Sprite rBeam = null;

        // Token: 0x0600347C RID: 13436 RVA: 0x00181DC8 File Offset: 0x001801C8
        public override void OnEnter()
        {
            if (rBeam == null)
            {
                rBeam = ImgHandler.LoadSprite("rbeam");
            }
            base.OnEnter();
            
            this.beamFired = false;
            this.beamStopped = false;
            if (this.IsEmpowered)
            {
                base.SetSkillLevel(2);
                this.beamThickness = 2f;
                this.beamRange = 5f;
                this.beamArcTime = 0.3f;
            }
            else
            {
                base.SetSkillLevel(1);
                this.beamThickness = 1.5f;
                this.beamRange = 4f;
                this.beamArcTime = 0.3f;
            }
            CameraController.ShakeCamera(0.25f, false);
        }

        // Token: 0x0600347D RID: 13437 RVA: 0x00181EE8 File Offset: 0x001802E8
        

        // Token: 0x0600347E RID: 13438 RVA: 0x0018210D File Offset: 0x0018050D
        public override void OnExit()
        {
            base.OnExit();
            this.beamStopped = true;
        }

        // Token: 0x0600347F RID: 13439 RVA: 0x0018211C File Offset: 0x0018051C
        private FireBeam CreateFireBeam(Vector2 givenPosition, Vector2 givenVector)
        {
            this.currentFB = base.ChaosInst<FireBeam>(FireBeam.Prefab, new Vector2?(givenPosition + givenVector * 0.5f), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null);
            this.currentFB.attack.SetAttackInfo(this.parent.skillCategory, this.skillID, this.currentLevel, this.isUltimate);
            this.currentFB.attack.knockbackOverwriteVector = givenVector;
            this.currentFB.hitCount = 12;
            this.currentFB.hitInterval = 0.1f;
            this.currentFB.beamThickness = this.beamThickness;
            this.currentFB.maxRange = this.beamRange;
            foreach (SpriteRenderer sr in this.currentFB.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = Color.magenta;
            }
            //this.currentFB.anim.enabled = false;
            foreach(ParticleSystem ps in this.currentFB.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.MainModule main = ps.main;
                main.maxParticles = 0;
                ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;

                Gradient grad = new Gradient();

                grad.SetKeys(new GradientColorKey[]
                {
                new GradientColorKey()
                {
                    color = new Color(0.4f, 0f, 0.4f, 0.784f),
                    time = 0f
                },
                new GradientColorKey()
                {
                    color = new Color(0.3f, 0f, 0.3f, 0.584f),
                    time = 0.5f
                },
                new GradientColorKey()
                {
                    color = new Color(0.2f, 0f, 0.2f, 0.384f),
                    time = 1f
                }
                }, col.color.gradient.alphaKeys);
                col.color = grad;
            }

            return this.currentFB;
        }

        // Token: 0x06003480 RID: 13440 RVA: 0x001821E0 File Offset: 0x001805E0
        private void CreateBeamArc(bool givenStatus)
        {
            this.parent.StartCoroutine(this.FireBeamArc((this.inputVector).normalized, this.inputVector, false));
        }

        // Token: 0x06003481 RID: 13441 RVA: 0x0018224C File Offset: 0x0018064C
        public IEnumerator FireBeamArc(Vector2 givenStart, Vector2 givenEnd, bool usePower = false)
        {
            Vector2 tempPosition = this.parent.attackOriginTrans.position;
            FireBeam tempBeam = this.CreateFireBeam(tempPosition + givenStart, givenStart);
            int arcStopwatchID = ChaosStopwatch.Begin(this.beamArcTime, false, 0f, 0, 0);
            float lerpValue = 0f;
            while (!this.beamStopped && tempBeam != null && lerpValue < 1f)
            {
                tempPosition = this.parent.attackOriginTrans.position;
                lerpValue = ChaosStopwatch.GetTimePercentage(arcStopwatchID);
                if (usePower)
                {
                    lerpValue = Mathf.Pow(lerpValue, 3f);
                }
                Vector2 lerpVector = Vector2.Lerp(givenStart, givenEnd, lerpValue);
                tempBeam.transform.position = tempPosition + lerpVector;
                tempBeam.transform.localEulerAngles = Globals.GetRotationVector(lerpVector);

                Transform t = tempBeam.transform.Find("BeamStart/FireBurst");
                if (t)
                {
                    GameObject.Destroy(t.gameObject);
                }
                t = tempBeam.transform.Find("BeamStart/FireSecondaryBurst");
                if (t)
                {
                    GameObject.Destroy(t.gameObject);
                }

                tempBeam.fireEmitterLeftTrans.gameObject.SetActive(false);
                tempBeam.fireEmitterRightTrans.gameObject.SetActive(false);
                yield return null;
            }
            if (tempBeam != null)
            {
                tempBeam.StopBeam();
                tempBeam.fireEmitterLeftTrans.gameObject.SetActive(false);
                tempBeam.fireEmitterRightTrans.gameObject.SetActive(false);
            }
            yield break;
        }
        public new static string staticID = "Mythical::UseHolyBeam";
        private FireBeam currentFB;
        private float beamThickness = 0.75f;
        private float beamRange = 7.5f;
        private float beamArcTime = 1f;
        private bool beamFired;
        private bool beamStopped;
        private int delayStopwatchID;
    }

    public class UseRadiantDashBeam : Player.BaseDashState
    {
        // Token: 0x0600347A RID: 13434 RVA: 0x00181D38 File Offset: 0x00180138
        public UseRadiantDashBeam(FSM parentFSM, Player parentEntity) : base(UseRadiantDashBeam.staticID, parentFSM, parentEntity)
        {
            this.SetAnimTimes(0.1f, 0.2f, 0.25f, 0.55f, 0.75f, 1f);
            this.InitChargeSkillSettings(1, 0f, this.skillData, this);
        }


        // Token: 0x0600347C RID: 13436 RVA: 0x00181DC8 File Offset: 0x001801C8
        public override void OnEnter()
        {
            base.OnEnter();
            if (!this.cooldownReady)
            {
                return;
            }
            this.perpendVec = Globals.GetPerpendicular(this.inputVector, true);
            this.beamFired = false;
            this.beamStopped = false;
            this.animStartTime = 0.1f;
            if (this.IsEmpowered)
            {
                base.SetSkillLevel(2);
                this.beamThickness = 1.5f;
                this.beamRange = 4f;
                this.beamArcTime = 0.5f;
            }
            else
            {
                base.SetSkillLevel(1);
                this.beamThickness = 1f;
                this.beamRange = 4f;
                this.beamArcTime = 0.5f;
            }

            if (this.beamFired)
            {
                if (ChaosStopwatch.Check(this.delayStopwatchID))
                {
                    base.ExecuteSkill();
                    this.beamStopped = true;
                }
                else if (this.parent.anim.AnimPlayed(0.45f))
                {
                    this.parent.anim.PlayDirectional(this.parent.PBAoEAnimStr, -1, 0.3f);
                }
            }
            else
            {
                this.beamFired = true;
                this.delayStopwatchID = ChaosStopwatch.Begin(this.beamArcTime * 0.75f, false, 0f, 0, 0);
                this.CreateBeamArc(true);
                this.CreateBeamArc(false);
            }

        }

        // Token: 0x0600347E RID: 13438 RVA: 0x0018210D File Offset: 0x0018050D
        public override void OnExit()
        {
            base.OnExit();
            this.beamStopped = true;
        }

        // Token: 0x0600347F RID: 13439 RVA: 0x0018211C File Offset: 0x0018051C
        private FireBeam CreateFireBeam(Vector2 givenPosition, Vector2 givenVector)
        {
            this.currentFB = base.ChaosInst<FireBeam>(FireBeam.Prefab, new Vector2?(givenPosition + givenVector * 0.5f), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null);
            this.currentFB.attack.SetAttackInfo(this.parent.skillCategory, this.skillID, this.currentLevel, this.isUltimate);
            this.currentFB.attack.knockbackOverwriteVector = givenVector;
            this.currentFB.hitCount = 12;
            this.currentFB.hitInterval = 0.1f;
            this.currentFB.beamThickness = this.beamThickness;
            this.currentFB.maxRange = this.beamRange;
            Color c = Color.HSVToRGB(UnityEngine.Random.value, 1, 1);
            foreach (SpriteRenderer sr in this.currentFB.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = c;
            }
            //this.currentFB.anim.enabled = false;
            foreach (ParticleSystem ps in this.currentFB.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.MainModule main = ps.main;
                main.maxParticles = 0;
                ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;

                Gradient grad = new Gradient();

                grad.SetKeys(new GradientColorKey[]
                {
                new GradientColorKey()
                {
                    color = c,
                    time = 0f
                },
                new GradientColorKey()
                {
                    color = c,
                    time = 0.5f
                },
                new GradientColorKey()
                {
                    color = c,
                    time = 1f
                }
                }, col.color.gradient.alphaKeys);
                col.color = grad;
            }

            //Material m;
            return this.currentFB;
        }

        // Token: 0x06003480 RID: 13440 RVA: 0x001821E0 File Offset: 0x001805E0
        private void CreateBeamArc(bool givenStatus)
        {
            this.calcVec = ((!givenStatus) ? (-this.perpendVec) : this.perpendVec);
            this.parent.StartCoroutine(this.FireBeamArc((this.inputVector - this.calcVec * 0.5f).normalized, this.calcVec, false));
        }

        // Token: 0x06003481 RID: 13441 RVA: 0x0018224C File Offset: 0x0018064C
        private IEnumerator FireBeamArc(Vector2 givenStart, Vector2 givenEnd, bool usePower = false)
        {
            Vector2 tempPosition = this.parent.attackOriginTrans.position;
            FireBeam tempBeam = this.CreateFireBeam(tempPosition + givenStart, givenStart);
            int arcStopwatchID = ChaosStopwatch.Begin(this.beamArcTime, false, 0f, 0, 0);
            float lerpValue = 0f;
            while (!this.beamStopped && tempBeam != null && lerpValue < 1f)
            {
                tempPosition = this.parent.attackOriginTrans.position;
                lerpValue = ChaosStopwatch.GetTimePercentage(arcStopwatchID);
                if (usePower)
                {
                    lerpValue = Mathf.Pow(lerpValue, 3f);
                }
                Vector2 lerpVector = Vector2.Lerp(givenStart, givenEnd, lerpValue);
                tempBeam.transform.position = tempPosition + lerpVector;
                tempBeam.transform.localEulerAngles = Globals.GetRotationVector(lerpVector);

                Transform t = tempBeam.transform.Find("BeamStart/FireBurst");
                if (t)
                {
                    GameObject.Destroy(t.gameObject);
                }
                t = tempBeam.transform.Find("BeamStart/FireSecondaryBurst");
                if (t)
                {
                    GameObject.Destroy(t.gameObject);
                }

                tempBeam.fireEmitterLeftTrans.gameObject.SetActive(false);
                tempBeam.fireEmitterRightTrans.gameObject.SetActive(false);
                yield return null;
            }
            if (tempBeam != null)
            {
                tempBeam.StopBeam();
                tempBeam.fireEmitterLeftTrans.gameObject.SetActive(false);
                tempBeam.fireEmitterRightTrans.gameObject.SetActive(false);
            }
            yield break;
        }

        // Token: 0x0400374B RID: 14155
        public new static string staticID = "Mythical::UseRadiantDashBeam";

        // Token: 0x0400374C RID: 14156
        private FireBeam currentFB;

        // Token: 0x0400374D RID: 14157
        private Vector2 perpendVec;

        // Token: 0x0400374E RID: 14158
        private Vector2 calcVec;

        // Token: 0x0400374F RID: 14159
        private float beamThickness = 0.75f;

        // Token: 0x04003750 RID: 14160
        private float beamRange = 7.5f;

        // Token: 0x04003751 RID: 14161
        private float beamArcTime = 1f;

        // Token: 0x04003752 RID: 14162
        private bool beamFired;

        // Token: 0x04003753 RID: 14163
        private bool beamStopped;

        // Token: 0x04003754 RID: 14164
        private int delayStopwatchID;

        // Token: 0x04003755 RID: 14165
        private int ultStopwatchID;

        // Token: 0x04003756 RID: 14166
        private bool ultStarted;
    }

}
