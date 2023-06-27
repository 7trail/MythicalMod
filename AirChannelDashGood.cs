using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    public class AirChannelDashGood : Player.BaseDashState
    {

        public AirChannelDashGood(FSM fsm, Player parentPlayer) : base("AirChannelDashGood", fsm, parentPlayer)
        {
            //this.hasEmpowered = true;
            this.applyStopElementStatus = true;
            base.InitChargeSkillSettings(2, 0f, this.skillData, this);
        }

        public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod)
        {
            base.SetEmpowered(givenStatus, givenMod);
            this.burstScale = ((!this.IsEmpowered) ? 1.75f : 2f);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.cooldownReady)
            {
                this.spawnPosition = this.parent.attackOriginTrans.position;
                this.CreateImplosion();
                SoundManager.PlayAudioWithDistance("StandardHeavySwing", new Vector2?(this.parent.transform.position), null, 24f, -1f, 1.4f, false);
                PoolManager.GetPoolItem<SectionedTrailEmitter>("WindTrail").Emit(this.spawnPosition, this.spawnPosition + this.inputVector * 5f, -1, false, -1f, true, 0.3f, 0.15f, null, true, null, null);
                PoolManager.GetPoolItem<SectionedTrailEmitter>("WindTrail").Emit(this.spawnPosition, this.spawnPosition + this.inputVector * 5f, -1, false, -1f, true, 0.4f, 0.15f, null, true, null, null);
            }
        }

        public override void OnExit()
        {
            if (this.cooldownReady && !this.fsm.nextStateName.Contains("Hurt") && !this.fsm.nextStateName.Contains("Dead"))
            {
                this.CreateAirChannel();
                if (this.IsEmpowered)
                {
                    this.spawnPosition = this.parent.attackOriginTrans.position;
                    this.CreateImplosion();
                }
            }
            base.OnExit();
        }

        private void CreateImplosion()
        {
            //Log.Warning(skillID);
            this.currentWB = WindBurst.CreateBurst(this.spawnPosition, this.parent.skillCategory, this.skillID, 1, this.burstScale);
            this.currentWB.emitParticles = false;
            PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(3), new Vector3?(this.spawnPosition), null, null, 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(this.spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 33f))), 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(this.spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(180f, 213f))), 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(this.spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f))), 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(this.spawnPosition), this.implosionOverrideLarge, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f))), 0f, null, null);
            DustEmitter poolItem = PoolManager.GetPoolItem<DustEmitter>();
            int particleCount = 150;
            float scale = 2f;
            Vector3? emitPosition = new Vector3?(this.spawnPosition);
            poolItem.EmitCircle(particleCount, scale, -8f, -1f, emitPosition, null);
        }

        private void CreateAirChannel()
        {
            this.currentAC = base.ChaosInst<AirChannel>(AirChannel.Prefab, new Vector2?(this.spawnPosition), new Quaternion?(Globals.GetRotationQuaternion(this.inputVector)), null);
            this.currentAC.attack.SetAttackInfo(this.parent.skillCategory, this.skillID, 2, false);
            this.currentAC.attack.knockbackOverwriteVector = this.inputVector;
            this.currentAC.targetVector = this.inputVector;
        }

        public new static string staticID = "AirChannelDash";

        private WindBurst currentWB;

        private AirChannel currentAC;

        private Vector2 spawnPosition;

        private float burstScale = 1.75f;

        private ParticleSystemOverride implosionOverride = new ParticleSystemOverride
        {
            startSize = new float?(5.5f),
            startLifetime = new float?(0.7f)
        };

        private ParticleSystemOverride implosionOverrideLarge = new ParticleSystemOverride
        {
            startSize = new float?(6.5f),
            startLifetime = new float?(0.6f)
        };


    }

    public class FireBurstDash : Player.BaseDashState
    {

        public FireBurstDash(FSM fsm, Player parentPlayer) : base(FireBurstDash.staticID, fsm, parentPlayer)
        {
            //this.hasEmpowered = true;
            this.applyStopElementStatus = true;
            this.isDash = true;
            base.InitChargeSkillSettings(2, 0f, this.skillData, this);
        }

        public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod)
        {
            base.SetEmpowered(givenStatus, givenMod);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.cooldownReady)
            {
                this.spawnPosition = this.parent.attackOriginTrans.position;
                //this.CreateImplosion();
                this.CreateExplosion(FlameBurst.burnSkillID, this.spawnPosition);
                SoundManager.PlayAudioWithDistance("StandardHeavySwing", new Vector2?(this.parent.transform.position), null, 24f, -1f, 1.4f, false);
                PoolManager.GetPoolItem<SectionedTrailEmitter>("FloorRift").Emit(this.spawnPosition, this.spawnPosition + this.inputVector * 5f, -1, false, -1f, true, 0.3f, 0.15f, null, true, null, null);
                PoolManager.GetPoolItem<SectionedTrailEmitter>("FloorRift").Emit(this.spawnPosition, this.spawnPosition + this.inputVector * 5f, -1, false, -1f, true, 0.4f, 0.15f, null, true, null, null);
            }
        }

        public override void OnExit()
        {
            if (this.cooldownReady && !this.fsm.nextStateName.Contains("Hurt") && !this.fsm.nextStateName.Contains("Dead"))
            {
                if (this.IsEmpowered)
                {
                    this.spawnPosition = this.parent.attackOriginTrans.position;
                    this.CreateExplosion(FlameBurst.burnSkillID, this.spawnPosition);
                }
            }
            Debug.Log("Doing the thing");
            base.OnExit();
        }
        public void CreateExplosion(string skillID, Vector2 givenPosition, bool empower = false)
        {
            global::FlameBurst.CreateBurst(givenPosition, this.parent.skillCategory, skillID, 1, 2.5f * (empower ? 1.25f : 1), true);
            global::SoundManager.PlayWithDistAndSPR("BlazingBlitzEnd", givenPosition, 1f);
            global::PoolManager.GetPoolItem<global::ParticleEffect>("SmokeEmitter").Emit(new int?(6), new Vector3?(givenPosition), null, null, 0f, null, null);
            global::CameraController.ShakeCamera(0.25f, false);
        }

        public new static string staticID = "Mythical::RadiantStorm";

        private Vector2 spawnPosition;


    }

    public class UseWindDefenseNerfed : Player.UseBuffSkill
    {
        // Token: 0x06003719 RID: 14105 RVA: 0x0019AF16 File Offset: 0x00199316
        public UseWindDefenseNerfed(FSM parentFSM, Player parentEntity) : base(UseWindDefenseNerfed.staticID, parentFSM, parentEntity)
        {
            this.whileStopwatch.SetDelay(0.125f, false, false);
        }

        // Token: 0x1700078F RID: 1935
        // (get) Token: 0x0600371A RID: 14106 RVA: 0x0019AF37 File Offset: 0x00199337
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

        // Token: 0x17000790 RID: 1936
        // (get) Token: 0x0600371B RID: 14107 RVA: 0x0019AF60 File Offset: 0x00199360
        private bool ParentIsMoving
        {
            get
            {
                this.checkState = (this.fsm.currentState as Player.SkillState);
                return (this.checkState != null && (this.checkState.isBasic || this.checkState.isMovementSkill)) || (this.fsm != null && (this.fsm.currentStateName == base.name || this.fsm.currentStateName == "Run" || this.fsm.currentStateName == Player.SlideState.staticID));
            }
        }

        // Token: 0x0600371C RID: 14108 RVA: 0x0019B00C File Offset: 0x0019940C
        public override void SetBuffStatus(bool givenStatus)
        {
            base.SetBuffStatus(givenStatus);
            this.evadeStatus = false;
            if (givenStatus)
            {
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
            }
            else
            {
                this.ToggleMods(false, true);
                this.parent.overheadPrompt.HidePrompt(string.Empty);
                if (this.currentEffect != null)
                {
                    this.currentEffect.Stop();
                    if (this.currentEffect.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(this.currentEffect.gameObject, 1f);
                    }
                    this.currentEffect = null;
                }
                PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(6), new Vector3?(this.attackPosition), null, null, 0f, null, null);
                string audioID = "TornadoEnd";
                Vector2? soundOrigin = new Vector2?(this.currentPosition);
                float num = UnityEngine.Random.Range(0.75f, 0.85f);
                SoundManager.PlayAudioWithDistance(audioID, soundOrigin, null, 24f, -1f, num, false);
            }
        }

        // Token: 0x0600371D RID: 14109 RVA: 0x0019B214 File Offset: 0x00199614
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

        // Token: 0x0600371E RID: 14110 RVA: 0x0019B2BC File Offset: 0x001996BC
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

        // Token: 0x04003A2E RID: 14894
        public new static string staticID = "UseWindDefenseNerfed";

        // Token: 0x04003A2F RID: 14895
        public static ParticleSystemOverride evadeEffectOverride = new ParticleSystemOverride
        {
            radius = new float?(0.75f),
            startColor = new Color32?(new Color(1f, 1f, 1f, 0.33f))
        };

        // Token: 0x04003A30 RID: 14896
        private static GameObject _effectPrefab;

        // Token: 0x04003A31 RID: 14897
        private static BoolVarStatMod fallMod = new BoolVarStatMod(Player.UseWindDefense.staticID, true, 10);

        // Token: 0x04003A32 RID: 14898
        private static NumVarStatMod evadeMod = new NumVarStatMod(Player.UseWindDefense.staticID, 1f, 0, VarStatModType.Override, false);

        // Token: 0x04003A33 RID: 14899
        private static NumVarStatMod moveMod = new NumVarStatMod(Player.UseWindDefense.staticID, 0.4f, 10, VarStatModType.Multiplicative, false);

        // Token: 0x04003A34 RID: 14900
        private ParticleEffect currentEffect;

        // Token: 0x04003A35 RID: 14901
        private bool evadeStatus;

        // Token: 0x04003A36 RID: 14902
        private bool empStatus;

        // Token: 0x04003A37 RID: 14903
        private Player.SkillState checkState;
    }


}
