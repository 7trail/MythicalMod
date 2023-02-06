using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            this.SetModStatus(true);
            
        }

        // Token: 0x06002186 RID: 8582 RVA: 0x000FE2D9 File Offset: 0x000FC6D9
        public override void Deactivate()
        {
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
}
