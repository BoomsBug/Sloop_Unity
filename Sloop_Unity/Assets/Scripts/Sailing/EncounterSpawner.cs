using UnityEngine;

public class EncounterSpawner : MonoBehaviour
{
    //public ShipEncounterController controller;
    public EncounterSystem encounterSystem;
    public float checkEvery = 10f;
    [Range(0f, 1f)] public float chance = 0.25f;
    private bool playedFirstEncounter = false;

    //[Range(0f, 1f)] public float chaseWeight = 0.4f; // 40% chase, 60% choice

    private float t = 7;

    void Start()
    {
        encounterSystem = GameObject.FindObjectOfType<EncounterSystem>();
        if (encounterSystem == null)
            Debug.Log("Missing encounter system");
    }

    void Update()
    {
        // Don't start anything if one is already active
        // if ((choiceEncounter && choiceEncounter.IsRunning) ||
        //     (chaseEncounter && chaseEncounter.IsRunning))
        //     return;

        t += Time.deltaTime;
        if (t < checkEvery) return;
        t = 0f;

        if (Random.value < chance && !playedFirstEncounter)
        {
            // controller.StartEncounter();
            playedFirstEncounter = true;
            encounterSystem.LoadEncounter();
        }

    }
}