using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public int islandID;
    public Vector2 tileCoordinates;
    public Vector2 islandCenter;
    public bool isIsland;
    public float size; // < 0 is small, 0 is medium, > 0 is large
    public string morality;
    public GameObject port;
    public GameObject treasureGlint;
    public bool hasTreasure;
    public EncounterSO islandEvent;


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(islandCenter), 0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        WorldGen worldGen = FindObjectOfType<WorldGen>();
        int worldSeed = worldGen.seed;
        List<GameObject> islands = worldGen.islands;

        if (isIsland && size >= 0) //only medium / large islands
        {
            //Place port
            Vector2[] points = GetComponent<PolygonCollider2D>().points;
            GameObject newPort = Instantiate(port, transform.TransformPoint(points[Random.Range(0,points.Length)]), Quaternion.identity, gameObject.transform);
            newPort.GetComponent<SpriteRenderer>().sortingLayerName = "Port";
            newPort.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        if (hasTreasure)
        {
            GameObject glint = Instantiate(treasureGlint, transform.TransformPoint(islandCenter), Quaternion.identity, gameObject.transform); //add glint
            glint.GetComponent<SpriteRenderer>().sortingOrder = 10;
            glint.GetComponent<SpriteRenderer>().sortingLayerName = "Port";
        }
    }
}
