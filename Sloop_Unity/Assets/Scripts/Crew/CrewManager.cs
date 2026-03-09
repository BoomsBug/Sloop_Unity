using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sloop.NPC;

public class CrewManager : MonoBehaviour
{
    /*
        Manages what crew the player has hired. Contains functions to add and remove crew
    */
    public static CrewManager Instance;
    public List<Crewmate> hiredCrew;

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

    public void HireCrew(NPCData newCrewmateData)
    {
        //construct crewmate using npc data
        Crewmate newCrewmate = new Crewmate {
            name = newCrewmateData.name,
            traits = newCrewmateData.traits,
            alignment = newCrewmateData.alignment,
            role = newCrewmateData.role,
            npcIndex = newCrewmateData.npcIndex
        };
    
        if (hiredCrew.Contains(newCrewmate))
        {
            Debug.Log("Cannot hire a crewmate twice");
            return;
        }
        hiredCrew.Add(newCrewmate);
        newCrewmate.HiredDialogue();
    }

    public void HireCrew(Crewmate newCrewmate)
    {
        if (hiredCrew.Contains(newCrewmate))
        {
            Debug.Log("Cannot hire a crewmate twice");
            return;
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
