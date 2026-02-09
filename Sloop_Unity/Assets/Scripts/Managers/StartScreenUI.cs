using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenUI : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.UpdateGameState(GameState.Sailing);
    }

    public void QuitGame()
    {
        Application.Quit();

        // quits for the editor but not the built version so we can test our code
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

    }
}
