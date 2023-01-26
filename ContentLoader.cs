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
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using XUnity.ResourceRedirector;
namespace Mythical {

    #region BepInPlugin Notes
    // The BepInPlugin attribute in [brackets] is setting parameters for bepin to load your mod

    // GUID: "TheTimeSweeper.CameraModExample"
    // The the identifier for your mod. This one simply follows the convention of "AuthorName.YourModName"

    // Name: "CameraModExample"
    // The the full human-readable name of your mod.

    // Version: "0.1.0"
    // The Version, as you'd expect, useful for differing between updates.
    //     Customary to follow Semantic Versioning (major.minor.patch). 
    //         You don't have to, but you'll just look silly in front of everyone. It's ok. I won't make fun of you.
    #endregion
    [BepInPlugin("Amber.TournamentEdition", "Tournament Edition", "2.1.0")]
    
    public class ContentLoader : BaseUnityPlugin {
        #region BaseUnityPlugin Notes
        // BaseUnityPlugin is the main class that gets loaded by bepin.
        // It inherits from MonoBehaviour, so it gains all the familiar Unity callback functions you can use: 
        //     Awake, Start, Update, FixedUpdate, etc.

        //     Awake is most important. it's basically where we initialize everything we do

        //     For further reading, you can check out https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

        // now, close these two Notes regions so the script looks little nicer to work with 
        #endregion
        public static BepInEx.Configuration.ConfigEntry<int> configContestantCount;
        public static BepInEx.Configuration.ConfigEntry<bool> enableTicket;
        //----------------
        public static List<Texture2D> palettes = new List<Texture2D>();
        public static List<string> bannedArcana = new List<string>();
        public static Dictionary<string, Texture2D> particles = new Dictionary<string, Texture2D>();
        public static Dictionary<string, HeadgearDef> headgears = new Dictionary<string, HeadgearDef>();
        public static Dictionary<string, UnityEngine.Color> trails = new Dictionary<string, UnityEngine.Color>();
        public static Dictionary<string, UnityEngine.Color> trails2 = new Dictionary<string, UnityEngine.Color>();

        public static void RegisterTrail(string id, UnityEngine.Color clr1, UnityEngine.Color clr2)
        {
            trails[id] = clr1;
            if (clr2 != null)
            {
                trails2[id] = clr2;
            }
        }


        public List<Sprite> titleScreens = new List<Sprite>();
        public bool hasAddedTitleCards;
        public static bool ChaosDrops = false;
        public static bool UseBanlist = false;
        public static bool FreezeStartPositions = false;
        // This Awake() function will run at the very start when the mod is initialized

        public Sprite cherrySprite;
        public Sprite basePalette;
        public Sprite orangeSprite;

        void CreateConfig()
        {
            configContestantCount =
                Config.Bind<int>("Tournament Edition",
                                 "Contestants",
                                 6,
                                 "How many Contestants you want to spawn. Defaults to 6.");
            enableTicket =
                Config.Bind<bool>("Tournament Edition",
                                 "???",
                                 false,
                                 "WITH EVERYONE THAT FALLS, A MESSAGE IN THEIR WAKE");
        }

        public int nextAssignableID = 32;

        public List<string> robeNames = new List<string>();


        public int AssignNewID(string file)
        {
            robeNames.Add(file);

            nextAssignableID += 1;
            return nextAssignableID-1;
        }

        public int AssignNewIDAlt(byte[] bytes)
        {

            palettes.Add(ImgHandler.LoadTex2D("", T2D: ImgHandler.LoadPNGAlt(bytes)));


            nextAssignableID += 1;
            return nextAssignableID - 1;
        }


        void Awake() {

            //Skills.Awake();
            //SampleSkillLoader.Awake();
            //UnityEngine.Texture2D img = ImgHandler.LoadTex2D("icon");
            //WindowIconTools.SetIcon(img.GetRawTextureData(), img.width, img.height, WindowIconKind.Big);
            //Screen.SetResolution(1200, 675, false);

            // LETS FUCKING GO
            CreateConfig();
            ContestantChanges.Init();
            UltraCouncilChallenge.Init();
            
            basePalette = ImgHandler.LoadSprite("Base");
            newPlayerSprite = ImgHandler.LoadTex2D("Walter2");
            cherrySprite = ImgHandler.LoadSprite("tree1", new Vector2(0.5f,0.2f));
            orangeSprite = ImgHandler.LoadSprite("tree2", new Vector2(0.5f, 0.2f));
            Debug.Log("Cherry Blossom Tree sprite from https://opengameart.org/content/lpc-plant-repack. Cropped to one singular tree.");
            Debug.Log("Cherry Orange Tree sprite from https://opengameart.org/content/lpc-orange-trees. Cropped to one singular tree.");

            OutfitInfo sev = new OutfitInfo();
            sev.name = "Replica";
            sev.outfit = new Outfit("Mythical::Replica", 0, new List<OutfitModStat>
                {
                    new OutfitModStat(OutfitModStat.OutfitModType.Health, 0f, 0.05f, 0f, false),
                    new OutfitModStat(OutfitModStat.OutfitModType.Speed, 0f, 0.1f, 0f, false),
                    new OutfitModStat(OutfitModStat.OutfitModType.Evade, 0.05f, 0f, 0f, false),
                    new OutfitModStat(OutfitModStat.OutfitModType.Cooldown, 0f, -0.1f, 0f, false)
                },false,false);
            Outfits.Register(sev);

            OutfitInfo outfitInfo = new OutfitInfo();
            outfitInfo.name = "Walter";
            outfitInfo.outfit = new global::Outfit("Mythical::Walter", AssignNewID("walter"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo.customDesc = ((bool b) => "The Living Menace.");
            outfitInfo.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            particles.Add("Mythical::Walter", ImgHandler.LoadTex2D("walterParticle"));
            Outfits.Register(outfitInfo);

            OutfitInfo outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Guardian";
            outfitInfo2.outfit = new global::Outfit("Mythical::Guardian", AssignNewID("guardian"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "The Defender");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            OutfitInfo outfitInfo7 = new OutfitInfo();
            outfitInfo7.name = "Scholar";
            outfitInfo7.outfit = new global::Outfit("Mythical::Scholar", AssignNewID("scholar"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo7.customDesc = ((bool b) => "The Keeper Of Knowledge");
            outfitInfo7.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo7);

            OutfitInfo outfitInfo6 = new OutfitInfo();
            outfitInfo6.name = "Fear";
            outfitInfo6.outfit = new global::Outfit("Mythical::Fear", AssignNewID("fear"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo6.customDesc = ((bool b) => "The Everlasting Terror");
            outfitInfo6.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo6);

            OutfitInfo outfitInfo5 = new OutfitInfo();
            outfitInfo5.name = "Conquest";
            outfitInfo5.outfit = new global::Outfit("Mythical::Conquest", AssignNewID("conquest"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo5.customDesc = ((bool b) => "The Iron Fist");
            outfitInfo5.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo5);

            

            OutfitInfo outfitInfo4 = new OutfitInfo();
            outfitInfo4.name = "Tycoon";  
            outfitInfo4.outfit = new global::Outfit("Mythical::Tycoon", AssignNewID("tycoon"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo4.customDesc = ((bool b) => "The Invisible Hand");
            outfitInfo4.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            particles.Add("Mythical::Tycoon", ImgHandler.LoadTex2D("tycoonParticle"));
            Outfits.Register(outfitInfo4);

            OutfitInfo outfitInfo3 = new OutfitInfo();
            outfitInfo3.name = "Surf";
            outfitInfo3.outfit = new global::Outfit("Mythical::Surf", AssignNewID("surf"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo3.customDesc = ((bool b) => "The Challenger");
            outfitInfo3.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo3);
            RegisterTrail("Mythical::Surf", new UnityEngine.Color(0.75f, 1f, 1, 0.8f), new UnityEngine.Color(0.75f, 1f, 1, 0.3f));

            OutfitInfo outfitInfo8 = new OutfitInfo();
            outfitInfo8.name = "Terror";
            outfitInfo8.outfit = new global::Outfit("Mythical::Terror", AssignNewID("terror"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo8.customDesc = ((bool b) => "The Worst Of The Best");
            outfitInfo8.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo8);

            // New ones
            OutfitInfo outfitInfo9 = new OutfitInfo();
            outfitInfo9.name = "Vision";
            outfitInfo9.outfit = new global::Outfit("Mythical::Vision", AssignNewID("vision"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo9.customDesc = ((bool b) => "The All Seeing Eye");
            outfitInfo9.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo9);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Archaic";
            outfitInfo2.outfit = new global::Outfit("Mythical::Archaic", AssignNewID("archaic"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "From Days Past - only_going_up_fr0m_here");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo = new OutfitInfo();
            outfitInfo.name = "Crimson";
            outfitInfo.outfit = new global::Outfit("Mythical::Crimson", AssignNewID("crimson"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, true, false);
            outfitInfo.customDesc = ((bool b) => "Designed by only_going_up_fr0m_here!");
            outfitInfo.customMod = ((player, b, b2) => { });
           // Outfits.Register(outfitInfo);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Nemesis";
            outfitInfo2.outfit = new global::Outfit("Mythical::Nemesis", AssignNewID("nemesis"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Wizard's Vestige");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            particles.Add("Mythical::Nemesis", ImgHandler.LoadTex2D("nemParticle"));
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Ayona";
            outfitInfo2.outfit = new global::Outfit("Mythical::Ayona", AssignNewID("ayona"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Spirit of Light!");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Jade";
            outfitInfo2.outfit = new global::Outfit("Mythical::Jade", AssignNewID("jade"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Green Demon");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Lotus";
            outfitInfo2.outfit = new global::Outfit("Mythical::Lotus", AssignNewID("lotus"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Dying Petals");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Empress";
            outfitInfo2.outfit = new global::Outfit("Mythical::Empress", AssignNewID("empress"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Aspect of Fire");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);


            outfitInfo = new OutfitInfo();
            outfitInfo.name = "Sovereign";
            outfitInfo.outfit = new global::Outfit("Mythical::Sovereign", AssignNewID("sovereign"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, true, false);
            outfitInfo.customDesc = ((bool b) => "Aspect of Wind");
            outfitInfo.customMod = ((player, b, b2) => { });
            //Outfits.Register(outfitInfo);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Atlas";
            outfitInfo2.outfit = new global::Outfit("Mythical::Earth", AssignNewID("earth"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Aspect of Earth");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Suman";
            outfitInfo2.outfit = new global::Outfit("Mythical::Thunder", AssignNewID("thunder"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Aspect of Thunder");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Freiya";
            outfitInfo2.outfit = new global::Outfit("Mythical::Frost", AssignNewID("frost"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Aspect of Frost");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Despair";
            outfitInfo2.outfit = new global::Outfit("Mythical::Despair", AssignNewID("despair"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "ABANDON ALL HOPE");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Psion";
            outfitInfo2.outfit = new global::Outfit("Mythical::Psion", AssignNewID("psion"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Meow You See Me..");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Academic";
            outfitInfo2.outfit = new global::Outfit("Mythical::Academic", AssignNewID("academic"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Everyone Starts Somewhere");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Camo";
            outfitInfo2.outfit = new global::Outfit("Mythical::Camo", AssignNewID("camo"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Lie In Wait");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Cope";
            outfitInfo2.outfit = new global::Outfit("Mythical::Cope", AssignNewID("cope"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Seethe");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
           // Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Intangible";
            outfitInfo2.outfit = new global::Outfit("Mythical::Intangible", AssignNewID("cope"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Bespoke Arrogance");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Jupiter";
            outfitInfo2.outfit = new global::Outfit("Mythical::Jupiter", AssignNewID("jupiter"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "To The Stars");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Malachite";
            outfitInfo2.outfit = new global::Outfit("Mythical::Malachite", AssignNewID("malachite"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "The Bright");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Opal";
            outfitInfo2.outfit = new global::Outfit("Mythical::Opal", AssignNewID("opal"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Pearlescent Beauty");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Roar";
            outfitInfo2.outfit = new global::Outfit("Mythical::Roar", AssignNewID("roar"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Shouts of Arcanus");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Patina";
            outfitInfo2.outfit = new global::Outfit("Mythical::Patina", AssignNewID("patina"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Aged Like Wine");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Pigmented";
            outfitInfo2.outfit = new global::Outfit("Mythical::Pigmented", AssignNewID("pigmented"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "A Colorful Embrace");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Cerberus";
            outfitInfo2.outfit = new global::Outfit("Mythical::Cerberus", AssignNewID("cerberus"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Trash Taste, Loser");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Abysmal";
            outfitInfo2.outfit = new global::Outfit("Mythical::Abysmal", AssignNewID("abysmal"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "i feel like desperately trying to beg for more matches with covetous");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Verde";
            outfitInfo2.outfit = new global::Outfit("Mythical::Verde", AssignNewID("verde"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Better Call Saul!");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            //Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Lunes";
            outfitInfo2.outfit = new global::Outfit("Mythical::Lunes", AssignNewID("lunes"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Feliz Jueves");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            // Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Jaccablu";
            outfitInfo2.outfit = new global::Outfit("Mythical::Jaccablu", AssignNewID("jaccablu"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Fire fades wills of steel to temper");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Rumor";
            outfitInfo2.outfit = new global::Outfit("Mythical::Rumor", AssignNewID("rumor"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Pay me no mind -- 1st Prize Halloween 2022");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Mando";
            outfitInfo2.outfit = new global::Outfit("Mythical::Mando", AssignNewID("mando"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "This is the Way -- 2nd Prize Halloween 2022");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Sandstorm";
            outfitInfo2.outfit = new global::Outfit("Mythical::Sandstorm", AssignNewID("sandstorm"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Darude -- 3rd Prize Halloween 2022");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Genius";
            outfitInfo2.outfit = new global::Outfit("Mythical::Genius", AssignNewID("genius"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Get schwifty -- 4th Prize Halloween 2022");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);
            RegisterTrail("Mythical::Genius", new UnityEngine.Color(1f, 1f, 1, 0.8f), new UnityEngine.Color(0.6f, 0.8f, 0.8f, 0.3f));

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Shade";
            outfitInfo2.outfit = new global::Outfit("Mythical::Shade", AssignNewID("shade"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "I was born into this -- 5th Prize Halloween 2022");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);
            

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Lover";
            outfitInfo2.outfit = new global::Outfit("Mythical::Lover", AssignNewID("lover"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Happy Valentines Day!");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);
            headgears.Add("Mythical::Lover", new HeadgearDef()
            {
                sprites = new Sprite[]
                {
                    ImgHandler.LoadSprite("headgear/heart1"),
                    ImgHandler.LoadSprite("headgear/heart2"),
                    ImgHandler.LoadSprite("headgear/heart3"),
                    ImgHandler.LoadSprite("headgear/heart4"),
                    ImgHandler.LoadSprite("headgear/heart5")
                }
            });
            RegisterTrail("Mythical::Lover", new UnityEngine.Color(1, 0.8f, 0.8f, 0.5f), new UnityEngine.Color(1, 0.8f, 0.8f, 0.2f));

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Genesis";
            outfitInfo2.outfit = new global::Outfit("Mythical::Genesis", AssignNewID("genesis"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Hood is off!");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            Outfits.Register(outfitInfo2);
            headgears.Add("Mythical::Genesis", new HeadgearDef()
            {
                sprites = new Sprite[]
                {
                    ImgHandler.LoadSprite("headgear/girl1"),
                    ImgHandler.LoadSprite("headgear/girl2"),
                    ImgHandler.LoadSprite("headgear/girl3"),
                    ImgHandler.LoadSprite("headgear/girl4"),
                    ImgHandler.LoadSprite("headgear/girl5")
                }
            });

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Icarus";
            outfitInfo2.outfit = new global::Outfit("Mythical::Icarus2", AssignNewID("icarus"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false),
                new OutfitModStat(OutfitModStat.OutfitModType.Health,250,0,0,false)
            }, false, false) ;
            outfitInfo2.customDesc = ((bool b) => "Melt'em Down! Reward for completing the Contestant Assault.");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            outfitInfo2.unlockCondition = () =>
            {
                return PlayerPrefs.GetInt("mythical::CA", 0) == 1;
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Gaia";
            outfitInfo2.outfit = new global::Outfit("Mythical::Gaia2", AssignNewID("gaia"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false),
                new OutfitModStat(OutfitModStat.OutfitModType.Fall,0,0,0,true)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "The Bearer of Life! Reward for completing the Contestant Assault.");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
                player.regenDelay = b2 ? 3 : 8;
            };
            outfitInfo2.unlockCondition = () =>
            {
                return PlayerPrefs.GetInt("mythical::CA", 0) == 1;
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Goddess";
            outfitInfo2.outfit = new global::Outfit("Mythical::Goddess", AssignNewID("goddess"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false),
                new OutfitModStat(OutfitModStat.OutfitModType.Health,0f,0.65f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Speed,0f,0.65f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Damage,0f,0.25f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.CritChance,0.2f,0,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Armor,0.25f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Evade,0.2f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Cooldown,-0f,-0.45f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.HealAmount,0.3f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.Gold,0.1f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.HealCrit,0.1f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.ODRate,0.2f,0f,0,false),
                new OutfitModStat(OutfitModStat.OutfitModType.ODDamage,0.25f,0,0,false),
                //new OutfitModStat(OutfitModStat.OutfitModType.Health,-200,0,0,false),
                //new OutfitModStat(OutfitModStat.OutfitModType.Damage,0,0.2f,0,false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Awarded to the Lucky Few who completed the Ultra Council Challenge!");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            outfitInfo2.unlockCondition = () =>
            {
                return PlayerPrefs.GetInt("mythical::UCC", 0) == 1;
            };
            Outfits.Register(outfitInfo2);

            outfitInfo2 = new OutfitInfo();
            outfitInfo2.name = "Challenger";
            outfitInfo2.outfit = new global::Outfit("Mythical::Challenger", AssignNewID("challenger"), new List<global::OutfitModStat>
            {
                new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
            }, false, false);
            outfitInfo2.customDesc = ((bool b) => "Awarded to the Lucky Few who completed the Ultra Council Challenge!");
            outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
            {
            };
            outfitInfo2.unlockCondition = () =>
            {
                return PlayerPrefs.GetInt("mythical::UCC", 0) == 1;
            };
            Outfits.Register(outfitInfo2);

            //List<string> robeNames = new List<string>() { "sovereign", "crimson", "vision","terror","scholar","fear","conquest","tycoon","surf","walter","guardian","relic","empress","Despair","nemesis","lotus","psion","ayona","jade","thunder","frost","earth","goddess","challenger","academic","camo","cope","intangible","jupiter","malachite","opal","roar","icarus","gaia","patina","pigmented","cerberus","abysmal","verde","lunes" };
            foreach (string robeName in robeNames)
            {
                palettes.Add(ImgHandler.LoadTex2D(robeName));
            }


            string path2 = "Custom Robes";
            string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path2);
            string[] fileEntries = Directory.GetFiles(text);

            foreach(string file in fileEntries)
            {
                if (file.EndsWith(".robe"))
                {
                    string data = File.ReadAllText(file);
                    string[] split = data.Split(new string[] { "***BREAK***" }, System.StringSplitOptions.RemoveEmptyEntries);
                    outfitInfo2 = new OutfitInfo();
                    outfitInfo2.name = split[0];
                    string id = split[2] + "::" + split[0];
                    outfitInfo2.outfit = new global::Outfit(id, AssignNewIDAlt(Convert.FromBase64String(split[3])), new List<global::OutfitModStat>
                    {
                        new global::OutfitModStat(Outfits.CustomModType, 0f, 0.1f, 0f, false)
                    }, false, false);
                    outfitInfo2.customDesc = ((bool b) => split[1]);
                    outfitInfo2.customMod = delegate (global::Player player, bool b, bool b2)
                    {
                    };

                    if (split.Length > 4)
                    {
                        UnityEngine.Color color1 = UnityEngine.Color.black;
                        UnityEngine.Color color2 = UnityEngine.Color.black;
                        int clr = 0;
                        for (int i = 4; i < split.Length; i++)
                        {
                            if (split[i].StartsWith("particle::"))
                            {
                                string s = split[i].Replace("particle::", "");
                                Texture2D tex = ImgHandler.LoadTex2D(id + " Particle", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(s)));
                                particles.Add(id, tex);
                                ;
                            }
                            if (split[i].StartsWith("color1::"))
                            {
                                clr = 1;
                                string s = split[i].Replace("color1::", "");
                                string[] spl = s.Split('-');
                                float[] values = new float[] { float.Parse(spl[0]), float.Parse(spl[01]), float.Parse(spl[02]), float.Parse(spl[03]) };
                                color1 = new UnityEngine.Color(values[0], values[1], values[2], values[3]);
                            }
                            if (split[i].StartsWith("color2::"))
                            {
                                clr = 2;
                                string s = split[i].Replace("color2::", "");
                                string[] spl = s.Split('-');
                                float[] values = new float[] { float.Parse(spl[0]), float.Parse(spl[01]), float.Parse(spl[02]), float.Parse(spl[03]) };
                                color2 = new UnityEngine.Color(values[0], values[1], values[2], values[3]);
                            }
                            if (split[i].StartsWith("headgear::"))
                            {
                                string s = split[i].Replace("headgear::", "");
                                string[] spl = s.Split(new string[] { "**HEADGEAR**" }, System.StringSplitOptions.RemoveEmptyEntries);
                                HeadgearDef def = new HeadgearDef()
                                {
                                    sprites = new Sprite[] {
                                        ImgHandler.LoadSprite(id + " Gear 1", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[0]))),
                                        ImgHandler.LoadSprite(id + " Gear 2", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[1]))),
                                        ImgHandler.LoadSprite(id + " Gear 3", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[2]))),
                                        ImgHandler.LoadSprite(id + " Gear 4", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[3]))),
                                        ImgHandler.LoadSprite(id + " Gear 5", T2D: ImgHandler.LoadPNGAlt(Convert.FromBase64String(spl[4])))
                                    }
                                };

                                headgears.Add(id, def);

                            }
                        }
                        if (clr==1) { RegisterTrail(id, color1, color1); }
                        else if (clr == 2) { RegisterTrail(id, color1, color2); }


                    }

                    Outfits.Register(outfitInfo2);
                }
            }
            text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Banlist.txt");
            bannedArcana = File.ReadAllLines(text).ToList();

            // Title screen additions

            DateTime date1 = new DateTime(2022, 7, 29, 0, 0, 0);
            DateTime date3 = new DateTime(2022, 7, 31, 0, 0, 0);
            DateTime date2 = DateTime.Now;
            int result = DateTime.Compare(date1, date2);
            int result2 = DateTime.Compare(date3, date2);
            if (result<0&&result2>=0)
            {
                titleScreens.Add(ImgHandler.LoadSprite("bg4"));
            }
            titleScreens.Add(ImgHandler.LoadSprite("bg1"));
            titleScreens.Add(ImgHandler.LoadSprite("bg2"));
            titleScreens.Add(ImgHandler.LoadSprite("bg3"));
            

            if (result2<0)
            {
                titleScreens.Add(ImgHandler.LoadSprite("bg4"));
            }
            titleScreens.Add(ImgHandler.LoadSprite("bg5"));
            titleScreens.Add(ImgHandler.LoadSprite("bg6"));
            titleScreens.Add(ImgHandler.LoadSprite("bg7"));
            /*
            // Item Spawner revisions

            On.ItemSpawner.Reset += (On.ItemSpawner.orig_Reset orig, ItemSpawner self) =>
            {
                foreach (ItemSpawner.ItemPoolEntry itemPoolEntry in self.itemPoolDict.Values)
                {
                    foreach (PickupItem pickupItem in itemPoolEntry.itemArray)
                    {
                        try
                        {
                            pickupItem.Disable();
                        }
                        catch
                        {
                            //Debug.Log("Cleaning up item failed");
                        }
                    }
                }
            };*/


            // Disable Drops

            On.Destructible.Break += (On.Destructible.orig_Break orig, Destructible self) =>
            {
                Debug.Log("Checking if Destructible broke");
                if (announcementPairs.ContainsKey(self.gameObject))
                {
                    announcementPairs.Remove(self.gameObject);
                }
                if (self.name.Contains("NoPickups") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Spell Drops Disabled",3f);
                    Debug.Log("No drops");
                    SpawnPickups = false;
                }
                if (self.name.Contains("NoEffects") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Stage Effects Disabled", 3f);
                    Debug.Log("No effects");
                    StageEffects = false;
                }
                if (self.name.Contains("NoHazards") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Stage Hazards Disabled", 3f);
                    Debug.Log("No hazards");
                    StageHazards = false;
                }
                if (self.name.Contains("MonoDrops") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Mono Element Drops", 3f);
                    Debug.Log("Mono Drops");
                    MonoElementDrops = true;
                }
                if (self.name.Contains("BestTo3") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Match Will Be First To 3", 3f);
                    Debug.Log("Best To 3");
                    BestTo3 = true;
                }
                if (self.name.Contains("Depletion") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Health Will Drain", 3f);
                    Debug.Log("Depletion");
                    Depletion = true;
                }
                if (self.name.Contains("SpawnMB") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Bosses Will Spawn", 3f);
                    Debug.Log("Spawn Bosses");
                    SpawnMiniBoss = true;
                }
                if (self.name.Contains("SaveArcana") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Arcana Will Be Saved", 3f);
                    Debug.Log("Save Arcana");
                    SaveArcana = true;
                }

                if (self.name.Contains("NoBuffs") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Robe Effects Disabled", 3f);
                    Debug.Log("Disable Buffs");

                    foreach (Player p in GameController.activePlayers)
                    {
                        Outfit.GetAvailableOutfit(p.outfitID).SetMods(false,false);
                    }

                    RobeBuffs = false;
                }

                if (self.name.Contains("UseBanList") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Arcana Banlist Enabled", 3f);
                    Debug.Log("Arcana Banlist Enabled");

                    UseBanlist = true;
                }

                if (self.name.Contains("Freeze") && inPVPScene)
                {
                    GameUI.BroadcastNoticeMessage("Freeze Position Enabled", 3f);
                    Debug.Log("Freeze Enabled");

                    UseBanlist = true;
                }

                orig(self);
            };

            On.GameController.TogglePvpMode += (On.GameController.orig_TogglePvpMode orig, bool b) =>
            {
                orig(b);
                if (!b)
                {
                   // Debug.Log("Killing all Mbosses");
                    foreach (MiniBoss mb in FindObjectsOfType<MiniBoss>())
                    {
                        mb.health.CurrentHealthValue = -1;
                        mb.fsm.QueueChangeState("Dead", false);
                        mb.health.AnnounceDeathEvent(mb);
                    }
                    //Debug.Log("Killing all Bosses");
                    foreach (Boss mb in FindObjectsOfType<Boss>())
                    {
                        mb.health.CurrentHealthValue = -1;
                        mb.fsm.QueueChangeState("Dead", false);
                        mb.health.AnnounceDeathEvent(mb);
                        Destroy(mb.gameObject, 5);
                    }
                    GameController.bosses.Clear();
                }
                
                if (b && SpawnMiniBoss)
                {
                    List<string> elements = new List<string>() { "Fire", "Earth", "Lightning", "Ice", "Air" };
                    List<Enemy.EName> bosses = new List<Enemy.EName>() { Enemy.EName.SuperKnight, Enemy.EName.SuperMage, Enemy.EName.SuperLancer, Enemy.EName.SuperArcher, Enemy.EName.SuperRogue, Enemy.EName.SuperCoffin};
                    //Debug.Log("Spawning Mbosses");
                    Enemy.Spawn(bosses[UnityEngine.Random.Range(0, bosses.Count)], ChaosArenaChanges.offset + Vector3.up * 6).chestLootTableID=String.Empty ;
                    Enemy.Spawn(bosses[UnityEngine.Random.Range(0, bosses.Count)], ChaosArenaChanges.offset - Vector3.up * 6).chestLootTableID = String.Empty;
                    //Debug.Log("Spawning Boss");
                    //string str = elements[UnityEngine.Random.Range(0, 5)];
                    /*string str = "Final";
                    try
                    {

                        Boss boss = UnityEngine.Object.Instantiate<GameObject>(ChaosBundle.Get(bossPrefabFilePaths[str + "Boss"]), ChaosArenaChanges.offset, Quaternion.identity).GetComponent<Boss>();
                        boss.fsm.ChangeState(boss.bossReadyState.name, false);
                        boss.chestLootTableID = String.Empty;
                    }
                    catch { }*/
                }
            };

            On.PvpController.HandleSkillSpawn += (On.PvpController.orig_HandleSkillSpawn orig, PvpController self) =>
            {
                if (SpawnPickups)
                {
                    orig(self);
                }
            };

            On.PvpController.Start += (On.PvpController.orig_Start orig, PvpController self) =>
            {
                orig(self);
                if (BestTo3)
                {
                    self.maxRoundCount += 2;
                }
            };

            On.PvpController.TogglePlayerInvulnerable += (On.PvpController.orig_TogglePlayerInvulnerable orig, PvpController self, bool b) =>
            {
                for (int i = 0; i < GameController.players.Count; i++)
                {
                    Player p = GameController.players[i].GetComponent<Player>();
                    if (FreezeStartPositions)
                    {
                        if (b)
                        {
                            valueIndex[i] = p.movement.moveSpeedStat.CurrentValue;
                            p.movement.moveSpeedStat.CurrentValue = 0;
                        } else
                        {
                            p.movement.moveSpeedStat.CurrentValue = valueIndex[i];
                        }
                        
                        
                    }
                }
                orig(self,b);
            };


            //List<Sprite> loadingTheListIDontNeedThisToAllocate = IconManager.TSBGSpriteList;
            On.IconManager.GetBGSprite += (On.IconManager.orig_GetBGSprite orig, int index) =>
            {
                if (!hasAddedTitleCards)
                {
                    hasAddedTitleCards = true;
                    List<Sprite> loadingTheListIDontNeedThisToAllocate = IconManager.TSBGSpriteList;
                    TitleScreen.bgCount += titleScreens.Count;

                    List<Sprite> sprites = new List<Sprite>();

                    foreach (Sprite spr in titleScreens)
                    {
                        sprites.Add(spr);
                    }
                    foreach(Sprite spr in IconManager.TSBGSpriteList)
                    {
                        sprites.Add(spr);
                    }

                    IconManager.tsbgSpriteList = sprites;


                }
                return orig(index);
            };
            

            //Palettes

            On.PvpController.ResetStage += PvpController_ResetStage;
            On.PvpController.ResetPlayers += PvpController_ResetPlayers;
            On.Player.SetPlayerOutfitColor += Us_AddOutfit;

            On.GameProgressBoard.SetPlayerColors += (On.GameProgressBoard.orig_SetPlayerColors orig, GameProgressBoard self) =>
             {
                 if (newPalette != null)
                 {
                     self.p1PieceImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                     self.p1PieceImage.material.SetTexture("_Palette", newPalette);
                 }
                 orig(self);
                 
             };

            On.OutfitMenu.LoadMenu += (On.OutfitMenu.orig_LoadMenu orig, OutfitMenu self, Player p) =>
            {
                self.outfitImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                self.outfitImage.material.SetTexture("_Palette", newPalette);
                orig(self, p);
            };
            
            On.DeathSummaryUI.Activate += (On.DeathSummaryUI.orig_Activate orig, DeathSummaryUI self, float f) => {
                orig(self,f);
                if (newPalette != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        self.playerRefs[i].outfitImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                        self.playerRefs[i].outfitImage.material.SetTexture("_Palette", newPalette);
                    }
                }
            };

            On.WardrobeUI.LoadOutfits += (On.WardrobeUI.orig_LoadOutfits orig, WardrobeUI self) =>
            {
                orig(self);
                if (newPalette != null)
                {
                    for (int j = 0; j < self.totalOutfitCount; j++)
                    {
                        
                        self.wrRef.outfitImageArray[j].material.SetFloat("_PaletteCount", 32+palettes.Count);
                        self.wrRef.outfitImageArray[j].material.SetTexture("_Palette", newPalette);
                    }
                }
            };
            On.DialogManager.Activate += (On.DialogManager.orig_Activate orig, DialogManager self, DialogMessage m, bool b, bool s) =>
            {
                orig(self, m, b, s);
                if (m.rightActive && m.RightSpeaker != null && newPalette != null)
                {
                    self.rightPlayerImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                    self.rightPlayerImage.material.SetTexture("_Palette", newPalette);
                }
            };
            On.UnlockNotifier.SetNotice += (On.UnlockNotifier.orig_SetNotice orig, UnlockNotifier self, UnlockNotifier.NoticeVars vars) =>
            {
                orig(self, vars);
                if (newPalette != null)
                {
                    self.outfitIconImage.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                    self.outfitIconImage.material.SetTexture("_Palette", newPalette);
                }
            };
            On.WardrobeUI.UpdateOutfits += (On.WardrobeUI.orig_UpdateOutfits orig, WardrobeUI self, int givenIndex) =>
            {
                orig(self, givenIndex);
                if (newPalette != null)
                {
                    self.wrRef.playerImages[self.currentPlayerImageIndex].material.SetFloat("_PaletteCount", 32 + palettes.Count);
                    self.wrRef.playerImages[self.currentPlayerImageIndex].material.SetTexture("_Palette", newPalette);
                }
            };
            On.OutfitStoreItem.Start += (On.OutfitStoreItem.orig_Start orig, OutfitStoreItem self) =>
            {
                orig(self);
                self.itemSpriteRenderer.material.SetFloat("_PaletteCount", 32 + palettes.Count);
                self.itemSpriteRenderer.material.SetTexture("_Palette", newPalette);
            };
            On.PlayerStatusBar.Awake += (On.PlayerStatusBar.orig_Awake orig, PlayerStatusBar self) =>
            {
                orig(self);
                self.gameObject.AddComponent<StatusBarMod>().self = self;
            };
            On.WardrobeUI.AssignOutfit += (On.WardrobeUI.orig_AssignOutfit orig, WardrobeUI self, Outfit o, int i) =>
            {
                self.wrRef.outfitImageArray[i].material.SetFloat("_PaletteCount", 32 + palettes.Count);
                self.wrRef.outfitImageArray[i].material.SetTexture("_Palette", newPalette);
                orig(self, o, i);
            };
            
            On.OutfitMenu.LoadMenu += (On.OutfitMenu.orig_LoadMenu orig , OutfitMenu self, Player p) => { orig(self, p); if (hasAddedPalettes) { self.outfitImage.material.SetTexture("_Palette", newPalette); } };
            On.OutfitMenu.SwapFocus += (On.OutfitMenu.orig_SwapFocus orig, OutfitMenu self, bool n) => { orig(self, n); if (hasAddedPalettes) { self.outfitImage.material.SetTexture("_Palette", newPalette); } };

            /*On.Player.InitComponents += (On.Player.orig_InitComponents orig, Player self) =>
            {
                orig(self);
                Debug.Log("Old Sprite Name: " + self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite.name);
                self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite = newPlayerSprite;
                Debug.Log("Set new palette: "+ self.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().sprite.name);
            };*/

            // Stage effects

            On.PvpController.ApplyAirBuffs += (On.PvpController.orig_ApplyAirBuffs orig, PvpController self) =>
            {
                if (StageEffects) { orig(self); }
            };
            On.PvpController.ApplyFireBuffs += (On.PvpController.orig_ApplyFireBuffs orig, PvpController self) =>
            {
                if (StageEffects) { orig(self); }
            };
            On.PvpController.ApplyEarthBuffs += (On.PvpController.orig_ApplyEarthBuffs orig, PvpController self) =>
            {
                if (StageEffects) { orig(self); }
            };
            
            // Music
            
            LoadSong("TitleScreen","Sprites/Vaporwave.ogg");
            LoadSong("Hub", "Sprites/Trap.ogg");
            LoadSong("Boss", "Sprites/Rock.ogg");
            
            BGMInfo bgm = new BGMInfo()
            {
                name = "PvP",
                fallback = BGMTrackType.Original,
                message = "Do you want to hear the Council's sick beats?",
                messageConfirm = "Fuck yes",
                messageCancel = "Fuck no",
                soundtrack = clipDict
            };

            LegendAPI.Music.Register(bgm);

            On.Player.Start += (On.Player.orig_Start orig, Player self) =>
            {
                orig(self);
                self.gameObject.AddComponent<TrailTEDManager>();
                self.gameObject.AddComponent<TEDLineManager>();
                Headgear gear = Globals.ChaosInst<Headgear>(HorseMaskItem.Prefab, 
                    (!self.headPosition) ? self.transform : self.headPosition.transform, null, null);
                /*gear.spriteVariations = new Sprite[] //Up,Right,Down,UR,DR
                {
                    ImgHandler.LoadSprite("headgear/heart1"),
                    ImgHandler.LoadSprite("headgear/heart2"),
                    ImgHandler.LoadSprite("headgear/heart3"),
                    ImgHandler.LoadSprite("headgear/heart4"),
                    ImgHandler.LoadSprite("headgear/heart5")
                };*/
                gear.gameObject.AddComponent<TEDHeadgear>();
                gear.Initialize(self, "TournamentEditionHeadgear", null);

            };

            // Boss, Hub, TitleScreen


            /* No Longer Necessary
             
            On.Player.Start += (On.Player.orig_Start orig, Player self) =>
            {
                orig(self);
                for (int i = 0; i < IconManager.WizardSpriteList.Count; i++)
                {
                    //IconManager.wizardSprites[playerSprites[i].name] = playerSprites[i];
                    string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Exports/");
                    Debug.Log("Saved File Here: " + text);
                    SaveSubSprite(ExtractAndName(IconManager.WizardSpriteList[i]), text);
                }
            };
            */
            //Insignia of Legend Changes

            On.PlayerWinItem.SetEventHandlers += (On.PlayerWinItem.orig_SetEventHandlers orig, PlayerWinItem self, bool b) =>
            {
                orig(self,b);
                ChaosDrops = b;
            };

            On.GameController.Start += (On.GameController.orig_Start orig, GameController self) =>
            {
                orig(self);
                // Chaos arena changes
                if (!addedGMHooks)
                {
                    On.LootManager.GetSkillID += (On.LootManager.orig_GetSkillID orig2, bool l, bool s) =>
                    {
                        string finalResult = "";
                        while(true) {
                            if (ChaosDrops && inAPVPScene && UnityEngine.Random.value < 0.25f)
                            {
                                finalResult= LootManager.chaosSkillList[UnityEngine.Random.Range(0, LootManager.chaosSkillList.Count)];
                            }
                            else if (MonoElementDrops && inAPVPScene)
                            {

                                List<ElementType> usableElements = new List<ElementType>();
                                if (monoskills.Count == 0)
                                {
                                    for (int i = 0; i < GameController.players.Count; i++)
                                    {
                                        usableElements.Add(GameController.players[i].GetComponent<Player>().assignedSkills[0].element);
                                    }

                                    for (int i = 0; i < LootManager.completeSkillList.Count; i++)
                                    {
                                        if (usableElements.Contains(GameController.players[0].GetComponent<Player>().GetSkill(LootManager.completeSkillList[i]).element))
                                        {
                                            // Debug.Log("Added " + LootManager.completeSkillList[i]);
                                            monoskills.Add(LootManager.completeSkillList[i]);
                                        }
                                    }
                                }
                                finalResult= monoskills[UnityEngine.Random.Range(0, monoskills.Count)];
                            }
                            else
                            {
                                finalResult = orig2(l, s);
                            }

                            if (!ContentLoader.UseBanlist||!bannedArcana.Contains(finalResult)){
                                break;
                            }

                        }
                        return finalResult;
                    };

                    On.LootManager.DropSkill += (On.LootManager.orig_DropSkill orig3, Vector3 v, int a, string id, float l, float s, HashSet<ElementType> set, bool life, bool emp) =>
                    {
                        if (inAPVPScene && LootManager.chaosSkillList.Contains(id))
                        {
                            emp = true;
                        }
                        orig3(v, a, id, l, s, set, life, emp);
                    };

                    

                    addedGMHooks = true;
                    ChaosArenaChanges.Init();
                    CursedRelics.Init();
                }
            };

            On.TitleScreen.AllowMultiplayer += (On.TitleScreen.orig_AllowMultiplayer orig, TitleScreen self) =>
            {
                return true;
            };

            /*On.Outfit.OutfitIsInUse += (On.Outfit.orig_OutfitIsInUse orig, string id, int playerID) =>
            {
                if (GameController.activePlayers.Length > 1)
                {
                    return GameDataManager.gameData.playerData[playerID].outfitName == id;
                } else
                {
                    return orig(id, playerID);
                }
            };*/

            On.Outfit.SetMods += (On.Outfit.orig_SetMods orig, Outfit self, bool b, bool b2) =>
            {
                orig(self, b&&(RobeBuffs), b2 && (RobeBuffs));
            };

            if (enableTicket.Value)
            {
                ItemInfo itemInfo2 = new ItemInfo();
                itemInfo2.name = "PrimeTicket";
                itemInfo2.item = new PrimeTicket();
                itemInfo2.tier = 1;
                TextManager.ItemInfo text2 = default(TextManager.ItemInfo);
                text2.displayName = "Arcana Ticket";
                text2.description = "With everyone that falls, a message in their wake.";
                text2.itemID = PrimeTicket.staticID;
                Sprite sprite = ImgHandler.LoadSprite("ticket");
                itemInfo2.text = text2;
                itemInfo2.icon = ((sprite != null) ? sprite : null);
                Items.Register(itemInfo2);
            }

            //Adjustments
            /*On.PlatWallet.ctor += (On.PlatWallet.orig_ctor orig, PlatWallet self, int i) =>
            {
                orig(self,i);
                self.maxBalance = 9999;
                self.balance = 9999;
            };*/


        }
        public static float[] valueIndex = new float[2] {0,0};
        public static bool RobeBuffs = true;
        public static bool SpawnMiniBoss = false;
        public static bool FreezeEnabled = false;
        public void Update()
        {

            /*if (Input.GetKeyDown(KeyCode.K))
            {
                Enemy.Spawn("Contestant", GameController.players[0].transform.position);
            }*/
            
            List<KeyValuePair<GameObject, string>> toRemove = new List<KeyValuePair<GameObject, string>>();
            if (GameController.Instance != null)
            {
                if (GameController.players!=null&&GameController.players.Count > 0)
                {
                    foreach (KeyValuePair<GameObject, string> pair in announcementPairs)
                    {
                        if (pair.Key!=null)
                        {
                            if (Vector3.Distance(GameController.players[0].transform.position, pair.Key.transform.position) < 4)
                            {
                                GameUI.BroadcastNoticeMessage(pair.Value, 1f);
                            }
                        }
                        else
                        {
                            toRemove.Add(pair);
                        }
                    }
                }
            }
            foreach(KeyValuePair<GameObject,string> p in toRemove)
            {
                Destroy(p.Key);
            }

            if (SceneManager.GetActiveScene().name.ToLower()!="pvp")
            {
                if (true)
                {
                    if (Time.time > nextTime)
                    {
                        nextTime = Time.time + 1f;
                        foreach (GameObject player in GameController.players)
                        {
                            if (player.GetComponent<Player>().inventory.ContainsItem("Mythical::SevenFlushChaos")) { 
                                SoundManager.PlayAudio("ImpactPhysicalHeavy",1,false,0.25f);
                                player.GetComponent<Player>().health.CurrentHealthValue-=5;
                            }
                        }
                    }
                }
            }

        }
        float nextTime = 0.25f;

        public static Sprite[] playerSprites;
        public static AssetBundle playerBundle;
        public static Texture2D newPlayerSprite;

        public bool hasSwappedAudioClips = false;
        public bool hasAddedPalettes = false;
        public static bool SpawnPickups = true;
        public static bool StageEffects = true;
        public static bool StageHazards = true;
        public static bool BestTo3 = false;
        public static bool Depletion = false;
        public static bool MonoElementDrops = false;
        public static bool addedGMHooks = false;
        public static bool loadedWizSprites=false;
        public static Texture2D newPalette = null;
        List<string> monoskills = new List<string>();
        public static Dictionary<GameObject,SpriteRenderer> newPlayerDict = new Dictionary<GameObject, SpriteRenderer>();


        private static Texture2D ExtractAndName(Sprite sprite)
        {
            var output = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var r = sprite.textureRect;
            var pixels = sprite.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
            output.SetPixels(pixels);
            output.Apply();
            output.name = sprite.texture.name + " " + sprite.name;
            return output;
        }
        private static void SaveSubSprite(Texture2D tex, string saveToDirectory)
        {
            if (!System.IO.Directory.Exists(saveToDirectory)) System.IO.Directory.CreateDirectory(saveToDirectory);
            System.IO.File.WriteAllBytes(System.IO.Path.Combine(saveToDirectory, tex.name + ".png"), tex.EncodeToPNG());
        }

        public bool inPVPScene { get
            {
                return SceneManager.GetActiveScene().name.ToLower() == "pvp";
            } }
        public static  bool inAPVPScene
        {
            get
            {
                return SceneManager.GetActiveScene().name.ToLower().Contains("pvp");
            }
        }
        public static GameObject interactableParent;
        Dictionary<GameObject, string> announcementPairs = new Dictionary<GameObject, string>();
        public void OnLevelWasLoaded()
        {
            try
            {
                Player.platWallet.balance = Player.platWallet.maxBalance; //Enjoy
                
            }
            catch { }
            try
            {
                if (SceneManager.GetActiveScene().name.ToLower()!="pvparena")
                {
                    if (!RobeBuffs)
                    {
                        foreach (Player p in GameController.activePlayers)
                        {
                            Outfit.GetAvailableOutfit(p.outfitID).SetMods(true, true);
                        }
                    }

                    RobeBuffs = true;
                }
                if (inPVPScene)
                {
                    GameObject area = GameObject.Find("StagingArea");
                    interactableParent = area.transform.Find("Interactables").gameObject;
                    interactableParent.transform.parent = null;
                    interactableParent.transform.Translate(0, 3, 0);
                    GameObject.Find("Npcs").transform.Translate(0, 2, 0);
                    GameObject.Find("ExitPortalDisabled").transform.Translate(0, -3, 0);
                    Destroy(GameObject.Find("WallPillars"));
                    area.transform.localScale = new Vector3(1.4f, 1.2f, 1);
                    area.transform.Translate(-10, 5, 0);



                    ChaosArenaChanges.ResetTileSet();
                    ChaosArenaChanges.AddCustomArenaPortals();

                    GameObject mimi = MimicNpc.Prefab;
                    pvpItems = new Dictionary<int, List<string>>();
                    arcana = new Dictionary<int, List<string>>();
                    Instantiate(mimi, new Vector3(0, 8, 0), Quaternion.identity);

                    SpawnPickups = true;
                    StageEffects = true;
                    StageHazards = true;
                    MonoElementDrops = false;
                    Depletion = false;
                    BestTo3 = false;
                    SpawnMiniBoss = false;
                    SaveArcana = false;
                    UseBanlist=false;
                    FreezeStartPositions = false;

                    GameObject noPickups = Instantiate(Tree.Prefab, new Vector3(-11, -3, 0), Quaternion.identity);
                    noPickups.name = "NoPickups";
                    announcementPairs[noPickups] = "Disable spell drops!";

                    GameObject noEffects = Instantiate(Tree.Prefab, new Vector3(11, -3, 0), Quaternion.identity);
                    noEffects.name = "NoEffects";
                    announcementPairs[noEffects] = "Disable the arenas' special effects!";

                    GameObject noHazards = Instantiate(MetalBarrelDeco.Prefab, new Vector3(23, 5, 0), Quaternion.identity);
                    noHazards.name = "NoHazards";
                    announcementPairs[noHazards] = "Disable the arenas' dangerous hazards!";

                    GameObject monoDrops = Instantiate(MetalBarrelDeco.Prefab, new Vector3(-23, 5, 0), Quaternion.identity);
                    monoDrops.name = "MonoDrops";
                    announcementPairs[monoDrops] = "Only drops spells of types already held! (Currently basics only for performance sake)";

                    GameObject bestTo3 = Instantiate(MetalBarrelDeco.Prefab, new Vector3(8, 3, 0), Quaternion.identity);
                    bestTo3.name = "BestTo3";
                    announcementPairs[bestTo3] = "Makes matches first to 3 instead of first to 2!";

                    GameObject spawnMB = Instantiate(MetalBarrelDeco.Prefab, new Vector3(-8, 3, 0), Quaternion.identity);
                    spawnMB.name = "SpawnMB";
                    announcementPairs[spawnMB] = "Spawn strong foes at the start of each round!";

                    GameObject noBuffs = Instantiate(MetalBarrelDeco.Prefab, new Vector3(-16, 3, 0), Quaternion.identity);
                    noBuffs.name = "NoBuffs";
                    announcementPairs[noBuffs] = "Disable robe buffs for the match!";

                    GameObject saveArcana = Instantiate(MetalBarrelDeco.Prefab, new Vector3(16, 3, 0), Quaternion.identity);
                    saveArcana.name = "SaveArcana";
                    announcementPairs[saveArcana] = "Save arcana picked up between rounds!";

                    GameObject banBarrel = Instantiate(MetalBarrelDeco.Prefab, new Vector3(23, 15, 0), Quaternion.identity);
                    banBarrel.name = "UseBanList";
                    announcementPairs[banBarrel] = "Use the banlist in the plugins folder!";

                    //GameObject freezeBarrel = Instantiate(MetalBarrelDeco.Prefab, new Vector3(23, 15, 0), Quaternion.identity);
                    //freezeBarrel.name = "Freeze";
                    //announcementPairs[banBarrel] = "Disable movement and attacks until round begins!";

                    //GameObject depletion = Instantiate(Tree.Prefab, new Vector3(-8, 3, 0), Quaternion.identity);
                    //depletion.name = "Depletion";
                    //announcementPairs[depletion] = "Player health will slowly decay!";

                    foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
                    {
                        if (obj.name.ToLower() == "loadoutnpc" || obj.name.ToLower().Contains("trainingdummy"))
                        {
                            Destroy(obj);
                        }
                    }

                    foreach (GameObject t in announcementPairs.Keys)
                    {
                        if (t != null)
                        {
                            Destructible tree = t.GetComponentInChildren<Destructible>();
                            if (tree != null)
                            {
                                //tree.l
                                tree.destroyOnContact = false;
                                tree.health.healthStat.BaseValue = 100;
                                tree.health.CurrentHealthValue = 100;
                                tree.health.healthStat.ModifiedValue = 100;
                                tree.health.healthStat.CurrentValue = 100;
                            }
                        }
                    }



                    

                }

                List<string> Destroynames = new List<string>() { };
                if (inAPVPScene && SceneManager.GetActiveScene().name.ToLower().Contains("arena"))
                {


                    monoskills = new List<string>();
                    nextTime = Time.time + 5;
                    if (!StageEffects || !StageHazards)
                    {
                        if (!StageEffects) { Destroynames.Add("overdrive"); Destroynames.Add("enemy"); }
                        if (!StageHazards) { Destroynames.Add("hazard"); }
                        foreach (GameObject o in FindObjectsOfType<GameObject>())
                        {
                            if (o.transform.root != null && o.transform.root.name.ToLower() == "pvprooms")
                            {
                                foreach (string s in Destroynames)
                                {
                                    if (o.name.ToLower().Contains(s))
                                    {
                                        Destroy(o);
                                    }
                                }
                            }
                        }
                    }

                    ApplyPvpTokens();


                }
            } catch { }
        }

        public Texture2D EXPOSED;

        public void ApplyPvpTokens()
        {
            for (int i = 0; i < GameController.players.Count; i++)
            {
                Player p = GameController.players[i].GetComponent<Player>();
                if (p.inventory != null)
                {
                    if (p.inventory.itemDict.Count > 0)
                    {
                        string relic = p.inventory.itemDict.ElementAt(0).Key;
                        if (relic == "TokenShuffler")
                        {
                            p.RandomizeBuild(true, false, true);
                        }
                        else if (relic == "TokenCursed")
                        {
                            for (int k = 0; k < 6; k++)
                            {
                                if (p.assignedSkills[k] != null)
                                {
                                    p.RemoveSkill(p.assignedSkills[k]);
                                }
                            }

                            p.AssignSkillSlot(1, "Dash", false, false);
                        } else if (relic == "TokenBanker")
                        {
                            string id = LootManager.GetSkillID(false, false);
                            p.AssignSkillSlot(4, id, false, false);
                            id = LootManager.GetSkillID(false, false);
                            p.AssignSkillSlot(5, id, false, false);
                        } else if (relic=="TokenTailor")
                        {
                            UpgradePlayer.Upgrade(p);
                        }
                        else if (relic == "PrimeTicket")
                        {
                            //UpgradePlayer.Upgrade(p); //Chaos reaper, chaotic rift, twin turbines, distortion beam, lightning dragons, mark of discord.
                            p.AssignSkillSlot(0, "UseChaosScytheBasic", false, false);
                            p.AssignSkillSlot(1, "ChaosDash", false, false);
                            p.AssignSkillSlot(2, "UseShockBoomerang", true, true);
                            p.AssignSkillSlot(3, "UseIceBoomerang", true, true);
                            p.AssignSkillSlot(4, "UseChaosBeam", false, false);
                            p.AssignSkillSlot(5, "UseShockDragon", true, true);
                            //p.AssignSkillSlot(5, "UseChaosSwordSummon", false, false);
                        }
                        p.lowerHUD.cooldownUI.RefreshEntries();
                    }
                }
            }
        }

        public void Us_AddOutfit(On.Player.orig_SetPlayerOutfitColor orig, Player self, NumVarStatMod mod, bool givenStatus)
        {
            orig(self, mod, givenStatus);

            if (!loadedWizSprites)
            {
                loadedWizSprites = true;
                
                Texture2D text = new Texture2D(1, 1); text.LoadImage(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sprites/Walter2.png")));
                Texture2D texture2 = self.spriteRenderer.sprite.texture;
                texture2.SetPixels32(text.GetPixels32());
                texture2.Apply();
                EXPOSED = texture2;
                StartCoroutine("BootUpCredits");
            }

            Texture2D tex = basePalette.texture;// (Texture2D) self.spriteMaterial.GetTexture("_Palette");
            if (newPalette == null)
            {
                //Debug.Log("1");
                newPalette = tex;
                if (!hasAddedPalettes)
                {
                    //Debug.Log("2");
                    hasAddedPalettes = true;
                    Texture2D t = newPalette;
                    Texture2D newT;
                    int h = t.height;
                    //Debug.Log("3");
                    foreach (Texture2D te in palettes)
                    {
                        //Debug.Log("Iterating over " + te.name);
                        newT = new Texture2D(newPalette.width, newPalette.height + 2,TextureFormat.RGBA32,false);
                        newT = FillColorAlpha(newT);
                        for(int x = 1; x < newT.width; x++)
                        {
                            for (int y = 0; y < newPalette.height; y++)
                            {
                                newT.SetPixel(x, y, newPalette.GetPixel(x, y));
                            }
                        }
                       // Debug.Log("Out of loop for " + te.name);
                        for (int x = 1; x < newT.width; x++)
                        {
                            newT.SetPixel(x, h, te.GetPixel(x, h));
                        }
                        for (int x = 1; x < newT.width; x++)
                        {
                            newT.SetPixel(x, h+1, te.GetPixel(x, h+1));
                        }

                        //Debug.Log("Out of loop 2 for " + te.name);
                        newT.filterMode = FilterMode.Point;
                        newT.Apply();
                        newPalette = newT;
                        h+=2;
                    }
                }
            }

            if (hasAddedPalettes)
            {
                self.spriteMaterial.SetFloat("_PaletteCount", 32 + palettes.Count);
                self.spriteMaterial.SetTexture("_Palette", newPalette);
            }

            //orig(self, mod, givenStatus);
            
        }
        public static bool SaveArcana = false;
        public static Texture2D FillColorAlpha(Texture2D tex2D, Color32? fillColor = null)
        {
            if (fillColor == null)
            {
                fillColor = UnityEngine.Color.clear;
            }
            Color32[] fillPixels = new Color32[tex2D.width * tex2D.height];
            for (int i = 0; i < fillPixels.Length; i++)
            {
                fillPixels[i] = (Color32)fillColor;
            }
            tex2D.SetPixels32(fillPixels);
            return tex2D;
        }

        public static Dictionary<int, List<string>> pvpItems = new Dictionary<int, List<string>>();
        public static Dictionary<int, List<string>> arcana = new Dictionary<int, List<string>>();
        public static Dictionary<int, List<bool>> arcanaEnhance = new Dictionary<int, List<bool>>();
        public void PvpController_ResetPlayers(On.PvpController.orig_ResetPlayers orig, PvpController self, bool b)
        {
            orig(self, b);
            int i = 0;
            foreach(Player p in GameController.activePlayers)
            {
                if (pvpItems.ContainsKey(i))
                {

                    //p.inventory.AddItem(p.inventory.GetItem(pvpItems[i]));
                    foreach (string s in pvpItems[i])
                    {
                        p.GiveDesignatedItem(s);
                    }
                    
                }

                if (SaveArcana && arcana.ContainsKey(i))
                {
                    int a = 0;
                    foreach(string s in arcana[i])
                    {
                        p.AssignSkillSlot(a+4, s, false,false);
                        p.assignedSkills[a + 4].SetEmpowered(arcanaEnhance[i][a], EmpowerStatMods.DefaultEmpowerMod);
                        a++;
                    }
                    p.lowerHUD.cooldownUI.RefreshEntries();
                }

                i++;
            }

            ApplyPvpTokens();
        }

        public void PvpController_ResetStage(On.PvpController.orig_ResetStage orig, PvpController self, bool b)
        {
            pvpItems = new Dictionary<int, List<string>>();
            arcana = new Dictionary<int, List<string>>();
            arcanaEnhance = new Dictionary<int, List<bool>>();
            int i = 0;
            try
            {
                if (GameController.activePlayers != null)
                {
                    foreach (Player p in GameController.activePlayers)
                    {
                        if (p.inventory != null && p.inventory.itemDict != null)
                        {
                            if (p.inventory.itemDict.Count > 0)
                            {
                                List<string> str = new List<string>();
                                foreach(string s in p.inventory.itemDict.Keys)
                                {
                                    str.Add(s);
                                }
                                pvpItems[i] = str;
                            }
                        }

                        List<string> a = new List<string>();
                        List<bool> bo = new List<bool>();
                        if (p.assignedSkills[4] != null)
                        {
                            a.Add(p.assignedSkills[4].skillID);
                            bo.Add(p.assignedSkills[4].IsEmpowered);
                        }
                        if (p.assignedSkills[5] != null)
                        {
                            a.Add(p.assignedSkills[5].skillID);
                            bo.Add(p.assignedSkills[5].IsEmpowered);
                        }
                        if (a.Count>0 && !b)
                        {
                            arcana[i] = a;
                            arcanaEnhance[i] = bo;
                        }

                        
                        

                        i++;
                    }
                }
            }
            catch(Exception e)
            {
                Debug.Log("AVERTED ERROR: " + e.Message);
            }
            orig(self,b);
        }

        public bool Inventory_AddItem(On.Inventory.orig_AddItem_Item_bool_bool orig, Inventory self, Item givenItem, bool showNotice, bool ignoreMax)
        {
            self.currentItemID = givenItem.ID;
            Player.newItems[givenItem.category].Remove(self.currentItemID);
            if (self.ContainsItem(self.currentItemID))
            {
                return false;
            }
            if (!self.CheckForMutallyExclusiveItems(self.currentItemID))
            {
                return false;
            }
            if (self.itemDict.Count >= 18 &&!self.CheckItemCombine(givenItem) && !ignoreMax && !self.DropItem(string.Empty))
            {
                return false;
            }
            givenItem.parentEntity = self.parentEntity;
            givenItem.SetParentSkillCategory();
            self.itemDict[self.currentItemID] = givenItem;
            givenItem.Activate();
            if (showNotice && self.parentEntity is Player)
            {
                ((Player)self.parentEntity).newItemNoticeUI.Display(TextManager.GetItemName(self.currentItemID), IconManager.GetItemIcon(self.currentItemID), null, false, false, false, true);
                if (givenItem.isCursed)
                {
                    SoundManager.PlayAudio("BuyCursedRelic", 1f, false, -1f, -1f);
                }
                else
                {
                    SoundManager.PlayAudio("Powerup", 1f, false, -1f, -1f);
                }
                if (givenItem.isCursed)
                {
                    PoolManager.GetPoolItem<ParticleEffect>("CursedAuraBurstEffect").Play(null, new Vector3?(self.parentEntity.transform.position), null, null, null, null, 0f, false);
                }
            }
            self.count++;
            self.AnnounceItemEvent(givenItem, true);
            if (givenItem.isCursed)
            {
                AchievementManager.UnlockAchievement(Achievement.HoldFourCursed, false);
            }
            return true;

            //return orig(self, item, show, true);
        }

        public void LoadSong(string title, string path)
        {
            path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
            StartCoroutine(LoadSongCoroutine(title,path));
            /*
            string url = string.Format("file://{0}", path);
            WWW www = new WWW(url);
            AudioClip song = www.GetAudioClipCompressed(false, AudioType.OGGVORBIS);
            song.name = title + " (CUSTOM)";
            Debug.Log("Got Clip: " + song.name);
            if (song != null)
            {
                Debug.Log("WOOHOO IT LOADED");
            }
            clipDict[title] = song;*/
        }

        IEnumerator LoadSongCoroutine(string title, string path)
        {
            Debug.Log("Starting song routine");
            string url = string.Format("file:///{0}", path);
            using (WWW www = new WWW(url))
            {
                yield return www;
                AudioClip song = www.GetAudioClipCompressed(false, AudioType.OGGVORBIS);
                song.name = title + " (CUSTOM)";
                clipDict[title] = song;
            }
           
            
        }

        IEnumerator BootUpCredits()
        {
            GameUI.BroadcastNoticeMessage("Wizard of Legend: Tournament Edition by only_going_up_fr0m_here", 3f);
            yield return new WaitForSeconds(3);
            GameUI.BroadcastNoticeMessage("Special Thanks to Rayman, Holy Grind, RandomlyAwesome, and Cerberus", 3f);
        }

        public static Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();
        public static Dictionary<string, string> bossPrefabFilePaths = new Dictionary<string, string>
        {
            {
                "IceBoss",
                "Assets/Prefabs/Bosses/IceBoss.prefab"
            },
            {
                "FireBoss",
                "Assets/Prefabs/Bosses/FireBoss.prefab"
            },
            {
                "EarthBoss",
                "Assets/Prefabs/Bosses/EarthBoss.prefab"
            },
            {
                "AirBoss",
                "Assets/Prefabs/Bosses/AirBoss.prefab"
            },
            {
                "LightningBoss",
                "Assets/Prefabs/Bosses/LightningBoss.prefab"
            },
            {
                "FinalBoss",
                "Assets/Prefabs/Bosses/FinalBoss.prefab"
            }
        };
    }
}

public static class Extensions
{
    public static ItemInfo loadSprite(this ItemInfo info, string name)
    {
        Sprite spr = Mythical.ImgHandler.LoadSprite(name);
        info.icon = (spr != null ? spr : null);
        return info;
    }
    public static Sprite loadSprite(string name)
    {
        Sprite spr = Mythical.ImgHandler.LoadSprite(name);
        return (spr != null ? spr : null);
    }
}

public class HeadgearDef
{
    public Sprite[] sprites = new Sprite[0];
}