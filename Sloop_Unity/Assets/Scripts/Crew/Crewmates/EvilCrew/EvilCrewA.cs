using System;
using System.Collections;
using System.Collections.Generic;
using Sloop.Economy;
using UnityEngine;

[Serializable]
public class EvilCrewA : Crewmate
{
    //Adds 2 honour to every cost, but gains an extra 10 gold

    public override ResourceAmount[] AlteredCost(ResourceAmount[] baseCosts)
    {
        int gold = 0;
        int food = 0;
        int wood = 0;
        int power = 0;
        int honour = 0;

        for (int i = 0; i < baseCosts.Length; i ++)
        {
            Resource resource = baseCosts[i].type;
            if (resource == Resource.Gold) gold = baseCosts[i].amount;
            if (resource == Resource.Wood) wood = baseCosts[i].amount;
            if (resource == Resource.Food) food = baseCosts[i].amount;
            if (resource == Resource.Power) power = baseCosts[i].amount;
            if (resource == Resource.Honour) honour = baseCosts[i].amount;
        }
        
        // ---------------- MAKE COST MODIFIERS HERE ----------------
        //ie. gold += 10, if (wood >= 10) food -= 5, etc...
        honour = (int) (honour * 1.1);
        //-----------------------------------------------------------

        ResourceAmount[] alteredCosts = {
            new ResourceAmount {type=Resource.Gold, amount = gold},
            new ResourceAmount {type=Resource.Food, amount = food},
            new ResourceAmount {type=Resource.Wood, amount = wood},
            new ResourceAmount {type=Resource.Power, amount = power},
            new ResourceAmount {type=Resource.Honour, amount = honour}
        };

        return alteredCosts;
    }
    public override ResourceAmount[] AlteredGain(ResourceAmount[] baseGains, bool callFunctions = false)
    {
        int gold = 0;
        int food = 0;
        int wood = 0;
        int power = 0;
        int honour = 0;

        for (int i = 0; i < baseGains.Length; i ++)
        {
            Resource resource = baseGains[i].type;
            if (resource == Resource.Gold) gold = baseGains[i].amount;
            if (resource == Resource.Wood) wood = baseGains[i].amount;
            if (resource == Resource.Food) food = baseGains[i].amount;
            if (resource == Resource.Power) power = baseGains[i].amount;
            if (resource == Resource.Honour) honour = baseGains[i].amount;
        }
        
        // ---------------- MAKE GAIN MODIFIERS HERE ----------------
        gold = (int) (gold * 1.1);
        //-----------------------------------------------------------

        ResourceAmount[] alteredGains = {
            new ResourceAmount {type=Resource.Gold, amount = gold},
            new ResourceAmount {type=Resource.Food, amount = food},
            new ResourceAmount {type=Resource.Wood, amount = wood},
            new ResourceAmount {type=Resource.Power, amount = power},
            new ResourceAmount {type=Resource.Honour, amount = honour}
        };

        return alteredGains;
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
