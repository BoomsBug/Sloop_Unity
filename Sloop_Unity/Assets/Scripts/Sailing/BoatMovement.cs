using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{

    private Rigidbody2D rigidbody;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        rigidbody.velocity = new Vector2(horizontalInput * 5.0f, verticalInput * 5.0f);
        //animator.SetFloat("speed", Mathf.Abs(rigidbody.velocity.x));

        if (rigidbody.velocity.x > 0 && transform.localScale.x < 0 ||
            rigidbody.velocity.x < 0 && transform.localScale.x > 0) {

            Vector3 newScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;

        }

    }
}
