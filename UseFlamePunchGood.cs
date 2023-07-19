using Chaos.AnimatorExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    public class UseFlamePunchGood : Player.BaseJumpState
    {
        // Token: 0x060034AF RID: 13487 RVA: 0x00183CD8 File Offset: 0x001820D8
        public UseFlamePunchGood(FSM parentFSM, Player parentEntity) : base(UseFlamePunchGood.staticID, parentFSM, parentEntity)
        {
            this.isMeleeSkill = true;
            this.startCooldownOnEnter = false;
            this.useSmartCD = true;
            this.SetAnimTimes(0.25f, 0.2f, 0.35f, 0.55f, 0.75f, 1f);
            this.dashSpdMod = new NumVarStatMod(UseFlamePunchGood.staticID, 16f, 10, VarStatModType.OverrideWithMods, false);
            this.dashDurMod = new NumVarStatMod(UseFlamePunchGood.staticID, 0.4f, 10, VarStatModType.OverrideWithMods, false);
            this.jumpTime = 0.3f;
            this.jumpRange = 2.5f;
            this.jumpHeight = 1.5f;
            this.jumpBalance = 0.7f;
            this.useCalcValues = false;
        }

        // Token: 0x17000754 RID: 1876
        // (get) Token: 0x060034B0 RID: 13488 RVA: 0x00183D90 File Offset: 0x00182190
        public override string OnEnterAnimStr
        {
            get
            {
                return this.parent.KickAnimStr;
            }
        }

        // Token: 0x060034B1 RID: 13489 RVA: 0x00183DA0 File Offset: 0x001821A0
        public override void OnEnter()
        {
            base.BaseOnEnter();
            this.dashSpdMod.modValue = 16f;
            this.holdStarted = false;
            this.holdReleased = false;
            this.skillExecuted = false;
            this.spinAnimPlayed = false;
            this.jumpStarted = false;
            this.movementStopped = false;
            this.hitCounter = 0;
            this.maxHitCount = 3;
            this.hitDelay = 0.15f;
            if (this.IsEmpowered)
            {
                this.baseScale = 1.75f;
                this.finalScale = 3f;
            }
            else
            {
                this.baseScale = 1f;
                this.finalScale = 2.25f;
            }
            this.inputVector = this.parent.GetInputVector(true, true, true);
            this.currentQuat = Globals.GetRotationQuaternion(this.inputVector);
            this.parent.movement.moveVector = this.inputVector;
            this.parent.movement.dashVector = this.inputVector;
            this.parent.movement.dashSpeedStat.AddMod(this.dashSpdMod);
            this.parent.movement.dashDurationStat.AddMod(this.dashDurMod);
            this.parent.movement.dashTimer = this.dashDurMod.modValue;
            this.parent.ToggleEnemyFloorCollisions(false);
            this.parent.anim.PlayDirectional(this.OnEnterAnimStr, -1, this.animStartTime);
        }

        // Token: 0x060034B2 RID: 13490 RVA: 0x00183F10 File Offset: 0x00182310
        public override void Update()
        {
            if (this.jumpStarted)
            {
                if (!this.spinAnimPlayed && this.parentThrow.PercentComplete > 0.4f)
                {
                    this.parent.anim.Play("SpinReverse", -1, 0f);
                    this.spinAnimPlayed = true;
                }
                base.Update();
            }
            else if (this.skillExecuted)
            {
                this.ConcludeSkill();
            }
            else if (this.parent.anim.AnimPlayed(this.animExecTime))
            {
                this.ExecuteSkill();
            }
        }

        // Token: 0x060034B3 RID: 13491 RVA: 0x00183FAC File Offset: 0x001823AC
        public override void FixedUpdate()
        {
            if (!this.movementStopped)
            {
                this.movementStopped = this.parent.movement.DashToTarget(null, false, false, true);
            }
        }

        // Token: 0x060034B4 RID: 13492 RVA: 0x00183FE8 File Offset: 0x001823E8
        public override void ExecuteSkill()
        {
            if (!ChaosStopwatch.Check(this.hitStopwatchID))
            {
                return;
            }
            this.StartCooldownTimer(-1f, true);
            this.hitStopwatchID = ChaosStopwatch.Begin(this.hitDelay, false, 0f, 0, 0);
            if (this.hitCounter >= 1)
            {
                this.hitDelay = 0.125f;
            }
            this.PlayAnim();
            if (this.hitCounter == this.maxHitCount - 1)
            {
                this.CreateFlamePunch((!this.IsEmpowered) ? 2 : 3);
            }
            else
            {
                this.CreateFlamePunch(1);
            }
            CameraController.ShakeCamera(0.25f, false);
            this.hitCounter++;
            if (this.hitCounter >= this.maxHitCount)
            {
                base.ExecuteSkill();
                this.InitJump();
            }
        }

        // Token: 0x060034B5 RID: 13493 RVA: 0x001840B8 File Offset: 0x001824B8
        public override void OnExit()
        {
            base.OnExit();
            this.parent.movement.dashSpeedStat.RemoveMod(UseFlamePunchGood.staticID);
            this.parent.movement.dashDurationStat.RemoveMod(UseFlamePunchGood.staticID);
            this.parent.ToggleEnemyFloorCollisions(true);
            this.CleanUpEventHandler(true);
        }

        // Token: 0x060034B6 RID: 13494 RVA: 0x00184114 File Offset: 0x00182514
        private void PlayAnim()
        {
            if (this.hitCounter == 0)
            {
                return;
            }
            if (this.hitCounter % 2 == 0)
            {
                this.parent.anim.PlayDirectional(this.parent.KickAnimStr, -1, 0.25f);
            }
            else
            {
                this.parent.anim.PlayDirectional(this.parent.ForehandAnimStr, -1, 0.25f);
            }
        }

        // Token: 0x060034B7 RID: 13495 RVA: 0x00184184 File Offset: 0x00182584
        private void CreateFlamePunch(int givenLevel)
        {
            this.CleanUpEventHandler(true);
            this.currentPosition = this.parent.attackOriginTrans.position + (Vector3) this.inputVector;
            this.currentFP = base.ChaosInst<FlamePunch>(FlamePunch.Prefab, new Vector2?(this.currentPosition), new Quaternion?(this.currentQuat), null);
            this.currentFP.SetScale((givenLevel <= 1) ? this.baseScale : this.finalScale);
            this.currentFP.attack.SetAttackInfo(this.parent.skillCategory, this.skillID, givenLevel, false);
            this.currentFP.attack.knockbackOverwriteVector = this.inputVector;
            this.currentFP.isFinalHit = (givenLevel > 1);
            if (!this.movementStopped)
            {
                Attack attack = this.currentFP.attack;
                attack.entityCollisionEventHandlers = (Attack.EntityCollisionEventHandler)Delegate.Remove(attack.entityCollisionEventHandlers, new Attack.EntityCollisionEventHandler(this.OnTargetHit));
                Attack attack2 = this.currentFP.attack;
                attack2.entityCollisionEventHandlers = (Attack.EntityCollisionEventHandler)Delegate.Combine(attack2.entityCollisionEventHandlers, new Attack.EntityCollisionEventHandler(this.OnTargetHit));
            }
            SoundManager.PlayWithDistAndSPR("BlazingBlitzEnd", this.currentPosition, 1f);
            SoundManager.PlayWithDistAndSPR("FireballCast", this.currentPosition, 0.9f);
        }

        // Token: 0x060034B8 RID: 13496 RVA: 0x001842E4 File Offset: 0x001826E4
        private void OnTargetHit(Entity givenEntity)
        {
            if (this.movementStopped)
            {
                return;
            }
            if (givenEntity != null && givenEntity is Enemy && givenEntity.health != null && givenEntity.health.ignoreGrab)
            {
                this.movementStopped = true;
                this.parent.movement.EndMovement();
                this.parent.movement.MoveToMoveVector(8f, false);
                this.CleanUpEventHandler(false);
                this.jumpRange = 4f;
            }
        }

        // Token: 0x060034B9 RID: 13497 RVA: 0x00184374 File Offset: 0x00182774
        private void CleanUpEventHandler(bool removeRef = true)
        {
            if (this.currentFP != null && this.currentFP.attack != null)
            {
                Attack attack = this.currentFP.attack;
                attack.entityCollisionEventHandlers = (Attack.EntityCollisionEventHandler)Delegate.Remove(attack.entityCollisionEventHandlers, new Attack.EntityCollisionEventHandler(this.OnTargetHit));
                if (removeRef)
                {
                    UnityEngine.Object.Destroy(this.currentFP.gameObject, 0.25f);
                    this.currentFP = null;
                }
            }
        }

        // Token: 0x060034BA RID: 13498 RVA: 0x001843F8 File Offset: 0x001827F8
        public override void OnLanding()
        {
            if (this.hasLanded || this.fsm.nextStateName.Contains("Hurt") || this.fsm.nextStateName.Contains("Dead"))
            {
                return;
            }
            base.OnLanding();
            this.parent.anim.PlayDirectional(this.parent.GSlamAnimStr, -1, 0.4f);
            this.parent.movement.moveVector = -this.inputVector;
            this.parent.movement.MoveToMoveVector(6f, false);
        }

        // Token: 0x060034BB RID: 13499 RVA: 0x001844A0 File Offset: 0x001828A0
        private void InitJump()
        {
            this.jumpStarted = true;
            this.startPosition = this.parent.transform.position;
            this.parent.spriteRenderer.sortingLayerName = "UI";
            this.SetThrowValues();
            this.parent.throwScript.ThrowToTargetVector(Globals.GetLinecastVector(this.startPosition, this.startPosition - this.inputVector * this.jumpRange, ChaosCollisions.layerAllWallAndObst) + this.inputVector * 0.5f);
            SoundManager.PlayAudioWithDistance("StandardJump", new Vector2?(this.startPosition), null, 24f, -1f, 1f, false);
            DustEmitter poolItem = PoolManager.GetPoolItem<DustEmitter>();
            int particleCount = 50;
            float scale = 0.25f;
            Vector3? emitPosition = new Vector3?(this.startPosition + Vector2.up * 0.5f);
            poolItem.EmitCircle(particleCount, scale, -1f, -1f, emitPosition, null);
            this.hasLanded = false;
            this.collidersEnabled = false;
            this.ToggleObjects(false);
            ThrownItem parentThrow = this.parentThrow;
            parentThrow.onLandingEventHandlers = (ThrownItem.OnLandingEventHandler)Delegate.Remove(parentThrow.onLandingEventHandlers, new ThrownItem.OnLandingEventHandler(this.OnLanding));
            ThrownItem parentThrow2 = this.parentThrow;
            parentThrow2.onLandingEventHandlers = (ThrownItem.OnLandingEventHandler)Delegate.Combine(parentThrow2.onLandingEventHandlers, new ThrownItem.OnLandingEventHandler(this.OnLanding));
            this.landingStopwatch.SetDelay(0.1f, false, false);
        }

        // Token: 0x04003782 RID: 14210
        public new static string staticID = "Mythical::UseFlamePunchGood";

        // Token: 0x04003783 RID: 14211
        private Vector2 currentPosition;

        // Token: 0x04003784 RID: 14212
        private Quaternion currentQuat;

        // Token: 0x04003785 RID: 14213
        private FlamePunch currentFP;

        // Token: 0x04003786 RID: 14214
        private float baseScale;

        // Token: 0x04003787 RID: 14215
        private float finalScale;

        // Token: 0x04003788 RID: 14216
        private int hitStopwatchID;

        // Token: 0x04003789 RID: 14217
        private int hitCounter;

        // Token: 0x0400378A RID: 14218
        private int maxHitCount;

        // Token: 0x0400378B RID: 14219
        private float hitDelay;

        // Token: 0x0400378C RID: 14220
        private NumVarStatMod dashSpdMod;

        // Token: 0x0400378D RID: 14221
        private NumVarStatMod dashDurMod;

        // Token: 0x0400378E RID: 14222
        private bool movementStopped;

        // Token: 0x0400378F RID: 14223
        private bool jumpStarted;

        // Token: 0x04003790 RID: 14224
        private bool spinAnimPlayed;
    }
}
