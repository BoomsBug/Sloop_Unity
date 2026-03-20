using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Resources;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Sloop.Economy;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

//TODO: Make possibleEncounters and completedEncounters persist when loading to island scene and back

public class EncounterSystem : MonoBehaviour
{
    public static EncounterSystem Instance;
    public GameObject encounterUIprefab;
    public List<EncounterSO> possibleEncounters;
    private List<EncounterSO> completedEncounters = new List<EncounterSO>();
    public bool cycleEncounters;
    public EncounterSO curEncounter;
    public GameObject continueButton;
    private bool canContinue = false;
    private bool canChoose = false;
    private bool isEncounterActive = false;

    [Header("Text Stuff")]
    public TextMeshProUGUI encounterText;
    public List<GameObject> optionPanels;

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

    void Start()
    {
        UnityEngine.Random.InitState(GameManager.Instance.worldSeed);
        curEncounter = possibleEncounters[UnityEngine.Random.Range(0, possibleEncounters.Count)];
    }

    public void LoadEncounter()
    {
        //ensures only one encounter at a time
        if (isEncounterActive) return;

        //Pauses game and enable encounter UI
        Time.timeScale = 0.0f;
        PauseManager.Paused = true;
        encounterUIprefab.SetActive(true);

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

            panel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = $"({costString})";
            panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().text = $"({gainString})";

            //make base cost and gain text darker
            panel.transform.Find("Cost").GetComponent<TextMeshProUGUI>().alpha = 0.5f;
            panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().alpha = 0.5f;

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
                //TODO: ocean spirits, player has enough honour to choose option 4, and game lets them, but still colours red
                //      Honour is unaffected by encounter choices
            }

            //If option gain is hidden, replaces gain and altered gain with ???
            if (curEncounter.options[i].isGainHidden)
            {
                panel.transform.Find("Gain").GetComponent<TextMeshProUGUI>().text = "???";
                panel.transform.Find("Altered Gain").GetComponent<TextMeshProUGUI>().text = "???";
            }
        }
    }

    public void ChooseOption(int option)
    {
        if (!canChoose) return;

        EncounterOptionSO selectedOption = curEncounter.options[option-1];

        //Detects if player has enough resources to choose this option
        ResourceAmount[] optionCosts = selectedOption.cost;

        //(re)calculates altered cost based on hired crew (same as above)
        optionCosts = CalculateAlteredCosts(optionCosts);

        //if cant afford option, then return
        if (!ResourceManager.Instance.CanAfford(optionCosts)) {
            return;
        }

        encounterText.text = selectedOption.outcome;
        //disables and hides option panels
        foreach (GameObject panel in optionPanels)
        {
            panel.SetActive(false);
            canChoose = false;
        }

        //Takes resources away from player
        ResourceManager.Instance.TrySpend(optionCosts);

        //Adds gained resources to player (also calculate altered gains)
        ResourceAmount[] optionGains = selectedOption.gain;
        optionGains = CalculateAlteredGains(optionGains);
        ResourceManager.Instance.Add(optionGains);


        //Checks each bool in option to see if it should call a specific function
        CallOptionFunctions(selectedOption);


        //Enables continue button
        continueButton.SetActive(true);
        canContinue = true;

        //Decides what next encounter is
        completedEncounters.Add(curEncounter);
        possibleEncounters.Remove(curEncounter);

        //If all possible encounters used up, replenish list with completed encounters that aren't one time use
        if (cycleEncounters && possibleEncounters.Count <= 0)
        {
            foreach (EncounterSO completedEncounter in completedEncounters)
            {
                if (!completedEncounter.oneTime) possibleEncounters.Add(completedEncounter);
            }
            completedEncounters = new List<EncounterSO>();
        }
        curEncounter = possibleEncounters[UnityEngine.Random.Range(0, possibleEncounters.Count)];
    }

    public void ContinueGame()
    {
        if (!canContinue) return;

        //Disables encounter UI and unpause game
        Time.timeScale = 1.0f;
        PauseManager.Paused = false;
        encounterUIprefab.SetActive(false);

        //allows another encounter to happen
        isEncounterActive = false;
    }

    private ResourceAmount[] CalculateAlteredCosts(ResourceAmount[] baseCosts)
    {
        //For each crewmember hired, call their resource function and pass baseResources
        //Each crewmember will add or subtract to each resource type
        List<Crewmate> hiredCrew = CrewManager.Instance.hiredCrew;

        //make copy of baseCosts
        ResourceAmount[] alteredCosts = new ResourceAmount[baseCosts.Length];
        Array.Copy(baseCosts, alteredCosts, baseCosts.Length);

        foreach (Crewmate crew in hiredCrew)
        {
            alteredCosts = crew.AlteredCost(alteredCosts);
        }
        
        return alteredCosts;
    }
    private ResourceAmount[] CalculateAlteredGains(ResourceAmount[] baseGains)
    {
        //For each crewmember hired, call their resource function and pass baseResources
        //Each crewmember will add or subtract to each resource type
        List<Crewmate> hiredCrew = CrewManager.Instance.hiredCrew;

        //make copy of baseGains
        ResourceAmount[] alteredGains = new ResourceAmount[baseGains.Length];
        Array.Copy(baseGains, alteredGains, baseGains.Length);

        foreach (Crewmate crew in hiredCrew)
        {
            alteredGains = crew.AlteredGain(alteredGains);
        }

        return alteredGains;
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

    public void CallOptionFunctions(EncounterOptionSO option)
    {
        // Given the selected option, calls relevant specific functions specified by the encounter option SO
        // for example, if the option says that the fishing minigame should be loaded, call it here
        if (option.callAddCrewmate)
            CrewManager.Instance.HireCrew(option.crewToAdd);

        if (option.callRemoveCrewmate && CrewManager.Instance.hiredCrew.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, CrewManager.Instance.hiredCrew.Count);
            CrewManager.Instance.RemoveCrew(CrewManager.Instance.hiredCrew[randomIndex]);
        }

        if (option.loadMinigame)
        {
            //TODO: load scene of name $"{option.minigameName}"
            //  make sure it saves everything properly

            Time.timeScale = 1.0f; //temorary, should have better state save system
            PauseManager.Paused = false;
            SceneManager.LoadScene(option.minigameName);
        }

        if (option.callAddEncounter && option.encounterToAdd != null)
        {
            possibleEncounters.Add(option.encounterToAdd);
        }
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
