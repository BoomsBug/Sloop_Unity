using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Most of the code in this script is AI-generated

Code functionality:
The code activated the rain animation every dryTimeRange for a rainTimeRange amount of time.
To affect boat movement, when there is rain, the boat movement gets affected by 
the rainWindMultiplier, gustStrength, and gustChangeRate. The BoatMovement.cs script
uses these values to affect the boat motion.
*/

public class RainScheduler : MonoBehaviour
{
    
    public static RainScheduler Instance { get; private set; }

    [Header("Assign")]
    public GameObject rainObject;   // rain_01: child of Main Camera, which is a child of the Ship

    [Header("Timing (seconds)")]
    public Vector2 dryTimeRange = new Vector2(5f, 9f);   // time between rain bursts
    public Vector2 rainTimeRange = new Vector2(4f, 9f);   // how long it rains each burst

    [Header("Gameplay effect")]
    [Range(0.2f, 1f)] public float rainSpeedMultiplier = 0.7f; // 70% speed when raining

    [Header("Storm effect")]
    public float rainWindMultiplier = 2.0f;  // wind is 2x when raining
    public float gustStrength = 0.5f;        // extra random wind amount
    public float gustChangeRate = 2f;        // how fast gust changes


    public bool IsRaining { get; private set; }
    public float CurrentSpeedMultiplier => IsRaining ? rainSpeedMultiplier : 1f;

    public float CurrentWindMultiplier => IsRaining ? rainWindMultiplier : 1f;
    public Vector2 GustVector { get; private set; }


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!rainObject)
        {
            Debug.LogError("RainScheduler: rainObject not assigned.");
            enabled = false;
            return;
        }

        rainObject.SetActive(false);
        IsRaining = false;
        StartCoroutine(RunWeather());
    }

    IEnumerator RunWeather()
    {
        while (true)
        {
            
            // Dry Period
            IsRaining = false;
            rainObject.SetActive(false);
            yield return new WaitForSeconds(Random.Range(dryTimeRange.x, dryTimeRange.y));

            // Rain Period
            IsRaining = true;
            rainObject.SetActive(true);
            yield return new WaitForSeconds(Random.Range(rainTimeRange.x, rainTimeRange.y));
        }
    }

    void Update()
    {
        if (!IsRaining)
        {
            GustVector = Vector2.zero;
            return;
        }

        // Smooth random gust (changes over time)
        float gx = Mathf.PerlinNoise(Time.time * gustChangeRate, 0.1f) * 2f - 1f;
        float gy = Mathf.PerlinNoise(0.2f, Time.time * gustChangeRate) * 2f - 1f;
        GustVector = new Vector2(gx, gy).normalized * gustStrength;
    }

}
