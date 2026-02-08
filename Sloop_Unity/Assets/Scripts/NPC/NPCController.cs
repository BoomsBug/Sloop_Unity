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


        [Header("Identity")]
        [SerializeField] private int npcIndex = 0;


        //[Header("Willingness Settings")]
        //[Tooltip("Base willingness before honor/alignment effects. Will tune this later or derive it from traits.")]
        //[Range(0, 100)]
        //[SerializeField] private int baseWillingness = 0;

        //[Tooltip("How strongly honor affects willingness (0..100).")]
        //[Range(0, 100)]
        //[SerializeField] private int honorInfluence = 40;

        [Header("Generation (for manually placed NPCs)")]
        [SerializeField] private NPCGenerator generator;     // assign in inspector (can be a scene singleton)
        [SerializeField] private int npcSeed = 12345;
        [SerializeField] private bool generateOnStart = true;


        [Header("UI")]
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

        private void Start()
        {
            if (!generateOnStart) return;

            if (!HasValidData())
            {
                if (generator == null)
                {
                    Debug.LogWarning($"{name}: NPC has no data and no generator assigned. Assign an NPCGenerator in the Inspector.");
                    return;
                }

                data = generator.Generate(
                    npcSeed,
                    NPCRole.Civilian,              // TEMP
                    MoralAlignment.Neutral,        // TEMP
                    0,                             // islandID TEMP
                    npcIndex                       // already exists
                );

                gameObject.name = $"NPC_{data.name}_{data.role}";
            }
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

            if (!HasValidData() && generator != null)
            {
                data = generator.Generate(
                    npcSeed,
                    NPCRole.Civilian,              // TEMP
                    MoralAlignment.Neutral,        // TEMP
                    0,                             // islandID TEMP
                    npcIndex                       // already exists
                );

            }

            if (data == null)
            {
                Debug.LogWarning("NPCController: No NPCData assigned. Did you forget to generate/initialize this NPC?");
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
                $"Calculated Willingness (-25..25): {willingness}\n" +
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


        // This is a variation to the willingness calculation below. 
        // It stays here as it has some additions to the algorithm I may want to implement later.
        /*
        /// <summary>
        /// Core willingness formula: correlates player honor with NPC alignment.
        /// This is intentionally simple for Sprint 1.
        /// </summary>
        public int CalculateWillingness(int playerHonor)
        {
            // Normalize honor around 50: [-50..+50]
            int centered = playerHonor - 50;

            // Alignment decides whether NPC likes honorable or dishonorable behavior
            // Honorable NPC: higher honor increases willingness
            // Ruthless NPC: higher honor decreases willingness
            // Neutral NPC: mild effect
            float alignmentSign = data.alignment switch
            {
                MoralAlignment.Honorable => +1f,
                MoralAlignment.Ruthless => -1f,
                _ => +0.25f
            };

            // Influence scales how much honor matters
            float delta = (centered / 50f) * honorInfluence * alignmentSign;

            int result = Mathf.RoundToInt(baseWillingness + delta);

            
            // Optional: quick trait nudges (tiny, safe, easy to expand)
            // This is an experimental idea compared to my simpler version
            if (data.traits != null)
            {
                foreach (var t in data.traits)
                {
                    if (string.IsNullOrWhiteSpace(t)) continue;
                    string trait = t.ToLowerInvariant();

                    if (trait.Contains("greedy")) result -= 3;
                    if (trait.Contains("loyal")) result += 3;
                    if (trait.Contains("suspicious")) result -= 2;
                    if (trait.Contains("charming")) result += 2;
                }
            }
            

            return Mathf.Clamp(result, 0, 100);
        }
        */

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
