using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public int islandID;
    public Vector2 tileCoordinates;
    public Vector2 islandCenter;
    public bool isIsland;
    public int size; // < 0 is small, 0 is medium, > 0 is large


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(islandCenter, 0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        WorldGen worldGen = FindObjectOfType<WorldGen>();
        int worldSeed = worldGen.seed;
        List<Island> islands = worldGen.islands;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
