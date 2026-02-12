using System;
using System.Collections.Generic;
using UnityEngine;
using Sloop.World;

namespace Sloop.NPC
{
    public class IslandNPCManager : MonoBehaviour
    {
        [Header("Generation")]
        [SerializeField] private NPCGenerator generator;

        [Header("Scene NPCs (manual placement)")]
        [Tooltip("Assign the 5 NPCControllers for this island. Their npcIndex should be 0..4")]
        [SerializeField] private List<NPCController> npcControllers = new();

        // Optional: inspector defaults for testing if context is missing
        [Header("Fallback (Editor Testing)")]
        [SerializeField] private int fallbackWorldSeed = 111111;
        [SerializeField] private int fallbackIslandID = 0;
        [SerializeField] private MoralAlignment fallbackIslandAlignment = MoralAlignment.Neutral;

        private bool generated;

        [ContextMenu("Generate + Assign Island NPCs (Editor)")]
        public void GenerateAndAssign()
        {
            GenerateAndAssign(fallbackWorldSeed, fallbackIslandID, fallbackIslandAlignment);
        }

        public void GenerateAndAssign(int worldSeed, int islandID, MoralAlignment islandAlignment)
        {
            if (generated) return;
            generated = true;

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

            // Deterministic island RNG from (worldSeed, islandID)
            int islandSeed = NPCSeedUtility.Combine(worldSeed, islandID, 0);
            var islandRng = new System.Random(islandSeed);

            // Enforce unique names per island
            var namePool = generator.GetNamePool();
            if (namePool.Count == 0)
            {
                Debug.LogWarning("IslandNPCManager: Name pool is empty.");
                return;
            }

            foreach (var ctrl in npcControllers)
            {
                if (ctrl == null) continue;

                int idx = ctrl.NpcIndex;
                var role = NPCSlotRules.RoleForSlot(idx);

                int npcSeed = NPCSeedUtility.Combine(worldSeed, islandID, idx);
                string uniqueName = PopRandom(namePool, islandRng);

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

            Debug.Log($"IslandNPCManager: Assigned NPCs for islandID={islandID} alignment={islandAlignment} worldSeed={worldSeed}");
        }

        private static string PopRandom(List<string> pool, System.Random rng)
        {
            int i = rng.Next(0, pool.Count);
            string chosen = pool[i];
            pool.RemoveAt(i);
            return chosen;
        }

        private void Start()
        {
            // If docking provided context, use it
            // (IslandVisitContext should be in a namespace you can reference)
            int worldSeed = Sloop.World.IslandVisitContext.WorldSeed;
            int islandID  = Sloop.World.IslandVisitContext.IslandID;
            string morality = Sloop.World.IslandVisitContext.Morality;

            MoralAlignment islandAlignment = morality switch
            {
                "R" => MoralAlignment.Ruthless,
                "H" => MoralAlignment.Honorable,
                _   => MoralAlignment.Neutral
            };

            // If context wasn't set (e.g., scene opened directly), fall back
            if (worldSeed == 0 && islandID == 0 && string.IsNullOrEmpty(morality))
            {
                GenerateAndAssign(fallbackWorldSeed, fallbackIslandID, fallbackIslandAlignment);
            }
            else
            {
                GenerateAndAssign(worldSeed, islandID, islandAlignment);
            }
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
