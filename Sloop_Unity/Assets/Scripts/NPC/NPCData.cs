using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sloop.NPC
{

    // This class needs some cleanup
    // Don't need mood (useless)
    // Don't need willingness or alignement here as they will be
    //      calculated at runtime based off Player + NPC Data
    
    [Serializable]
    public class NPCData
    {
        public string id;
        public int seed;

        public string name;
        public List<string> traits = new List<string>();

        public string skill;
        public string mood; // Useless now...

        // Gonna change: willingness will be determined at the instance of player interaction where it is calculated by the players honor correlated with the NPC's moralAlignment
        [Range(0, 100)]
        public int willingness;


        // Gonna change: alignment will be determined at the instance of player interaction where it is calculated by the players honor correlated with the NPC's overall trait score
        public MoralAlignment alignment;

        public override string ToString()
        {
            string traitStr = (traits == null || traits.Count == 0) ? "(none)" : string.Join(", ", traits);
            return
                $"Name: {name}\n" +
                $"Traits: {traitStr}\n" +
                $"Skill: {skill}\n" +
                $"Mood: {mood}\n" +
                $"Willingness: {willingness}\n" +
                $"Alignment: {alignment}\n" +
                $"Seed: {seed}\n" +
                $"ID: {id}";
        }
    }
}

