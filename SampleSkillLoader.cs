using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Mythical
{
    class SampleSkillLoader
    {
        public static void Awake()
        {
            

            Skills.SkillInfo skillInfo = new Skills.SkillInfo("PleaseWork");
            skillInfo.ID = "AirChannelDashNew";
            skillInfo.description = "A modified Channel Dash!";
            skillInfo.cooldown = 2;
            skillInfo.chargeCooldown = 0;
            skillInfo.startingCharges = 3;
            skillInfo.newState = typeof(AirChannelDashGood);
            skillInfo.attackInfo = LoadFromEmbeddedJson<AttackInfo>("AttackInfo1.json");
            skillInfo.elementType = ElementType.Air;
            skillInfo.isNewSkill = true;

            Skills.Register(skillInfo);


        }

        public static T LoadFromEmbeddedJson<T>(string jsn)
        {

            string jsnstring;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Mythical.{jsn}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                jsnstring = reader.ReadToEnd();
            }

            return JsonUtility.FromJson<T>(jsnstring);
        }
    }
}
