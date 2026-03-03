using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "ScriptableObjects/Encounter", order = 1)]
public class EncounterSO : ScriptableObject
{
    //list of resources: [gold, food, power, morality]

    public string encounterName;
    [TextArea]
    public string text;

    [Header("Option 1")]
    [TextArea] public string opt1Text;
    public int[] opt1Cost = new int[4];
    public int[] opt1Gain = new int[4];
    public bool opt1GainHidden;
    [TextArea] public string opt1Outcome;

    [Header("Option 2")]
    [TextArea] public string opt2Text;
    public List<int> opt2Cost;
    public List<int> opt2Gain;
    public bool opt2GainHidden;
    [TextArea] public string opt2Outcome;

    [Header("Option 3")]
    [TextArea] public string opt3Text;
    public List<int> opt3Cost;
    public List<int> opt3Gain;
    public bool opt3GainHidden;
    [TextArea] public string opt3Outcome;

    [Header("Option 4")]
    [TextArea] public string opt4Text;
    public List<int> opt4Cost;
    public List<int> opt4Gain;
    public bool opt4GainHidden;
    [TextArea] public string opt4Outcome;

    public void TestFunction()
    {
        Debug.Log("Test");
    }
}
