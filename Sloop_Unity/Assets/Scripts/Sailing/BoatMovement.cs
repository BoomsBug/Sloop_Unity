using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{

    private Rigidbody2D rigidbody;
    public float boatSpeed = 6.0f;
    private float boatAcceleration;
    bool accelerated = false;
    

    // Start is called before the first frame update
    void Start()
    {
        boatAcceleration = boatSpeed * 2f;
        rigidbody = GetComponent<Rigidbody2D>();
        
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (accelerated) {
            rigidbody.velocity = new Vector2(horizontalInput * boatAcceleration, verticalInput * boatAcceleration);
        }
        else {
            rigidbody.velocity = new Vector2(horizontalInput * boatSpeed, verticalInput * boatSpeed);
        } 

        //animator.SetFloat("speed", Mathf.Abs(rigidbody.velocity.x));

        if (rigidbody.velocity.x > 0 && transform.localScale.x < 0 ||
            rigidbody.velocity.x < 0 && transform.localScale.x > 0) {

            Vector3 newScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;

        }

        //float boatAcceleration = boatSpeed * 1.2f;

        


    }

    // Update is called once per frame
    void Update()
    {
        accelerated = Input.GetKey(KeyCode.Space);
        
    }
}
