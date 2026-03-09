using System.Collections;
using System.Collections.Generic;
using Sloop.Economy;
using UnityEngine;
using System;

[Serializable]
public class HyukHyukHarry : Crewmate
{

    public override void AlteredCost(ResourceAmount[] baseCosts)
    {
        for (int i = 0; i < baseCosts.Length; i ++)
        {
            Resource resource = baseCosts[i].type;
            int amount = baseCosts[i].amount;

            if (resource == Resource.Gold)
            {
                
            }
            if (resource == Resource.Wood)
            {
                
            }
            if (resource == Resource.Food)
            {
                
            }
            if (resource == Resource.Power)
            {
                
            }
            if (resource == Resource.Honour)
            {
                
            }
            
        }
    }
    public override void AlteredGain(ResourceAmount[] baseGains)
    {
        for (int i = 0; i < baseGains.Length; i ++)
        {
            Resource resource = baseGains[i].type;
            int amount = baseGains[i].amount;

            if (resource == Resource.Gold)
            {
                baseGains[i].amount += 9;
            }
            if (resource == Resource.Wood)
            {
                baseGains[i].amount += 9;
            }
            if (resource == Resource.Food)
            {
                baseGains[i].amount += 9;
            }
            if (resource == Resource.Power)
            {
                baseGains[i].amount += 9;
            }
            if (resource == Resource.Honour)
            {
                baseGains[i].amount += 9;
            }
        }
    }
    public override void HiredDialogue()
    {
        //bring up text box and character portrait, say dialogue, and wait for player to click continue
        Debug.Log("Arrgh. I am a pirate");
    }
    public override void FiredDialogue()
    {
        //bring up text box and character portrait, say dialogue, and wait for player to click continue
        Debug.Log("Arrgh. I am no longer a pirate");
    }
}
