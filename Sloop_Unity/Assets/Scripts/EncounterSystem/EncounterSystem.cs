using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Resources;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Sloop.Economy;
using UnityEngine.UI;
//using System;

//TODO: Find a way to add custom functions to different options when clicked (ie. minigames, add crew, remove crew, etc...)

public class EncounterSystem : MonoBehaviour
{
    public GameObject encounterUI;
    public List<EncounterSO> possibleEncounters;
    private List<EncounterSO> completedEncounters = new List<EncounterSO>();
    public EncounterSO curEncounter;
    public GameObject continueButton;
    private bool canContinue = false;
    private bool canChoose = false;
    private bool isEncounterActive = false;

    [Header("Text Stuff")]
    public TextMeshProUGUI encounterText;
    public List<GameObject> optionPanels;

    public void LoadEncounter()
    {
        //ensures only one encounter at a time
        if (isEncounterActive) return;

        //Pauses game and enable encounter UI
        Time.timeScale = 0.0f;
        PauseManager.Paused = true;
        encounterUI.SetActive(true);

        isEncounterActive = true;
        canChoose = true;

        encounterText.text = curEncounter.text;

        //ensures continue button is disabled
        continueButton.SetActive(false);
        canContinue = false;
        
        for (int i = 0; i < curEncounter.options.Count; i ++)
        {
            GameObject panel = optionPanels[i];

            //enables all option panels
            panel.SetActive(true);

            //finds the right child object with tmp component and loads in correct text
            panel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = curEncounter.options[i].text;
            string costString = PrintListOfResources(curEncounter.options[i].cost);
            string gainString = PrintListOfResources(curEncounter.options[i].gain);

            panel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = costString;
            panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().text = gainString;

            //If option gain is hidden, replaces gain and altered gain with ???
            if (curEncounter.options[i].isGainHidden)
            {
                panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().text = "???";
                panel.transform.Find("Altered Gain").GetComponent<TextMeshProUGUI>().text = "???";
            }

            //calls functions to calculate altered cost and gain based on hired crew;
            ResourceAmount[] alteredCost = CalculateAlteredCosts(curEncounter.options[i].cost);
            ResourceAmount[] alteredGain = CalculateAlteredGains(curEncounter.options[i].gain);
            
            string alteredCostString = PrintListOfResources(alteredCost);
            string alteredGainString = PrintListOfResources(alteredGain);

            panel.transform.Find("Altered Cost").GetComponent<TextMeshProUGUI>().text = alteredCostString;
            panel.transform.Find("Altered Gain").GetComponent<TextMeshProUGUI>().text = alteredGainString;

            //Based on altered cost, shades button red if player cannot afford it
            if (!ResourceManager.Instance.CanAfford(alteredCost))
            {
                panel.GetComponent<Image>().color = Color.red;
                //ocean spirits, player has enough honour to choose option 4, and game lets them, but still colours red
            }
        }
    }

    public void ChooseOption(int option)
    {
        if (!canChoose) return;

        //Detects if player has enough resources to choose this option
        ResourceAmount[] optionCosts = curEncounter.options[option-1].cost;

        //(re)calculates altered cost based on hired crew (same as above)
        optionCosts = CalculateAlteredCosts(optionCosts);

        //if cant afford option, then return
        if (!ResourceManager.Instance.CanAfford(optionCosts)) {
            return;
        }

        encounterText.text = curEncounter.options[option - 1].outcome;
        //disables and hides option panels
        foreach (GameObject panel in optionPanels)
        {
            panel.SetActive(false);
            canChoose = false;
        }

        //Takes resources away from player
        ResourceManager.Instance.TrySpend(optionCosts);

        //Adds gained resources to player (also calculate altered gains)
        ResourceAmount[] optionGains = curEncounter.options[option-1].gain;
        optionGains = CalculateAlteredGains(optionGains);
        ResourceManager.Instance.Add(optionGains);

        //Enables continue button
        continueButton.SetActive(true);
        canContinue = true;

        //Decides what next encounter is
        completedEncounters.Add(curEncounter);
        possibleEncounters.Remove(curEncounter);
        //If all possible encounters used up, replenish list with completed encounters
        if (possibleEncounters.Count <= 0)
        {
            possibleEncounters = new List<EncounterSO>(completedEncounters);
            completedEncounters = new List<EncounterSO>();
        }
        curEncounter = possibleEncounters[Random.Range(0, possibleEncounters.Count)];

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

    private ResourceAmount[] CalculateAlteredCosts(ResourceAmount[] baseCosts)
    {
        //TODO
        return baseCosts;
        //For each crewmember hired, call their resource function and pass baseResources
        //Each crewmember will add or subtract to each resource type
    }
    private ResourceAmount[] CalculateAlteredGains(ResourceAmount[] baseGains)
    {
        //TODO
        return baseGains;
        //For each crewmember hired, call their resource function and pass baseResources
        //Each crewmember will add or subtract to each resource type
    }

    //Takes an array of ResourceAmount (which is type and amount), iterates through and constructs a string
    // the string takes the form "<amount><first letter of type> ..."
    private string PrintListOfResources(ResourceAmount[] list)
    {
        if (list.Count() <= 0 || list == null) return "";

        string str = "";
        for (int i = 0; i < list.Length; i ++)
        {
            Resource type = list[i].type;
            int amount = list[i].amount;
            if (amount > 0) str += $"{amount}{type.ToString().ToCharArray()[0]}";
            if (i < list.Length - 1) str += " ";
        }
        return str;
    }

    void Update()
    {
        if      (Input.GetKeyDown(KeyCode.Alpha1)) ChooseOption(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChooseOption(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChooseOption(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ChooseOption(4);

        if (Input.GetKeyDown(KeyCode.C)) LoadEncounter();
        if (Input.GetKeyDown(KeyCode.Space)) ContinueGame();
    } 
}
