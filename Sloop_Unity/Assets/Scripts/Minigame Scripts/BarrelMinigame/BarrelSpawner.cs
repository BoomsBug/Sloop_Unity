using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    public GameObject barrelPrefab;
    public GameObject coinPrefab;
    public GameObject flyingCannonPrefab;

    [Header("Spawn Interval")]
    public float initialMinInterval = 0.8f;
    public float initialMaxInterval = 2.2f;
    public float minIntervalLimit = 0.3f;
    public float maxIntervalLimit = 0.8f;
    public float intervalDecreaseRate = 0.02f;

    [Header("Barrel Settings")]
    public Vector2 barrelSizeRange = new Vector2(0.5f, 1.5f);
    [Range(0f, 1f)] public float barrelSpawnProbability = 0.7f;

    [Header("Coin Settings")]
    [Range(0f, 1f)] public float coinSpawnProbability = 0.3f;
    public Vector2 coinOffset = new Vector2(0f, 1f);

    [Header("Flying Cannon Settings")]
    [Range(0f, 1f)] public float cannonSpawnProbability = 0.5f;
    public Vector2 cannonHeightRange = new Vector2(1f, 3f);

    private float currentMinInterval;
    private float currentMaxInterval;
    private float nextSpawnTime;
    private bool spawning = false;
    private bool spawnCannons = true; // True for Hard mode, false for Easy

    void Update()
    {
        if (!spawning) return;

        if (Time.time >= nextSpawnTime)
        {
            // Always try to spawn a barrel
            if (barrelPrefab != null && Random.value < barrelSpawnProbability)
                SpawnBarrel();

            // Only spawn cannons if hard mode is enabled
            if (spawnCannons && flyingCannonPrefab != null && Random.value < cannonSpawnProbability)
                SpawnFlyingCannon();

            // Decrease intervals after each spawn tick
            currentMinInterval = Mathf.Max(currentMinInterval - intervalDecreaseRate, minIntervalLimit);
            currentMaxInterval = Mathf.Max(currentMaxInterval - intervalDecreaseRate, maxIntervalLimit);

            nextSpawnTime = Time.time + Random.Range(currentMinInterval, currentMaxInterval);
        }
    }

    void SpawnBarrel()
    {
        GameObject barrel = Instantiate(barrelPrefab, transform.position, Quaternion.identity);
        float scale = Random.Range(barrelSizeRange.x, barrelSizeRange.y);
        barrel.transform.localScale = Vector3.one * scale;

        if (coinPrefab != null && Random.value < coinSpawnProbability)
        {
            Vector3 worldOffset = new Vector3(coinOffset.x, coinOffset.y, 0);
            GameObject coin = Instantiate(coinPrefab, barrel.transform.position + worldOffset, Quaternion.identity);
            CoinFollow follow = coin.AddComponent<CoinFollow>();
            follow.target = barrel.transform;
            follow.offset = worldOffset;
        }
    }

    void SpawnFlyingCannon()
    {
        float yOffset = Random.Range(cannonHeightRange.x, cannonHeightRange.y);
        Vector3 spawnPos = transform.position + new Vector3(0, yOffset, 0);
        Instantiate(flyingCannonPrefab, spawnPos, Quaternion.identity);
    }

    public void SetDifficulty(bool hardMode)
    {
        spawnCannons = hardMode;
    }

    public void StartSpawning()
    {
        spawning = true;
        currentMinInterval = initialMinInterval;
        currentMaxInterval = initialMaxInterval;
        nextSpawnTime = Time.time + Random.Range(currentMinInterval, currentMaxInterval);
    }

    public void StopSpawning()
    {
        spawning = false;

        foreach (GameObject barrel in GameObject.FindGameObjectsWithTag("Barrel"))
            Destroy(barrel);
        foreach (GameObject coin in GameObject.FindGameObjectsWithTag("Loot"))
            Destroy(coin);
        foreach (GameObject cannon in GameObject.FindGameObjectsWithTag("FlyingCannon"))
            Destroy(cannon);
    }
}