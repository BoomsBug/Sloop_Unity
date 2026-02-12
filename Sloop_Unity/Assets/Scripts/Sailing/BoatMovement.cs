using Sloop.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


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

    private Animator boatAnimator;

    int currentDirection = 0;
    int targetDirection = 0;
    float turnDelay = 0.1f; 
    float turnTimer = 0f;

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

    void FixedUpdate()
    {
        // Arrow Controls or AWSD
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Vector2 windVelocity = WindVectorDirection(windDirection) * windStrength;

        float windMult = (RainScheduler.Instance != null) ? RainScheduler.Instance.CurrentWindMultiplier : 1f;
        Vector2 gust = (RainScheduler.Instance != null) ? RainScheduler.Instance.GustVector : Vector2.zero;

        Vector2 windVelocity = WindVectorDirection(windDirection) * windStrength * windMult + gust;
        Vector2 input = new Vector2(horizontalInput, verticalInput);
        //if (accelerated) {
        //    boatRigidbody.velocity = new Vector2(horizontalInput * boatAcceleration, verticalInput * boatAcceleration);
        //}
        //else {
        //    boatRigidbody.velocity = new Vector2(horizontalInput * boatSpeed, verticalInput * boatSpeed);
        //} 

        //boatRigidbody.velocity += windVelocity; // add wind velocity to boat's

        /*
        if (boatRigidbody.velocity.x > 0 && transform.localScale.x < 0 ||
            boatRigidbody.velocity.x < 0 && transform.localScale.x > 0) { // if going in opposite direction

            Vector3 newScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z); // Flip sprite to face other direction
            transform.localScale = newScale;

        }
        */
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

        //if (boatRigidbody.velocity.x > 0 && Mathf.Abs(boatRigidbody.velocity.y) < 0.01) {
        //    boatAnimator.SetInteger("direction", 0); // E
        //}
        //else if (boatRigidbody.velocity.x > 0 && boatRigidbody.velocity.y > 0) {
        //    boatAnimator.SetInteger("direction", 1); // NE
        //}
        //else if (Mathf.Abs(boatRigidbody.velocity.x) < 0.01 && boatRigidbody.velocity.y > 0) {
        //    boatAnimator.SetInteger("direction", 2); // N
        //}
        //else if (boatRigidbody.velocity.x < 0 && boatRigidbody.velocity.y > 0) {
        //    boatAnimator.SetInteger("direction", 3); // NW
        //}
        //else if (boatRigidbody.velocity.x < 0 && Mathf.Abs(boatRigidbody.velocity.y) < 0.01) {
        //    boatAnimator.SetInteger("direction", 4); // W
        //}
        //else if (boatRigidbody.velocity.x < 0 && boatRigidbody.velocity.y < 0) {
        //    boatAnimator.SetInteger("direction", 5); // SW
        //}
        //else if (Mathf.Abs(boatRigidbody.velocity.x) < 0.01 && boatRigidbody.velocity.y < 0) {
        //    boatAnimator.SetInteger("direction", 6); // S
        //}
        //else if (boatRigidbody.velocity.x > 0 && boatRigidbody.velocity.y < 0) {
        //    boatAnimator.SetInteger("direction", 7); // SE
        //}
        Vector2 moveDirection = DirectionToVector(currentDirection);
        float speed = accelerated ? boatSpeed * 1.5f : boatSpeed;

        boatRigidbody.velocity = Vector2.Lerp(
            boatRigidbody.velocity,
            moveDirection * speed,
            2f * Time.fixedDeltaTime
        );
        boatAnimator.SetInteger("direction", currentDirection);
        //boatRigidbody.velocity += windVelocity; // add wind velocity to boat's, but still affected by rain


        if (dockPressed) {
            boatRigidbody.velocity = Vector2.zero;  // dock (e.g. boat stops moving)


            // Switch to dock on island scene if at port and press E
            //if (GameManager.Instance != null)
            //{
                //GameManager.Instance.DockOnIsland(); Handled in Update instead
            //}
        }

        

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

            WorldGen worldGen = FindObjectOfType<WorldGen>();
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
            if (islandCollider && islandCollider.gameObject.GetComponent<Island>().hasTreasure)
            {
                SceneManager.LoadScene("Treasure");
            }
        }
        
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
