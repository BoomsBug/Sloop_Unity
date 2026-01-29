using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameState state;


    // GameManager instance to grab from anywhere in game
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.StartScreen:
                break;
            case GameState.Sailing: 
                break;
            case GameState.Island:
                break;
            case GameState.Port:
                break;
            case GameState.PauseMenu:
                break;
            case GameState.VictoryMenu:
                break;
            case GameState.LossMenu:
                break;

        }
    }
}

public enum GameState
{
    StartScreen,
    Sailing,
    Island,
    Port,
    PauseMenu,
    VictoryMenu,
    LossMenu
}
