using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Used Chat GPT to help me construct parts of this script


public class CannonBattleScript : MonoBehaviour
{
    public static CannonBattleScript Instance;

    // Prefabs to be loaded in
    public GameObject cannonBallPrefab;


    // Particle system prefabs
    public GameObject CannonShotPrefab;
    public GameObject SmokePrefab;

    Vector3 mousePos;

    private bool roundEnded = false;  // flag so we only calculate once


    [Header("UI")]
    public TextMeshProUGUI WinLossText;
    public GameObject WinLossTextObj;

    [Header("UIButtons")]
    public GameObject ResetButton;
    public GameObject ReturnButton;

    [Header("Health")]
    public int PlayerHealth = 100;
    public int EnemyHealth = 100;
    public TextMeshProUGUI PlayerHealthText;
    public TextMeshProUGUI EnemyHealthText;

    [Header("Player")]
    public float FireSpeed = 30f;
    public float FireCooldown = 0.75f;
    private float NextFireTime = 0f;
    public Transform FirePoint;
    public Transform PlayerTransform;
    public Rigidbody2D playerRB;


    [Header("Enemy")]
    public Transform EnemyFirePoint;
    public float EnemyFireCooldown = 1.5f;
    private float NextEnemyFireTime = 0f;


    [Header("AudioStuff")]
    public AudioSource CannonAudioSource;
    public AudioClip CannonAudioClip;
    public AudioClip ExplosionClip;


    private void Start()
    {
        Instance = this;
        UpdateHealthUI();
    }


    public void UpdateHealthUI()
    {
        PlayerHealthText.text = "Health: " + PlayerHealth;
        EnemyHealthText.text = "Health: " + EnemyHealth;
    }



    void Update()
    {
        if (!roundEnded && (PlayerHealth <= 0 || EnemyHealth <= 0))
        {        
            roundEnded = true;
        }

        if (roundEnded)
        {
            ReturnButton.SetActive(true);

            if (PlayerHealth <= 0)
            {
                WinLossText.text = "You Lose!";
            }

            else if (EnemyHealth <= 0)
            {
                WinLossText.text = "You Win!";
            }

            WinLossTextObj.SetActive(true);
            return;
        }


        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get mouse pos
        mousePos.z = 0f; // 2D so z must be 0

        Aim();  // Rotate cannon to face mouse position at every update (constantly)

        if (!roundEnded && Time.time >= NextEnemyFireTime)
        {
            EnemyFire();
            NextEnemyFireTime = Time.time + EnemyFireCooldown;
        }

        if (!roundEnded && Input.GetMouseButtonDown(0) && Time.time >= NextFireTime)
        // If click left mouse and fire cooldown is over, and round not ended
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

        cannonBall.GetComponent<CannonBallBattleScript>().ownerTag = "Player"; // this cannonball is players

        // Fire from cannon muzzle
        GameObject muzzleflash = Instantiate(CannonShotPrefab, FirePoint.position, Quaternion.identity);
        muzzleflash.transform.parent = cannonBall.transform;

        // Smoke from cannon fire
        Instantiate(SmokePrefab, transform.position, Quaternion.identity);


        // Get Rigidbody to apply physics
        Rigidbody2D rb = cannonBall.GetComponent<Rigidbody2D>();

        // Get dir from firepoint to destination (mouse)
        Vector2 direction = (mousePos - FirePoint.position).normalized;

        // Apply force
        rb.AddForce(direction * FireSpeed, ForceMode2D.Impulse);


        CannonAudioSource.PlayOneShot(CannonAudioClip, 1f); // Play 1 second of cannon firing

    }

    // Enemy boat firing logic
    void EnemyFire()
    {
        GameObject cannonBall = Instantiate(cannonBallPrefab, EnemyFirePoint.position, Quaternion.identity);

        cannonBall.GetComponent<CannonBallBattleScript>().ownerTag = "Enemy"; // this cannonball is players

        // Fire from cannon muzzle
        GameObject muzzleflash = Instantiate(CannonShotPrefab, EnemyFirePoint.position, Quaternion.identity);
        muzzleflash.transform.parent = cannonBall.transform;

        // Smoke from cannon fire
        Instantiate(SmokePrefab, EnemyFirePoint.position, Quaternion.identity);


        // Get Rigidbody to apply physics
        Rigidbody2D rb = cannonBall.GetComponent<Rigidbody2D>();


        Vector2 playerVelocity = playerRB.velocity; // get player velocity

        // estimate dist from player
        float distance = Vector2.Distance(PlayerTransform.position, EnemyFirePoint.position);

        float TimeToPlayer = distance / FireSpeed;

        // predicted player position
        Vector2 NewPredictedTarget = (Vector2)PlayerTransform.position + playerVelocity * TimeToPlayer;



        // Get dir from firepoint to destination (predicted player position)
        Vector2 direction = (NewPredictedTarget - (Vector2)EnemyFirePoint.position).normalized;

        // Throws off direction so enemy is not a perfect shot
        direction += new Vector2(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f));
        direction.Normalize();

        // Apply force
        rb.AddForce(direction * FireSpeed, ForceMode2D.Impulse);


        CannonAudioSource.PlayOneShot(CannonAudioClip, 1f); // Play 1 second of cannon firing

    }

}