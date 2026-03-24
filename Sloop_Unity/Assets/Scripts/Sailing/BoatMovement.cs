using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sloop.World;
using UnityEngine.Animations;


public class BoatMovement : MonoBehaviour
{

    private Rigidbody2D boatRigidbody;
    public float boatSpeed = 6.0f;
    private float boatAcceleration;
    bool accelerated = false;

    public float dockCheckRadius;
    bool dockPressed = false;
    
    public enum WindDir {E, NE, N, NW, W, SW, S, SE} // 8 directions: 4 cardinal and 4 intercardinal direction
    public WindDir windDirection = WindDir.SW;
    public float windStrength = 1.5f;
    int currentDirection = 0;
    int targetDirection = 0;

    float turnDelay = 0.2f;
    float turnTimer = 0f;
    private Animator boatAnimator;

    //So the boat knows where it is on the map and can access the relevant island information
    public GameObject curOceanTile;

    // Start is called before the first frame update
    void Start()
    {
        boatAcceleration = boatSpeed * 2f;
        boatRigidbody = GetComponent<Rigidbody2D>();
        boatAnimator = GetComponent<Animator>();

        var gm = GameManager.Instance;
        if (gm != null && gm.hasBoatState)
        {
            transform.position = gm.boatPosition;
            boatRigidbody.velocity = gm.boatVelocity;
        }
        
        
    }
    Vector2 DirectionToVector(int dir)
    {
        switch (dir)
        {
            case 0: return new Vector2(1, 0);    // E
            case 1: return new Vector2(1, 1).normalized; // NE
            case 2: return new Vector2(0, 1);    // N
            case 3: return new Vector2(-1, 1).normalized; // NW
            case 4: return new Vector2(-1, 0);   // W
            case 5: return new Vector2(-1, -1).normalized; // SW
            case 6: return new Vector2(0, -1);   // S
            case 7: return new Vector2(1, -1).normalized;  // SE
        }

        return Vector2.right;
    }
    void FixedUpdate()
    {

        Vector2 gust = (RainScheduler.Instance != null) ? RainScheduler.Instance.GustVector : Vector2.zero;
        float windMult = (RainScheduler.Instance != null) ? RainScheduler.Instance.CurrentWindMultiplier : 1f;

        Vector2 windVelocity = WindVectorDirection(windDirection) * windStrength * windMult + gust;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 input = new Vector2(horizontalInput, verticalInput);


        if (input.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            angle = (angle + 360) % 360;

            targetDirection = Mathf.RoundToInt(angle / 45f) % 8;
        }

        turnTimer += Time.fixedDeltaTime;

        if (turnTimer >= turnDelay && currentDirection != targetDirection)
        {
            turnTimer = 0f;

            int diff = (targetDirection - currentDirection + 8) % 8;

            if (diff > 4)
                currentDirection = (currentDirection - 1 + 8) % 8;
            else
                currentDirection = (currentDirection + 1) % 8;
        }

        Vector2 moveDirection = DirectionToVector(currentDirection);

        float speed = accelerated ? boatSpeed * 1.5f : boatSpeed;
        

        Vector2 thrust = Vector2.zero;

        if (input.magnitude > 0.1f)
        {
            thrust = moveDirection * speed;
        }

        // Wind always affects boat
        Vector2 windDrift = windVelocity;

        // Final target velocity
        Vector2 targetVelocity = thrust + windDrift;

        // Smooth toward it
        boatRigidbody.velocity = Vector2.Lerp(
            boatRigidbody.velocity,
            targetVelocity,
            2f * Time.fixedDeltaTime
        );

    
        boatAnimator.SetInteger("direction", currentDirection);



        GameManager.Instance.UpdateBoatState(transform.position, boatRigidbody.velocity);


    }

    // This function returns the wind vector direction based on the direction chosen
    private Vector2 WindVectorDirection(WindDir dir) {
        switch (dir) {

            case WindDir.E:
                return new Vector2(1f, 0f);

            case WindDir.NE:
                return new Vector2(1f, 1f).normalized;

            case WindDir.N:
                return new Vector2(0f, 1f);

            case WindDir.NW:
                return new Vector2(-1f, 1f).normalized;

            case WindDir.W:
                return new Vector2(-1f, 0f);

            case WindDir.SW:
                return new Vector2(-1f, -1f).normalized;

            case WindDir.S:
                return new Vector2(0f, -1f);

            case WindDir.SE:
                return new Vector2(1f, -1f).normalized;

            default:
                return new Vector2(-1f, -1f);

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, dockCheckRadius);
    }

    // Update is called once per frame
    void Update()
    {
        accelerated = Input.GetKey(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.E) && Physics2D.OverlapCircle(gameObject.transform.position, dockCheckRadius, LayerMask.GetMask("Port"))) { // if close to Land and pressed "E"
            
            /*
            //call port scene
            Island curIsland = curOceanTile.GetComponent<Island>();
            if (curIsland.morality == "R")
            {
                //call ruthless port
                Debug.Log("Docking at ruthless");
            } else if (curIsland.morality == "N")
            {
                //call neutral port
                Debug.Log("Docking at neutral");
            } else if (curIsland.morality == "H")
            {
                //call honourable port
                Debug.Log("Docking at Honourable");
            } else Debug.Log("Invalid Island Morality");
            */

            dockPressed = !dockPressed;

            Island curIsland = curOceanTile.GetComponent<Island>();
            if (curIsland == null)
            {
                Debug.LogWarning("No Island component found on curOceanTile.");
                return;
            }

            //When player docks at a port, find the island that has the treasure, 
            WorldGen worldGen = FindObjectOfType<WorldGen>();
            Island treasureIsland = worldGen.islands[worldGen.treasureIsland].GetComponent<Island>();
            //  calculate the direction between that island and the player location,
            float angle = Vector2.SignedAngle(gameObject.transform.right, treasureIsland.tileCoordinates - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
            Debug.Log(treasureIsland.tileCoordinates);
            //  turn that angle into a cardinal direction, and give that string to GameManager.
            string cardinalDirection = AngleToDirection(angle);
            Random.InitState(GameManager.Instance.worldSeed);
            string randomDirection = AngleToDirection(Random.Range(0,360));
            //  GameManager will use it in the island scene to tell the player which way to head  
            GameManager.Instance.directionToTreasure = cardinalDirection;
            GameManager.Instance.randomDirection = randomDirection;

            int worldSeed = worldGen != null ? worldGen.seed : 0;

            // Save context for port scene NPC generation
            IslandVisitContext.Set(worldSeed, curIsland.islandID, curIsland.morality, curIsland.hasTreasure);

            Debug.LogWarning($"IslandID {curIsland.islandID}");


            // Save Boat context
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.boatPosition = transform.position;
                gm.boatVelocity = boatRigidbody.velocity;
                gm.hasBoatState = true;

                gm.currentIslandID = curIsland.islandID;
                gm.currentIslandMorality = curIsland.morality;
            }

            if (curIsland.hasTreasure)
            {
                SceneManager.LoadScene("Treasure");
            }

            // Load the correct port scene
            switch (curIsland.morality)
            {
                case "R":
                    SceneManager.LoadScene("R-IslandPort");
                    break;
                case "N":
                    SceneManager.LoadScene("N-IslandPort");
                    break;
                case "H":
                    SceneManager.LoadScene("H-IslandPort");
                    break;
                default:
                    Debug.LogWarning($"Invalid Island Morality: {curIsland.morality}");
                    break;
            }

        }

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            Collider2D islandCollider = Physics2D.OverlapCircle(gameObject.transform.position, dockCheckRadius, LayerMask.GetMask("Island"));
            if (islandCollider && !islandCollider.isTrigger && islandCollider.gameObject.GetComponent<Island>().hasTreasure)
            {
                SceneManager.LoadScene("Treasure");
            }
            else if (islandCollider && !islandCollider.isTrigger)
            {
                Debug.Log(islandCollider.gameObject.name);
                EncounterSystem.Instance.LoadEncounter(true); //true means it loads land encounters
            }
        }
        
    }

    private string AngleToDirection(float angle)
    {
        Debug.Log(angle);
        //takes a float representing the angle between two vectors in degrees and returns a cardinal direction (E, SE, N, etc...)

        if (angle < 0) angle += 360; //turns range from -180 -> 180 to 0 -> 360
        angle += 22.5f;
        if (angle > 360) angle %= 360; //turns 361 into just 1
        Debug.Log(angle);

        if (angle < 45) return "east";
        if (angle < 90) return "north east";
        if (angle < 135) return "north";
        if (angle < 180) return "north west";
        if (angle < 225) return "west";
        if (angle < 270) return "south west";
        if (angle < 315) return "south";
        if (angle < 360) return "south east";
        else return "INVALID DIRECTION";
    }

    // When the ship enters a new ocean tile, set curIsland to that tile
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ocean Tile")) 
        {
            curOceanTile = collision.gameObject;
        }
    }

}
