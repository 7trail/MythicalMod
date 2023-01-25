using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
	class PrimeTicket : Item
	{
		public PrimeTicket()
		{
			this.category = global::Item.Category.Misc;
			this.ID = staticID;
			//this.damageMod = new global::NumVarStatMod(this.ID, -0.3f, 10, VarStatModType.Multiplicative, false);
		}


		// Token: 0x0600000E RID: 14
		public override void Activate()
		{
			this.SetModStatus(true);
		}

		// Token: 0x0600000F RID: 15
		public override void Deactivate()
		{
			this.SetModStatus(false);
		}

		// Token: 0x06000010 RID: 16
		public virtual void SetModStatus(bool givenStatus)
		{
			

		}

		
		// Token: 0x06000019 RID: 25


		// Token: 0x04000003 RID: 3
		public static string staticID = "PrimeTicket";


	}
}
