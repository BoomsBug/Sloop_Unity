using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterOption", menuName = "ScriptableObjects/Encounter/Option", order = 1)]
public class EncounterOptionSO : ScriptableObject
{
        //list of resources: [gold, food, power, morality]
    [TextArea] public string text;
    public int[] cost = new int[4];
    public int[] gain = new int[4];
    public bool isGainHidden;
    [TextArea] public string outcome;
}
