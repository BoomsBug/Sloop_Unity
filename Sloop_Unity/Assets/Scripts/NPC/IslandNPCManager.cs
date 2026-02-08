using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sloop.NPC
{
    public class IslandNPCManager : MonoBehaviour
    {
        [Header("Island Identity")]
        [SerializeField] private int worldSeed = 111111;       // TEMP until real world seed exists
        [SerializeField] private int islandID = 0;
        [SerializeField] private MoralAlignment islandAlignment = MoralAlignment.Neutral;

        [Header("Generation")]
        [SerializeField] private NPCGenerator generator;

        [Header("Scene NPCs (manual placement)")]
        [Tooltip("Assign the 5 NPCControllers for this islland. Their npcIndex should be 0..4")]
        [SerializeField] private List<NPCController> npcControllers = new();


        private bool generated; // Test-guard so generation doesn't double-run

        [ContextMenu("Generate + Assign Island NPCS")]
        public void GenerateAndAssign()
        {
            if (generator == null)
            {
                Debug.LogWarning("IslandNPCManager: No NPCGenerator assigned.");
                return;
            }

            if (npcControllers == null || npcControllers.Count == 0)
            {
                Debug.LogWarning("IslandNPCManager: npcControllers list is empty.");
                return;
            }

            // Build a deterministic island RNG from (worldSeed, islandID)
            int islandSeed = NPCSeedUtility.Combine(worldSeed, islandID, 0);
            var islandRng = new System.Random(islandSeed);

            // Create a local name pool to enforce uniqueness on this island
            var namePool = generator.GetNamePool(); // GenNamePool() exists in NPCGenerator.cs
            if (namePool.Count == 0)
            {
                Debug.LogWarning("IslandNPCManager: Name pool is empty.");
                return;
            }

            // Assign for each controller based on its npcIndex
            foreach (var ctrl in npcControllers)
            {
                if (ctrl == null) continue;

                int idx = ctrl.NpcIndex;    // expose read-only property
                var role = NPCSlotRules.RoleForSlot(idx);

                // Derive npc seed from world+island+index
                int npcSeed = NPCSeedUtility.Combine(worldSeed, islandID, idx);

                // Pick Unique name deterministically from islandRng
                string uniqueName = PopRandom(namePool, islandRng);

                // Generate NPCData (generator will still pick traits etc.)
                NPCData data = generator.Generate(
                    npcSeed,
                    role,
                    islandAlignment,
                    islandID,
                    idx,
                    forcedName: uniqueName
                );

                ctrl.Initialize(data);
            }

            Debug.Log($"IslandNPCManager: Assigned NPCs for islandID={islandID} alignment={islandAlignment}");
        }

        private static string PopRandom(List<string> pool, System.Random rng)
        {
            int i = rng.Next(0, pool.Count);
            string chosen = pool[i];
            pool.RemoveAt(i);
            return chosen;
        }

        private void Awake()
        {
            if (generated) return;
            generated = true;
            GenerateAndAssign();
        }

        private void Start()
        {
            if (generated) return;
            generated = true;
            GenerateAndAssign();
        }

        /// <summary>
        /// 
        /// For now, worldSeed + islandSeed are set in inspector. 
        /// The following function will be used to retreive these actual values before loading the port scene (provided a gameManager doesn't do it)
        /// 
        /// How it works:
        /// Before loading port scene:
        ///     set IslandContext.WorldSeed = ...
        ///     set IslandContext.IslandID = ...
        ///     set IslandContext.IslandAlignment = ...
        ///    
        /// Then in IslandNPCManager.Awake()
        ///     read values and generate.
        /// 
        /// Note: Having a GameManager that does this instead will be cleaner, but this is option B
        /// </summary>
        /*
        public static class IslandContext
        {
            public static int WorldSeed;
            public static int IslandID;
            public static MoralAlignment IslandAlignment;
        }
        */


    }
}
