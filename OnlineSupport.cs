using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Mythical
{
    internal class OnlineSupport
    {
        public static string url = " https://stimulatingcomplexdirectories-json-store-1--coder100.repl.co/db/d2ca6403-aacb-4e24-b57b-46ac352e5e38";

        public static Dictionary<string, object> player0input = new Dictionary<string, object>();
        public static Dictionary<string, object> player1input = new Dictionary<string, object>(); //TODO - Make the part of the code that overrides P1 inputs with the online vector
        public static void Hooks()
        {
            On.ChaosInputDevice.GetButton += (On.ChaosInputDevice.orig_GetButton orig, ChaosInputDevice self, string buttonName) =>
            {
                bool b = orig(self, buttonName);
                if (self.playerID == 0)
                {
                    player0input[buttonName] = b;
                }
                return b;
            };
            On.ChaosInputDevice.GetButtonDown += (On.ChaosInputDevice.orig_GetButtonDown orig, ChaosInputDevice self, string buttonName) =>
            {
                bool b = orig(self, buttonName);
                if (self.playerID == 0)
                {
                    player0input[buttonName] = b;
                }
                return b;
            };
            On.ChaosInputDevice.GetButtonUp += (On.ChaosInputDevice.orig_GetButtonUp orig, ChaosInputDevice self, string buttonName) =>
            {
                bool b = orig(self, buttonName);
                if (self.playerID == 0)
                {
                    player0input[buttonName] = b;
                }
                return b;
            };
        }

        
        public static IEnumerator UploadData(string uri)
        {
            string output = JsonUtility.ToJson(player0input);

            using (UnityWebRequest webRequest = UnityWebRequest.Post(uri+"/"+ContentLoader.OnlineID.Value, output))
            {
                float time = Time.time;
                yield return webRequest.Send();
                response = webRequest.downloadHandler.text;
                Debug.Log(response);
                Debug.Log("Time taken: " + (Time.time - time));
            }

            yield return GetRequest(url);
        }

        public static IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                float time = Time.time;   
                yield return webRequest.Send();
                response = webRequest.downloadHandler.text;
                Debug.Log(response);
                Debug.Log("Time taken: " + (Time.time - time));
            }
        }

        public static string response;

    }
}
