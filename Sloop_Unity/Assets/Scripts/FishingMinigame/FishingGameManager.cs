using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class FishingGameManager : MonoBehaviour
{
    public PlayerResources resources;

    [Header("Rewards")]
    public int foodPerFish = 2;
    public int goldPerSack = 15;

    [Header("Streak Bonus (optional)")]
    public int streak = 0;
    public int streakFoodBonusEvery = 3; // every 3 catches give +1 food

    public TMP_Text fishCounterText;      // top text 
    public RectTransform uiRoot;          // Canvas 
    public GameObject floatingTextPrefab; // floating text prefab 

    int fishCaught = 0;


    void Awake()
    {
        if (!resources) resources = FindObjectOfType<PlayerResources>();
        UpdateFishCounter();
    }

    public void CatchFish(GameObject fish)
    {
        Destroy(fish);

        streak++;
        int food = foodPerFish;

        if (streakFoodBonusEvery > 0 && streak % streakFoodBonusEvery == 0)
            food += 1;

        if (resources) resources.AddFood(food);
        Debug.Log($"Caught fish! +{food} food");

        fishCaught++;
        UpdateFishCounter();
        Popup("Caught!", fish.transform.position);
    }

    public void CatchGold(GameObject loot)
    {
        Destroy(loot);

        streak++;
        if (resources) resources.AddGold(goldPerSack);
        Debug.Log($"Got gold sack! +{goldPerSack} gold");

        Popup("Gold!", loot.transform.position);
    }

    public void Miss()
    {
        streak = 0;
        Debug.Log("Miss!");
    }

    void UpdateFishCounter()
    {
        if (fishCounterText != null)
            fishCounterText.text = $"Count: {fishCaught}";
    }

    void Popup(string msg, Vector3 worldPos)
    {
        if (!floatingTextPrefab || !uiRoot || Camera.main == null) return;

        var go = Instantiate(floatingTextPrefab, uiRoot);
        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
            rt.position = Camera.main.WorldToScreenPoint(worldPos);

        var ft = go.GetComponent<FloatingTextPopup>();
        if (ft != null) ft.Show(msg);
    }
}