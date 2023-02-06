using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
    public class StunDown : Item
    {
        // Token: 0x06002292 RID: 8850 RVA: 0x0010280C File Offset: 0x00100C0C
        public StunDown()
        {
            this.ID = StunDown.staticID;
            this.category = Item.Category.Offense;
            this.damageMod = new NumVarStatMod(this.ID, 0.2f, 10, VarStatModType.Multiplicative, false);
        }

        // Token: 0x06002293 RID: 8851 RVA: 0x0010288F File Offset: 0x00100C8F
        public override void Activate()
        {
            this.SetModStatus(true);
        }

        // Token: 0x06002294 RID: 8852 RVA: 0x00102898 File Offset: 0x00100C98
        public override void Deactivate()
        {
            this.SetModStatus(false);
        }

        // Token: 0x06002295 RID: 8853 RVA: 0x001028A4 File Offset: 0x00100CA4
        private void SetModStatus(bool givenStatus)
        {
            if (this.parentEntity == null)
            {
                return;
            }
            this.parentEntity.health.damageTakenStat.Modify(this.damageMod, givenStatus);
            //StatManager.ModifyAllStatData(this.modDict, this.parentSkillCategory, new StatManager.ModApplyConditional(base.IgnoreStatusConditional), givenStatus);
        }

        // Token: 0x04002863 RID: 10339
        public static string staticID = "Mythical::StunDown";

        public override string ExtraInfo  {
            get
            {
                return base.PercentToStr(0.25f, "-") + " | " + base.PercentToStr(0.2f, "+");
            }
        }

        // Token: 0x04002864 RID: 10340
        private NumVarStatMod damageMod;

    }

}
