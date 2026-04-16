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

public class EncounterSystem : MonoBehaviour
{
    public static EncounterSystem Instance;
    public GameObject encounterUI;
    public List<EncounterSO> possibleSeaEncounters;
    public List<EncounterSO> completedSeaEncounters = new List<EncounterSO>();
    public List<EncounterSO> possibleLandEncounters;
    public List<EncounterSO> completedLandEncounters = new List<EncounterSO>();
    public EncounterSO startingEncounter;
    public bool cycleEncounters;
    public EncounterSO curEncounter;
    public EncounterSO nextLandEncounter;
    public EncounterSO nextSeaEncounter;
    public GameObject continueButton;
    private bool canContinue = false;
    private bool canChoose = false;
    private bool isEncounterActive = false;
    private bool loseGameOnContinue = false;
    private bool winGameOnContinue = false;

    [Header("Text Stuff")]
    public TextMeshProUGUI encounterText;
    public List<GameObject> optionPanels;
    public Typewriter typewriter;
    public GameObject rewardsPanel;

    public Island currentIsland;


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
        nextLandEncounter = possibleLandEncounters[UnityEngine.Random.Range(0, possibleLandEncounters.Count)];
        nextSeaEncounter = startingEncounter;
    }


    public void LoadEncounter(Island curIsland = null, bool landEncounter = false)
    {
        //ensures only one encounter at a time
        if (isEncounterActive) return;

        //if player is on land, set curEncounter to nextLandEncounter
        if (landEncounter)
        {
            currentIsland = curIsland;
            if(curIsland.islandEvent){
                curEncounter = curIsland.islandEvent;
            }
            else
            {
                curIsland.islandEvent = nextLandEncounter;
                curEncounter = curIsland.islandEvent;
            }
        }
        else
        {
            curEncounter = nextSeaEncounter;
        }
        


        //Pauses game and enable encounter UI
        Time.timeScale = 0.0f;
        PauseManager.Paused = true;
        encounterUI.SetActive(true);

        //disable crew UI
        CrewManager.Instance.disableUI();

        isEncounterActive = true;
        canChoose = true;

        encounterText.text = curEncounter.text;

        //enables typewriter effect
        typewriter.StartTypewriter();

        //ensures rewards panel is disables
        rewardsPanel.SetActive(false);

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
            ResourceAmount[] alteredGain = CalculateAlteredGains(curEncounter.options[i].gain, false);
            
            string alteredCostString = PrintListOfResources(alteredCost);
            string alteredGainString = PrintListOfResources(alteredGain);

            panel.transform.Find("Altered Cost").GetComponent<TextMeshProUGUI>().text = alteredCostString;
            panel.transform.Find("Altered Gain").GetComponent<TextMeshProUGUI>().text = alteredGainString;

            //Based on altered cost, shades button red if player cannot afford it
            if (!ResourceManager.Instance.CanAfford(alteredCost))
            {
                panel.GetComponent<Image>().color = Color.red;
                //TODO: ocean spirits, player has enough honour to choose option 4, and game lets them, but still colours red
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

        //enables typewriter effect
        typewriter.StartTypewriter();

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
        
        ResourceAmount[] alteredOptionGains = CalculateAlteredGains(optionGains, true);
        ResourceManager.Instance.Add(alteredOptionGains);
        if (selectedOption.isGainNegative)
        {
            ResourceManager.Instance.Spend(optionGains);
            ResourceManager.Instance.Spend(optionGains);
            
        }

        //enables rewards panel
        rewardsPanel.SetActive(true);
        rewardsPanel.transform.Find("Rewards Text").GetComponent<TextMeshProUGUI>().text = PrintListOfResources(optionGains);

        //Enables continue button
        continueButton.SetActive(true);
        canContinue = true;

        if (curEncounter.landEncounter) 
        {
            nextLandEncounter = ManagerEncounterList(possibleLandEncounters, completedLandEncounters);
        }
        else if (!curEncounter.landEncounter) 
        {
            nextSeaEncounter = ManagerEncounterList(possibleSeaEncounters, completedSeaEncounters);
        }

        //Checks each bool in option to see if it should call a specific function
        CallOptionFunctions(selectedOption);
    }

    public void ContinueGame()
    {
        if (!canContinue) return;

        //Disables encounter UI and unpause game
        Time.timeScale = 1.0f;
        PauseManager.Paused = false;
        encounterUI.SetActive(false);

        if (loseGameOnContinue)
        {
            SceneManager.LoadScene("GameOver");
        }

        if(winGameOnContinue)
        {
            SceneManager.LoadScene("Treasure");
        }

        //allows another encounter to happen
        isEncounterActive = false;

        //enables crew UI
        CrewManager.Instance.enableUI();
        
        //disable rewards panel
        rewardsPanel.SetActive(false);
    }

    private EncounterSO ManagerEncounterList(List<EncounterSO> possible, List<EncounterSO> completed)
    {
        //Decides what next encounter is
        completed.Add(curEncounter);
        possible.Remove(curEncounter);

        //If all possible encounters used up, replenish list with completed encounters that aren't one time use
        if (cycleEncounters && possible.Count <= 0)
        {
            foreach (EncounterSO completedEncounter in new List<EncounterSO>(completed))
            {
                if (!completedEncounter.oneTime) 
                {
                    possible.Add(completedEncounter);
                    completed.Remove(completedEncounter);
                }
            }
        }
        return possible[UnityEngine.Random.Range(0, possible.Count)];
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
    private ResourceAmount[] CalculateAlteredGains(ResourceAmount[] baseGains, bool callFunctions = false)
    {
        //For each crewmember hired, call their resource function and pass baseResources
        //Each crewmember will add or subtract to each resource type
        List<Crewmate> hiredCrew = CrewManager.Instance.hiredCrew;

        //make copy of baseGains
        ResourceAmount[] alteredGains = new ResourceAmount[baseGains.Length];
        Array.Copy(baseGains, alteredGains, baseGains.Length);

        foreach (Crewmate crew in new List<Crewmate>(hiredCrew))
        {
            alteredGains = crew.AlteredGain(alteredGains, callFunctions);
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

        //Removes a crewmate at random
        if (option.callRemoveCrewmate && CrewManager.Instance.hiredCrew.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, CrewManager.Instance.hiredCrew.Count);
            CrewManager.Instance.RemoveCrew(CrewManager.Instance.hiredCrew[randomIndex]);
        }

        //hires
        if (option.crewToAdd != null)
            CrewManager.Instance.HireEncounterCrew(option.crewToAdd);

        if (option.minigameName.Length > 0)
        {

            Time.timeScale = 1.0f; //temorary, should have better state save system
            PauseManager.Paused = false;
            encounterUI.SetActive(false);
            CrewManager.Instance.disableUI();
            canContinue = false;
            //isEncounterActive = false;
            GameManager.Instance.state = GameState.Minigame;
            SceneManager.LoadScene(option.minigameName);
        }

        //adds encounter
        if (option.encounterToAdd != null)
        {
            if (option.encounterToAdd.landEncounter)
            {
                if (option.encounterToReplace)
                {
                    WorldGen world = FindObjectOfType<WorldGen>();
                    bool isGenerated = false;
                    foreach (GameObject i in world.islands)
                    {
                        if(i.GetComponent<Island>().islandEvent == option.encounterToReplace)
                        {
                            currentIsland = i.GetComponent<Island>();
                            isGenerated = true;
                            break;
                        }
                    }
                    if (!isGenerated)
                    {
                        foreach (GameObject i in world.islands)
                        {
                            if(i.GetComponent<Island>().islandEvent == null)
                            {
                                currentIsland = i.GetComponent<Island>();
                                break;
                                
                            }
                        }
                    }
                    
                }
                currentIsland.islandEvent = option.encounterToAdd;
            }  else if (!option.encounterToAdd.landEncounter)
            {
                possibleSeaEncounters.Add(option.encounterToAdd);
            } 
        }

        if (option.loseGame)
        {
            loseGameOnContinue = true;
        }
        else if (option.winGame)
        {
            winGameOnContinue = true;
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

        // if (GameManager.Instance.state == GameState.Sailing && !isEncounterActive)
        // {
        //     CrewManager.Instance.enableUI();
        // }
        // else CrewManager.Instance.disableUI();
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
        if (scene.name == "PRODUCTION" && isEncounterActive)
        {
            encounterUI.SetActive(true);
            canContinue = true;
        }
    }
}
