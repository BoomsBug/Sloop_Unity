using System;
using System.Collections.Generic;

namespace Sloop.NPC.dialogue
{
    [Serializable]
    public class DialogueDatabaseJson
    {
        public List<DialogueEntry> entries = new();
    }

    [Serializable]
    public class DialogueEntry
    {
        public string id;
        public string type;     // "bark" or "interact" (for now)
        public List<string> tags;   // e.g "alignment:ruthless", "role:merchant", "willingness:friendly"
        public string text;
    }
}