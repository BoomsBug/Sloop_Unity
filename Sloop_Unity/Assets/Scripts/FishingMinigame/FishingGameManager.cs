using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingGameManager : MonoBehaviour
{
    public PlayerResources resources;

    [Header("Rewards")]
    public int foodPerFish = 2;
    public int goldPerSack = 15;

    [Header("Streak Bonus (optional)")]
    public int streak = 0;
    public int streakFoodBonusEvery = 3; // every 3 catches give +1 food

    void Awake()
    {
        if (!resources) resources = FindObjectOfType<PlayerResources>();
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
    }

    public void CatchGold(GameObject loot)
    {
        Destroy(loot);

        streak++;
        if (resources) resources.AddGold(goldPerSack);
        Debug.Log($"Got gold sack! +{goldPerSack} gold");
    }

    public void Miss()
    {
        streak = 0;
        Debug.Log("Miss!");
    }
}