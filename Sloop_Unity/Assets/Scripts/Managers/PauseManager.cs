using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    public static bool Paused = false;
    public static PauseManager Instance;

    public GameObject PauseMenuOverlay;

    // On awake, make Pauemanager persist accross scenes
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



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Resume game
    public void Resume()
    {
        PauseMenuOverlay.SetActive(false);
        Time.timeScale = 1.0f;
        Paused = false;
    }

    // Pause game
    public void Pause()
    {
        PauseMenuOverlay.SetActive(true);
        Time.timeScale = 0.0f;
        Paused = true;
    }

    // Go to main menu/ start screen
    public void StartScreenSelect()
    {
        if (GameManager.Instance != null)
        {
            Resume();
            GameManager.Instance.UpdateGameState(GameState.StartScreen);
            
        }
    }

    // Quit game immediately
    public void QuitCurrentGame()
    {
        Application.Quit();

        // quits for the editor but not the built version so we can test our code
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }






}
