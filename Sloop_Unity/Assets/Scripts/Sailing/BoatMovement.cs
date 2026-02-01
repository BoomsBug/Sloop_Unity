using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{

    private Rigidbody2D boatRigidbody;
    public float boatSpeed = 6.0f;
    private float boatAcceleration;
    bool accelerated = false;

    public Transform dockCheck;
    bool dockPressed = false;
    
    public enum WindDir {E, NE, N, NW, W, SW, S, SE} // 8 directions: 4 cardinal and 4 intercardinal direction
    public WindDir windDirection = WindDir.SW;
    public float windStrength = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        boatAcceleration = boatSpeed * 2f;
        boatRigidbody = GetComponent<Rigidbody2D>();
        
    }

    void FixedUpdate()
    {
        // Arrow Controls or AWSD
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 windVelocity = WindVectorDirection(windDirection) * windStrength;

        if (accelerated) {
            boatRigidbody.velocity = new Vector2(horizontalInput * boatAcceleration, verticalInput * boatAcceleration);
        }
        else {
            boatRigidbody.velocity = new Vector2(horizontalInput * boatSpeed, verticalInput * boatSpeed);
        } 

        boatRigidbody.velocity += windVelocity; // add wind velocity to boat's

        if (boatRigidbody.velocity.x > 0 && transform.localScale.x < 0 ||
            boatRigidbody.velocity.x < 0 && transform.localScale.x > 0) { // if going in opposite direction

            Vector3 newScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z); // Flip sprite to face other direction
            transform.localScale = newScale;

        }

        if (dockPressed) {
            boatRigidbody.velocity = Vector2.zero;  // dock (e.g. boat stops moving)
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




    // Update is called once per frame
    void Update()
    {
        accelerated = Input.GetKey(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.E) && Physics2D.OverlapCircle(dockCheck.position, 8f, LayerMask.GetMask("Land"))) { // if close to Land and pressed "E"
            dockPressed = !dockPressed;
        }
        
    }
}
