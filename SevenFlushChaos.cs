using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{

    // Token: 0x0200065F RID: 1631
    public class SevenFlushChaos : SevenFlush
    {
        // Token: 0x060028A8 RID: 10408 RVA: 0x0011DC60 File Offset: 0x0011C060
        public SevenFlushChaos() : base(SevenFlushChaos.staticID, ElementType.Chaos)
        {
            this.category = Item.Category.Offense;
            this.dmgMod = new NumVarStatMod(SevenFlushChaos.staticID, 0.5f, this.modPriority, VarStatModType.Multiplicative, false);
            
        }

        // Token: 0x170005AD RID: 1453
        // (get) Token: 0x060028A9 RID: 10409 RVA: 0x0011DD0B File Offset: 0x0011C10B
        public override string ExtraInfo
        {
            get
            {
                return base.PercentToStr(0.5f, "+");
            }
        }

        // Token: 0x060028AA RID: 10410 RVA: 0x0011DD20 File Offset: 0x0011C120
        public override void ApplyMods(bool givenStatus)
        {
            StatManager.ModifyAllStatData(this.dmgMod, this.parentSkillCategory, StatData.damageStr, new StatManager.ModApplyConditional(base.IgnoreStatusConditional), givenStatus);

        }

        // Token: 0x04002C17 RID: 11287
        public static string staticID = "Mythical::SevenFlushChaos";

        // Token: 0x04002C18 RID: 11288
        private NumVarStatMod dmgMod;

    }

}
