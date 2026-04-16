using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Sloop.Economy;

public class BarrelDashTestManager : MonoBehaviour
{
    // Singleton accessor
    public static BarrelDashTestManager Instance { get; private set; }

    public enum GameMode { Tutorial, Play }

    [Header("UI References")]
    public GameObject gamePanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI dashCounterText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI parryCounterText;   // New: displays birds parried (0-9)
    public Button tutorialButton;
    public Button playButton;
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
    public float playCannonChance = 0.5f;

    [Header("Tutorial Instructions")]
    [TextArea(3, 5)]
    public string tutorialMessage = "Jump to avoid the barrels\nLeft Click to hit the birds and gain a Dash\nRight Click to dash through a bird";

    [Header("Scene Management")]
    public string sailingSceneName = "Production";

    [Header("Player Spawn")]
    public float playerSpawnOffsetX = 0.2f;

    private bool gameActive = false;
    private float currentSpeed;
    private int score;
    private bool hasAwardedGold = false;
    private bool isTutorialMode = false;

    private Camera mainCamera;
    private Transform ground;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
        playButton.onClick.AddListener(() => StartGameWithMode(GameMode.Play));
        closeButton.onClick.AddListener(CloseGame);

        gamePanel.SetActive(true);
        player.gameObject.SetActive(false);

        scoreText.gameObject.SetActive(false);
        if (dashCounterText != null) dashCounterText.gameObject.SetActive(false);
        if (instructionText != null) instructionText.gameObject.SetActive(false);
        if (livesText != null) livesText.gameObject.SetActive(false);
        if (parryCounterText != null) parryCounterText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameActive) return;

        if (isTutorialMode && Input.GetKeyDown(KeyCode.P))
        {
            ExitTutorial();
            return;
        }

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

    void ExitTutorial()
    {
        gameActive = false;
        isTutorialMode = false;
        spawner.StopSpawning();
        player.gameObject.SetActive(false);

        tutorialButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        resultText.text = "Tutorial exited.\nChoose a mode to play.";
        scoreText.gameObject.SetActive(false);
        if (dashCounterText != null) dashCounterText.gameObject.SetActive(false);
        if (instructionText != null) instructionText.gameObject.SetActive(false);
        if (livesText != null) livesText.gameObject.SetActive(false);
        if (parryCounterText != null) parryCounterText.gameObject.SetActive(false);
    }

    void StartGameWithMode(GameMode mode)
    {
        if (mode == GameMode.Tutorial)
        {
            isTutorialMode = true;
            StartCoroutine(ShowInstructionsAndStart());
        }
        else
        {
            isTutorialMode = false;
            tutorialButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(true);
            if (dashCounterText != null) dashCounterText.gameObject.SetActive(true);
            resultText.text = "";

            ConfigureGameMode(GameMode.Play);
            BeginGame();
        }
    }

    IEnumerator ShowInstructionsAndStart()
    {
        tutorialButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        if (dashCounterText != null) dashCounterText.gameObject.SetActive(false);
        resultText.text = "";

        if (instructionText != null)
        {
            instructionText.text = tutorialMessage + "\n\nPress Space to continue\nPress P to exit tutorial any time";
            instructionText.gameObject.SetActive(true);
        }

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                ExitTutorial();
                yield break;
            }
            yield return null;
        }

        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        scoreText.gameObject.SetActive(true);
        if (dashCounterText != null) dashCounterText.gameObject.SetActive(true);

        ConfigureGameMode(GameMode.Tutorial);
        BeginGame();
    }

    void ConfigureGameMode(GameMode mode)
    {
        if (mode == GameMode.Tutorial)
        {
            spawner.tutorialMode = true;
            spawner.tutorialSpawnInterval = 1.5f;
            spawner.tutorialFirstSpawnIsCannon = true;
            spawner.cannonSpawnProbability = 1f;
        }
        else
        {
            spawner.tutorialMode = false;
            spawner.cannonSpawnProbability = playCannonChance;
        }
    }

    void BeginGame()
    {
        gameActive = true;
        hasAwardedGold = false;
        resultText.text = "";
        scoreText.text = "Score: 0";
        if (dashCounterText != null)
            dashCounterText.text = "Dashes: 0";

        // Show lives and parry counter UI
        if (livesText != null)
        {
            livesText.gameObject.SetActive(true);
            livesText.text = "Lives: " + player.GetCurrentLives();
        }
        if (parryCounterText != null)
        {
            parryCounterText.gameObject.SetActive(true);
            parryCounterText.text = "Birds Parried: 0";
        }

        SpawnPlayerAtSafePosition();
        player.ResetPlayer();   // This will update lives and parry counter UI
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

    public void GameOver()
    {
        if (!gameActive) return;
        gameActive = false;
        spawner.StopSpawning();
        player.gameObject.SetActive(false);

        int goldReward = 0;
        if (!isTutorialMode)
        {
            goldReward = score * goldPerPoint;
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
        }

        if (isTutorialMode)
        {
            resultText.text = $"Tutorial Score: {score}\nNo gold awarded in tutorial.";
        }
        else
        {
            resultText.text = $"You scored {score}!\nEarned {goldReward} Gold.";
        }

        tutorialButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        if (dashCounterText != null)
            dashCounterText.gameObject.SetActive(false);
        if (livesText != null)
            livesText.gameObject.SetActive(false);
        if (parryCounterText != null)
            parryCounterText.gameObject.SetActive(false);
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

    // Called from PlayerController to update the lives UI
    public void UpdateLivesUI(int lives)
    {
        if (livesText != null)
            livesText.text = "Lives: " + lives;
    }

    // Called from PlayerController to update the parry counter UI (shows 0-9)
    public void UpdateParryCounter(int parryCount)
    {
        if (parryCounterText != null)
        {
            int displayValue = parryCount % 10;
            parryCounterText.text = "Birds Parried: " + displayValue;
        }
    }
}