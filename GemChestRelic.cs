using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mythical
{
    class GemChestRelic : RandomChestItem
    {
        public GemChestRelic() : base(GemChestRelic.staticID)
        {
            this.ID = staticID;
            this.category = Category.Misc;
        }
        public override void DropItem(Vector2 givenPosition, int givenAmount)
        {
            base.DropItem(givenPosition, givenAmount);
            LootManager.DropPlat(givenPosition, givenAmount * 3);
        }

        public static string staticID = "GemChestRelic";
    }
}
