using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortUI : MonoBehaviour
{
    // Go to sailing game state
    public void SetSail()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.Sailing);
        }
    }
}
