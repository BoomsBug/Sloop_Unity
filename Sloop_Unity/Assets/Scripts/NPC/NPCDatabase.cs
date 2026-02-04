using System.Collections.Generic;
using UnityEngine;

namespace Sloop.NPC
{
    [CreateAssetMenu(fileName = "NPCDatabase", menuName = "Sloop/NPC/NPC Database", order = 1)]
    public class NPCDatabase : ScriptableObject
    {
        [Header("Names")]
        public List<string> names = new List<string>();

        [Header("Traits")]
        public List<string> traits = new List<string>();

        [Header("Skills")]
        public List<string> skills = new List<string>();

        // To Be Removed; useless
        [Header("Moods")]
        public List<string> moods = new List<string>();

        [Header("Generation Settings")]
        [Min(1)] public int minTraits = 2;
        [Min(1)] public int maxTraits = 3;

        public void Validate()
        {
            if (minTraits > maxTraits) maxTraits = minTraits;
        }
    }
}
