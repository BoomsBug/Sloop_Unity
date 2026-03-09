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

    public virtual void AlteredCost(ResourceAmount[] baseCosts)
    {
        
    }

    public virtual void AlteredGain(ResourceAmount[] baseGains)
    {
        
    }
    public virtual void HiredDialogue()
    {
        
    }
    public virtual void FiredDialogue()
    {
        
    }
}
