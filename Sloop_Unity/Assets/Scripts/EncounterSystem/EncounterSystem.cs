using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    public GameObject encounterUI;
    public List<ScriptableObject> possibleEncounters;
    public EncounterSO nextEncounter;
    public GameObject continueButton;
    private bool canContinue;
    private bool canChoose;
    private bool isEncounterActive;

    [Header("Text Stuff")]
    public TextMeshProUGUI encounterText;
    public List<GameObject> optionPanels;

    public void LoadNextEncounter()
    {
        //ensures only one encounter at a time
        if (isEncounterActive) return;

        //Pauses game and enable encounter UI
        Time.timeScale = 0.0f;
        PauseManager.Paused = true;
        encounterUI.SetActive(true);

        isEncounterActive = true;
        canChoose = true;

        encounterText.text = nextEncounter.text;

        //ensures continue button is disabled
        continueButton.SetActive(false);
        canContinue = false;
        
        for (int i = 0; i < optionPanels.Count; i ++)
        {
            GameObject panel = optionPanels[i];

            //enables all option panels
            panel.SetActive(true);

            //finds the right child object with tmp component and loads in correct text
            panel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = nextEncounter.options[i].text;
            //for each different resource, if that count is >0, adds it to the cost string
            string cost = $"";
            if (nextEncounter.options[i].cost[0] > 0) cost += $"{nextEncounter.options[i].cost[0]}G "; //gold
            if (nextEncounter.options[i].cost[1] > 0) cost += $"{nextEncounter.options[i].cost[1]}F "; //food
            if (nextEncounter.options[i].cost[2] > 0) cost += $"{nextEncounter.options[i].cost[2]}P "; //power
            if (nextEncounter.options[i].cost[3] > 0) cost += $"{nextEncounter.options[i].cost[3]}H"; //honour
            string gain = $"";
            if (nextEncounter.options[i].gain[0] > 0) gain += $"{nextEncounter.options[i].gain[0]}G "; //gold
            if (nextEncounter.options[i].gain[1] > 0) gain += $"{nextEncounter.options[i].gain[1]}F "; //food
            if (nextEncounter.options[i].gain[2] > 0) gain += $"{nextEncounter.options[i].gain[2]}P "; //power
            if (nextEncounter.options[i].gain[3] > 0) gain += $"{nextEncounter.options[i].gain[3]}H"; //honour

            panel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = cost;
            panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().text = gain;

            //If option gain is hidden, replaces gain and altered gain with ???
            if (nextEncounter.options[i].isGainHidden)
            {
                panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().text = "???";
                panel.transform.Find("Altered Gain").GetComponent<TextMeshProUGUI>().text = "???";
            }

            //TODO: calls functions to calculate altered cost and gain based on hired crew;

        }
    }

    public void ChooseOption(int option)
    {
        if (!canChoose) return;

        //TODO: detect if player has enough resources to choose this option

        encounterText.text = nextEncounter.options[option - 1].outcome;
        //disable and hide option panels
        foreach (GameObject panel in optionPanels)
        {
            panel.SetActive(false);
            canChoose = false;
        }

        //TODO: Take resources away from player

        //TODO: Add gained resources to player

        //Enable continue button
        continueButton.SetActive(true);
        canContinue = true;

        //TODO: decide what next encounter is

    }

    public void ContinueGame()
    {
        if (!canContinue) return;

        //Disables encounter UI and unpause game
        Time.timeScale = 1.0f;
        PauseManager.Paused = false;
        encounterUI.SetActive(false);

        //allows another encounter to happen
        isEncounterActive = false;
    }

    void Update()
    {
        if      (Input.GetKeyDown(KeyCode.Alpha1)) ChooseOption(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChooseOption(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChooseOption(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ChooseOption(4);

        if (Input.GetKeyDown(KeyCode.C)) LoadNextEncounter();
        if (Input.GetKeyDown(KeyCode.Space)) ContinueGame();
    } 

    void Start()
    {
        LoadNextEncounter();
    }
}
