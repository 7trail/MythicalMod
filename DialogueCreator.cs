using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using On;
namespace Mythical
{
    
    class DialogueCreator
    {
        public struct DialogTemp
        {
            public List<string> statements;
            public int optionsPoint;
            public string id;
        }
        public static DialogTemp GenerateDialog(List<string> statements, int optionsPoint = -1)
        {
            DialogTemp entry = new DialogTemp();
            entry.statements = statements;
            entry.optionsPoint = optionsPoint;
            return entry;
        }
        static List<DialogTemp> tempDialogues = new List<DialogTemp>();
        public static void Init()
        {
            On.DialogManager.InitDialogDicts += InitDialogueDicts;
        }

        public static void InitDialogueDicts(On.DialogManager.orig_InitDialogDicts orig, DialogManager self, ChaosLang lang)
        {
            orig(self,lang);
            foreach(DialogTemp temp in tempDialogues)
            {
                DialogEntry e = MakeEntry(temp);
                e.ID = temp.id;
                if (dict.ContainsKey(temp.id))
                {
                    dict[temp.id] = e;
                }
                else
                {
                    dict.Add(temp.id, e);
                }
            }
            foreach(KeyValuePair<string,DialogEntry> pair in dict)
            {
                if (dict.ContainsKey(pair.Key))
                {
                    dict[pair.Key] = pair.Value;
                }
                else
                {
                    dict.Add(pair.Key, pair.Value);
                }
            }
        }
        public static DialogEntry MakeEntry(DialogTemp temp)
        {
            DialogEntry entry = new DialogEntry();
            List<DialogMessage> messages = new List<DialogMessage>();
            foreach (string str in temp.statements)
            {
                DialogMessage msg = new DialogMessage(DialogManager.dialogDict.ElementAt(0).Value.messages[0]);
                msg.message = str;
                msg.leftActive = false;
                msg.leftSpeakerInitialized = false;
                msg.rightActive = false;
                msg.rightSpeakerInitialized = false;

                messages.Add(msg);
            }
            entry.messages = messages.ToArray();
            return entry;
        }
        public static void RegisterDialogue(string id, DialogTemp entry)
        {
            entry.id = id;
            tempDialogues.Add(entry);
        }
        public static Dictionary<string, DialogEntry> dict = new Dictionary<string, DialogEntry>();
    }
}
