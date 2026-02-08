using UnityEngine;


namespace Sloop.NPC
{
    [RequireComponent(typeof(Collider2D))]
    public class NPCController : MonoBehaviour
    {
        [Header("NPC Data (runtime)")]
        [SerializeField] private NPCData data;

        [Header("Interaction")]
        //[SerializeField] private KeyCode interactKey = KeyCode.E; // Will change back to this for 2022 Unity version
        [SerializeField] private string playerTag = "Player";

        [Header("Identity (slot)")]
        [SerializeField] private int npcIndex = 0;
        public int NpcIndex => npcIndex;

        //[Header("UI")]
        //[SerializeField] private NPCDialogueUI dialogueUI; // assign in inspector or find at runtime

        private bool playerInRange;
        private Transform playerTransform;


        private bool HasValidData()
        {
            return data != null && !string.IsNullOrWhiteSpace(data.name);
        }


        private void Reset()
        {
            // Ensure the collider is set as trigger for interaction
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        /// <summary>
        /// Called by spawner/generator after instantiating this NPC.
        /// </summary>
        public void Initialize(NPCData npcData)
        {
            data = npcData;
            // Optional: rename GameObject for debugging
            gameObject.name = $"NPC_{data.name}_{data.role}";
        }

        private void Awake()
        {
            //if (dialogueUI == null)
                //dialogueUI = FindFirstObjectByType<NPCDialogueUI>();
        }


        private void Update()
        {
            if (!playerInRange) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }


        private void Interact()
        {
            if (data == null)
            {
                Debug.LogWarning($"{name}: NPC not initialized (no NPCData). Did IslandNPCManager run?");
                return;
            }

            int playerHonor = GetPlayerHonor();
            int willingness = CalculateWillingness(playerHonor);

            Debug.Log(
                "=== NPC INTERACTION ===\n" +
                $"Name: {data.name}\n" +
                $"Alignment: {data.alignment}\n" +
                $"Traits: {(data.traits == null ? "(none)" : string.Join(", ", data.traits))}\n" +
                $"Role: {data.role}\n" +
                $"Player Honor (0..100): {playerHonor}\n" +
                $"Calculated Willingness (-50..50): {willingness}\n" +
                "======================="
            );
        }


        private int GetPlayerHonor()
        {
            if (playerTransform == null) return 50; // safe default

            // Replace this with your actual player honor component name if different
            var honorComp = playerTransform.GetComponent<Sloop.Player.PlayerHonor>();
            if (honorComp == null)
                return 50;

            return Mathf.Clamp(honorComp.Honor, 0, 100);
        }


        // Essentially compares NPC Moral Alignment with player honor and determines their "willingness" to help the player.
        // NPC + Player Same Honor level => high willingness
        // NPC + Player different Honor lvl => Low willingness
        private int CalculateWillingness(int playerHonor)
        {
            // Willingness now centered at 0
            int willingness = 0;

            bool highHonor = playerHonor >= 50;

            switch (data.alignment)
            {
                case MoralAlignment.Honorable:
                    willingness += highHonor ? +25 : -25;
                    break;

                case MoralAlignment.Ruthless:
                    willingness += highHonor ? -25 : +25;
                    break;

                case MoralAlignment.Neutral:
                default:
                    willingness += highHonor ? +5 : -5;
                    break;
            }

            return Mathf.Clamp(willingness, -50, 50);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            playerInRange = true;
            playerTransform = other.transform;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            playerInRange = false;
            playerTransform = null;
        }
    }
}
