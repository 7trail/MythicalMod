using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    public class SurfsGambit : Item
    {
        public SurfsGambit()
        {
            this.ID = SurfsGambit.staticID;
            this.category = Item.Category.Offense;
            this.damageMod = new NumVarStatMod(this.ID, 0.3f, 10, VarStatModType.Multiplicative, false);
        }

        // Token: 0x17000504 RID: 1284
        // (get) Token: 0x06002184 RID: 8580 RVA: 0x000FE2BD File Offset: 0x000FC6BD
        public override string ExtraInfo
        {
            get
            {
                return base.PercentToStr(this.damageMod, "+");
            }
        }

        // Token: 0x06002185 RID: 8581 RVA: 0x000FE2D0 File Offset: 0x000FC6D0
        public override void Activate()
        {
            this.SetModStatus(true);
            this.SetParentAsPlayer();
            Player p = this.parentPlayer;
            p.RemoveSkill(p.assignedSkills[0]);
        }

        // Token: 0x06002186 RID: 8582 RVA: 0x000FE2D9 File Offset: 0x000FC6D9
        public override void Deactivate()
        {
            this.SetModStatus(false);
        }

        // Token: 0x06002187 RID: 8583 RVA: 0x000FE2E2 File Offset: 0x000FC6E2
        public virtual void SetModStatus(bool givenStatus)
        {
            StatManager.ModifyAllStatData(this.damageMod, this.parentSkillCategory, StatData.damageStr, new StatManager.ModApplyConditional(base.IgnoreStatusConditional), givenStatus);
        }

        // Token: 0x040027E5 RID: 10213
        public static string staticID = "SurfsGambit";

        // Token: 0x040027E6 RID: 10214
        protected NumVarStatMod damageMod;
    }

    public class BlinkModule : Item
    {
        public BlinkModule()
        {
            this.ID = BlinkModule.staticID;
            this.category = Item.Category.Defense;
            //this.damageMod = new NumVarStatMod(this.ID, 0.3f, 10, VarStatModType.Multiplicative, false);
        }

        // Token: 0x17000504 RID: 1284
        // (get) Token: 0x06002184 RID: 8580 RVA: 0x000FE2BD File Offset: 0x000FC6BD

        // Token: 0x06002185 RID: 8581 RVA: 0x000FE2D0 File Offset: 0x000FC6D0
        public override void Activate()
        {
            this.SetParentAsPlayer();
            this.SetModStatus(true);

            

        }

        // Token: 0x06002186 RID: 8582 RVA: 0x000FE2D9 File Offset: 0x000FC6D9
        public override void Deactivate()
        {
            this.SetParentAsPlayer();
            this.SetModStatus(false);
        }

        // Token: 0x06002187 RID: 8583 RVA: 0x000FE2E2 File Offset: 0x000FC6E2
        public virtual void SetModStatus(bool givenStatus)
        {
            Player p = this.parentPlayer;
            for(int i = 0; i < 6; i++)
            {
                if (p.assignedSkills[i] is Player.BaseDashState)
                {
                    ((Player.BaseDashState)p.assignedSkills[i]).dashDuration *= givenStatus ? 1.25f : 0.8f;
                }
            }
        }

        // Token: 0x040027E5 RID: 10213
        public static string staticID = "BlinkModule";

        // Token: 0x040027E6 RID: 10214
        //protected NumVarStatMod damageMod;
    }

    public class PetSquid : OnTakeDamageItem
    {
        public PetSquid()
        {
            /*this.currentBP = (BubbleProjectile)Projectile.CreateProjectile(this.parent, BubbleProjectile.Prefab, new Vector3?(this.spawnPosition + givenVector * 0.5f), null, null);
            this.currentBP.isLarge = this.IsEmpowered;
            base.SetAttackInfo(this.currentBP.attack);
            this.currentBP.moveVector = givenVector;
            this.currentBP.moveSpeed = UnityEngine.Random.Range(3f, 5f);
            this.currentBP.flyTime = UnityEngine.Random.Range(1f, 1.375f);
            SoundManager.PlayWithDistAndSPR("BubblePop", this.spawnPosition, 1f);*/

            this.ID = PetSquid.staticID;
            this.category = Item.Category.Defense;
            //this.damageMod = new NumVarStatMod(this.ID, 0.3f, 10, VarStatModType.Multiplicative, false);
        }

        // Token: 0x17000504 RID: 1284
        // (get) Token: 0x06002184 RID: 8580 RVA: 0x000FE2BD File Offset: 0x000FC6BD

        // Token: 0x06002185 RID: 8581 RVA: 0x000FE2D0 File Offset: 0x000FC6D0
        public override void Activate()
        {
            this.SetParentAsPlayer();
            base.Activate();
            this.itemHeld = true;
            this.SetItemBarStatus();
        }

        // Token: 0x06002551 RID: 9553 RVA: 0x0010F708 File Offset: 0x0010DB08
        public override void Deactivate()
        {
            this.SetParentAsPlayer();
            base.Deactivate();
            this.itemHeld = false;
            this.SetItemBarStatus();
        }

        // Token: 0x06002552 RID: 9554 RVA: 0x0010F720 File Offset: 0x0010DB20
        private void SetItemBarStatus()
        {
            if (!this.itemHeld)
            {
                this.RemoveFromItemBar();
            }
            else if (this.durationStopwatch.IsRunning)
            {
                this.UpdateItemBar(ItemStatusBar.ItemState.Active);
            }
            else if (this.cdStopwatch.IsRunning)
            {
                this.UpdateItemBar(ItemStatusBar.ItemState.Disabled);
            }
            else
            {
                this.UpdateItemBar(ItemStatusBar.ItemState.Ready);
            }
        }

        // Token: 0x06002553 RID: 9555 RVA: 0x0010F784 File Offset: 0x0010DB84
        public override void OnTakeDamageActual(AttackInfo givenAtkInfo, Entity givenEnt)
        {
            if (this.cdStopwatch.IsRunning || this.parentPlayer == null || givenAtkInfo == null || givenAtkInfo.damage <= 0)
            {
                return;
            }
            if (!this.hurtStopwatch.IsRunning)
            {
                this.hurtCounter = 1;
                this.hurtStopwatch.IsRunning = true;
                return;
            }
            this.hurtCounter++;
            if (this.hurtCounter < this.activateHurtCount)
            {
                this.hurtStopwatch.IsRunning = true;
                return;
            }
            this.hurtCounter = 0;
            this.hurtStopwatch.IsRunning = false;
            //this.parentPlayer.health.SetInvulnerabilityDuration(this.durationStopwatch.Delay, true);
            for(int i = 0; i < 6; i++)
            {
                this.currentBP = (BubbleProjectile)Projectile.CreateProjectile(this.parentPlayer, BubbleProjectile.Prefab, new Vector3?(this.spawnPosition), null, null);;
                this.currentBP.moveVector = new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f,0).normalized;
                this.currentBP.moveSpeed = UnityEngine.Random.Range(3f, 5f);
                this.currentBP.flyTime = UnityEngine.Random.Range(1f, 1.375f);
                SoundManager.PlayWithDistAndSPR("BubblePop", this.spawnPosition, 1f);
            }
            this.parentPlayer.StartCoroutine(this.StartTimers());
        }

        private Vector2 spawnPosition;

        // Token: 0x040033D0 RID: 13264
        private BubbleProjectile currentBP;

        // Token: 0x040033D1 RID: 13265
        private WaterDropEmitter dropEmitter;

        // Token: 0x06002554 RID: 9556 RVA: 0x0010F858 File Offset: 0x0010DC58
        private IEnumerator StartTimers()
        {
            this.durationStopwatch.IsRunning = true;
            this.cdStopwatch.IsRunning = true;
            this.SetItemBarStatus();
            while (this.durationStopwatch.IsRunning)
            {
                yield return null;
            }
            this.SetItemBarStatus();
            while (this.cdStopwatch.IsRunning)
            {
                yield return null;
            }
            this.SetItemBarStatus();
            yield break;
        }
        // Token: 0x04002A00 RID: 10752
        private int hurtCounter;

        // Token: 0x04002A01 RID: 10753
        private int activateHurtCount = 3;

        // Token: 0x04002A02 RID: 10754
        private ChaosQuickStopwatch hurtStopwatch = new ChaosQuickStopwatch(3f);

        // Token: 0x04002A03 RID: 10755
        private ChaosQuickStopwatch durationStopwatch = new ChaosQuickStopwatch(3f);

        // Token: 0x04002A04 RID: 10756
        private ChaosQuickStopwatch cdStopwatch = new ChaosQuickStopwatch(30f);

        // Token: 0x04002A05 RID: 10757
        private bool itemHeld;

        // Token: 0x040027E5 RID: 10213
        public static string staticID = "PetSquid";

        // Token: 0x040027E6 RID: 10214
        //protected NumVarStatMod damageMod;
    }
}
