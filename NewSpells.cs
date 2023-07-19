using Chaos.AnimatorExtensions;
using Chaos.ListExtensions;
using JetBrains.Annotations;
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

    public class UseShockLaceLine : Player.SkillState
    {
        // Token: 0x06003619 RID: 13849 RVA: 0x00191058 File Offset: 0x0018F458
        public UseShockLaceLine(FSM parentFSM, Player parentEntity) : base(UseShockLaceLine.staticID, parentFSM, parentEntity)
        {
            this.hasSignatureVariant = true;
            this.SetAnimTimes(0f, 0.2f, 0.1f, 0.8f, 0.9f, 1f);
        }

        // Token: 0x17000779 RID: 1913
        // (get) Token: 0x0600361A RID: 13850 RVA: 0x0019108B File Offset: 0x0018F48B
        public override string OnEnterAnimStr
        {
            get
            {
                return this.parent.PBAoEAnimStr;
            }
        }

        // Token: 0x0600361B RID: 13851 RVA: 0x00191098 File Offset: 0x0018F498
        public override void ExecuteSkill()
        {
            base.ExecuteSkill();
			Vector2 Vec = this.inputVector;

            if (isUltimate)
            {
                ShockLace shockLace = base.ChaosInst<ShockLace>(ShockLace.Prefab, new Vector2?(Globals.GetSafeLinecastVector(this.parent.transform.position, this.inputVector, 0, ChaosCollisions.layerAllWallAndObst)), null, null);
                shockLace.SetSkillInfo(this.parent.skillCategory, this.skillID, this.inputVector);
                shockLace.nodeCount = 8;
                shockLace.atkRadius = 3f;
                shockLace.atkCount = 20;
                shockLace.atkInterval = 0.2f;
                for (int i = 0; i < 12; i++)
                {
                    Vector2 vec = Globals.GetRotatedCircleVector(12, i, this.inputVector);
                    shockLace = base.ChaosInst<ShockLace>(ShockLace.Prefab, new Vector2?(Globals.GetSafeLinecastVector(this.parent.transform.position, vec, 5, ChaosCollisions.layerAllWallAndObst)), null, null);
                    shockLace.SetSkillInfo(this.parent.skillCategory, this.skillID, vec);
                    shockLace.nodeCount = 8;
                    shockLace.atkRadius = 2f;
                    shockLace.atkCount = 10;
                    shockLace.atkInterval = 0.2f;
                    
                }
                CameraController.ShakeCamera(0.25f, false);

            } else
            {
                for (int i = 0; i < 3; i++)
                {
                    ShockLace shockLace = base.ChaosInst<ShockLace>(ShockLace.Prefab, new Vector2?(Globals.GetSafeLinecastVector(this.parent.transform.position, Vec, 4f + ((8 - i) * i), ChaosCollisions.layerAllWallAndObst)), null, null);
                    shockLace.SetSkillInfo(this.parent.skillCategory, this.skillID, this.inputVector);
                    if (this.IsEmpowered)
                    {
                        shockLace.nodeCount = 6;
                        shockLace.atkRadius = 4.25f - (i);
                        shockLace.atkCount = 10;
                        shockLace.atkInterval = 0.2f;
                    }
                    else
                    {
                        shockLace.nodeCount = 6;
                        shockLace.atkRadius = 3.25f - (i);
                        shockLace.atkCount = 8;
                        shockLace.atkInterval = 0.3f;
                    }
                    CameraController.ShakeCamera(0.25f, false);
                }
            }
            
        }

        // Token: 0x0400390E RID: 14606
        public new static string staticID = "Mythical::UseShockLaceLine";
    }

    public class UseSlicingBarrageGood : Player.SkillState
    {
        // Token: 0x0600364C RID: 13900 RVA: 0x00192BD0 File Offset: 0x00190FD0
        public UseSlicingBarrageGood(FSM parentFSM, Player parentEntity) : base(UseSlicingBarrageGood.staticID, parentFSM, parentEntity)
        {
            this.hasSignatureVariant = true;
            this.isMovementSkill = true;
            this.isMeleeSkill = true;
            this.SetAnimTimes(0.2f, 0.2f, 0.35f, 0.6f, 0.8f, 1f);
            this.speedMod = new NumVarStatMod(UseSlicingBarrageGood.staticID, 8f, 5, VarStatModType.OverrideWithMods, false);
        }

        // Token: 0x0600364D RID: 13901 RVA: 0x00192C73 File Offset: 0x00191073
        public override void OnEnter()
        {
            base.OnEnter();
            this.atkStarted = false;
            this.wasEmpowered = this.IsEmpowered;
            this.parent.movement.EndMovement();
        }

        // Token: 0x0600364E RID: 13902 RVA: 0x00192CA0 File Offset: 0x001910A0
        public override void ExecuteSkill()
        {
            if (!this.atkStarted)
            {
                this.atkStarted = true;
                this.atkCounter = 0;
                this.atkMaxCount = ((!this.isUltimate) ? ((!this.IsEmpowered) ? 7 : 10) : 13);
                this.atkStopwatchID = ChaosStopwatch.Begin(0f, true, this.atkInterval, this.atkMaxCount, 0);
                this.parent.movement.moveVector = this.parent.GetInputVector();
            }
            StopwatchState stopwatchState = ChaosStopwatch.CheckInterval(this.atkStopwatchID, true);
            if (stopwatchState != StopwatchState.Running)
            {
                if (stopwatchState != StopwatchState.Ready)
                {
                    if (stopwatchState == StopwatchState.Done)
                    {
                        base.ExecuteSkill();
                    }
                }
                else
                {
                    this.atkCounter++;
                    this.isFinalAtk = (this.atkCounter >= this.atkMaxCount);
                    this.parent.movement.moveVector = this.parent.GetInputVector();
                    if (this.isFinalAtk)
                    {
                        CameraController.ShakeCamera(0.25f, false);
                        
                        this.parent.anim.PlayDirectional(this.parent.PBAoEAnimStr, -1, this.animExecTime);
                        if (this.isUltimate)
                        {
                            this.speedMod.modValue = 40f;
                            this.CreateWindSlash(this.parent.GetInputVector(), true);
                            this.CreateWindSlash(this.parent.GetInputVector(), false);
                        }
                        else
                        {
                            this.speedMod.modValue = 30f;
                            this.CreateWindSlash(this.parent.GetInputVector(), false);
                        }
                    }
                    else
                    {
                        if (this.atkCounter > 1)
                        {
                            this.parent.anim.PlayDirectional(this.parent.NextHandAnimStr, -1, this.animExecTime);
                        }
                        if (this.isUltimate)
                        {
                            this.speedMod.modValue = 35f;
                            this.CreateWindSlash(this.parent.GetInputVector(), false);
                        }
                        else
                        {
                            this.speedMod.modValue = 25f;
                            this.CreateSlicingBarrage(this.parent.attackOriginTrans.position, this.parent.GetInputVector());
                        }
                    }
                    this.parent.movement.moveSpeedStat.Modify(this.speedMod, true);
                    this.parent.movement.MoveToMoveVector(0f, false);
                }
            }
        }

        // Token: 0x0600364F RID: 13903 RVA: 0x00192F84 File Offset: 0x00191384
        private void CreateSlicingBarrage(Vector2 givenPosition, Vector2 givenVector)
        {
            this.currentSB = base.ChaosInst<SlicingBarrage>(SlicingBarrage.Prefab, new Vector2?(givenPosition + givenVector * 0.25f), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null);
            this.currentSB.attack.SetAttackInfo(this.parent.skillCategory, this.skillID, 1, this.isUltimate);
            this.currentSB.attack.knockbackOverwriteVector = this.inputVector;
            int num = this.atkCounter % 3;
            if (num != 1)
            {
                if (num == 2)
                {
                    this.currentSB.transform.localScale = this.negScale;
                }
            }
            else if (!this.parent.isFacingRight)
            {
                this.currentSB.transform.localScale = this.negScale;
            }
            this.currentSB.anim.speed = 0.75f;
            this.currentSB.anim.Play((!this.wasEmpowered && !this.isFinalAtk) ? Player.UseSlicingBarrage.medAnimStr : Player.UseSlicingBarrage.lrgAnimStr, -1, 0f);
            PoolManager.GetPoolItem<DustEmitter>().EmitDirBurst(4, Globals.GetRotationVector(givenVector).z, 4f, -1f, 0.3f, new Vector3?(givenPosition));
        }

        // Token: 0x06003650 RID: 13904 RVA: 0x001930F0 File Offset: 0x001914F0
        private void CreateWindSlash(Vector2 givenVector, bool posOffset = false)
        {
            this.currentWS = (WindSlashProjectile)Projectile.CreateProjectile(this.parent, WindSlashProjectile.Prefab, new Vector3?(this.parent.attackOriginTrans.position), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null);
            this.currentWS.moveVector = givenVector;
            this.atkLevel = 1;
            if (this.isUltimate)
            {
                this.currentWS.lifeTime = 0.5f;
                this.currentWS.moveSpeed = 30f;
                this.currentWS.flyTime = 0.24f;
                this.atkLevel = 2;
            }
            this.currentWS.attackBox.SetAttackInfo(this.parent.skillCategory, this.skillID, this.atkLevel, this.isUltimate);
            this.currentWS.attackBox.knockbackOverwriteVector = this.inputVector;
            this.currentWS.alwaysWinCollision = true;
            string text = Player.UseSlicingBarrage.audioID;
            Vector2? soundOrigin = new Vector2?(this.parent.transform.position);
            float overridePitch = UnityEngine.Random.Range(0.75f, 0.85f);
            SoundManager.PlayAudioWithDistance(text, soundOrigin, null, 24f, -1f, overridePitch, false);
        }

        // Token: 0x06003651 RID: 13905 RVA: 0x00193306 File Offset: 0x00191706
        public override void OnExit()
        {
            this.parent.movement.moveSpeedStat.Modify(this.speedMod, false);
            base.OnExit();
        }

        // Token: 0x04003940 RID: 14656
        public new static string staticID = "Mythical::UseSlicingBarrageGood";

        // Token: 0x04003941 RID: 14657
        private static string medAnimStr = "MedSlashAdvancing";

        // Token: 0x04003942 RID: 14658
        private static string lrgAnimStr = "LargeSlashAdvancing";

        // Token: 0x04003943 RID: 14659
        private static string audioID = "WindArrowEnd";


        // Token: 0x04003945 RID: 14661
        private Vector2 perpendVec;

        // Token: 0x04003946 RID: 14662
        private Vector3 negScale = new Vector3(-1f, 1f, 1f);

        // Token: 0x04003947 RID: 14663
        private SlicingBarrage currentSB;

        // Token: 0x04003948 RID: 14664
        private WindSlashProjectile currentWS;

        // Token: 0x04003949 RID: 14665
        private NumVarStatMod speedMod;

        // Token: 0x0400394A RID: 14666
        private int atkMaxCount = 7;

        // Token: 0x0400394B RID: 14667
        private float atkInterval = 0.1f;

        // Token: 0x0400394C RID: 14668
        private int atkStopwatchID;

        // Token: 0x0400394D RID: 14669
        private bool atkStarted;

        // Token: 0x0400394E RID: 14670
        private int atkCounter;

        // Token: 0x0400394F RID: 14671
        private int atkLevel;

        // Token: 0x04003950 RID: 14672
        private bool wasEmpowered;

        // Token: 0x04003951 RID: 14673
        private bool isFinalAtk;
    }

    public class UseCombustionWaveGood : Player.SkillState
    {
        // Token: 0x060033EB RID: 13291 RVA: 0x0017CCE8 File Offset: 0x0017B0E8
        public UseCombustionWaveGood(FSM parentFSM, Player parentEntity) : base(UseCombustionWaveGood.staticID, parentFSM, parentEntity)
        {
            this.hasSignatureVariant = true;
            this.SetAnimTimes(0.15f, 0.2f, 0.3f, 0.6f, 0.8f, 1f);
        }

        // Token: 0x17000744 RID: 1860
        // (get) Token: 0x060033EC RID: 13292 RVA: 0x0017CD1B File Offset: 0x0017B11B
        public override string OnEnterAnimStr
        {
            get
            {
                return this.parent.GSlamAnimStr;
            }
        }

        // Token: 0x060033ED RID: 13293 RVA: 0x0017CD28 File Offset: 0x0017B128
        public override void OnEnter()
        {
            base.OnEnter();
            this.parent.ToggleEnemyFloorCollisions(false);
        }

        // Token: 0x060033EE RID: 13294 RVA: 0x0017CD3C File Offset: 0x0017B13C
        public override void ExecuteSkill()
        {
            base.ExecuteSkill();

            Vector2 v = Globals.GetPerpendicular(this.inputVector, true) * 0.4f;


            if (this.isUltimate)
            {
                CreateWave((inputVector + (v*1.5f)).normalized);
                CreateWave((inputVector - (v * 1.5f)).normalized);
                CreateWave((inputVector).normalized);
                CreateWave((inputVector + v).normalized);
                CreateWave((inputVector - v).normalized);
            } else
            {
                CreateWave((inputVector + v).normalized);
                CreateWave((inputVector).normalized);
                CreateWave((inputVector - v).normalized);
            }

        }

        public void CreateWave(Vector2 vector)
        {
            this.currentCW = base.ChaosInst<CombustionWave>(CombustionWave.Prefab, new Vector2?(this.parent.transform.position), null, null);
            this.currentCW.SetSkillData(this.parent.skillCategory, this.skillID, -1, false, false);
            this.currentCW.moveVector = vector*1.5f;
            this.currentCW.maxWaveCount = this.isUltimate?8:6;
            this.currentCW.spawnRate = 0.15f;
            this.currentCW.isEmpowered = this.IsEmpowered;
        }

        // Token: 0x060033EF RID: 13295 RVA: 0x0017CDE1 File Offset: 0x0017B1E1
        public override void OnExit()
        {
            base.OnExit();
            this.parent.ToggleEnemyFloorCollisions(true);
        }

        // Token: 0x040036BE RID: 14014
        public new static string staticID = "Mythical::UseCombustionWaveGood";

        // Token: 0x040036BF RID: 14015
        private CombustionWave currentCW;
    }

    public class UseRandomMinion : Player.UseBaseMinion
    {
        // Token: 0x060035F3 RID: 13811 RVA: 0x00190170 File Offset: 0x0018E570
        public UseRandomMinion(FSM parentFSM, Player parentEntity) : base(UseRandomMinion.staticID, parentFSM, parentEntity)
        {
            this.minionPrefab = FireMinion.Prefab;
            this.hasSignatureVariant = true;

        }
        public override void SummonAllMinions()
        {
            List<GameObject> prefabs = new List<GameObject>()
            {
                FireMinion.prefab,
                EarthMinion.prefab,
                WaterMinion.prefab,
                LightningMinion.prefab,
                WindMinion.prefab,
                ChaosMinion.prefab
            };

            List<string> strings = new List<string>()
            {
                "UseFireMinion",
                "UseEarthMinion",
                "UseWaterMinion",
                "UseLightningMinion",
                "UseWindMinion",
                "UseChaosMinion",
            };

            List<GameObject> inst = new List<GameObject>();
            int id = UnityEngine.Random.Range(0, prefabs.Count);
            if (this.isUltimate)
            {
                inst.AddRange(prefabs);
            }

            this.minionList.RemoveNullValues<BasicMinion>();
            this.parent.CleanUpPlayerSummonsList();
            this.currentCount = 0;
            while (this.currentCount < this.summonCount)
            {
                if (!isUltimate)
                {
                    id = UnityEngine.Random.Range(0, prefabs.Count);
                    inst.Clear();
                    inst.Add(prefabs[id]);
                }
                foreach (GameObject o in inst)
                {
                    zz = strings[prefabs.IndexOf(o)];
                    this.minionPrefab = o;
                    this.SummonMinion();
                    if (!isUltimate)
                    {
                        this.LimitToMaxMinions();
                    }
                    this.currentCount++;
                }
            }
        }

        public override BasicMinion SummonMinion()
        {
            this.currentMinion = base.ChaosInst<BasicMinion>(this.minionPrefab, new Vector2?(this.GetSpawnLocation()+new Vector2(UnityEngine.Random.value*3-1.5f, UnityEngine.Random.value * 3 - 1.5f)), null, null);
            this.currentMinion.gameObject.name = this.minionPrefab.name + this.parent.skillCategory;
            this.currentMinion.parentTrans = this.parent.transform;
            this.currentMinion.parentSummonList = this.parent.playerSummonsList;
            this.currentMinion.SetSkillInfo(this.parent.skillCategory, zz, this.currentLevel, this.isUltimate);
            this.currentMinion.summonDuration = this.summonDuration;
            this.currentMinion.startingHealth = this.summonHealth;
            this.currentMinion.isEmpowered = this.IsEmpowered;
            if (Inventory.EitherPlayerHasItem(FlatDamage.staticID) && FlatDamage.damageMod != null)
            {
                this.currentMinion.health.damageTakenStat.Modify(FlatDamage.damageMod, true);
            }
            this.minionList.Add(this.currentMinion);
            this.parent.playerSummonsList.Add(this.currentMinion.gameObject);
            return this.currentMinion;
        }

        string zz = "";
        // Token: 0x040038EB RID: 14571
        public new static string staticID = "Mythical::UseRandomMinions";
    }

    public class DragonCross : Player.MeleeAttackState
    {
        // Token: 0x0600312A RID: 12586 RVA: 0x0016801C File Offset: 0x0016641C
        public DragonCross(FSM parentFSM, Player parentEntity) : base(DragonCross.staticID, parentFSM, parentEntity)
        {
            this.isBasic = true;
            this.setParent = false;
            this.destroyAttackObjectOnExit = false;
            this.maxComboCount = 3;
            this.animStartTime = 0.2f;
            this.animExecTime = 0.4f;
            this.baseCancelThreshold = 0.6f;
            this.finalCancelThreshold = 0.7f;
            this.baseRunThreshold = 0.8f;
            this.finalRunThreshold = 0.85f;
            this.baseExitThreshold = 1f;
            this.finalExitThreshold = 1f;
            this.flipObjectForBackhand = false;
            this.attackMoveSpeedMod = new NumVarStatMod(this.atkMoveModStr, 0f, 10, VarStatModType.OverrideWithMods, false);
            this.finalAttackMoveSpeedMod = this.attackMoveSpeedMod;
            this.sfxNames = new string[this.maxComboCount];
            for (int i = 0; i < this.maxComboCount; i++)
            {
                this.sfxNames[i] = "FlameLight";
            }
            this.autoHoldStat.BaseValue = true;
        }

        // Token: 0x17000709 RID: 1801
        // (get) Token: 0x0600312B RID: 12587 RVA: 0x00168136 File Offset: 0x00166536
        public bool IsDoubleShot
        {
            get
            {
                return base.HitsRemaining == 0;
            }
        }

        // Token: 0x0600312C RID: 12588 RVA: 0x00168144 File Offset: 0x00166544
        public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod)
        {
            base.SetEmpowered(givenStatus, givenMod);
        }

        // Token: 0x0600312D RID: 12589 RVA: 0x001681A8 File Offset: 0x001665A8
        public override void SetMoveSpeedAndCancelThreshold(int attackCount)
        {
            if (this.IsDoubleShot)
            {
                this.cancelThreshold = this.finalCancelThreshold;
                this.parent.movement.moveSpeedStat.AddMod(this.finalAttackMoveSpeedMod);
            }
            else
            {
                this.cancelThreshold = this.baseCancelThreshold;
                this.parent.movement.moveSpeedStat.AddMod(this.attackMoveSpeedMod);
            }
        }

        // Token: 0x0600312E RID: 12590 RVA: 0x00168213 File Offset: 0x00166613
        public override void PlayAnimation(int attackCount)
        {
            this.parent.anim.PlayDirectional((!this.IsDoubleShot) ? this.parent.NextHandAnimStr : this.parent.PBAoEAnimStr, -1, this.animStartTime);
        }

        // Token: 0x0600312F RID: 12591 RVA: 0x00168254 File Offset: 0x00166654
        public override void ExecuteAttack()
        {
            this.HandleComboCount(this.comboCounter);
            this.HandleSelfTransition(this.comboCounter);
            this.AnnounceSkillExecuteEvent();
            if (this.IsDoubleShot)
            {
                FireFireArc((Vector2)this.parent.attackOriginTrans.position + this.inputVector, inputVector, true);
                FireFireArc((Vector2)this.parent.attackOriginTrans.position + this.inputVector, inputVector, false);
                CameraController.ShakeCamera(0.15f, false);

                this.parent.movement.EndMovement();
                this.parent.movement.moveVector = -this.inputVector;
                this.parent.movement.MoveToMoveVector(20, false);
            }
            else
            {
                bool arc = ((!this.parent.isFacingRight) ? this.parent.playForehandAnim : (!this.parent.playForehandAnim));
                FireFireArc((Vector2)this.parent.attackOriginTrans.position + this.inputVector , inputVector, arc);
                //this.CreateFireCross((!this.parent.isFacingRight) ? Globals.GetPerpendicular(this.inputVector, this.parent.forehandAnimPlayed) : Globals.GetPerpendicular(this.inputVector, !this.parent.forehandAnimPlayed), false);
            }
            base.CancelToDash(false);
        }


        private void FireFireArc(Vector2 givenPosition, Vector2 givenVector, bool positiveArc)
        {
            this.currentFA = (FireArc)this.CreateProjectile(FireArc.Prefab, new Vector3?(givenPosition + UnityEngine.Random.insideUnitCircle * 0.5f), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null, true);
            this.currentFA.targetVector = givenVector;
            this.currentFA.attackBox.knockbackOverwriteVector = givenVector;
            this.currentFA.positiveApproach = positiveArc;
            this.currentFA.arcRange = 3;
            this.currentFA.flyTime *= 0.4f;
            this.currentFA.moveSpeed *= 0.7f;
            this.currentFA.lifeTime *= 0.3f;
            this.currentFA.disableOnHit = true;
            this.currentFA.projectileSpriteRenderer.sprite = SampleSkillLoader.newArc;
            darkArcs.Add(this.currentFA);

            ParticleSystem ps = this.currentFA.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            main.startColor = new Color(0.5f, 0.5f, 0, 0.784f);
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
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
            },colorOverLifetime.color.gradient.alphaKeys);

            colorOverLifetime.color = grad;
        }

        public Projectile CreateProjectile(GameObject prefab, Vector3? givenPosition = null, Quaternion? givenQuaternion = null, Transform parentTrans = null, bool setAtkInfo = true)
        {
            this.newProjectile = Projectile.CreateProjectile(this.parent, prefab, new Vector3?((givenPosition == null) ? this.parent.attackOriginTrans.position : givenPosition.Value), new Quaternion?((givenQuaternion == null) ? Quaternion.identity : givenQuaternion.Value), parentTrans ?? this.parent.transform.parent);
            if (setAtkInfo)
            {
                this.SetProjectileAtkInfo(this.newProjectile);
            }
            return this.newProjectile;
        }

        public void SetProjectileAtkInfo(Projectile projectile)
        {
            projectile.attackBox.SetAttackInfo(this.parent.skillCategory, this.skillID, this.currentLevel, false);
            projectile.UpdateCalculatedDamage(true);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            CameraController.ShakeCamera(0.25f, false);
            this.fbEmitter = PoolManager.GetPoolItem<FireBurst>();
        }

        // Token: 0x04003400 RID: 13312
        public new static string staticID = "Mythical::DragonCross";

        // Token: 0x04003401 RID: 13313
        private FireArc currentFA;
        private FireBurst fbEmitter;
        private Projectile newProjectile;

        public static List<FireArc> darkArcs = new List<FireArc>(); 
    }



    public class UseFrostRing : Player.ProjectileAttackState
    {
        // Token: 0x060034ED RID: 13549 RVA: 0x001860F8 File Offset: 0x001844F8
        public UseFrostRing(FSM parentFSM, Player parentEntity) : base(UseFrostRing.staticID, parentFSM, parentEntity)
        {
            this.SetAnimTimes(0.1f, 0.2f, 0.3f, 0.4f, 0.75f, 1f);
        }

        // Token: 0x1700075A RID: 1882
        // (get) Token: 0x060034EE RID: 13550 RVA: 0x00186148 File Offset: 0x00184548
        public override string OnEnterAnimStr
        {
            get
            {
                return (!this.parent.isFacingRight) ? this.parent.ForehandAnimStr : this.parent.BackhandAnimStr;
            }
        }

        // Token: 0x060034EF RID: 13551 RVA: 0x00186178 File Offset: 0x00184578
        public override void OnEnter()
        {
            base.OnEnter();
            
            this.shotStarted = false;
            this.shotFinished = false;
            base.SetSkillLevel((!this.IsEmpowered) ? 1 : 2);
            this.parent.movement.moveVector = -this.inputVector;
            this.parent.movement.MoveToMoveVector(18, false);
        }

        // Token: 0x060034F0 RID: 13552 RVA: 0x00186264 File Offset: 0x00184664
        public override void ExecuteSkill()
        {
            if (!this.shotStarted)
            {
                this.shotStarted = true;
                this.shotStopwatchID = ChaosStopwatch.Begin(0f, true, 4f, 1, 0);
                SoundManager.PlayWithDistAndSPR("StandardThrow", this.parent.transform.position, 1f);
                SoundManager.PlayWithDistAndSPR("IceDash", this.parent.transform.position, 1f);
                CameraController.ShakeCamera(0.3f, false);
                FrostFanProjectiles.Clear();
                EndPositions.Clear();
                for (int i = 0; i < 24; i++)
                {
                    Vector2 v = Globals.GetRotatedCircleVector(24, i, this.inputVector);

                    float dist = Vector2.Distance(v.normalized * 0.5f, this.inputVector.normalized*0.5f);
                    float fac = (1 - dist)*0.75f + 0.25f;

                    this.ThrowFrostFanProjectile(this.parent.attackOriginTrans.position, v);
                    EndPositions.Add((Vector2)this.parent.attackOriginTrans.position +  v * ((9 + (i % 2)) * fac * (IsEmpowered ? 1.25f : 1)));
                }

            }
            base.ExecuteSkill();
        }

        // Token: 0x060034F1 RID: 13553 RVA: 0x001863E4 File Offset: 0x001847E4
        public override void FixedUpdate()
        {
            if (!this.shotStarted || this.shotFinished)
            {
                return;
            }

            if (!this.shotFinished)
            {
                for (int i = 0; i < 24; i++)
                {
                    FrostFanProjectile ffp = FrostFanProjectiles[i];
                    if (ffp != null)
                    {
                        ffp.transform.position = Vector2.Lerp(ffp.transform.position, EndPositions[i], Time.fixedDeltaTime * (IsEmpowered ? 6 : 4));
                    }
                }
            }

        }

        // Token: 0x060034F2 RID: 13554 RVA: 0x00186548 File Offset: 0x00184948
        private void ThrowFrostFanProjectile(Vector2 givenPosition, Vector2 givenVector)
        {
            this.currentFP = (FrostFanProjectile)this.CreateProjectile(FrostFanProjectile.Prefab, new Vector3?(givenPosition + givenVector), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null, true);
            FrostFanProjectiles.Add(this.currentFP);
            this.currentFP.lifeTime = 4;
            this.currentFP.flyTime = 4;

            this.FireProjectile(this.currentFP, givenVector*0.03f, true, 0f, 0f, string.Empty, false);
            PoolManager.GetPoolItem<ParticleEffect>("IcicleBreakEffect").Emit(new int?(2), new Vector3?(givenPosition + givenVector), null, null, 0f, null, null);
        }

        public List<FrostFanProjectile> FrostFanProjectiles = new List<FrostFanProjectile>();
        public List<Vector3> EndPositions = new List<Vector3>();
        // Token: 0x040037C8 RID: 14280
        public new static string staticID = "Mythical::UseFrostRing";

        // Token: 0x040037C9 RID: 14281
        private static int projectileCount = 5;

        // Token: 0x040037CA RID: 14282
        private int shotCounter;

        // Token: 0x040037CB RID: 14283
        private int shotStopwatchID;

        // Token: 0x040037CC RID: 14284
        private bool shotStarted;

        // Token: 0x040037CD RID: 14285
        private bool shotFinished;

        // Token: 0x040037CE RID: 14286
        private List<Vector2> targetVecList = new List<Vector2>();

        // Token: 0x040037CF RID: 14287
        private Vector2 perpendOffset;

        // Token: 0x040037D0 RID: 14288
        private Vector2 attackPosition;

        // Token: 0x040037D1 RID: 14289
        private static float moveSpeed = 12f;

        // Token: 0x040037D2 RID: 14290
        private FrostFanProjectile currentFP;

        // Token: 0x040037D3 RID: 14291
        private int ultCounter;

        // Token: 0x040037D4 RID: 14292
        private bool ultFinalShot;
    }
    public class ObsidianDash : Player.BaseDashState
    {
        public ObsidianDash(FSM fsm, Player parentPlayer) : base(ObsidianDash.staticID, fsm, parentPlayer)
        {
            this.isProjectileSkill = true;
            this.isDash = true;
            base.InitChargeSkillSettings(1, 0f, this.skillData, this);
        }

        // Token: 0x06003731 RID: 14129 RVA: 0x0019C1B7 File Offset: 0x0019A5B7
        public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod)
        {
            base.SetEmpowered(givenStatus, givenMod);
            if (this.IsEmpowered)
            {
                this.spawnCount = 5;
                this.spawnOffset = 2;
            }
            else
            {
                this.spawnCount = 3;
                this.spawnOffset = 1;
            }
        }

        // Token: 0x06003732 RID: 14130 RVA: 0x0019C1F0 File Offset: 0x0019A5F0
        public override void OnEnter()
        {
            base.OnEnter();
            if (!this.cooldownReady)
            {
                return;
            }
            this.spawnPos = (Vector2)this.parent.attackOriginTrans.position + this.parent.movement.moveVector.normalized * 0.5f;
            for (int i = 0; i < this.spawnCount; i++)
            {
                Vector2 v = Globals.GetRotatedCircleVector(36, i - this.spawnOffset, this.parent.movement.moveVector.normalized);
                this.CreateEarthCascade(this.spawnPos + v,v,0);
            }
        }
        private void CreateEarthCascade(Vector2 givenPosition, Vector2 givenVector, float givenDelay = 0f)
        {
            this.currentEC = (EarthCascade)this.CreateProjectile(EarthCascade.Prefab, new Vector3?(givenPosition), null, null, true);
            this.currentEC.disableOnHit = !this.IsEmpowered;
            this.FireProjectile(this.currentEC, givenVector, true, 0f, 0f, string.Empty, false);
            this.currentEC.SetReleaseVars(givenVector, givenDelay);
            if (this.isUltimate)
            {
                this.currentEC.disableOnHit = false;
            }
        }

        public virtual Projectile CreateProjectile(GameObject prefab, Vector3? givenPosition = null, Quaternion? givenQuaternion = null, Transform parentTrans = null, bool setAtkInfo = true)
        {
            this.newProjectile = Projectile.CreateProjectile(this.parent, prefab, new Vector3?((givenPosition == null) ? this.parent.attackOriginTrans.position : givenPosition.Value), new Quaternion?((givenQuaternion == null) ? Quaternion.identity : givenQuaternion.Value), parentTrans ?? this.parent.transform.parent);
            if (setAtkInfo)
            {
                this.SetProjectileAtkInfo(this.newProjectile);
            }
            return this.newProjectile;
        }
        public virtual void FireProjectile(Projectile projectile, Vector2 moveVector, bool setKnockbackVector = true, float newMoveSpeed = 0f, float newFlyTime = 0f, string audioString = "", bool setAtkInfo = false)
        {
            projectile.transform.SetParent(null);
            if (setAtkInfo)
            {
                this.SetProjectileAtkInfo(projectile);
            }
            projectile.moveVector = moveVector;
            if (setKnockbackVector)
            {
                projectile.attackBox.knockbackOverwriteVector = moveVector;
            }
            if (newMoveSpeed != 0f)
            {
                projectile.moveSpeed = newMoveSpeed;
            }
            if (newFlyTime != 0f)
            {
                projectile.flyTime = newFlyTime;
                projectile.lifeTime = newFlyTime * 2f;
            }
            projectile.RefreshFlightTime(false);
            if (audioString != string.Empty)
            {
                SoundManager.PlayWithDistAndSPR(audioString, projectile.transform.position, 1f);
            }
        }

        // Token: 0x060031AC RID: 12716 RVA: 0x0016A9BF File Offset: 0x00168DBF
        public virtual void SetProjectileAtkInfo(Projectile projectile)
        {
            projectile.attackBox.SetAttackInfo(this.parent.skillCategory, this.skillID, this.currentLevel, this.isUltimate);
            projectile.UpdateCalculatedDamage(true);
        }

        // Token: 0x04003479 RID: 13433
        private Projectile newProjectile;

        // Token: 0x04003A4D RID: 14925
        public new static string staticID = "Mythical::ObsidianDash";

        // Token: 0x04003A4E RID: 14926
        private Vector2 spawnPos;
        private EarthCascade currentEC;
        // Token: 0x04003A4F RID: 14927
        private EarthDrillProjectile currentDP;

        // Token: 0x04003A50 RID: 14928
        private RockBurstDirectionalEffect currentFX;

        // Token: 0x04003A51 RID: 14929
        private int spawnCount = 3;

        // Token: 0x04003A52 RID: 14930
        private int spawnOffset = 1;
    }

    public class DarkDragonDash : Player.BaseDashState
    {
        public DarkDragonDash(FSM fsm, Player parentPlayer) : base(DarkDragonDash.staticID, fsm, parentPlayer)
        {
            this.isProjectileSkill = true;
            this.isDash = true;
            base.InitChargeSkillSettings(1, 0f, this.skillData, this);
        }
        // Token: 0x06003732 RID: 14130 RVA: 0x0019C1F0 File Offset: 0x0019A5F0
        public override void OnEnter()
        {
            base.OnEnter();
            if (!this.cooldownReady)
            {
                return;
            }
            this.spawnPos = (Vector2)this.parent.attackOriginTrans.position + this.inputVector * 0.5f;
            Vector2 perpen = Globals.GetPerpendicular(this.inputVector);
            FireFireArc(this.spawnPos + perpen, this.inputVector, false, 1.2f);
            FireFireArc(this.spawnPos - perpen, this.inputVector, true, 1.2f);
            
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!this.cooldownReady)
            {
                return;

            }
            this.spawnPos = (Vector2)this.parent.attackOriginTrans.position + this.inputVector * 0.5f;
            Vector2 perpen = Globals.GetPerpendicular(this.inputVector);
            FireFireArc(this.spawnPos + this.inputVector - perpen * 0.5f, this.inputVector, true, 0.7f);
            FireFireArc(this.spawnPos + this.inputVector + perpen * 0.5f, this.inputVector, false, 0.7f);
        }

        private void FireFireArc(Vector2 givenPosition, Vector2 givenVector, bool positiveArc, float fac)
        {
            this.currentFA = (FireArc)this.CreateProjectile(FireArc.Prefab, new Vector3?(givenPosition + UnityEngine.Random.insideUnitCircle * 0.5f), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null, true);
            this.currentFA.targetVector = givenVector;
            this.currentFA.attackBox.knockbackOverwriteVector = givenVector;
            this.currentFA.positiveApproach = positiveArc;
            this.currentFA.arcRange = 4;
            this.currentFA.flyTime *= fac;
            this.currentFA.moveSpeed *= fac;
            this.currentFA.lifeTime *= fac;
            this.currentFA.disableOnHit = true;
            this.currentFA.projectileSpriteRenderer.sprite = SampleSkillLoader.newArc;
            DragonCross.darkArcs.Add(this.currentFA);

            ParticleSystem ps = this.currentFA.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            main.startColor = new Color(0.5f, 0.5f, 0, 0.784f);
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
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
            }, colorOverLifetime.color.gradient.alphaKeys);

            colorOverLifetime.color = grad;

        }

        public virtual Projectile CreateProjectile(GameObject prefab, Vector3? givenPosition = null, Quaternion? givenQuaternion = null, Transform parentTrans = null, bool setAtkInfo = true)
        {
            this.newProjectile = Projectile.CreateProjectile(this.parent, prefab, new Vector3?((givenPosition == null) ? this.parent.attackOriginTrans.position : givenPosition.Value), new Quaternion?((givenQuaternion == null) ? Quaternion.identity : givenQuaternion.Value), parentTrans ?? this.parent.transform.parent);
            if (setAtkInfo)
            {
                this.SetProjectileAtkInfo(this.newProjectile);
            }
            return this.newProjectile;
        }
        public virtual void FireProjectile(Projectile projectile, Vector2 moveVector, bool setKnockbackVector = true, float newMoveSpeed = 0f, float newFlyTime = 0f, string audioString = "", bool setAtkInfo = false)
        {
            projectile.transform.SetParent(null);
            if (setAtkInfo)
            {
                this.SetProjectileAtkInfo(projectile);
            }
            projectile.moveVector = moveVector;
            if (setKnockbackVector)
            {
                projectile.attackBox.knockbackOverwriteVector = moveVector;
            }
            if (newMoveSpeed != 0f)
            {
                projectile.moveSpeed = newMoveSpeed;
            }
            if (newFlyTime != 0f)
            {
                projectile.flyTime = newFlyTime;
                projectile.lifeTime = newFlyTime * 2f;
            }
            projectile.RefreshFlightTime(false);
            if (audioString != string.Empty)
            {
                SoundManager.PlayWithDistAndSPR(audioString, projectile.transform.position, 1f);
            }
        }

        // Token: 0x060031AC RID: 12716 RVA: 0x0016A9BF File Offset: 0x00168DBF
        public virtual void SetProjectileAtkInfo(Projectile projectile)
        {
            projectile.attackBox.SetAttackInfo(this.parent.skillCategory, this.skillID, this.currentLevel, this.isUltimate);
            projectile.UpdateCalculatedDamage(true);
        }

        // Token: 0x04003479 RID: 13433
        private Projectile newProjectile;

        // Token: 0x04003A4D RID: 14925
        public new static string staticID = "Mythical::DarkDragonDash";

        // Token: 0x04003A4E RID: 14926
        private Vector2 spawnPos;

        // Token: 0x04003A4F RID: 14927
        private FireArc currentFA;

        // Token: 0x04003A50 RID: 14928
        private RockBurstDirectionalEffect currentFX;

        // Token: 0x04003A51 RID: 14929
        private int spawnCount = 3;

        // Token: 0x04003A52 RID: 14930
        private int spawnOffset = 1;
    }

    public class LightDragonDash : Player.BaseDashState
    {
        public LightDragonDash(FSM fsm, Player parentPlayer) : base(LightDragonDash.staticID, fsm, parentPlayer)
        {
            this.isProjectileSkill = true;
            this.isDash = true;
            base.InitChargeSkillSettings(1, 0f, this.skillData, this);
        }
        // Token: 0x06003732 RID: 14130 RVA: 0x0019C1F0 File Offset: 0x0019A5F0
        public override void OnEnter()
        {
            base.OnEnter();
            if (!this.cooldownReady)
            {
                return;
            }
            this.spawnPos = (Vector2)this.parent.attackOriginTrans.position + this.inputVector * 0.5f;
            Vector2 perpen = Globals.GetPerpendicular(this.inputVector) * 0.25f;
            FireFireArc(this.spawnPos + perpen, -this.inputVector, false, 1f);
            FireFireArc(this.spawnPos - perpen, -this.inputVector, true, 1f);
            
            
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!this.cooldownReady)
            {
                return;

            }
            this.spawnPos = (Vector2)this.parent.attackOriginTrans.position + this.inputVector * 0.5f;
            Vector2 perpen = Globals.GetPerpendicular(this.inputVector) * 0.5f;
            FireFireArc(this.spawnPos + perpen, this.inputVector, false, 0.5f,1).transform.localScale *= 1.75f;
        }

        private FireArc FireFireArc(Vector2 givenPosition, Vector2 givenVector, bool positiveArc, float fac, int offset = 0)
        {
            fac *= IsEmpowered ? 1.2f : 1;
            this.currentFA = (FireArc)this.CreateProjectile(FireArc.Prefab, new Vector3?(givenPosition + UnityEngine.Random.insideUnitCircle * 0.5f), new Quaternion?(Globals.GetRotationQuaternion(givenVector)), null, true,offset);
            this.currentFA.targetVector = givenVector;
            this.currentFA.attackBox.knockbackOverwriteVector = givenVector;
            this.currentFA.positiveApproach = positiveArc;
            this.currentFA.arcRange = 4;
            this.currentFA.flyTime *= fac;
            this.currentFA.moveSpeed *= fac;
            this.currentFA.lifeTime *= fac;
            this.currentFA.disableOnHit = true;
            this.currentFA.projectileSpriteRenderer.sprite = SampleSkillLoader.newArc2;
            DragonCross.darkArcs.Add(this.currentFA);

            ParticleSystem ps = this.currentFA.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;
            main.startColor = new Color(1f, 1f, 1, 0.784f);
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
            Gradient grad = new Gradient();

            grad.SetKeys(new GradientColorKey[]
            {
                new GradientColorKey()
                {
                    color = new Color(1, 1, 1f, 0.784f),
                    time = 0f
                },
                new GradientColorKey()
                {
                    color = new Color(0.8f, 0.8f, 0.8f, 0.584f),
                    time = 0.5f
                },
                new GradientColorKey()
                {
                    color = new Color(0.7f, 0.7f, 0.7f, 0.384f),
                    time = 1f
                }
            }, colorOverLifetime.color.gradient.alphaKeys);

            colorOverLifetime.color = grad;
            return this.currentFA;
        }

        public virtual Projectile CreateProjectile(GameObject prefab, Vector3? givenPosition = null, Quaternion? givenQuaternion = null, Transform parentTrans = null, bool setAtkInfo = true, int offset = 0)
        {
            this.newProjectile = Projectile.CreateProjectile(this.parent, prefab, new Vector3?((givenPosition == null) ? this.parent.attackOriginTrans.position : givenPosition.Value), new Quaternion?((givenQuaternion == null) ? Quaternion.identity : givenQuaternion.Value), parentTrans ?? this.parent.transform.parent);
            if (setAtkInfo)
            {
                this.SetProjectileAtkInfo(this.newProjectile,offset);
            }
            return this.newProjectile;
        }
        public virtual void FireProjectile(Projectile projectile, Vector2 moveVector, bool setKnockbackVector = true, float newMoveSpeed = 0f, float newFlyTime = 0f, string audioString = "", bool setAtkInfo = false)
        {
            projectile.transform.SetParent(null);
            if (setAtkInfo)
            {
                this.SetProjectileAtkInfo(projectile);
            }
            projectile.moveVector = moveVector;
            if (setKnockbackVector)
            {
                projectile.attackBox.knockbackOverwriteVector = moveVector;
            }
            if (newMoveSpeed != 0f)
            {
                projectile.moveSpeed = newMoveSpeed;
            }
            if (newFlyTime != 0f)
            {
                projectile.flyTime = newFlyTime;
                projectile.lifeTime = newFlyTime * 2f;
            }
            projectile.RefreshFlightTime(false);
            if (audioString != string.Empty)
            {
                SoundManager.PlayWithDistAndSPR(audioString, projectile.transform.position, 1f);
            }
        }

        // Token: 0x060031AC RID: 12716 RVA: 0x0016A9BF File Offset: 0x00168DBF
        public virtual void SetProjectileAtkInfo(Projectile projectile, int offset = 0)
        {
            projectile.attackBox.SetAttackInfo(this.parent.skillCategory, this.skillID, this.currentLevel + offset, this.isUltimate);
            projectile.UpdateCalculatedDamage(true);
        }

        // Token: 0x04003479 RID: 13433
        private Projectile newProjectile;

        // Token: 0x04003A4D RID: 14925
        public new static string staticID = "Mythical::LightDragonDash";

        // Token: 0x04003A4E RID: 14926
        private Vector2 spawnPos;

        // Token: 0x04003A4F RID: 14927
        private FireArc currentFA;

        // Token: 0x04003A50 RID: 14928
        private RockBurstDirectionalEffect currentFX;

        // Token: 0x04003A51 RID: 14929
        private int spawnCount = 3;

        // Token: 0x04003A52 RID: 14930
        private int spawnOffset = 1;
    }
    public class UseEarthCascadeChain : Player.ProjectileChainAttackState
    {
        // Token: 0x06003431 RID: 13361 RVA: 0x0017EDB4 File Offset: 0x0017D1B4
        public UseEarthCascadeChain(FSM parentFSM, Player parentEntity) : base(UseEarthCascadeChain.staticID, parentFSM, parentEntity)
        {
            this.hasSignatureVariant = true;
            this.SetAnimTimes(0.1f, 0.2f, 0.3f, 0.5f, 0.6f, 1f);
            this.shotDelay = 0.15f;
            //this.InitChargeSkillSettings(8, 0, this.skillData, this);
        }

        public override bool IsReady()
        {
            return this.cooldownRef.chargeCount > 3 && base.IsReady();
        }

        // Token: 0x1700074B RID: 1867
        // (get) Token: 0x06003432 RID: 13362 RVA: 0x0017EE00 File Offset: 0x0017D200
        public override string OnEnterAnimStr
        {
            get
            {
                return this.parent.BackhandAnimStr;
            }
        }

        // Token: 0x06003433 RID: 13363 RVA: 0x0017EE0D File Offset: 0x0017D20D
        public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod)
        {
            base.SetEmpowered(givenStatus, givenMod);
            base.SetSkillLevel((!this.IsEmpowered) ? 1 : 2);
        }

        // Token: 0x06003434 RID: 13364 RVA: 0x0017EE30 File Offset: 0x0017D230
        public override void OnEnter()
        {
            base.OnEnter();
            this.targetVector = this.inputVector;
            if (this.isUltimate)
            {
                this.ultStarted = false;
                this.firstAnim = true;
                
                this.perpendVec = Globals.GetPerpendicular(this.targetVector, true) * 0.42f;
            }
        }

        public override void FireChainProjectile()
        {
            base.FireChainProjectile();
            this.targetVector = this.GetInputVector();
            this.perpendVec = Globals.GetPerpendicular(this.inputVector, !this.parent.isFacingRight) * 0.25f;
            this.spawnPosition = (Vector2)this.parent.attackOriginTrans.position + this.inputVector;
            if (this.isUltimate)
            {
                if (!this.ultStarted)
                {
                    this.ultStarted = true;
                    this.ultStopwatchID = ChaosStopwatch.Begin(0f, true, 0.0625f, 15, 0);
                }
                this.SetUltInput();
                if (this.firstAnim)
                {
                    this.firstAnim = false;
                }
                else
                {
                    this.parent.anim.PlayDirectional(this.parent.NextHandAnimStr, -1, this.animExecTime);
                }
                this.CreateEarthCascade(this.spawnPosition, this.targetVector, 0f);
                this.CreateEarthCascade(this.spawnPosition + this.perpendVec, this.targetVector, 0f);
                this.CreateEarthCascade(this.spawnPosition - this.perpendVec, this.targetVector, 0f);
                this.CreateEarthCascade(this.spawnPosition + this.perpendVec*2, this.targetVector, 0f);
                this.CreateEarthCascade(this.spawnPosition - this.perpendVec*2, this.targetVector, 0f);
                this.CreateEarthCascade(this.spawnPosition + this.perpendVec*3, this.targetVector, 0f);
                this.CreateEarthCascade(this.spawnPosition - this.perpendVec*3, this.targetVector, 0f);
                SoundManager.PlayAudioWithDistance("StandardThrow", new Vector2?(this.spawnPosition), null, 24f, -1f, 1f, false);
            }
            else
            {
                if (this.IsEmpowered)
                {
                    this.CreateEarthCascade(this.spawnPosition, this.targetVector, 0f);
                }
                this.CreateEarthCascade(this.spawnPosition + this.perpendVec, this.targetVector, 0f);
                this.CreateEarthCascade(this.spawnPosition - this.perpendVec, this.targetVector, 0f);
                SoundManager.PlayAudioWithDistance("StandardThrow", new Vector2?(this.spawnPosition), null, 24f, -1f, 1f, false);
            }
            //this.cooldownRef.chargeCount--;
        }
        // Token: 0x06003436 RID: 13366 RVA: 0x0017F0E4 File Offset: 0x0017D4E4
        private void SetUltInput()
        {
            this.inputVector = base.GetInputVector(false, true, false);
            if (this.inputVector != Vector2.zero)
            {
                this.targetVector = (this.targetVector + this.inputVector * 0.125f).normalized;
                this.parent.FaceTarget(this.targetVector + (Vector2)this.parent.transform.position, 4, false);
                this.perpendVec = Globals.GetPerpendicular(this.targetVector, true) * 0.75f;
            }
        }

        // Token: 0x06003437 RID: 13367 RVA: 0x0017F190 File Offset: 0x0017D590
        private void CreateEarthCascade(Vector2 givenPosition, Vector2 givenVector, float givenDelay = 0f)
        {
            this.currentEC = (EarthCascade)this.CreateProjectile(EarthCascade.Prefab, new Vector3?(givenPosition), null, null, true);
            this.FireProjectile(this.currentEC, givenVector, true, 0f, 0f, string.Empty, false);
            this.currentEC.SetReleaseVars(givenVector, givenDelay);
            this.currentEC.moveSpeed *= (0.7f + UnityEngine.Random.value * 0.2f);
            if (this.isUltimate)
            {
                this.currentEC.disableOnHit = false;
            }
        }

        // Token: 0x040036FE RID: 14078
        public new static string staticID = "Mythical::UseEarthCascadeChain";

        // Token: 0x040036FF RID: 14079
        private int projectileCount = 5;

        // Token: 0x04003700 RID: 14080
        private Vector2 spawnPosition;

        // Token: 0x04003701 RID: 14081
        private Vector2 targetVector;

        // Token: 0x04003702 RID: 14082
        private Vector2 perpendVec;

        // Token: 0x04003703 RID: 14083
        private EarthCascade currentEC;

        // Token: 0x04003704 RID: 14084
        private int ultStopwatchID;

        // Token: 0x04003705 RID: 14085
        private bool ultStarted;

        // Token: 0x04003706 RID: 14086
        private bool firstAnim;
    }

}
