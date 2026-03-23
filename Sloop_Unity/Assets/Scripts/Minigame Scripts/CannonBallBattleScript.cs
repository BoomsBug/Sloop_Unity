using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Simple script that destroys cannonball after 5 seconds or on collision with
// ocean. Also, instantiates an explosion particle system prefab on collision with 
// Keg objects in scene.
public class CannonBallBattleScript : MonoBehaviour
{
    public GameObject ExplosionPrefab;
    public int DamagePerHit = 10;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f); // Destroy after 5 seconds regardless
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ocean"))
        {

            Destroy(gameObject);

        }

        if (collision.gameObject.CompareTag("Player"))
        {
            CannonBattleScript.Instance.PlayerHealth -= DamagePerHit;
            CannonBattleScript.Instance.UpdateHealthUI();

            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            CannonBattleScript.Instance.CannonAudioSource.PlayOneShot(CannonBattleScript.Instance.ExplosionClip, 1f); // Play 1 second of explosion sound upon keg hit


            Destroy(gameObject);

        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            CannonBattleScript.Instance.EnemyHealth -= DamagePerHit;
            CannonBattleScript.Instance.UpdateHealthUI();

            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            CannonBattleScript.Instance.CannonAudioSource.PlayOneShot(CannonBattleScript.Instance.ExplosionClip, 1f); // Play 1 second of explosion sound upon keg hit


            Destroy(gameObject);

        }

    }
}
