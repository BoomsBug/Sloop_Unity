using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sloop.NPC
{
    /// <summary>
    /// Runtime identity for an NPC generated deterministically from seed + island context.
    /// Behavior (willingness, dialogue outcomes, etc.) is computed at interaction time.
    /// </summary>
    [Serializable]
    public class NPCData
    {
        public string name;

        public NPCRole role;
        public MoralAlignment alignment;

        // "Traits" are gameplay-impactful tags (strings) for now; later these can map to systems/events
        public List<string> traits = new List<string>();

        public int islandID;
        public int npcIndex;
        //cs script that the encounters use to calculate costs and gains based on hired crew
        //Randomly generated from NPCGenerator based on alignment of crew
        //stores it as the index of that alignment's list of subclasses, CrewManager.HireCrew() takes the int and assigns it the correct subclass
        public int subclassIndex;

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

