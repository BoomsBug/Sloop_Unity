using System.Collections;
using TMPro;
using UnityEngine;

/*
Most of the code in this script is AI-generated

Code functionality:
This script is responsible for displaying some floating text that fades away, 
for a few seconds, depending on the duration and holdTime values.
*/

public class FloatingTextPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public float floatUp = 1.5f;
    public float duration = 3f;
    public float holdTime = 0.6f;


    void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();
    }

    public void Show(string message)
    {
        if (text == null)
        {
            Debug.LogError("FloatingTextPopup: No TMP_Text found on prefab.");
            Destroy(gameObject);
            return;
        }

        text.text = message;
        StartCoroutine(Animate());
    }

    /*
    IEnumerator Animate()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * floatUp;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = t / duration;

            transform.position = Vector3.Lerp(start, end, a);

            var c = text.color;
            c.a = Mathf.Lerp(1f, 0f, a);
            text.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
    */
    IEnumerator Animate()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * floatUp;

        // stay visible first
        yield return new WaitForSeconds(holdTime);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = t / duration;

            transform.position = Vector3.Lerp(start, end, a);

            var c = text.color;
            c.a = Mathf.Lerp(1f, 0f, a);
            text.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }



}
