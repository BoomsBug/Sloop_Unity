using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarrelDashTestManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gamePanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI resultText;
    public Button startButton;
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

    private bool gameActive = false;
    private float currentSpeed;
    private int score;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        closeButton.onClick.AddListener(CloseGame);

        gamePanel.SetActive(true);
        player.gameObject.SetActive(false);
    }

    void StartGame()
    {
        gameActive = true;
        startButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        resultText.text = "";
        scoreText.text = "Score: 0";

        // Reset player using its method
        player.ResetPlayer();
        player.gameObject.SetActive(true);

        // Reset spawner
        spawner.StartSpawning();

        // Reset speed & score
        currentSpeed = baseSpeed;
        score = 0;
    }

    void Update()
    {
        if (!gameActive) return;

        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);

        foreach (GameObject barrelObj in GameObject.FindGameObjectsWithTag("Barrel"))
        {
            Barrel barrel = barrelObj.GetComponent<Barrel>();
            barrel.speed = currentSpeed;
        }

        score += Mathf.RoundToInt(currentSpeed * Time.deltaTime);
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameActive = false;
        spawner.StopSpawning();
        player.gameObject.SetActive(false);

        int goldReward = score * goldPerPoint;
        resultText.text = $"You scored {score}!\nEarned {goldReward} Gold.";
        startButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    void CloseGame()
    {
        startButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        resultText.text = "";
        player.gameObject.SetActive(false);
        spawner.StopSpawning();
    }

    public void AddCoinScore(int coinValue)
    {
        if (!gameActive) return;
        score += coinValue;
        scoreText.text = "Score: " + score;
    }
}