using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{

    private Rigidbody2D rigidbody;
    public float boatSpeed = 6.0f;
    private float boatAcceleration;
    bool accelerated = false;
    public Transform dockCheck;
    bool dockPressed = false;
    

    // Start is called before the first frame update
    void Start()
    {
        boatAcceleration = boatSpeed * 2f;
        rigidbody = GetComponent<Rigidbody2D>();
        
    }

    void FixedUpdate()
    {
        // Arrow Controls or AWSD
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (accelerated) {
            rigidbody.velocity = new Vector2(horizontalInput * boatAcceleration, verticalInput * boatAcceleration);
        }
        else {
            rigidbody.velocity = new Vector2(horizontalInput * boatSpeed, verticalInput * boatSpeed);
        } 

        if (rigidbody.velocity.x > 0 && transform.localScale.x < 0 ||
            rigidbody.velocity.x < 0 && transform.localScale.x > 0) { // if going in opposite direction

            Vector3 newScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z); // Flip sprite to face other direction
            transform.localScale = newScale;

        }

        if (dockPressed) {
            rigidbody.velocity = Vector2.zero;  // dock (e.g. boat stops moving)
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
