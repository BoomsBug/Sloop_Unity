using System.Collections;
using System.Collections.Generic;
using Sloop.Economy;
using UnityEngine;
using System;

[Serializable]
public class Frogroth : Crewmate
{
    [SerializeField] private Crewmate frog;

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
        if (callFunctions)
        {
            if (UnityEngine.Random.value <= 1) //25% chance each encounter to turn a crewmate into a frog
            {
                Crewmate crewToRemove = CrewManager.Instance.hiredCrew[UnityEngine.Random.Range(0, CrewManager.Instance.hiredCrew.Count)];
                if (crewToRemove.crewName != "Frogroth, King of Frogs" && crewToRemove.crewName != "Frog")
                {
                    CrewManager.Instance.RemoveCrew(crewToRemove);
                    CrewManager.Instance.HireEncounterCrew(frog);
                }
            }
        }

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
        Debug.Log("I AM FROGROTH, KING OF FROGS!! Thanks for inviting me");
    }
    public override void FiredDialogue()
    {
        //bring up text box and character portrait, say dialogue, and wait for player to click continue
        Debug.Log("YOU WILL RUE THIS DAY");
    }
}
