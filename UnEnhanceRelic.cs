using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
    class UnEnhanceRelic : Item
    {
        public UnEnhanceRelic()
        {
            this.category = Category.Misc;
            //this.isCursed = true;
            this.ID = staticID;
            emp = new BoolVarStatMod(staticID, true, 10);
            cooldownReduction = new NumVarStatMod(staticID, -0.4f, 10, VarStatModType.Multiplicative, false, false);
        }

        public override void Activate()
        {
            this.SetModStatus(true);
        }


        // Token: 0x0600000F RID: 15
        public override void Deactivate()
        {
            this.SetModStatus(false);
        }

        public virtual void SetModStatus(bool givenStatus)
        {
            //global::StatManager.ModifyAllStatData(this.damageMod, this.parentSkillCategory, global::StatData.damageStr, new global::StatManager.ModApplyConditional(base.IgnoreStatusConditional), givenStatus);
            if (givenStatus)
            {
                foreach(Player.SkillState skill in this.parentPlayer.assignedSkills)
                {
                    if (skill != null)
                    {
                        skill.SetEmpowered(false, emp);
                    }
                }
            }
            StatManager.ModifyAllStatData(cooldownReduction, this.parentSkillCategory, "cooldown", new StatManager.ModApplyConditional(this.Conditional), givenStatus);
            GameUI.RefreshCDUI();
        }
        public bool Conditional(StatData data)
        {
            return true;
        }
        public override string ExtraInfo
        {
            get
            {
                return base.PercentToStr(0.4f, "-");
            }
        }

        public static BoolVarStatMod emp;
        public static NumVarStatMod cooldownReduction;
        public static string staticID = "unenhanceRelic";
    }
}
