using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCannon : MonoBehaviour
{
    public static float GlobalSpeed = 5f;

    private bool moving = true;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            // Freeze rotation to prevent spinning
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Update()
    {
        if (moving && rb != null)
        {
            rb.velocity = new Vector2(-GlobalSpeed, 0f);
            // Also explicitly set angular velocity to zero (extra safety)
            rb.angularVelocity = 0f;
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void StopMoving()
    {
        moving = false;
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    public void Parry()
    {
        Destroy(gameObject);
    }

    // Stub method for any Animation Event that may exist on the BirdFly clip.
    private void BirdFlyEvent() { }
}