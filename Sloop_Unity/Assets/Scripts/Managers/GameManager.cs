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
            case GameState.IslandPort2:
                HandleIslandPort2();
                break;
            case GameState.IslandPort3:
                HandleIslandPort3();
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

    private void HandleIslandPort2()
    {
        SceneManager.LoadScene("IslandPort 2");
    }

    private void HandleIslandPort3()
    {
        SceneManager.LoadScene("IslandPort 3");
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
            GameState.IslandPort,
            GameState.IslandPort2,
            GameState.IslandPort3
        };

        int index = UnityEngine.Random.Range(0, Ports.Length);

        UpdateGameState(Ports[index]);


        //pseudocode for next steps in linking ports

        /*
         If IslandID == "Good" {
            UpdateGameState(GameState.IslandPort);
         }
         
         If IslandID == "Neutral" {
            UpdateGameState(GameState.IslandPort2);
         }
         
         If IslandID == "Bad" {
             UpdateGameState(GameState.IslandPort3);
         }
        */


    }



}

public enum GameState
{
    StartScreen,
    Sailing,
    Island,
    IslandPort,
    IslandPort2,
    IslandPort3,
    VictoryMenu,
    LossMenu
}

