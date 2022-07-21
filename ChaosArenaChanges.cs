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
        static List<Texture2D> arenas = new List<Texture2D>();
        static Texture2D chosenArena=null;
        static Vector3 offset = new Vector3(24, -22, 0);
        static int arenaCount = 3;
        public static void Init()
        {
            
            

            for(int i = 0; i < arenaCount; i++)
            {
                arenas.Add(ImgHandler.LoadTex2D("Arenas/arena" + (i + 1)));
            }

            
            
            On.PvpController.SetStageLayout += (On.PvpController.orig_SetStageLayout orig, PvpController self) =>
            {
                
                if (ContentLoader.StageEffects&& PvpController.selectedRoomName.Contains("Chaos"))
                {
                    chosenArena = arenas[UnityEngine.Random.Range(0, arenas.Count)];

                    GameObject statue =null;
                    List<string> Destroynames = new List<string>() {"spawner" ,"statue"};
                    foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>())
                    {
                        foreach (string s in Destroynames)
                        {
                            if (o.name.ToLower().Contains("chaos") && o.name.ToLower().Contains("statue"))
                            {
                                statue = o;
                                statue.GetComponentInChildren<BoxCollider2D>().enabled = true;
                            }
                            if (o.name.ToLower().Contains(s) && (statue == null || (o!=statue && o.transform.parent!=statue)))
                            {
                                GameObject.Destroy(o);
                            }
                        }
                    } //Destroys the spawner fucker and redundant statues;
                    
                    for(int x=0;x<chosenArena.width;x++)
                    {
                        for (int y = 0; y < chosenArena.width; y++)
                        {
                            Vector3 coordinate = new Vector3(x*2-(chosenArena.width),y*2-(chosenArena.height),0);
                            if (chosenArena.GetPixel(x, y).maxColorComponent < 0.01f)
                            {
                                GameObject.Instantiate(statue, coordinate + offset, Quaternion.identity);
                            }
                        }
                    }
                    GameObject.Destroy(statue);
                }
                orig(self);
            };
        }

    }
}
