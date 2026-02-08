using Sloop.NPC.dialogue;
using UnityEngine;

namespace Sloop.NPC.Dialogue
{
    public class DialogueService : MonoBehaviour
    {
        public static DialogueService Instance { get; private set; }

        [SerializeField] private string fileName = "dialogue.json";

        public DialogueDatabaseJson Database { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Database = DialogueDatabaseLoader.LoadFromStreamingAssets(fileName);
            Debug.Log($"DialogueService: Loaded {Database.entries.Count} dialogue entries from {fileName}");
        }
    }
}