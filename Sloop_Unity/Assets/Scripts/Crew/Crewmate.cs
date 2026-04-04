using System;
using System.Collections;
using System.Collections.Generic;
using Sloop.Economy;
using Sloop.NPC;
using UnityEngine;

[Serializable]
public class Crewmate : MonoBehaviour
{
    //Contains stats about a crewmate, all crewmates inherit this class and ovveride the cost and gain functions
    public string crewName;
    public List<string> traits;
    public MoralAlignment alignment;
    public NPCRole role;
    public int npcIndex;
    public Sprite sprite;
    [TextArea] public string description; //their impact on resource costs and gains
    [TextArea] public string[] dialogues;
    public int crewID;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // crew mates can have certain encounters that are only added to the possibleEncounter list when they are hired
    public EncounterSO crewEncounter;

    public virtual ResourceAmount[] AlteredCost(ResourceAmount[] baseCosts)
    {
        return baseCosts;
    }

    public virtual ResourceAmount[] AlteredGain(ResourceAmount[] baseGains, bool callFunctions = false)
    {
        return baseGains;
    }

    public virtual void HiredDialogue()
    {
        
    }
    public virtual void FiredDialogue()
    {
        
    }
}
