using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Sloop.Economy; // <-- Required for ResourceManager and Resource enum

public class BarrelDashTestManager : MonoBehaviour
{
    public enum GameMode { Easy, Hard }

    [Header("UI References")]
    public GameObject gamePanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI resultText;
    public Button easyButton;
    public Button hardButton;
    public Button closeButton;

    [Header("Game Objects")]
    public PlayerController player;
    public BarrelSpawner spawner;
    public Transform playerStartPos;

    [Header("Settings")]
    public float baseSpeed = 5f;
    public float maxSpeed = 12f;
    public float speedIncreaseRate = 0.1f;
    public int goldPerPoint = 1;

    [Header("Mode Settings")]
    public float easyCannonChance = 0f;       // Chance for cannon to spawn in Easy mode
    public float hardCannonChance = 0.5f;     // Chance for cannon to spawn in Hard mode

    [Header("Scene Management")]
    public string sailingSceneName = "Production";

    private bool gameActive = false;
    private float currentSpeed;
    private int score;
    private bool hasAwardedGold = false;

    private Camera mainCamera;
    private Transform ground;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogWarning("No main camera found!");

        GameObject groundObj = GameObject.FindGameObjectWithTag("Ground");
        if (groundObj != null)
            ground = groundObj.transform;
        else
            Debug.LogWarning("No object with tag 'Ground' found!");

        easyButton.onClick.AddListener(() => StartGameWithMode(GameMode.Easy));
        hardButton.onClick.AddListener(() => StartGameWithMode(GameMode.Hard));
        closeButton.onClick.AddListener(CloseGame);

        gamePanel.SetActive(true);
        player.gameObject.SetActive(false);
    }

    void StartGameWithMode(GameMode mode)
    {
        // Set the cannon spawn probability based on mode
        if (mode == GameMode.Easy)
            spawner.cannonSpawnProbability = easyCannonChance;
        else
            spawner.cannonSpawnProbability = hardCannonChance;

        gameActive = true;
        hasAwardedGold = false;
        easyButton.gameObject.SetActive(false);
        hardButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        resultText.text = "";
        scoreText.text = "Score: 0";

        SpawnPlayerAtSafePosition();
        player.gameObject.SetActive(true);
        spawner.StartSpawning();

        currentSpeed = baseSpeed;
        score = 0;
    }

    private void SpawnPlayerAtSafePosition()
    {
        if (mainCamera == null)
        {
            if (playerStartPos != null)
                player.transform.position = playerStartPos.position;
            return;
        }

        float cameraLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float spawnX = cameraLeft + 1f;

        float groundY = 0f;
        if (ground != null)
        {
            Collider2D groundCollider = ground.GetComponent<Collider2D>();
            if (groundCollider != null)
                groundY = groundCollider.bounds.max.y;
            else
                groundY = ground.position.y;
        }

        Collider2D playerCollider = player.GetComponent<Collider2D>();
        float playerHalfHeight = playerCollider != null ? playerCollider.bounds.extents.y : 0.5f;
        float spawnY = groundY + playerHalfHeight;

        player.transform.position = new Vector3(spawnX, spawnY, 0f);
    }

    void Update()
    {
        if (!gameActive) return;

        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);

        Barrel.GlobalSpeed = currentSpeed;
        FlyingCannon.GlobalSpeed = currentSpeed;

        score += Mathf.RoundToInt(currentSpeed * Time.deltaTime);
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        if (!gameActive) return;
        gameActive = false;
        spawner.StopSpawning();
        player.gameObject.SetActive(false);

        int goldReward = score * goldPerPoint;

        if (!hasAwardedGold)
        {
            var rm = ResourceManager.Instance;
            if (rm != null)
            {
                rm.Add(Resource.Gold, goldReward);
                Debug.Log($"Awarded {goldReward} gold.");
            }
            else
            {
                Debug.LogWarning("ResourceManager not found – gold not awarded.");
            }
            hasAwardedGold = true;
        }

        resultText.text = $"You scored {score}!\nEarned {goldReward} Gold.";
        easyButton.gameObject.SetActive(true);
        hardButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    void CloseGame()
    {
        SceneManager.LoadScene(sailingSceneName);
    }

    public void AddCoinScore(int coinValue)
    {
        if (!gameActive) return;
        score += coinValue;
        scoreText.text = "Score: " + score;
    }

    public void AddParryScore(int value)
    {
        if (!gameActive) return;
        score += value;
        scoreText.text = "Score: " + score;
    }

    public Vector3 GetSafeSpawnPosition()
    {
        if (mainCamera == null || ground == null)
            return playerStartPos != null ? playerStartPos.position : Vector3.zero;

        float cameraLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float spawnX = cameraLeft + 1f;

        Collider2D groundCollider = ground.GetComponent<Collider2D>();
        float groundY = groundCollider != null ? groundCollider.bounds.max.y : ground.position.y;

        Collider2D playerCollider = player.GetComponent<Collider2D>();
        float playerHalfHeight = playerCollider != null ? playerCollider.bounds.extents.y : 0.5f;
        float spawnY = groundY + playerHalfHeight;

        return new Vector3(spawnX, spawnY, 0f);
    }
}