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
    public bool isGainNegative;
    [TextArea] public string outcome;

    //specify the minigame scene name (if any) you want this option to laod
    public bool loadMinigame;
    public string minigameName;

    //for each option specific function in EncounterSystem.cs, have a boolean for if you want this option to trigger that function
    public bool callAddCrewmate;
    public Crewmate crewToAdd;
    public bool callRemoveCrewmate; //can't specify which crewmate to remove, will remove one at random
    public bool callAddEncounter; //Does selecting this option add an encounter to the pool
    public EncounterSO encounterToAdd;
    public EncounterSO encounterToReplace;

}
