using System;
using System.Collections.Generic;
using UnityEngine;

/// NPC-System Change Proposal
///     MIGHT BE BETTER to have each island port represent a “Faction”. 
///     Where each NPC on the island is either “Ruthless”, or “Neutral”, or “Honourable”
///     Can scrap the trait idea
///     Replace traits with “modifiers”
///	      Such as “prone to illness”, “psychotic”, “glutton”, etc. etc.
/// 

namespace Sloop.NPC
{
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

        /// Adjusting code to generate on a per island basis 5 NPC's that are morally aligned
        ///
        public NPCData Generate(int seed, NPCRole role, MoralAlignment alignment, int islandID, int npcIndex, string forcedName = null)
        {
            if (database == null)
                throw new InvalidOperationException("NPCGenerator: No NPCDatabase assigned.");

            // Temporarily use System.Random for seed generation. Will likely be getting that seed from the WorldGen script once it is built
            var rng = new System.Random(seed);

            /// Here is current Data gen, will modify to fit design spec
            /// 
            /// 
            /// 
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

            // Alignment derived from traits (simple scoring)
            //npc.alignment = DeriveAlignment(npc.traits);

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


        // Algorithm researched online
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
        // The function below simply calculates the MoralAlignment of the NPC and returns it.
        // The caller will determine the willingness of the NPC at instance
        private static MoralAlignment DeriveAlignment(List<string> traits)
        {
            if (traits == null || traits.Count == 0)
                return MoralAlignment.Neutral;

            int score = 0;

            foreach (var t in traits)
            {
                if (string.IsNullOrWhiteSpace(t)) continue;
                string trait = t.Trim().ToLowerInvariant();

                // Honorable-ish
                if (trait.Contains("kind") ||
                    trait.Contains("loyal") ||
                    trait.Contains("respectful") ||
                    trait.Contains("brave") ||
                    trait.Contains("generous"))
                {
                    score += 1;
                }

                // Ruthless-ish
                if (trait.Contains("bloodthirsty") ||
                    trait.Contains("cruel") ||
                    trait.Contains("greedy") ||
                    trait.Contains("vengeful"))
                {
                    score -= 1;
                }
            }

            if (score >= 1) return MoralAlignment.Honorable;
            if (score <= -1) return MoralAlignment.Ruthless;
            return MoralAlignment.Neutral;
        }




        /*
        // The rest of this file contains experimental functions for the purpose of building a file architecture first, then plugging in these functions where necessary.
        // On a separate file, this function below. Needs work ... there is a better way I can do this ...
        public static float CalculateWillingness(float playerHonor, MoralAlignment npcMoralAlignment)
        {
            float willingness;
            if (playerHonor < -15.0)
            {
                if (moralAlignment == "Ruthless")
                    willingness += 25;

                if (moralAlignment == "Neutral")
                    willingness -= 10;
                
                if (moralAlignment == "Honorable")
                    willingness -= 25;
            }

            if (playerHonor > 15.0)
            {
                if (moralAlignment == "Ruthless")
                    willingness -= 25;

                if (moralAlignment == "Neutral")
                    willingness += 10;
                
                if (moralAlignment == "Honorable")
                    willingness += 25;
            }

            else
            {
                if (moralAlignment == "Ruthless")
                    willingness -= 10;

                if (moralAlignment == "Neutral")
                    willingness = 10;
                
                if (moralAlignment == "Honorable")
                    willingness += 10;
            }

            return willingness;

        }

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
  