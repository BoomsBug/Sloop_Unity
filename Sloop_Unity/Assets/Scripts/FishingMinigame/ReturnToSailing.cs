
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToSailing : MonoBehaviour
{
    
    [SerializeField] private string sailingSceneName = "PRODUCTION";

    public void GoBack()
    {
        SceneManager.LoadScene(sailingSceneName);
    }

    /*
    void Update()
    {
        // Optional: allow Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
            GoBack();
    }
    */

}