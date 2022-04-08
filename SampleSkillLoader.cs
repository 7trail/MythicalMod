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
            

            Skills.SkillInfo skillInfo = new Skills.SkillInfo("Fire Storm");
            skillInfo.ID = "Mythical::FireStorm";
            skillInfo.description = "Create an explosion where you dash!";
            skillInfo.empowered = "Create an explosion where you land as well!";
            skillInfo.cooldown = 2;
            skillInfo.chargeCooldown = 0;
            skillInfo.startingCharges = 2;
            skillInfo.newState = typeof(AirChannelDashGood);
            skillInfo.isNewSkill = true;
            skillInfo.skillIcon = Extensions.loadSprite("firestorm");
            skillInfo.attackInfo = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfo1.json");
            skillInfo.data = Utils.LoadFromEmbeddedJson<SkillStats>("StatData1.json");
            skillInfo.elementType = ElementType.Fire;

            Skills.Register(skillInfo);

            skillInfo = new Skills.SkillInfo("Evading Zephyr");
            skillInfo.ID = "UseWindDefense";
            skillInfo.description = "After a slight delay, summon a current of air that causes all incoming attacks to miss while you are moving or using basic arcana!";
            skillInfo.empowered = "You can move over pits and move faster!";
            skillInfo.newState = typeof(ZephyrNerf);
            skillInfo.isNewSkill =false;
            skillInfo.attackInfo = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfo1.json");
            //skillInfo.data = Utils.LoadFromEmbeddedJson<SkillStats>("StatData1.json");
            skillInfo.elementType = ElementType.Air;

            Skills.Register(skillInfo);
        }

        
    }
}


public static class Utils
{
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
