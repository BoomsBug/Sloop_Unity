using UnityEngine;
using TMPro; // Required for TextMeshPro

public class InputHandler : MonoBehaviour
{
    public TMP_InputField nameInputField; 

    public void OnEndEdit()
    {
        //get the text from the input field
        string seed = nameInputField.text;

        //if (seed.Length <= 0 || seed == "" || seed == null) seed = Random.Range(0,999999).ToString();
        
        //proccess input
        GameManager.Instance.worldSeed = int.Parse(seed);
    }
}