using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RainScheduler : MonoBehaviour
{
    [Header("Assign")]
    public GameObject rainObject;   // rain_01: child of Main Camera, which is a child of the Ship

    [Header("Timing (seconds)")]
    public Vector2 dryTimeRange = new Vector2(6f, 10f);   // time between rain bursts
    public Vector2 rainTimeRange = new Vector2(4f, 10f);   // how long it rains each burst

    [Header("Start")]
    public bool startRainingImmediately = false;

    void Start()
    {
        if (!rainObject) { Debug.LogError("RainScheduler: rainObject not assigned."); return; }

        rainObject.SetActive(startRainingImmediately);
        StartCoroutine(RunWeather());
    }

    IEnumerator RunWeather()
    {
        while (true)
        {
            // Dry period
            if (!startRainingImmediately)
            {
                float dry = Random.Range(dryTimeRange.x, dryTimeRange.y);
                rainObject.SetActive(false);
                yield return new WaitForSeconds(dry);
            }
            startRainingImmediately = false;

            // Rain period
            float rain = Random.Range(rainTimeRange.x, rainTimeRange.y);
            rainObject.SetActive(true);
            yield return new WaitForSeconds(rain);
        }
    }
}
