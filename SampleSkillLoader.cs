using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using LegendAPI;

namespace Mythical
{
    class SampleSkillLoader
    {
        public static void Awake()
        {
            //ElementType element = Elements.Register(newElement);
            //Debug.Log("New element name: " + element.ToString());

            ElementInfo newElement = new ElementInfo();
            newElement.name = "Radiant";
            newElement.color = Color.yellow;
            newElement.weakTo = new List<ElementType>() { ElementType.Chaos};
            newElement.icon = ImgHandler.LoadSprite("radiant");
            newElement.iconInk = newElement.icon;
            newElement.impactAudioID = "ImpactShield";
            ElementType element = Elements.Register(newElement);


            Skills.SkillInfo skillInfo = new Skills.SkillInfo("Zeal's Retribution");
            skillInfo.ID = "Mythical::RadiantStorm";
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
            skillInfo.elementType = element;

            Skills.Register(skillInfo);

            skillInfo = new Skills.SkillInfo("Atlas's Magnus Opus");
            skillInfo.ID = "Mythical::UseRadiantBlast";
            skillInfo.description = "A god's best work should never go unused.";
            skillInfo.empowered = "Poison is more powerful!";
            skillInfo.cooldown = 7;
            skillInfo.chargeCooldown = 0.25f;
            skillInfo.startingCharges = 1;
            skillInfo.newState = typeof(UsePoisonBlastState);
            skillInfo.isNewSkill = true;
            skillInfo.skillIcon = Extensions.loadSprite("poisonBlast");
            skillInfo.attackInfo = null;
            //skillInfo.attackInfo = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfo2.json");
            skillInfo.data = Utils.LoadFromEmbeddedJson<SkillStats>("StatData2.json");
            skillInfo.elementType = element;

            Skills.Register(skillInfo);
            
            skillInfo = new Skills.SkillInfo("Poison Beam");
            skillInfo.ID = "Mythical::UseRadiantBeam";
            skillInfo.description = "Terrarias, is that you?";
            skillInfo.empowered = "Poison is more powerful!";
            skillInfo.cooldown = 0.5f;
            skillInfo.chargeCooldown = 0;
            skillInfo.startingCharges = 5;
            skillInfo.newState = typeof(UsePoisonBeam);
            skillInfo.isNewSkill = true;
            skillInfo.atkChanges = true;
            skillInfo.skillIcon = Extensions.loadSprite("poisonBeam");
            //skillInfo.attackInfo = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfo1.json");
            skillInfo.data = Utils.LoadFromEmbeddedJson<SkillStats>("StatData3.json");
            skillInfo.elementType = element;

            Skills.Register(skillInfo);

            skillInfo = new Skills.SkillInfo("Radiant Dash");
            skillInfo.ID = "RadiantDash";
            skillInfo.description = "Sura would kill for a dash like this.";
            skillInfo.empowered = "Go even farther!";
            skillInfo.cooldown = 0;
            skillInfo.chargeCooldown = 0;
            skillInfo.startingCharges = 1;
            skillInfo.newState = typeof(RadiantDashState);
            skillInfo.isNewSkill = true;
            skillInfo.atkChanges = true;
            skillInfo.skillIcon = Extensions.loadSprite("radiantDash");
            //skillInfo.attackInfo = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfo1.json");
            skillInfo.data = Utils.LoadFromEmbeddedJson<SkillStats>("RadiantDash.json");
            skillInfo.elementType = element;

            Skills.Register(skillInfo);

            /*skillInfo = new Skills.SkillInfo("Evading Zephyr");
            skillInfo.ID = "UseWindDefense";
            skillInfo.description = "After a slight delay, summon a current of air that causes all incoming attacks to miss while you are moving or using basic arcana!";
            skillInfo.empowered = "You can move over pits and move faster!";
            skillInfo.newState = typeof(ZephyrNerf);
            skillInfo.isNewSkill =false;
            skillInfo.atkChanges = false;
            skillInfo.startingCharges = 1;
            skillInfo.chargeCooldown = 0;
            //skillInfo.attackInfo = Utils.LoadFromEmbeddedJson<AttackInfo>("AttackInfo1.json");
            //skillInfo.data = Utils.LoadFromEmbeddedJson<SkillStats>("StatData1.json");
            skillInfo.elementType = ElementType.Air;

            Skills.Register(skillInfo);*/
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
