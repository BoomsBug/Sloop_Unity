using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sloop.NPC;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    [Header("UI")]
    public GameObject crewUI;
    public List<GameObject> panels;
    private int maxCrewID = 0;

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

        enableUI();
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    void Start()
    {
        UnityEngine.Random.InitState(GameManager.Instance.worldSeed);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PRODUCTION")
            enableUI();
    }

    //Takes NPC Data which has traits etc... but also has a Crewmate subclass, this makes the new Crewmate have the functionality of passed subclass
    //NPCGenerator will give the random NPC a random subclass drawn from a list of generic ones
    public void HireCrew(int crewSubclassIndex, NPCData newCrewmateData, Sprite sprite)
    {
        if (crewSubclassIndex == -1)
        {
            Debug.Log("Error: Trying to hire a crewmate with null subclass. Is list of that crew's alignment empty?");
            return;
        }

        if (hiredCrew.Count >= 18)
        {
            Debug.Log("Crew is full!");
            return;
        }

        Debug.Log(crewSubclassIndex);
        Debug.Log(newCrewmateData.name);

        //construct crewmate using npc data, specify which subclass to use given the crew's alignment and given subclass index
        //subclass index is a random value that corresponds to what subclass from that alignment's list
        Crewmate newCrewmate;
        if (newCrewmateData.alignment == MoralAlignment.Honorable)
            newCrewmate = Instantiate(goodCrew[crewSubclassIndex]);
        else if (newCrewmateData.alignment == MoralAlignment.Neutral)
            newCrewmate = Instantiate(neutralCrew[crewSubclassIndex]);
        else
            newCrewmate = Instantiate(evilCrew[crewSubclassIndex]);

        newCrewmate.crewName = newCrewmateData.name;
        newCrewmate.traits = newCrewmateData.traits;
        newCrewmate.alignment = newCrewmateData.alignment;
        newCrewmate.role = newCrewmateData.role;
        newCrewmate.npcIndex = newCrewmateData.npcIndex;
        newCrewmate.sprite = sprite;

        Debug.Log($"New crewmate name: {newCrewmate.crewName}, new sprite: {newCrewmate.sprite}");
    
        // if (hiredCrew.Contains(newCrewmate))
        // {
        //     Debug.Log("Cannot hire a crewmate twice");
        //     return;
        // }
        hiredCrew.Add(newCrewmate);
        newCrewmate.HiredDialogue();

        newCrewmate.crewID = maxCrewID;
        maxCrewID ++;

        //add crew to UI
        AddCrewUI(newCrewmate);
    }

    //This has same functionality of above method, but is given a handmade NPC that can only be hired from random encounters
    //No randomness for these crew
    public void HireEncounterCrew(Crewmate newCrewmate)
    {
        // if (hiredCrew.Contains(newCrewmate))
        // {
        //     Debug.Log("Cannot hire a crewmate twice");
        //     return;
        // }

        newCrewmate = Instantiate(newCrewmate); //make it a copy

        if (hiredCrew.Count >= 18)
        {
            Debug.Log("Crew is full!");
            return;
        }

        //if new crew has their own encounter, add it to possibleEncounter list
        //^^ should be done here when the encounter is tied to the new crew
        if (newCrewmate.crewEncounter != null)
        {
            EncounterSystem encounterSystem = FindObjectOfType<EncounterSystem>();
            if (encounterSystem != null)
            {
                Debug.Log($"adding {newCrewmate.crewEncounter.encounterName} to encountes");
                if (newCrewmate.crewEncounter.landEncounter) encounterSystem.possibleLandEncounters.Add(newCrewmate.crewEncounter);    
                else if (!newCrewmate.crewEncounter.landEncounter) encounterSystem.possibleSeaEncounters.Add(newCrewmate.crewEncounter);
            }
        }

        newCrewmate.crewID = maxCrewID;
        maxCrewID ++;

        Debug.Log($"hiring {newCrewmate.crewName}");
        hiredCrew.Add(newCrewmate);
        newCrewmate.HiredDialogue();

        //add crew to UI
        AddCrewUI(newCrewmate);
    }

    public void RemoveCrew(Crewmate crewToRemove)
    {
        if (!hiredCrew.Contains(crewToRemove))
        {
            Debug.Log("Cannot remove crewmate that has not been hired");
            return;
        }

        //look through possible and completed encounter lists and remove crew's encounter
        if (crewToRemove.crewEncounter)
        {
            EncounterSystem.Instance.possibleLandEncounters.Remove(crewToRemove.crewEncounter);
            EncounterSystem.Instance.possibleSeaEncounters.Remove(crewToRemove.crewEncounter);
            EncounterSystem.Instance.completedLandEncounters.Remove(crewToRemove.crewEncounter);
            EncounterSystem.Instance.completedSeaEncounters.Remove(crewToRemove.crewEncounter);
        }

        //must find what panel the crewToRemove is located at
        foreach (GameObject panel in panels)
        {
            if (panel.GetComponent<Panel>().crewID == crewToRemove.crewID)
            {
                Debug.Log($"Removing crew at {panel.name}");
                panel.SetActive(false);
                panel.GetComponent<Panel>().UnloadBubble();
            }
        }
        Debug.Log($"Removing {crewToRemove.crewName}");

        hiredCrew.Remove(crewToRemove);
        crewToRemove.FiredDialogue();
    }

    // public void RemoveCrewAtRandom(GameObject except)
    // {
    //     Crewmate crewToRemove = hiredCrew[UnityEngine.Random.Range(0, hiredCrew.Count - 1)];

    //     if (except && crewToRemove != except.GetComponent<Crewmate>())
    //         RemoveCrew(crewToRemove);
    //     else if (!except)
    //         RemoveCrew(crewToRemove);
    // }

    public void AddCrewUI(Crewmate newCrewmate)
    {
        int i = 0;
        //get next empty crew panel slot to add new crew to
        foreach (GameObject panel in panels)
        {
            if (!panel.activeSelf)
                break;
            else i ++;
        }

        if (i >= 18)
        {
            Debug.Log("crew is full");
            return;
        }

        panels[i].SetActive(true); //ex for the 3rd hired crew, you want to enable the 3rd panel (index 2)
        panels[i].transform.Find("Sprite").GetComponent<Image>().sprite = newCrewmate.sprite;
        panels[i].transform.Find("Sprite").localScale = new Vector2 (0.7f, 0.9f);
        panels[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = newCrewmate.crewName;
        panels[i].transform.Find("Description").GetComponent<TextMeshProUGUI>().text = newCrewmate.description;
        panels[i].GetComponent<Panel>().crewID = newCrewmate.crewID;
    }

    public void enableUI()
    {
        crewUI.SetActive(true);
    }
    public void disableUI()
    {
        crewUI.SetActive(false);
    }
}
