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
    public int worldSeed;
    public static event Action<GameState> OnGameStateChanged;

    [Header("Boat State")]
    public Vector3 boatPosition;
    public Vector2 boatVelocity;
    public bool hasBoatState = false;

    [Header("Docking")]
    public int currentIslandID = -1;
    public string currentIslandMorality;


    // GameManager instance to grab from anywhere in game
    public static GameManager Instance { get; private set; }

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

        worldSeed = UnityEngine.Random.Range(0, 999999);
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
            case GameState.HIslandPort:
                HandleHIslandPort();
                break;
            case GameState.NIslandPort:
                HandleNIslandPort();
                break;
            case GameState.RIslandPort:
                HandleRIslandPort();
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
        SceneManager.LoadScene("PRODUCTION");
    }

    private void HandleIsland()
    {
        SceneManager.LoadScene("Island");
    }

    private void HandleHIslandPort()
    {
        SceneManager.LoadScene("H-islandPort");
    }

    private void HandleNIslandPort()
    {
        SceneManager.LoadScene("N-islandPort");
    }

    private void HandleRIslandPort()
    {
        SceneManager.LoadScene("R-islandPort");
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
        GameState[] Ports = {
            GameState.HIslandPort,
            GameState.NIslandPort,
            GameState.RIslandPort
        };

        int index = UnityEngine.Random.Range(0, Ports.Length);

        UpdateGameState(Ports[index]);


        //pseudocode for next steps in linking ports

        /*
         If IslandID == "Good" {
            UpdateGameState(GameState.HIslandPort);
         }
         
         If IslandID == "Neutral" {
            UpdateGameState(GameState.NIslandPort);
         }
         
         If IslandID == "Bad" {
             UpdateGameState(GameState.RRIslandPort);
         }
        */


    }



}

public enum GameState
{
    StartScreen,
    Sailing,
    Island,
    HIslandPort,
    NIslandPort,
    RIslandPort,
    VictoryMenu,
    LossMenu
}

