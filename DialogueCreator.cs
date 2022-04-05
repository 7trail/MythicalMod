using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mythical
{
    class DialogueCreator
    {
        public static DialogEntry GenerateDialog(List<string> statements, int optionsPoint=-1)
        {
            DialogEntry entry = new DialogEntry();
            List<DialogMessage> messages = new List<DialogMessage>();
            foreach(string str in statements)
            {
                DialogMessage msg = new DialogMessage(DialogManager.dialogDict.ElementAt(0).Value.messages[0]);
                msg.message = str;
                msg.leftActive = false;
                msg.leftSpeakerInitialized = false;
                msg.rightActive = false;
                msg.rightSpeakerInitialized = false;

                messages.Add(msg);
            }
            return entry;
        }

        public static void RegisterDialogue(string id, DialogEntry entry)
        {
            if (DialogManager.dialogDict.ContainsKey(id))
            {
                DialogManager.dialogDict[id] = entry;
            } else
            {
                DialogManager.dialogDict.Add(id, entry);
            }
        }
    }
}
