using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    public List<ScriptableObject> possibleEncounters;
    public EncounterSO nextEncounter;
    private bool canChoose;

    [Header("Text Stuff")]
    public TextMeshProUGUI encounterText;
    public List<GameObject> optionPanels;

    public void LoadNextEncounter()
    {
        encounterText.text = nextEncounter.text;

        //enable all option panels
        foreach (GameObject panel in optionPanels)
        {
            panel.SetActive(true);
            canChoose = true;
        }
    }

    public void ChooseOption(int option)
    {
        if (!canChoose) return;

        if (option == 1)
        {
            //load nextEncounter.opt1Outcome
            encounterText.text = nextEncounter.opt1Outcome;
        }
        else if (option == 2)
        {
            
        }
        else if (option == 3)
        {
            
        }
        else if (option == 4)
        {
            
        }
        else
        {
            
        }
        //disable and hide option panels
        foreach (GameObject panel in optionPanels)
        {
            panel.SetActive(false);
            canChoose = false;
        }
    }

    void Update()
    {
        //if input enabled
        if      (Input.GetKeyDown(KeyCode.Alpha1)) ChooseOption(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChooseOption(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChooseOption(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ChooseOption(4);
    } 

    void Start()
    {
        LoadNextEncounter();
    }
}
