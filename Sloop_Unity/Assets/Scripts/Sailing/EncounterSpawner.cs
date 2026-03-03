using UnityEngine;

public class EncounterSpawner : MonoBehaviour
{
    public ShipEncounterController choiceEncounter;
    public ShipChaseController chaseEncounter;

    public float checkEvery = 10f;
    [Range(0f, 1f)] public float chance = 0.3f;

    [Range(0f, 1f)] public float chaseWeight = 0.4f; // 40% chase, 60% choice

    float t;

    void Update()
    {
        // Don't start anything if one is already active
        if ((choiceEncounter && choiceEncounter.IsRunning) ||
            (chaseEncounter && chaseEncounter.IsRunning))
            return;

        t += Time.deltaTime;
        if (t < checkEvery) return;
        t = 0f;

        if (Random.value >= chance) return;

        // pick which encounter
        if (chaseEncounter && Random.value < chaseWeight)
            chaseEncounter.StartChase();
        else if (choiceEncounter)
            choiceEncounter.StartEncounter();
    }
}