using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;



// Used Skeleton from Tarodev's tutorial for initial setup
// https://www.youtube.com/watch?v=4I0vonyqMi8

public class GameManager : MonoBehaviour
{

    public GameState state;

    public static event Action<GameState> OnGameStateChanged;


    // GameManager instance to grab from anywhere in game
    public static GameManager Instance;

    // One game manager only and game manager 
    // continues from scene to scene
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        // Starting game state
        if (SceneManager.GetActiveScene().name != "StartScreen")
        {
            UpdateGameState(GameState.StartScreen);
        }
        

    }

    public void StartGame()
    {
        UpdateGameState(GameState.Sailing);
    }

    public void QuitGame()
    {
        Application.Quit();

        // quits for the editor but not the built version so we can test our code
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.StartScreen:
                HandleStartScreen();
                break;
            case GameState.Sailing:
                HandleSailing();
                break;
            case GameState.Island:
                HandleIsland();
                break;
            case GameState.IslandPort:
                HandleIslandPort();
                break;
            case GameState.VictoryMenu:
                HandleVictoryMenu();
                break;
            case GameState.LossMenu:
                HandleLossMenu();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

        }

        // Has anything changed? If so, invoke newState 
        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleStartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }

    private void HandleSailing()
    {
        SceneManager.LoadScene("Sailing");
    }

    private void HandleIsland()
    {
        SceneManager.LoadScene("Island");
    }

    private void HandleIslandPort()
    {
        SceneManager.LoadScene("IslandPort");
    }

    private void HandleVictoryMenu()
    {
        SceneManager.LoadScene("VictoryMenu");
    }

    private void HandleLossMenu()
    {
        SceneManager.LoadScene("LossMenu");
    }


    public void DockOnIsland()
    {
        UpdateGameState(GameState.IslandPort);
    }



}

public enum GameState
{
    StartScreen,
    Sailing,
    Island,
    IslandPort,
    VictoryMenu,
    LossMenu
}

