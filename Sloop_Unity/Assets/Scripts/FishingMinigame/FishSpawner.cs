using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;
    public GameObject goldSackPrefab;

    //public float laneY = -2.5f;
    public float[] laneYs = new float[] { -3f, -2f, -1f, -4f };
    public float spawnX = 25f;
    public float destroyX = 35f;

    public Vector2 spawnInterval = new Vector2(0.4f, 1.0f);
    public Vector2 fishSpeedRange = new Vector2(3f, 10f);
    public Vector2 goldSpeedRange = new Vector2(2.5f, 7f);

    [Range(0f, 1f)] public float goldChance = 0.12f;

    float nextSpawnTime;

    void OnEnable()
    {
        ScheduleNext();
    }

    void ScheduleNext()
    {
        float dt = Random.Range(spawnInterval.x, spawnInterval.y);
        nextSpawnTime = Time.time + dt;
    }

    void Update()
    {
        // Debug every ~2 seconds
        if (Time.frameCount % 120 == 0)
            Debug.Log($"Spawner alive. active={isActiveAndEnabled} timeScale={Time.timeScale} time={Time.time} next={nextSpawnTime}");

        if (Time.time < nextSpawnTime) return;
        //Debug.Log("SPAWN!");
        SpawnOne();
        ScheduleNext();

        if (Time.time < nextSpawnTime) return;
        SpawnOne();
        ScheduleNext();
    }

    void SpawnOne()
    {
        if (!fishPrefab) Debug.LogError("fishPrefab is NULL");
        if (!goldSackPrefab) Debug.LogWarning("goldSackPrefab is NULL (ok if you want only fish)");

        bool spawnGold = goldSackPrefab && Random.value < goldChance;
        GameObject prefab = spawnGold ? goldSackPrefab : fishPrefab;
        if (!prefab) { Debug.LogError("Chosen prefab is NULL"); return; }

        /*
        bool spawnGold = goldSackPrefab && Random.value < goldChance;
        GameObject prefab = spawnGold ? goldSackPrefab : fishPrefab;
        if (!prefab) return;
        */

        bool fromLeft = Random.value < 0.5f;
        float x = fromLeft ? -spawnX : spawnX;
        Vector2 dir = fromLeft ? Vector2.right : Vector2.left;

        //var go = Instantiate(prefab, new Vector3(x, laneY, 0f), Quaternion.identity);
        float y = laneYs[Random.Range(0, laneYs.Length)];
        var go = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);

        var mover = go.GetComponent<LaneMover>();
        if (mover)
        {
            mover.direction = dir;
            mover.destroyX = destroyX;
            mover.speed = spawnGold
                ? Random.Range(goldSpeedRange.x, goldSpeedRange.y)
                : Random.Range(fishSpeedRange.x, fishSpeedRange.y);
        }

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr) sr.flipX = fromLeft;

        //Debug.Log($"Spawned {prefab.name} at x={x}, y={laneY}, destroyX={destroyX}, speed={mover?.speed}");

    }
}