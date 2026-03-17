using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Simple script that destroys cannonball after 5 seconds or on collision with
// ocean. Also, instantiates an explosion particle system prefab on collision with 
// Keg objects in scene.
public class CannonBallScript : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public int PointsPerBarrel = 10;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f); // Destroy after 5 seconds regardless
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( collision.gameObject.CompareTag("Ocean"))
        {
            Destroy(gameObject);
            
        }

        if (collision.gameObject.CompareTag("Keg"))
        {
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);

            CannonController.Instance.CannonAudioSource.PlayOneShot(CannonController.Instance.ExplosionClip, 1f); // Play 1 second of explosion sound upon keg hit

            Rigidbody2D kegRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            // Direction from cannonball to keg to get force dir 
            Vector2 forceDir = (collision.transform.position - transform.position).normalized;

            // Apply impulse force in force dir
            kegRb.AddForce(forceDir * 100f, ForceMode2D.Impulse);

            CannonController.Instance.AddScore(PointsPerBarrel);

            //collision.gameObject.SetActive(false); // hide hit target
            Destroy(collision.gameObject);
            Destroy(gameObject);
            
        }

    }

    private void OnDestroy()
    {
        CannonController.Instance.activeCannonballs--;
    }
}
