using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Sloop.Economy;
using System;

[CreateAssetMenu(fileName = "EncounterOption", menuName = "ScriptableObjects/Encounter/Option", order = 1)]
public class EncounterOptionSO : ScriptableObject
{
        //list of resources: [gold, wood, food, power, honour]
    [TextArea] public string text;
    public ResourceAmount[] cost;
    public ResourceAmount[] gain;
    public bool isGainHidden;
    [TextArea] public string outcome;
}
