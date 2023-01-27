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
using UnityEngine;
using UnityEngine.SceneManagement;
using XUnity.ResourceRedirector;

namespace Mythical
{
    public static class ChaosArenaChanges
    {
        public static List<Texture2D> arenas = new List<Texture2D>();
        public static List<ArenaDef> tilesets = new List<ArenaDef>();
        public static Texture2D chosenArena=null;
        public static Texture2D arenaTextureReference = null;
        public static Texture2D arenaTextureCarbonCopy = null;
        public static Vector3 offset = new Vector3(25, -22, 0);
        static int arenaCount = 3;
        static int customTilesetCount = 1;
        static bool soloPortals = true;
        public static int arenaTileID = 0;
        public static Material mainMaterial;

        

        public static void Init()
        {

            arenaTextureReference = ImgHandler.LoadTex2D("tilesetMain",true);

            tilesets.Add(new ArenaDef()
            {
                name = "Dorm",
                description = "Face the Contestant Assault!",
                position = new Vector3(-19, -6.5f, 0),
                //tileset = ImgHandler.LoadTex2D("Arenas/tileset1", true),
                layout= ImgHandler.LoadTex2D("Arenas/arena1"),
                id = 0
            });
            /*
            tilesets.Add(new ArenaDef()
            {
                name = "Hell",
                description = "Misery and Torment",
                position = new Vector3(20, -6.5f, 0),
                //tileset = ImgHandler.LoadTex2D("Arenas/tileset2", true),
                layout = ImgHandler.LoadTex2D("Arenas/arena2"),
                element = ElementType.Fire,
                id = 1
            });*/

            tilesets.Add(new ArenaDef()
            {
                name = "Domicile",
                description = "Face the Ultra Council Challenge!",
                position = new Vector3(20, -6.5f, 0),
                //tileset = ImgHandler.LoadTex2D("Arenas/tileset2", true),
                layout = ImgHandler.LoadTex2D("Arenas/arena2"),
                element = ElementType.Fire,
                id = 1
            });


            On.PvpController.SetStageLayout += (On.PvpController.orig_SetStageLayout orig, PvpController self) =>
            {
                orig(self);
                if (!ContentLoader.StageEffects) { self.forceEmpowerDrop = false; }
                Debug.Log("Layout hook");
                if (PvpController.selectedRoomName == "DefaultRoom" || PvpController.selectedRoomName == "Neutral")
                {
                    foreach(MeshRenderer r in self.selectedRoom.GetComponentsInChildren<MeshRenderer>())
                    {
                        if (r.material.name.ToLower().Contains("plazatile"))
                        {
                            //arenaTextureReference = (Texture2D) r.material.mainTexture;
                            if (arenaTextureCarbonCopy == null)
                            {
                                arenaTextureCarbonCopy = new Texture2D(arenaTextureReference.width,arenaTextureReference.height);
                                arenaTextureCarbonCopy.filterMode = FilterMode.Point;
                                arenaTextureCarbonCopy.SetPixels32(arenaTextureReference.GetPixels32());
                                arenaTextureCarbonCopy.Apply();
                                
                            }
                            r.material.mainTexture = arenaTextureCarbonCopy;

                        }
                    }

                }

                //The part that handles changing

                if (arenaTileID >= 0)
                {
                    Debug.Log("Setting Tileset");
                    if (tilesets[arenaTileID].tileset != null)
                    {
                        arenaTextureCarbonCopy.SetPixels32(tilesets[arenaTileID].tileset.GetPixels32());
                        arenaTextureCarbonCopy.Apply();
                    }
                    EditStage();
                    Pathfinder.GenerateNodeMap(self.selectedRoom.tileGrid);

                    // Ultra Council Challenge spawn
                    if (ContentLoader.StageEffects)
                    {
                        switch (tilesets[arenaTileID].name) {
                            case "Domicile":
                                self.gameObject.AddComponent<UltraCouncilChallenge>().self = self;
                                break;
                            case "Dorm":
                                self.gameObject.AddComponent<ContestantAssault>().self = self;
                                break;
                        }
                    }

                } else
                {
                    ResetTileSet();
                }

            };

            On.PvpStageLoader.LoadNextLevel += (On.PvpStageLoader.orig_LoadNextLevel orig, PvpStageLoader self) =>
            {
                if (self.gameObject.name== "CUSTOMLOADER")
                {
                    arenaTileID = tilesets.IndexOf(portalDict[self.gameObject]);
                } else
                {
                    arenaTileID = -1;
                }
                orig(self);
            };
            if (soloPortals)
            {
                On.PvpStageLoader.Update += (On.PvpStageLoader.orig_Update orig, PvpStageLoader self) =>
                {
                    if (self.player == null || self.portalEntered)
                    {
                        return;
                    }
                    if (self.player.IsDead || Vector2.Distance(self.player.transform.position, self.transform.position) > 2f)
                    {
                        self.player = null;
                        self.CurrentOverheadPrompt = null;
                        if (self.exitPortal != null)
                        {
                            self.exitPortal.Deactivate();
                        }
                        return;
                    }
                    if (self.player.IsAvailable && self.player.inputDevice.GetButtonDown("Interact"))
                    {
                        self.CurrentOverheadPrompt = null;
                        foreach (Player player in GameController.activePlayers)
                        {
                            player.transform.position = self.transform.position;
                            player.fall.ignoreFall.AddMod(new BoolVarStatMod("LevelLoad", true, 0));
                            player.hurtBoxObj.SetActive(false);
                        }
                        if (self.exitPortal != null)
                        {
                            self.exitPortal.Deactivate();
                            self.exitPortal.Close();
                        }
                        else
                        {
                            self.LoadNextLevel();
                        }
                        self.portalEntered = true;
                    }
                };

                On.PvpStageLoader.OnTriggerStay2D += (On.PvpStageLoader.orig_OnTriggerStay2D orig, PvpStageLoader self, Collider2D c) =>
                {
                    if (self.forceTriggerExit)
                    {
                        return;
                    }
                    if (!self.portalEntered && self.player == null)
                    {
                        self.player = Player.CheckForPlayer(c);
                        if (self.player != null && self.player.IsAvailable)
                        {
                            self.CurrentOverheadPrompt.Initialize(self.transform, self.player.inputDevice.inputScheme, self.overheadPromptHeight, 0f);
                            self.CurrentOverheadPrompt.DisplayPrompt("Interact", false, false);
                            self.exitPortal.Prepare();
                        }
                    }
                };

                On.PvpStageLoader.OnTriggerExit2D += (On.PvpStageLoader.orig_OnTriggerExit2D orig, PvpStageLoader self, Collider2D c) =>
                {
                    bool flag = Player.CheckForPlayer(c);
                    if (self.forceTriggerExit && flag)
                    {
                        self.forceTriggerExit = false;
                    }
                    if (flag && self.player != null && self.player.IsAvailable)
                    {
                        self.CurrentOverheadPrompt = null;
                        self.player = null;
                        if (self.exitPortal != null)
                        {
                            self.exitPortal.Deactivate();
                        }
                    }
                };
            }
            //Custom Dialogues

            On.DialogEntry.IsValidIndex += (On.DialogEntry.orig_IsValidIndex orig, DialogEntry self, int i, bool b) =>
            {
                if (self.messages==null)
                {
                    return true;
                }

                return orig(self, i, b);
            };

            On.DialogManager.DisplayDialog += (On.DialogManager.orig_DisplayDialog orig, DialogManager self, string dialogID, int overrideIndex, string playerIDOverride, string leftIDOverride, string rightIDOverride, bool isSign, bool skipGhostWriter) =>
            {
                if (!addedDialogues)
                {
                    addedDialogues = true;
                    foreach (ArenaDef a in tilesets)
                    {
                        Debug.Log("Adding " + a.name);
                        DialogSpeakerInfo s = new DialogSpeakerInfo("WOL-TEDS-" + a.name, a.name);

                        DialogMessage m = new DialogMessage(DialogManager.dialogDict["PvP-Statue-Default"].messages[0]);
                        m.message = a.description;
                        //m.leftActive = true;
                        m.leftSpeakerID = "WOL-TEDS-" + a.name;

                        DialogEntry e = new DialogEntry(0);
                        e.currentIndex = 0;
                        e.ID = "WOL-TED-" + a.name;
                        e.messages = new DialogMessage[1] { m };

                        DialogManager.dialogDict["WOL-TED-" + a.name] = e;
                        DialogManager.speakerDict["WOL-TEDS-" + a.name] = s;

                    }
                }
                DialogMessage dialogMessage = null;
                if (MaliceAdditions.MaliceActive && MaliceAdditions.maliceDialog.ContainsKey(dialogID))

                {
                    if (!MaliceAdditions.maliceDialog.ContainsKey(dialogID))
                    {
                        return false;
                    }
                    MaliceAdditions.maliceDialog[dialogID].CurrentIndex = overrideIndex;
                    if (MaliceAdditions.maliceDialog[dialogID].ScriptEnd)
                    {
                        MaliceAdditions.maliceDialog[dialogID].Reset();
                        return false;
                    }
                    dialogMessage = new DialogMessage(MaliceAdditions.maliceDialog[dialogID].GetMessage());
                }
                else {
                    if (!DialogManager.dialogDict.ContainsKey(dialogID))
                    {
                        return false;
                    }
                    DialogManager.dialogDict[dialogID].CurrentIndex = overrideIndex;
                    if (DialogManager.dialogDict[dialogID].ScriptEnd)
                    {
                        DialogManager.dialogDict[dialogID].Reset();
                        return false;
                    }
                    dialogMessage = new DialogMessage(DialogManager.dialogDict[dialogID].GetMessage());
                }
                
                if (DialogManager.speakerDict.ContainsKey(leftIDOverride))
                {
                    dialogMessage.OverwriteSpeaker(leftIDOverride, true);
                }
                if (DialogManager.speakerDict.ContainsKey(rightIDOverride))
                {
                    dialogMessage.OverwriteSpeaker(leftIDOverride, false);
                }
                if (dialogMessage.leftSpeakerID != null && dialogMessage.leftSpeakerID.Contains("Player"))
                {
                    if (DialogManager.speakerDict.ContainsKey(playerIDOverride))
                    {
                        dialogMessage.OverwriteSpeaker(playerIDOverride, true);
                    }
                    else
                    {
                        if (GameController.activePlayers == null || !(GameController.activePlayers[0] != null))
                        {
                            return false;
                        }
                        dialogMessage.OverwriteSpeaker(dialogMessage.leftSpeakerID = DialogManager.speakerDict[GameController.activePlayers[0].skillCategory].speakerID, true);
                    }
                }
                if (dialogMessage.rightSpeakerID != null && dialogMessage.rightSpeakerID.Contains("Player"))
                {
                    if (DialogManager.speakerDict.ContainsKey(playerIDOverride))
                    {
                        dialogMessage.rightSpeakerID = playerIDOverride;
                    }
                    else
                    {
                        if (GameController.activePlayers == null || GameController.activePlayers.Length <= 0 || !(GameController.activePlayers[0] != null))
                        {
                            return false;
                        }
                        dialogMessage.rightSpeakerID = DialogManager.speakerDict[GameController.activePlayers[0].skillCategory].speakerID;
                    }
                }
                dialogMessage.message = self.ParseMessage(dialogMessage);
                self.Activate(dialogMessage, isSign, skipGhostWriter);
                DialogManager.dialogInProgress = true;
                DialogManager.currentDialogID = dialogID;
                return true;
            };

            
        }

        public static void ResetTileSet()
        {
            if (arenaTextureCarbonCopy && arenaTextureReference)
            {
                arenaTextureCarbonCopy.SetPixels32(arenaTextureReference.GetPixels32());
                arenaTextureCarbonCopy.Apply();
            }
        }
        static bool addedDialogues = false;
        public static void AddCustomArenaPortals()
        {
            GameObject arenaPortal = GameObject.FindObjectOfType<ExitPortal>().gameObject;
            GameObject placard = GameObject.FindObjectOfType<MuseumPlacard>().gameObject;
            GameObject statue = GameObject.Find("DefaultStatue");
            foreach(ArenaDef arena in tilesets)
            {
                Debug.Log("Adding portal for " + arena.name);

                Vector3 pos = new Vector3(0, 15, 0);
                GameObject portal = GameObject.Instantiate(arenaPortal, pos+arena.position, arenaPortal.transform.rotation);
                GameObject.Instantiate(statue, pos + arena.position + Vector3.up * 1.5f, statue.transform.rotation);
                ExitPortal exitPortal = portal.GetComponent<ExitPortal>();
                exitPortal.nextLevelLoader = exitPortal.transform.Find("NextLevelTrigger").GetComponent<NextLevelLoader>();
                exitPortal.nextLevelLoader.name = "CUSTOMLOADER";
                ((PvpStageLoader)exitPortal.nextLevelLoader).pvpRoomName = "DefaultRoom";
                portalDict[exitPortal.nextLevelLoader.gameObject] = arena;
                GameObject newPlacard = GameObject.Instantiate(placard, pos + arena.position + new Vector3(0, -0.25f, 0),placard.transform.rotation);
                MuseumPlacard m = newPlacard.GetComponent<MuseumPlacard>();
                m.dialogID = "WOL-TED-" + arena.name;
                m.name = "WOL-TED-" + arena.name;
                m.useObjectNameAsDialogID = false;
            }
        }
        static Dictionary<GameObject, ArenaDef> portalDict = new Dictionary<GameObject, ArenaDef>();

        public static void EditStage()
        {
            chosenArena = tilesets[arenaTileID].layout;

            foreach (ElementalSpriteSelector o in GameObject.FindObjectsOfType<ElementalSpriteSelector>())
            {
                o.SetElement(tilesets[arenaTileID].element);
            }

            GameObject statue = null;
            List<string> Destroynames = new List<string>() { "enemy", "statue" };
            foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>())
            {
                foreach (string s in Destroynames)
                {
                    if (statue == null && o.name.ToLower()=="wizardstatue" && o.GetComponentInChildren<Collider2D>())
                    {
                        Debug.Log("Assigned a Statue for duplication");
                        statue = o;
                        foreach (Collider2D b in statue.GetComponentsInChildren<Collider2D>())
                        {
                            b.enabled = true;
                        }
                    }
                    if (o.name.ToLower().Contains(s) && (statue == null || (o!=statue && o.transform.parent!=statue)))
                    {
                        GameObject.Destroy(o);
                    }
                }
            } //Destroys the spawner fucker and redundant statues;
                    
            for (int x = 0; x < chosenArena.width; x++)
            {
                for (int y = 0; y < chosenArena.height; y++)
                {
                    Vector3 coordinate = new Vector3(x * 2 - (chosenArena.width), y * 2 - (chosenArena.height), 0);
                    if (chosenArena.GetPixel(x, y).maxColorComponent < 0.01f)
                    {
                        GameObject o = GameObject.Instantiate(statue, coordinate + offset, Quaternion.identity);
                        Statue s = o.GetComponent<Statue>();
                        s.element = tilesets[arenaTileID].element;
                        if (s.randomizeSprite)
                        {
                            s.statueRenderer.sprite = s.statueVars.GetElementalSprite(s.element);
                            s.baseRenderer.sprite = s.baseVars.GetElementalSprite(s.element);
                            s.floorRenderer.sprite = s.floorVars.GetElementalSprite(s.element);
                        }

                        foreach(SortSpriteLayer layer in s.GetComponentsInChildren<SortSpriteLayer>())
                        {
                            layer.enabled = true;
                        }

                    }
                }
            }
            GameObject.Destroy(statue);
            
        }


    }
    public class ArenaDef
    {
        public string name = "Arena";
        public string description = "No Special Effects...";
        public Vector3 position = Vector3.zero;
        public Texture2D tileset;
        public Texture2D layout;
        public ElementType element = ElementType.Neutral;
        public int id = -1;
    }
}

