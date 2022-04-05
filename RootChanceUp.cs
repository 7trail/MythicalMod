using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
    class RootChanceUp : ElementalStatusUp
    {
        public RootChanceUp() : base(RootChanceUp.staticID,ElementType.Plant,0.05f)
        {
            this.statStr = StatData.rootChStr;
        }

        public static string staticID = "RootChanceUp";
    }
}
