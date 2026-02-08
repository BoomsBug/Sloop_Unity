using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sloop.NPC
{

    // This class needs some cleanup
    // Don't need mood (useless)
    // Don't need willingness, will be
    //      calculated at runtime based off Player + NPC Data

    /// <summary>
    /// The NPC data will be adjusted:
    /// 
    /// Each NPC has:
    ///    Name (procedural)
    ///    Role (Merchant / Deckhand / Civilian)
    ///    MoralAlignment (Honorable / Neutral / Ruthless)
    ///    Modifiers (strings for now, later hooks into systems)
    ///    Home Island (islandID)
    /// 
    /// 
    /// </summary>
    
    [Serializable]
    public class NPCData
    {
        //public string id;
        //public int seed;

        public string name;

        public NPCRole role;
        public MoralAlignment alignment;

        public List<string> traits = new List<string>();

        public int islandID;
        public int npcIndex;

        //public string skill;
        //public string mood; // Useless now...

        // Gonna change: willingness will be determined at the instance of player interaction where it is calculated by the players honor correlated with the NPC's moralAlignment
        //[Range(0, 100)]
        //public int willingness;


        public override string ToString()
        {
            return
                $"Name: {name}\n" +
                $"Role: {role}\n" +
                $"Alignment: {alignment}\n" +
                $"Traits: {(traits.Count == 0 ? "(none)" : string.Join(", ", traits))}\n" +
                $"IslandID: {islandID}\n" +
                $"NPC Index: {npcIndex}";
        }
    }
}

