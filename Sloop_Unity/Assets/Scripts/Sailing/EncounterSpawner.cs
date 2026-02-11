using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSpawner : MonoBehaviour
{
    public ShipEncounterController controller;
    public float checkEvery = 10f;
    [Range(0f, 1f)] public float chance = 0.3f;

    float t;

    void Update()
    {
        t += Time.deltaTime;
        if (t < checkEvery) return;
        t = 0f;

        if (Random.value < chance)
            controller.StartEncounter();
    }
}
