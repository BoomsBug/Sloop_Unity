using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used Chat GPT to help me construct this script


public class CannonController : MonoBehaviour
{
    // Prefabs to be loaded in
    public GameObject cannonBallPrefab;

    // Particle system prefabs
    public GameObject CannonShotPrefab;
    public GameObject SmokePrefab;

    public Transform FirePoint;

    public float FireSpeed = 10f;
    public float FireCooldown = 0.5f;
    private float NextFireTime = 0f;

    Vector3 mousePos;


    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get mouse pos
        mousePos.z = 0f; // 2D so z must be 0

        Aim();  // Rotate cannon to face mouse position at every update (constantly)

        if (Input.GetMouseButtonDown(0) && Time.time >= NextFireTime)
            // If click left mouse and fire cooldown is over
        {
            Fire();
            NextFireTime = Time.time + FireCooldown; // Set next cooldown
        }
    }

    void Aim()
    {

        Vector2 direction = mousePos - transform.position;

        // angle to calculate where to fire with arc to hit target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // This makes the cannon rotate to face the mouse based on previous calc
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Fire()
    {
        // Make a cannonball from firepoint
        GameObject cannonBall = Instantiate(cannonBallPrefab, FirePoint.position, Quaternion.identity);

        // Fire from cannon muzzle
        Instantiate(CannonShotPrefab, transform.position, Quaternion.identity);

        // Smoke from cannon fire
        Instantiate(SmokePrefab, transform.position, Quaternion.identity);

        // Get Rigidbody to apply physics
        Rigidbody2D rb = cannonBall.GetComponent<Rigidbody2D>();

        // Get dir from firepoint to destination (mouse)
        Vector2 direction = (mousePos - FirePoint.position).normalized;

        // Apply force
        rb.AddForce(direction * FireSpeed, ForceMode2D.Impulse);
    }
}