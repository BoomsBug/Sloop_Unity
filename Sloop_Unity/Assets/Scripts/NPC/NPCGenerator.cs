using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sloop.NPC
{
    /// <summary>
    /// Deterministically generates NPC identity from a seed and island context.
    /// IslandNPCManager is responsible for choosing island alignment and enforcing unique names.
    /// </summary>
    public class NPCGenerator : MonoBehaviour
    {
        [Header("Data Source")]
        [SerializeField] private NPCDatabase database;

        [Header("Optional Debug")]
        [SerializeField] private bool logGeneratedNPC = false;

        private void OnValidate()
        {
            if (database != null) database.Validate();
        }

        public NPCData Generate(int seed, NPCRole role, MoralAlignment alignment, int islandID, int npcIndex, string forcedName = null)
        {
            if (database == null)
                throw new InvalidOperationException("NPCGenerator: No NPCDatabase assigned.");

            var rng = new System.Random(seed);
            int traitCount = rng.Next(database.minTraits, database.maxTraits + 1);

            var npc = new NPCData
            {
                name = string.IsNullOrWhiteSpace(forcedName)
                    ? PickOrFallback(database.names, rng, "Nameless")
                    : forcedName,
                role = role,
                alignment = alignment,
                islandID = islandID,
                npcIndex = npcIndex,
                traits = PickUnique(database.traits, traitCount, rng)
            };                                                           

            if (logGeneratedNPC)
                Debug.Log($"Generated NPC:\n{npc}");

            return npc;
        }


        // ---------- Helpers ----------
        private static string PickOrFallback(List<string> list, System.Random rng, string fallback)
        {
            if (list == null || list.Count == 0) return fallback;
            int idx = rng.Next(0, list.Count);
            return string.IsNullOrWhiteSpace(list[idx]) ? fallback : list[idx];
        }


        // Deterministic unique selection (shuffle indices then take first N)
        private static List<string> PickUnique(List<string> list, int count, System.Random rng)
        {
            var result = new List<string>(count);

            if (list == null || list.Count == 0)
                return result;

            // Clamp count to available unique entries
            int max = Mathf.Min(count, list.Count);

            // Copy indices and shuffle deterministically
            var indices = new List<int>(list.Count);
            for (int i = 0; i < list.Count; i++) indices.Add(i);

            // Fisher–Yates shuffle
            for (int i = indices.Count - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                (indices[i], indices[j]) = (indices[j], indices[i]);
            }

            // Take first max, skipping blanks
            int k = 0;
            while (result.Count < max && k < indices.Count)
            {
                string candidate = list[indices[k]];
                if (!string.IsNullOrWhiteSpace(candidate))
                    result.Add(candidate);
                k++;
            }

            return result;
        }


        public List<string> GetNamePool()
        {
            // Return a COPY (remove used names).
            if (database == null || database.names == null) return new List<string>();
            return new List<string>(database.names);
        }



        // NPC: Type: Crewmember, merchant, civ
        // 

        // Called on instance of Player interaction
        // Given NPC Moral Alignment: their willingness to "help" the player is determined by:

        //  

        //      (NPC)          (Player)
        //    Honorable   +   High Honor: =>      (+) willingness
        //    Honorable   +   Low Honor: =>       (-) willingness
        //    Ruthless    +   High Honor: =>      (-) willingness
        //    Ruthless    +   Low Honor: =>       (+) willingness
        // 
        // Any state + Neutral => Willingness State
        //      Neutral + Any honor => 
        // The caller will determine the willingness of the NPC at instance


        /// The following will be how willingness directly impacts player experience ans thus the emergent narrative.
        /// This is simple, can expand much more later.
        /// For now, system of willingness is built, we can add how this affects the game state later.
        ///
        /// To be Called by...
        ///     if willingness low:
        ///         if npc.class == Merchant:
        ///             merchant.saleMultiplier = x1.7;
        ///         if npc.class == civ:
        ///             civ.directionClue{False}.pickRandom();
        ///         if npc.class == deckHand:
        ///             hirabilityMultiplier = x;
        /// 
        ///     if willingness High:
        ///         if npc.class == Merchant:
        ///             merchant.saleMultiplier = x0.4;
        ///         if npc.class == civ:
        ///             civ.directionClue{True}.pickRandom();
        ///         if npc.class == deckHand:
        ///             hirabilityMultiplier = x0.4 to Hiring Cost;
        /// 
        ///     if willingness neutral:
        ///         if npc.class == civ:
        ///             civ.directionClue{pickRandom()}.pickRandom();
        ///

        ///
        /// */

    }   
}
  