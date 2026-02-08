using System.IO;
using Sloop.NPC.dialogue;
using UnityEngine;

namespace Sloop.NPC.Dialogue
{
    public static class DialogueDatabaseLoader
    {
        public static DialogueDatabaseJson LoadFromStreamingAssets(string fileName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"Dialogue JSON not found at: {path}");
                return new DialogueDatabaseJson();
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<DialogueDatabaseJson>(json) ?? new DialogueDatabaseJson();
        }
    }
}