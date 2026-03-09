using System;
using System.Collections;
using System.Collections.Generic;
using Sloop.Economy;
using UnityEngine;

[Serializable]
public class EvilSteve : Crewmate
{
    //Adds 2 honour to every cost, but gains an extra 10 gold

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
                baseCosts[i].amount += 2;
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
                baseGains[i].amount += 10;
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
    public override void HiredDialogue()
    {
        //bring up text box and character portrait, say dialogue, and wait for player to click continue
        Debug.Log("It's me... Evil Steve!");
    }
    public override void FiredDialogue()
    {
        //bring up text box and character portrait, say dialogue, and wait for player to click continue
        Debug.Log("Noooooo...");
    }
}
