using BepInEx;
using BepInEx.Configuration;
using LegendAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XUnity.ResourceRedirector;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Mythical
{
    public static class SDI
    {
        public static Dictionary<Player, float> SDIValues = new Dictionary<Player, float>();
        public static float SDIIncrement = 0.5f;

        public static float GetSDI(Player player)
        {
            if (SDIValues.ContainsKey(player))
            {
                return SDIValues[player];
            }
            //This code won't be executed unless it doesn't contain the key thanks to return statements
            SDIValues[player] = 0;
            return 0;
        }

        public static void SetSDI(Player player, float value)
        {
            SDIValues[player] = value;
        }
        public static void Init()
        {
            On.Entity.HurtState.ExitTransition += HurtState_ExitTransition;
            On.Entity.ApplyKnockback += Entity_ApplyKnockback;
        }

        private static bool Entity_ApplyKnockback(On.Entity.orig_ApplyKnockback orig, Entity self, AttackInfo atkInfo)
        {
            bool b= orig(self, atkInfo);
            if (self is Player) {
                Player p = self as Player;
                if (!p.rigidbody2D.isKinematic && p.rigidbody2D.bodyType != RigidbodyType2D.Kinematic)
                {
                    float value = GetSDI(p);
                    value += SDIIncrement;
                    SetSDI(p, value);
                    p.rigidbody2D.velocity = atkInfo.knockbackVector + (value * p.GetInputVector());
                }
            }
            return b;
        }

        private static void HurtState_ExitTransition(On.Entity.HurtState.orig_ExitTransition orig, Entity.HurtState self)
        {
            orig(self);
            if (self.parent && self.parent is Player)
            {
                //Reset SDI
                if (SDIValues.ContainsKey((self.parent as Player)))
                    SDIValues.Remove((self.parent as Player));  
            }
        }
    }
}
