using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sloop.NPC;
using System;

public class CrewManager : MonoBehaviour
{
    /*
        Manages what crew the player has hired. Contains functions to add and remove crew
    */
    public static CrewManager Instance;
    public List<Crewmate> hiredCrew;

    [Header("Possible crew subclasses for random NPCs")]
    public List<Crewmate> goodCrew;
    public List<Crewmate> neutralCrew;
    public List<Crewmate> evilCrew;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //Takes NPC Data which has traits etc... but also has a Crewmate subclass, this makes the new Crewmate have the functionality of passed subclass
    //NPCGenerator will give the random NPC a random subclass drawn from a list of generic ones
    public void HireCrew(int crewSubclassIndex, NPCData newCrewmateData)
    {
        if (crewSubclassIndex == -1)
        {
            Debug.Log("Error: Trying to hire a crewmate with null subclass. Is list of that crew's alignment empty?");
        }
        Debug.Log(crewSubclassIndex);
        Debug.Log(newCrewmateData.name);

        //construct crewmate using npc data, specify which subclass to use given the crew's alignment and given subclass index
        //subclass index is a random value that corresponds to what subclass from that alignment's list
        Crewmate newCrewmate;
        if (newCrewmateData.alignment == MoralAlignment.Honorable)
            newCrewmate = goodCrew[crewSubclassIndex];
        else if (newCrewmateData.alignment == MoralAlignment.Neutral)
            newCrewmate = neutralCrew[crewSubclassIndex];
        else
            newCrewmate = evilCrew[crewSubclassIndex];

        newCrewmate.crewName = newCrewmateData.name;
        newCrewmate.traits = newCrewmateData.traits;
        newCrewmate.alignment = newCrewmateData.alignment;
        newCrewmate.role = newCrewmateData.role;
        newCrewmate.npcIndex = newCrewmateData.npcIndex;
    
        if (hiredCrew.Contains(newCrewmate))
        {
            Debug.Log("Cannot hire a crewmate twice");
            return;
        }
        hiredCrew.Add(newCrewmate);
        newCrewmate.HiredDialogue();
    }

    //This has same functionality of above method, but is given a handmade NPC that can only be hired from random encounters
    //No randomness for these crew
    public void HireCrew(Crewmate newCrewmate)
    {
        if (hiredCrew.Contains(newCrewmate))
        {
            Debug.Log("Cannot hire a crewmate twice");
            return;
        }

        //if new crew has their own encounter, add it to possibleEncounter list
        if (newCrewmate.crewEncounter != null)
        {
            EncounterSystem encounterSystem = FindObjectOfType<EncounterSystem>();
            if (encounterSystem != null)
                encounterSystem.possibleEncounters.Add(newCrewmate.crewEncounter);
        }

        hiredCrew.Add(newCrewmate);
        newCrewmate.HiredDialogue();
    }

    public void RemoveCrew(Crewmate crewToRemove)
    {
        if (!hiredCrew.Contains(crewToRemove))
        {
            Debug.Log("Cannot remove crewmate that has not been hired");
            return;
        }
        hiredCrew.Remove(crewToRemove);
        crewToRemove.FiredDialogue();
    }
}
