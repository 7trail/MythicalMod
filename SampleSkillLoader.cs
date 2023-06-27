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
            newElement.weakTo = new List<ElementType>() { ElementType.Chaos };
            newElement.icon = ImgHandler.LoadSprite("radiant");
            newElement.iconInk = newElement.icon;
            newElement.impactAudioID = "ImpactShield";
            //ElementType element = Elements.Register(newElement);


            SkillInfo skillInfo = new SkillInfo() {
                DisplayName = "Zeal's Signature Finisher",
                ID = "Mythical::RadiantStorm",
                Description = "Create an explosion where you dash!",
                EnhancedDescription = "Create an explosion where you land as well!",
                StateType = typeof(FireBurstDash),
                Sprite = Extensions.loadSprite("firestorm"),
                SkillStats = Utils.LoadFromEmbeddedJson<SkillStats>("StatData1.json"),
                Element = ElementType.Fire
            };

            Skills.Register(skillInfo);

            skillInfo = new SkillInfo() {
                DisplayName = "Atlas's Magnum Opus",
                ID = "Mythical::UseRadiantBlast",
                Description = "Shoot forth a burst of poisonous air!",
                EnhancedDescription = "Poison is more powerful!",
                StateType = typeof(UsePoisonBlastState),
                Sprite = Extensions.loadSprite("poisonBlast"),
                SkillStats = Utils.LoadFromEmbeddedJson<SkillStats>("StatData2.json"),
                Element = ElementType.Earth
            };
            Skills.Register(skillInfo);

            skillInfo = new SkillInfo() {
                DisplayName = "Poison Beam",
                ID = "Mythical::UseRadiantBeam",
                Description = "Fire a deadly beam of poison!",
                EnhancedDescription = "Poison is more powerful!",
                StateType = typeof(UsePoisonBeam),
                Sprite = Extensions.loadSprite("poisonBeam"),
                SkillStats = Utils.LoadFromEmbeddedJson<SkillStats>("StatData3.json"),
                Element = ElementType.Earth
            };

            Skills.Register(skillInfo);

            SkillInfo NewSkill = new SkillInfo()
            {
                ID = "AirChannelDashGood",
                DisplayName = "Gust Burst",
                Description = "Dash forward with such force that enemies in the area are pulled into your wake! (Revived through SpellsAPI)",
                EnhancedDescription = "Creates a secondary burst on landing!",
                tier = 1,
                StateType = typeof(AirChannelDashGood),
                SkillStats = Utils.LoadFromEmbeddedJson<SkillStats>("AirChannelDashGoodSkillStats.json"),
                Element = ElementType.Air,
                Sprite = ImgHandler.LoadSprite("AirChannelDashGood")
            };
            Skills.Register(NewSkill);

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
