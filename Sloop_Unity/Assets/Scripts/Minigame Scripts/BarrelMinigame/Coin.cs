using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 10;
    private BarrelDashTestManager manager;
    private bool canBeCollected = false;

    void Start()
    {
        manager = FindObjectOfType<BarrelDashTestManager>();
        // Small delay to prevent immediate collection if overlapping at spawn
        Invoke(nameof(EnableCollection), 0.1f);
    }

    void EnableCollection()
    {
        canBeCollected = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Coin triggered with: {other.gameObject.name}, tag: {other.tag}");

        if (!canBeCollected) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Coin collected by player!");
            if (manager != null)
                manager.AddCoinScore(value);
            Destroy(gameObject);
        }
    }
}