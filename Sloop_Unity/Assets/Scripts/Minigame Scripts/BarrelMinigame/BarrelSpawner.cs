using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    public GameObject barrelPrefab;
    public GameObject coinPrefab;

    [Header("Spawn Interval")]
    public float initialMinInterval = 0.8f;   // Starting minimum
    public float initialMaxInterval = 2.2f;   // Starting maximum
    public float minIntervalLimit = 0.3f;     // Smallest allowed min
    public float maxIntervalLimit = 0.8f;     // Smallest allowed max
    public float intervalDecreaseRate = 0.02f; // Amount subtracted from both per spawn

    [Header("Barrel Settings")]
    public Vector2 barrelSizeRange = new Vector2(0.5f, 1.5f);

    [Header("Coin Settings")]
    public float coinSpawnProbability = 0.3f;
    public Vector2 coinOffset = new Vector2(0f, 1f); // World offset above barrel

    private float currentMinInterval;
    private float currentMaxInterval;
    private float nextSpawnTime;
    private bool spawning = false;

    void Update()
    {
        if (!spawning) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnBarrel();

            // Decrease intervals after each spawn
            currentMinInterval = Mathf.Max(currentMinInterval - intervalDecreaseRate, minIntervalLimit);
            currentMaxInterval = Mathf.Max(currentMaxInterval - intervalDecreaseRate, maxIntervalLimit);

            // Schedule next spawn with a random interval within the new range
            nextSpawnTime = Time.time + Random.Range(currentMinInterval, currentMaxInterval);
        }
    }

    void SpawnBarrel()
    {
        // Instantiate barrel
        GameObject barrel = Instantiate(barrelPrefab, transform.position, Quaternion.identity);
        float scale = Random.Range(barrelSizeRange.x, barrelSizeRange.y);
        barrel.transform.localScale = Vector3.one * scale;

        // Possibly spawn a coin above the barrel
        if (coinPrefab != null && Random.value < coinSpawnProbability)
        {
            // Calculate world offset – for example, place coin directly above barrel's center
            Vector3 worldOffset = new Vector3(coinOffset.x, coinOffset.y, 0);

            // Instantiate coin at barrel position + offset (so it's already above)
            GameObject coin = Instantiate(coinPrefab, barrel.transform.position + worldOffset, Quaternion.identity);

            // Add follow script and set target and offset
            CoinFollow follow = coin.AddComponent<CoinFollow>();
            follow.target = barrel.transform;
            follow.offset = worldOffset;
        }
    }

    public void StartSpawning()
    {
        spawning = true;
        // Reset intervals to initial values
        currentMinInterval = initialMinInterval;
        currentMaxInterval = initialMaxInterval;
        nextSpawnTime = Time.time + Random.Range(currentMinInterval, currentMaxInterval);
    }

    public void StopSpawning()
    {
        spawning = false;

        // Destroy all barrels
        foreach (GameObject barrel in GameObject.FindGameObjectsWithTag("Barrel"))
            Destroy(barrel);

        // Destroy all coins
        foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Loot"))
            Destroy(coin);
    }
}