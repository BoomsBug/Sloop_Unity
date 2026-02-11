using UnityEngine;
using Sloop.NPC.Dialogue;

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

        [SerializeField] private MerchantShop merchantShop;



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
            if (merchantShop == null)
                merchantShop = GetComponent<MerchantShop>();
        }


        private void Update()
        {
            if (!playerInRange) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
                OpenDialogueUI();
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

            WillingnessBand band = 
                willingness <= 25 ? WillingnessBand.Hostile :
                willingness >= 25 ? WillingnessBand.Friendly :
                WillingnessBand.Neutral;

            // Print two dialogue types to console (testing)
            var db = DialogueService.Instance?.Database;

            string bark = DialogueSelector.GetLine(db, DialogueType.Bark, data, band);
            string interactLine = DialogueSelector.GetLine(db, DialogueType.Interact, data, band);

            Debug.Log($"[BARK] {bark}");
            Debug.Log($"[INTERACT] {interactLine}");
            // Later will use UI instead of console print
            

            // Print NPC Data to console
            Debug.Log(
                "=== NPC INTERACTION ===\n" +
                $"Name: {data.name}\n" +
                $"Alignment: {data.alignment}\n" +
                $"Traits: {(data.traits == null ? "(none)" : string.Join(", ", data.traits))}\n" +
                $"Role: {data.role}\n" +
                $"Player Honor (0..100): {playerHonor}\n" +
                $"Calculated Willingness (-50..50): {willingness} => {band}\n" +
                "======================="
            );
        }


        private int GetPlayerHonor()
        {
            // OLD PLAYERHONOR script
            //if (playerTransform == null) return 50; // safe default

            // Replace this with your actual player honor component name if different
            //var honorComp = playerTransform.GetComponent<Sloop.Player.PlayerHonor>();
            //if (honorComp == null)
                //return 50;

            //return Mathf.Clamp(honorComp.Honor, 0, 100);

            // NEW PlayerStateManager script holds playerHonor
            var ps = Sloop.Player.PlayerStateManager.Instance;
            if (ps == null) return 50;
            return Mathf.Clamp(ps.Honor, 0, 100);
        }


        // Essentially compares NPC Moral Alignment with player honor and determines their "willingness" to help the player.
        // NPC + Player Same Honor level => high willingness
        // NPC + Player different Honor lvl => Low willingness
        // Super simple design for now, may expand later
        // also, will convert from numbers to a willingness "band" (a string such as: Hostile, neutral, or friendly)
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

        private static WillingnessBand ToBand(int willingness)
        {
            if (willingness <= -25) return WillingnessBand.Hostile;
            if (willingness >= 25) return WillingnessBand.Friendly;
            return WillingnessBand.Neutral;
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

        // Below is all dialogue-related functions
        public string GetDisplayName()
        {
            if (data == null) return $"NPC #{npcIndex}";
            return $"{data.name} - {data.role}";
        }

        public void OpenDialogueUI()
        {
            var ui = FindFirstObjectByType<NPCDialogueUI>(FindObjectsInactive.Include);
            if (ui == null)
            {
                Debug.LogWarning("No NPCDialogueUI found in scene.");
                return;
            }

            ui.ShowForNPC(this);
        }

        public void UI_BarkPressed(NPCDialogueUI ui)
        {
            if (!EnsureInitialized(ui)) return;

            int willingness = CalculateWillingness(GetPlayerHonor());
            var band = ToBand(willingness);

            var db = DialogueService.Instance?.Database;
            string bark = DialogueSelector.GetLine(db, DialogueType.Bark, data, band);

            ui.SetLine(bark);
            ui.HideChoices();
        }

        public void UI_InteractPressed(NPCDialogueUI ui)
        {
            if (!EnsureInitialized(ui)) return;

            int willingness = CalculateWillingness(GetPlayerHonor());
            var band = ToBand(willingness);

            var db = DialogueService.Instance?.Database;
            string line = DialogueSelector.GetLine(db, DialogueType.Interact, data, band);

            ui.SetLine(line);

            // Role-Specific follow-up Choices
            switch (data.role)
            {
                case NPCRole.Deckhand:
                    ShowDeckhandChoices(ui, willingness);
                    break; 

                case NPCRole.Merchant:
                    ShowMerchantChoices(ui, willingness);
                    break; 

                case NPCRole.Civilian:
                default:
                    ShowCivilianChoices(ui, willingness);
                    break; 
            }
        }

        private bool EnsureInitialized(NPCDialogueUI ui)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.name))
            {
                ui.SetLine("(NPC not initialized. Did IslandNPCManager run?)");
                return false;
            }
            if (DialogueService.Instance == null)
            {
                ui.SetLine("(DialogueService missing in scene.)");
                return false;
            }
            return true;
        }

        private void ShowDeckhandChoices(NPCDialogueUI ui, int willingness)
        {
            int hireCost = 100; // TEMP UNTIL RESOURCE SYSTEM BUILT
            // Later: baseCost * multiplier based on willingness + traits

            ui.ShowChoices(
                $"Hire ({hireCost} gold)",
                () =>
                {
                    ui.SetLine("You hired the deckhand. (stub)");
                    ui.HideChoices();
                    // TODO: call CrewManager.AddCrew(data), deduct gold, etc.
                },
                "Maybe Later",
                () =>
                {
                    ui.SetLine("Maybe another time.");
                    ui.HideChoices();
                }
            );
        }

        /*
        private void ShowMerchantChoices(NPCDialogueUI ui, int willingness)
        {
            //int priceMultiplier = 1; // TEMP UNTIL RESOURCE SYSTEM BUILT
            // Later: basePrice * multiplier based on willingness + traits

            ui.ShowChoices(
                $"Open Shop",
                () =>
                {
                    ui.SetLine("Shop Opened. (stub)");
                    ui.HideChoices();
                    // TODO: fire event OnShopOpen(this, willingness)
                    // TODO: open shop UI, apply price multiplier, close shop, return
                },
                "Nevermind",
                () =>
                {
                    ui.SetLine("Come back anytime.");
                    ui.HideChoices();
                }
            );
        }*/

        private void ShowMerchantChoices(NPCDialogueUI ui, int willingness)
        {
            ui.ShowChoices(
                "Open Shop",
                () =>
                {
                    if (merchantShop == null)
                    {
                        ui.SetLine("(MerchantShop missing on this NPC.)");
                        ui.HideChoices();
                        return;
                    }

                    merchantShop.OpenShop(ui);
                },
                "Nevermind",
                () =>
                {
                    ui.SetLine("Come back anytime.");
                    ui.HideChoices();
                }
            );
        }


        
        private void ShowCivilianChoices(NPCDialogueUI ui, int willingness)
        {
            var band = ToBand(willingness);

            ui.ShowChoices(
                $"Ask Directions",
                () =>
                {
                    if (band == WillingnessBand.Friendly || band == WillingnessBand.Neutral)
                    {
                        ui.SetLine("I heard of ancient treasure due East (True)");
                    }
                    else
                    {
                        ui.SetLine("I heard of ancient treasure due West (False)");
                    }
                    ui.HideChoices();
                },
                "Maybe Later",
                () =>
                {
                    ui.SetLine("Maybe another time.");
                    ui.HideChoices();
                }
            );
        }



    }
}
