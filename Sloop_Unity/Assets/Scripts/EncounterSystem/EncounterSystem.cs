using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Resources;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Sloop.Economy;
using UnityEngine.UI;

public class EncounterSystem : MonoBehaviour
{
    public GameObject encounterUI;
    public List<EncounterSO> possibleEncounters;
    private List<EncounterSO> completedEncounters = new List<EncounterSO>();
    public EncounterSO curEncounter;
    public GameObject continueButton;
    private bool canContinue;
    private bool canChoose;
    private bool isEncounterActive;

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
            //for each different resource, if that count is >0, adds it to the cost string
            string cost = $"";
            if (curEncounter.options[i].cost[0] > 0) cost += $"{curEncounter.options[i].cost[0]}G "; //gold
            if (curEncounter.options[i].cost[1] > 0) cost += $"{curEncounter.options[i].cost[1]}W "; //wood
            if (curEncounter.options[i].cost[2] > 0) cost += $"{curEncounter.options[i].cost[2]}F "; //food
            if (curEncounter.options[i].cost[3] > 0) cost += $"{curEncounter.options[i].cost[3]}P "; //power
            if (curEncounter.options[i].cost[4] > 0) cost += $"{curEncounter.options[i].cost[4]}H";  //honour
            string gain = $"";
            if (curEncounter.options[i].gain[0] > 0) gain += $"{curEncounter.options[i].gain[0]}G "; //gold
            if (curEncounter.options[i].gain[1] > 0) gain += $"{curEncounter.options[i].gain[1]}W "; //wood
            if (curEncounter.options[i].gain[2] > 0) gain += $"{curEncounter.options[i].gain[2]}F "; //food
            if (curEncounter.options[i].gain[3] > 0) gain += $"{curEncounter.options[i].gain[3]}P "; //power
            if (curEncounter.options[i].gain[4] > 0) gain += $"{curEncounter.options[i].gain[4]}H";  //honour

            panel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = cost;
            panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().text = gain;

            //If option gain is hidden, replaces gain and altered gain with ???
            if (curEncounter.options[i].isGainHidden)
            {
                panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().text = "???";
                panel.transform.Find("Altered Gain").GetComponent<TextMeshProUGUI>().text = "???";
            }

            //calls functions to calculate altered cost and gain based on hired crew;
            int[] alteredCost = CalculateAlteredResources(curEncounter.options[i].cost);
            int[] alteredGain = CalculateAlteredResources(curEncounter.options[i].gain);

            string alteredCostString = $"";
            if (alteredCost[0] > 0) alteredCostString += $"{alteredCost[0]}G "; //gold
            if (alteredCost[1] > 0) alteredCostString += $"{alteredCost[1]}G "; //wood
            if (alteredCost[2] > 0) alteredCostString += $"{alteredCost[2]}G "; //food
            if (alteredCost[3] > 0) alteredCostString += $"{alteredCost[3]}G "; //power
            if (alteredCost[4] > 0) alteredCostString += $"{alteredCost[4]}G";  //honour
            string alteredGainString = $"";
            if (alteredGain[0] > 0) alteredGainString += $"{alteredGain[0]}G "; //gold
            if (alteredGain[1] > 0) alteredGainString += $"{alteredGain[1]}G "; //wood
            if (alteredGain[2] > 0) alteredGainString += $"{alteredGain[2]}G "; //food
            if (alteredGain[3] > 0) alteredGainString += $"{alteredGain[3]}G "; //power
            if (alteredGain[4] > 0) alteredGainString += $"{alteredGain[4]}G";  //honour
            
            panel.transform.Find("Altered Cost").GetComponent<TextMeshProUGUI>().text = alteredCostString;
            panel.transform.Find("Altered Gain").GetComponent<TextMeshProUGUI>().text = alteredGainString;

            //Based on altered cost, shades button red if player cannot afford it
            IEnumerable<ResourceAmount> alteredResourceCosts = BasicCosts2ResourceCosts(alteredCost);
            if (!ResourceManager.Instance.CanAfford(alteredResourceCosts))
            {
                panel.GetComponent<Image>().color = Color.red;
            }
        }
    }

    public void ChooseOption(int option)
    {
        if (!canChoose) return;

        //Detects if player has enough resources to choose this option
        int[] optionCosts = curEncounter.options[option-1].cost;

        //(re)calculates altered cost based on hired crew (same as above)
        optionCosts = CalculateAlteredResources(optionCosts);

        //then converts int[] to IEnumrable<ResourceAmount>
        IEnumerable<ResourceAmount> resourceCosts = BasicCosts2ResourceCosts(optionCosts);

        //if cant afford option, then return
        if (!ResourceManager.Instance.CanAfford(resourceCosts)) {
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
        ResourceManager.Instance.TrySpend(resourceCosts);

        //Adds gained resources to player (also calculate altered gains)
        int[] optionGains = curEncounter.options[option-1].gain;
        optionGains = CalculateAlteredResources(optionGains);
        IEnumerable<ResourceAmount> resourceGains = BasicCosts2ResourceCosts(optionGains);
        ResourceManager.Instance.Add(resourceGains);

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

    private IEnumerable<ResourceAmount> BasicCosts2ResourceCosts(int[] costs)
    {
        //list of resources: [gold, wood, food, power, honour]
        ResourceAmount[] cost =
        {  
            new ResourceAmount{type=Resource.Gold, amount=costs[0]},
            new ResourceAmount{type=Resource.Wood, amount=costs[1]},
            new ResourceAmount{type=Resource.Food, amount=costs[2]},
            new ResourceAmount{type=Resource.Power, amount=costs[3]},
            new ResourceAmount{type=Resource.Honour, amount=costs[4]}
        };
        return cost;
    }

    private int[] CalculateAlteredResources(int[] baseResources)
    {
        //TODO
        return baseResources;
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

    void Start()
    {
        LoadEncounter();
    }
}
