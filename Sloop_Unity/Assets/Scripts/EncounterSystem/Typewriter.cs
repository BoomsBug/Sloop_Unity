using UnityEngine;
using TMPro;
using System.Collections;

public class Typewriter : MonoBehaviour
{
    public TMP_Text textComponent; 
    public float letterDelay = 0.1f;
    private int totalCharacters;
    private bool typewriterActive;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void StartTypewriter()
    {
        totalCharacters = textComponent.text.Length;
        textComponent.maxVisibleCharacters = 0;
        typewriterActive = true;

        StartCoroutine(Delay(letterDelay));
    }

    void Update()
    {
        //for skipping to completed text
        if (!typewriterActive) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            textComponent.maxVisibleCharacters = totalCharacters;
            typewriterActive = false;
        }
    }

    IEnumerator Delay(float time)
    {
        while (textComponent.maxVisibleCharacters <= totalCharacters)
        {
            textComponent.maxVisibleCharacters ++;

            yield return new WaitForSecondsRealtime(time);
        }
        typewriterActive = false;
    }
}
