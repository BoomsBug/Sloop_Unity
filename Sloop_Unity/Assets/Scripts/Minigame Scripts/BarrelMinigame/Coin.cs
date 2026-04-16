using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 10;
    private BarrelDashTestManager manager;
    private bool canBeCollected = false;


    public AudioClip coinClip;
    [Range(0f, 1f)]
    public float catchGoldVolume = 1f;

    [Range(2.2f, 3f)]
    public float catchGoldPitchMin = 2.3f;

    [Range(2.2f, 3)]
    public float catchGoldPitchMax = 2.9f;

    public AudioSource audioSource;

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
            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(catchGoldPitchMin, catchGoldPitchMax);
                audioSource.PlayOneShot(coinClip, catchGoldVolume);
            }
            if (manager != null)
                manager.AddCoinScore(value);
            Destroy(gameObject);
        }
    }
}