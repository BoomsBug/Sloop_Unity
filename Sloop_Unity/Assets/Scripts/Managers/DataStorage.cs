using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

// Class for in game data which should be saved
public class DataStorage
{
    // World data
    public GameState gameState;
    public int worldGenSeed;

    // Sloop-O-War data
    public float[] boatPosition;
    public float[] boatVelocity;
    public bool hasBoatState;

    // Island data
    public int currentIslandID;
    public string currentIsalndMorality;

    // Player data
    public float[] playerPosition;


}
