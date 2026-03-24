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
    public float easyCannonProbability = 0f;   // No cannons in Easy
    public float hardCannonProbability = 0.5f;  // Cannons in Hard

    [Header("Scene Management")]
    public string sailingSceneName = "Production";

    private bool gameActive = false;
    private float currentSpeed;
    private int score;
    private bool hasAwardedGold = false;

    void Start()
    {
        easyButton.onClick.AddListener(() => StartGameWithMode(GameMode.Easy));
        hardButton.onClick.AddListener(() => StartGameWithMode(GameMode.Hard));
        closeButton.onClick.AddListener(CloseGame);

        gamePanel.SetActive(true);
        player.gameObject.SetActive(false);
    }

    void StartGameWithMode(GameMode mode)
    {
        // Set spawner's cannon probability based on mode
        if (mode == GameMode.Easy)
            spawner.cannonSpawnProbability = easyCannonProbability;
        else
            spawner.cannonSpawnProbability = hardCannonProbability;

        // Start the game
        gameActive = true;
        hasAwardedGold = false;
        easyButton.gameObject.SetActive(false);
        hardButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        resultText.text = "";
        scoreText.text = "Score: 0";

        player.ResetPlayer();
        player.gameObject.SetActive(true);

        spawner.StartSpawning();

        currentSpeed = baseSpeed;
        score = 0;
    }

    void Update()
    {
        if (!gameActive) return;

        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);

        // Update global speed for all barrels and cannons
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
}
