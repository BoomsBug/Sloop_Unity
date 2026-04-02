using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class islandParent : MonoBehaviour
{
    public static islandParent Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
