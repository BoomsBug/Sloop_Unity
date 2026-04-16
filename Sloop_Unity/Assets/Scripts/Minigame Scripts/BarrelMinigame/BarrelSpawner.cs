using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    public GameObject barrelPrefab;
    public GameObject coinPrefab;
    public GameObject flyingCannonPrefab;

    [Header("Barrel Spawn Settings")]
    public float barrelInitialMinInterval = 0.8f;
    public float barrelInitialMaxInterval = 2.2f;
    public float barrelMinIntervalLimit = 0.3f;
    public float barrelMaxIntervalLimit = 0.8f;
    public float barrelIntervalDecreaseRate = 0.02f;
    [Range(0f, 1f)] public float barrelSpawnProbability = 0.7f;
    public Vector2 barrelSizeRange = new Vector2(0.5f, 1.5f);
    public Vector2 barrelHeightRange = new Vector2(0.5f, 2.5f);

    [Header("Cannon Spawn Settings")]
    public float cannonInitialMinInterval = 1.5f;
    public float cannonInitialMaxInterval = 3.0f;
    public float cannonMinIntervalLimit = 0.5f;
    public float cannonMaxIntervalLimit = 1.2f;
    public float cannonIntervalDecreaseRate = 0.02f;
    [Range(0f, 1f)] public float cannonSpawnProbability = 0.8f;
    public float cannonHeightAboveGround = 2.0f;

    [Header("Coin Settings (attached to barrels)")]
    [Range(0f, 1f)] public float coinSpawnProbability = 0.3f;
    public Vector2 coinOffset = new Vector2(0f, 1f);

    [Header("Spawn Offsets")]
    public float spawnOffsetX = 0.5f;

    [Header("Tutorial Mode")]
    public bool tutorialMode = false;
    public float tutorialSpawnInterval = 1.5f;
    public bool tutorialFirstSpawnIsCannon = true;  // This was missing

    // Track spawned objects to efficiently clean up
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private float currentBarrelMinInterval;
    private float currentBarrelMaxInterval;
    private float currentCannonMinInterval;
    private float currentCannonMaxInterval;
    private float nextBarrelSpawnTime;
    private float nextCannonSpawnTime;
    private bool spawning = false;

    private Camera mainCamera;
    private float groundY;

    private float nextTutorialSpawnTime;
    private bool nextIsCannon;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("BarrelSpawner: No main camera found!");

        GameObject groundObj = GameObject.FindGameObjectWithTag("Ground");
        if (groundObj != null)
        {
            Collider2D groundCollider = groundObj.GetComponent<Collider2D>();
            if (groundCollider != null)
                groundY = groundCollider.bounds.max.y;
            else
                groundY = groundObj.transform.position.y;
        }
        else
        {
            Debug.LogWarning("BarrelSpawner: No Ground found! Using fallback Y = 0.");
            groundY = 0f;
        }
    }

    void Update()
    {
        if (!spawning) return;

        if (tutorialMode)
        {
            if (Time.time >= nextTutorialSpawnTime)
            {
                if (nextIsCannon)
                    SpawnCannon();
                else
                    SpawnBarrel();

                nextIsCannon = !nextIsCannon;
                nextTutorialSpawnTime = Time.time + tutorialSpawnInterval;
            }
        }
        else
        {
            if (Time.time >= nextBarrelSpawnTime)
            {
                if (barrelPrefab != null && Random.value < barrelSpawnProbability)
                    SpawnBarrel();

                currentBarrelMinInterval = Mathf.Max(currentBarrelMinInterval - barrelIntervalDecreaseRate, barrelMinIntervalLimit);
                currentBarrelMaxInterval = Mathf.Max(currentBarrelMaxInterval - barrelIntervalDecreaseRate, barrelMaxIntervalLimit);
                nextBarrelSpawnTime = Time.time + Random.Range(currentBarrelMinInterval, currentBarrelMaxInterval);
            }

            if (Time.time >= nextCannonSpawnTime)
            {
                if (flyingCannonPrefab != null && Random.value < cannonSpawnProbability)
                    SpawnCannon();

                currentCannonMinInterval = Mathf.Max(currentCannonMinInterval - cannonIntervalDecreaseRate, cannonMinIntervalLimit);
                currentCannonMaxInterval = Mathf.Max(currentCannonMaxInterval - cannonIntervalDecreaseRate, cannonMaxIntervalLimit);
                nextCannonSpawnTime = Time.time + Random.Range(currentCannonMinInterval, currentCannonMaxInterval);
            }
        }
    }

    void SpawnBarrel()
    {
        float rightEdgeX = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        float spawnX = rightEdgeX + spawnOffsetX;

        float heightOffset = Random.Range(barrelHeightRange.x, barrelHeightRange.y);
        Vector3 spawnPos = new Vector3(spawnX, groundY + heightOffset, 0f);

        GameObject barrel = Instantiate(barrelPrefab, spawnPos, Quaternion.identity);
        spawnedObjects.Add(barrel);

        float scale = Random.Range(barrelSizeRange.x, barrelSizeRange.y);
        barrel.transform.localScale = Vector3.one * scale;

        if (coinPrefab != null && Random.value < coinSpawnProbability)
        {
            Vector3 worldOffset = new Vector3(coinOffset.x, coinOffset.y, 0);
            GameObject coin = Instantiate(coinPrefab, barrel.transform.position + worldOffset, Quaternion.identity);
            spawnedObjects.Add(coin);
            CoinFollow follow = coin.AddComponent<CoinFollow>();
            follow.target = barrel.transform;
            follow.offset = worldOffset;
        }
    }

    void SpawnCannon()
    {
        float rightEdgeX = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        float spawnX = rightEdgeX + spawnOffsetX;
        Vector3 spawnPos = new Vector3(spawnX, groundY + cannonHeightAboveGround, 0f);
        GameObject cannon = Instantiate(flyingCannonPrefab, spawnPos, Quaternion.identity);
        spawnedObjects.Add(cannon);
    }

    public void StartSpawning()
    {
        spawning = true;

        if (tutorialMode)
        {
            nextIsCannon = tutorialFirstSpawnIsCannon;
            nextTutorialSpawnTime = Time.time + tutorialSpawnInterval;
        }
        else
        {
            currentBarrelMinInterval = barrelInitialMinInterval;
            currentBarrelMaxInterval = barrelInitialMaxInterval;
            currentCannonMinInterval = cannonInitialMinInterval;
            currentCannonMaxInterval = cannonInitialMaxInterval;

            nextBarrelSpawnTime = Time.time + Random.Range(currentBarrelMinInterval, currentBarrelMaxInterval);
            nextCannonSpawnTime = Time.time + Random.Range(currentCannonMinInterval, currentCannonMaxInterval);
        }
    }

    public void StopSpawning()
    {
        spawning = false;

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }
}