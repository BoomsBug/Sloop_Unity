using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "ScriptableObjects/Encounter/Encounter", order = 1)]
public class EncounterSO : ScriptableObject
{
    public string encounterName;
    [TextArea]
    public string text;

    public List<EncounterOptionSO> options;
    public bool oneTime;
    public bool landEncounter;
}
