using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
    class MonsterTooth : Item
    {
        public int healthGained = 3;
        public static string staticID = "Monster Tooth";
        public MonsterTooth()
        {
            this.category = Category.Offense;
            this.ID = staticID;    
            
        }

        public override void Activate()
        {
            base.Activate();
            On.Entity.OnDestroy += Entity_OnDestroy;
        }
        public override void Deactivate()
        {
            base.Deactivate();
            On.Entity.OnDestroy -= Entity_OnDestroy;
        }

        public void Entity_OnDestroy(On.Entity.orig_OnDestroy orig, Entity self)
        {
            orig(self);
            this.parentEntity.health.RestoreHealth(healthGained,true,true,false,true);
        }
    }
}
