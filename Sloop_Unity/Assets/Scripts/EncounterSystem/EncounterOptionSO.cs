using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterOption", menuName = "ScriptableObjects/Encounter/Option", order = 1)]
public class EncounterOptionSO : ScriptableObject
{
        //list of resources: [gold, wood, food, power, honour]
    [TextArea] public string text;
    [Tooltip("[gold, wood, food, power, honour]")]
    public int[] cost = new int[5];
    [Tooltip("[gold, wood, food, power, honour]")]
    public int[] gain = new int[5];
    public bool isGainHidden;
    [TextArea] public string outcome;
}
