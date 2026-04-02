using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public int crewID;
    public GameObject bubblePrefab;
    public float secondsBubbleIsActive = 3;
    public float bubbleChance;
    public float bubbleOccurance;
    private float timer;
    private GameObject newBubble;

    void Start()
    {
        Random.InitState(GameManager.Instance.worldSeed);
        //StartCoroutine(WaitToLoad());
    }

    void Update()
    {
        //run timer to load bubble
        timer += Time.deltaTime;
        if (timer >= bubbleOccurance)
        {
            timer = 0;
            if (Random.value <= bubbleChance) LoadBubble();
            
        }
    }

    //handle crew speech bubbles
    public void LoadBubble()
    {
        Crewmate crew = null;
        foreach (Crewmate i in CrewManager.Instance.hiredCrew)
        {
            if (i.crewID == crewID) crew = i;
        }
        if (crew == null) Debug.Log("ERROR: Failed to find crew when loading bubble!");

        int index = Random.Range(0, crew.dialogues.Length);
        string text = crew.dialogues[index];
        
        newBubble = Instantiate(bubblePrefab, parent:gameObject.transform, position:gameObject.transform.position + new Vector3(0, 200), rotation:Quaternion.identity);

        newBubble.GetComponentInChildren<TextMeshProUGUI>().text = text;
        newBubble.GetComponentInChildren<Typewriter>().StartTypewriter();

        StartCoroutine(WaitToUnload());
    }
    
    public void UnloadBubble()
    {
        Destroy(newBubble);
    }

    IEnumerator WaitToUnload()
    {
        yield return new WaitForSecondsRealtime(secondsBubbleIsActive);
        UnloadBubble();
    }

    IEnumerator WaitToLoad()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(bubbleOccurance);
            if (Random.value < bubbleChance) {
                LoadBubble();
                //StartCoroutine(WaitToLoad());
            }
        }
    }
}
