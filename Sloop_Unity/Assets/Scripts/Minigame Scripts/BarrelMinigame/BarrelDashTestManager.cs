using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Sloop.Economy;

public class BarrelDashTestManager : MonoBehaviour
{
    public enum GameMode { Tutorial, Hard }

    [Header("UI References")]
    public GameObject gamePanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI dashCounterText;
    public TextMeshProUGUI instructionText;   // <-- New: text for tutorial instructions
    public Button tutorialButton;
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
    public float tutorialCannonChance = 1f;
    public float hardCannonChance = 0.5f;

    [Header("Tutorial Instructions")]
    [TextArea(3, 5)]
    public string tutorialMessage = "Jump to avoid the barrels\nLeft Click to hit the birds and gain a Dash\nRight Click to dash through a bird";
    public float instructionDisplayTime = 3f;   // seconds before game starts

    [Header("Scene Management")]
    public string sailingSceneName = "Production";

    [Header("Player Spawn")]
    public float playerSpawnOffsetX = 0.2f;

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

        tutorialButton.onClick.AddListener(() => StartGameWithMode(GameMode.Tutorial));
        hardButton.onClick.AddListener(() => StartGameWithMode(GameMode.Hard));
        closeButton.onClick.AddListener(CloseGame);

        gamePanel.SetActive(true);
        player.gameObject.SetActive(false);

        // Hide instruction text initially
        if (instructionText != null)
            instructionText.gameObject.SetActive(false);
    }

    void StartGameWithMode(GameMode mode)
    {
        if (mode == GameMode.Tutorial)
        {
            // Show instructions first, then start the game after delay
            StartCoroutine(ShowInstructionsAndStart());
        }
        else // Hard mode – start immediately
        {
            ConfigureGameMode(GameMode.Hard);
            BeginGame();
        }
    }

    IEnumerator ShowInstructionsAndStart()
    {
        // Disable buttons and hide other UI elements during instructions
        tutorialButton.gameObject.SetActive(false);
        hardButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        if (dashCounterText != null) dashCounterText.gameObject.SetActive(false);
        resultText.text = "";

        // Show instruction text with "Press Space to continue"
        if (instructionText != null)
        {
            instructionText.text = tutorialMessage + "\n\nPress Space to continue";
            instructionText.gameObject.SetActive(true);
        }

        // Wait until Space is pressed
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        // Hide instructions
        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        // Re-enable score and dash counter for the game
        scoreText.gameObject.SetActive(true);
        if (dashCounterText != null) dashCounterText.gameObject.SetActive(true);

        // Configure and start the actual tutorial game
        ConfigureGameMode(GameMode.Tutorial);
        BeginGame();
    }

    void ConfigureGameMode(GameMode mode)
    {
        if (mode == GameMode.Tutorial)
        {
            spawner.tutorialMode = true;
            spawner.tutorialSpawnInterval = 1.5f;
            spawner.tutorialStartWithCannon = true;
            spawner.cannonSpawnProbability = 1f;
        }
        else // Hard
        {
            spawner.tutorialMode = false;
            spawner.cannonSpawnProbability = hardCannonChance;
        }
    }

    void BeginGame()
    {
        gameActive = true;
        hasAwardedGold = false;
        resultText.text = "";
        scoreText.text = "Score: 0";
        if (dashCounterText != null)
        {
            dashCounterText.text = "Dashes: 0";
            dashCounterText.gameObject.SetActive(true);
        }

        SpawnPlayerAtSafePosition();
        player.ResetPlayer();
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
        float spawnX = cameraLeft + playerSpawnOffsetX;

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

        if (dashCounterText != null && player != null)
        {
            dashCounterText.text = "Dashes: " + player.GetDashCharges();
        }
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
        tutorialButton.gameObject.SetActive(true);
        hardButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);   // keep score visible? Actually show result text instead, but keep layout
        if (dashCounterText != null)
            dashCounterText.gameObject.SetActive(false);
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
        float spawnX = cameraLeft + playerSpawnOffsetX;

        Collider2D groundCollider = ground.GetComponent<Collider2D>();
        float groundY = groundCollider != null ? groundCollider.bounds.max.y : ground.position.y;

        Collider2D playerCollider = player.GetComponent<Collider2D>();
        float playerHalfHeight = playerCollider != null ? playerCollider.bounds.extents.y : 0.5f;
        float spawnY = groundY + playerHalfHeight;

        return new Vector3(spawnX, spawnY, 0f);
    }
}