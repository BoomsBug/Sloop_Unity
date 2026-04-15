using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Used Chat GPT to help me construct parts of this script


public class CannonController : MonoBehaviour
{
    public static CannonController Instance;

    // Prefabs to be loaded in
    public GameObject cannonBallPrefab;

    public GameObject TargetPrefab;

    // Particle system prefabs
    public GameObject CannonShotPrefab;
    public GameObject SmokePrefab;

    public Transform FirePoint;

    public float FireSpeed = 18f;
    public float FireCooldown = 0.75f;
    private float NextFireTime = 0f;

    Vector3 mousePos;

    public int MaxShots = 10;
    private int ShotsLeft;
    private int TotalScore = 0;
    private int CurrentScore = 0;
    private int shotsFired = 0;
    public int shotsHit = 0;
    float accuracy = 0f;
    int finalAccuracy;


    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI WinLossText;
    public GameObject WinLossTextObj;

    [Header("UIButtons")]
    public GameObject ResetButton;
    public GameObject ReturnButton;


    [Header("TimerSettings")]
    public float RoundTime = 60f;  // 30 seconds
    private float TimeLeft;

    [Header("TargetSpawnSettings")]
    public Transform SpawnBorderA;  // border for target spawn
    public Transform SpawnBorderB;  // border for target spawn
    public int MaxTargets = 8;
    public int MinTargets = 5;


    [Header("AudioStuff")]
    public AudioSource CannonAudioSource;
    public AudioClip CannonAudioClip;
    public AudioClip ExplosionClip;


    public int activeCannonballs = 0; // counts cannonballs in flight
    private bool roundEnded = false;  // flag so we only calculate once




    private void Start()
    {
        Instance = this;
        ShotsLeft = MaxShots;
        TimeLeft = RoundTime;

        SpawnRandomTargets();
    }

    void UpdateAmmoMenu()
    {
        ammoText.text = "Ammo: " + ShotsLeft;
    }

    void UpdateTimerMenu()
    {
        timerText.text = "Time Remaining: " + Mathf.CeilToInt(TimeLeft) + " Seconds"; // Ceiling function turning float to int to show cleaner UI
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
        TotalScore += points;
        scoreText.text = "Score:  " + TotalScore;

        if (CurrentScore >= 50)
        {
            StartCoroutine(WaitForEndOfCannonShot(1.5f));
            
        }
    }

    // Wait for 1.5 seconds so that last cannonball doesn't affect spawned in targets
    IEnumerator WaitForEndOfCannonShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetTargets();
    }


    void ResetTargets()
    {
        CurrentScore = 0;
        ShotsLeft = MaxShots;
        UpdateAmmoMenu();

        SpawnRandomTargets();

    }

    // Spawn the targets
    void SpawnRandomTargets()
    {

        // Destroy old targets
        GameObject[] previousTargets = GameObject.FindGameObjectsWithTag("Keg");
        foreach (GameObject previousTarget in previousTargets) 
        { 
            Destroy(previousTarget);
        }

        int TargetCount = Random.Range(MinTargets, MaxTargets + 1);

        // Random position and target number in scene on respawn
        for (int i = 0; i < TargetCount; i++)
        {
            Vector3 RandomPosition = new Vector3(Random.Range(SpawnBorderA.position.x, SpawnBorderB.position.x), 
                Random.Range(SpawnBorderA.position.y, SpawnBorderB.position.y), 
                0f);

            GameObject Target = Instantiate(TargetPrefab, RandomPosition, Quaternion.identity);
            Target.SetActive(true);
        }
 
    }



    void Update()
    {
        TimeLeft -= Time.deltaTime;
        UpdateTimerMenu();

        if (TimeLeft < 0)
        {
            timerText.text = "Time Remaining: 0 Seconds";
            roundEnded = true;

            //ResetButton.SetActive(true);
        }
        if (roundEnded && activeCannonballs == 0) {

            if (TotalScore >= 100)
            {
                ReturnButton.SetActive(true);
            }

            if (shotsFired > 0)
            {
                accuracy = (float)shotsHit / shotsFired * 100;
            }
            finalAccuracy = Mathf.RoundToInt(accuracy);



            // Score texts

            if (TotalScore < 25)
            {
                WinLossText.text = "Arr... ye shoot like the cannon's pointed backwards!";
            }

            else if (TotalScore >= 25 && TotalScore < 50)
            {
                WinLossText.text = "I've seen cannonballs roll straighter than that!";
            }

            else if (TotalScore >= 50 && TotalScore < 75)
            {
                WinLossText.text = "Had a bit too much rum, aye?";
            }

            else if (TotalScore >= 75 && TotalScore < 100)
            {
                WinLossText.text = "Yer gettin’ there… don’t lose yer sea legs now!";
            }

            else if (TotalScore >= 100 && TotalScore < 150)
            {
                WinLossText.text = "Aye, not bad shootin' Cap'n!";
            }

            else if (TotalScore >= 150 && TotalScore < 200)
            {
                WinLossText.text = "Deadly aim, matey!";
            }

            else if (TotalScore >= 200 && TotalScore < 250)
            {
                WinLossText.text = "Ships will flee at the sight of ye!";
            }

            else if (TotalScore >= 250 && TotalScore < 300)
            {
                WinLossText.text = "The ocean itself fears yer aim!";
            }

            else if (TotalScore >= 300)
            {
                WinLossText.text = "Ye be the greatest pirate to ever live!";
            }

            WinLossText.text += "\n\nFinal Score: " + TotalScore + "\nAccuracy: " + finalAccuracy + "%";


            WinLossTextObj.SetActive(true);
            return;
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get mouse pos
        mousePos.z = 0f; // 2D so z must be 0

        Aim();  // Rotate cannon to face mouse position at every update (constantly)

        if (!roundEnded && Input.GetMouseButtonDown(0) && Time.time >= NextFireTime)
            // If click left mouse and fire cooldown is over, and round not ended
        {
            if (ShotsLeft > 0) {
                Fire();
                NextFireTime = Time.time + FireCooldown; // Set next cooldown

                ShotsLeft--;
                UpdateAmmoMenu();
            }
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

        activeCannonballs++;
        shotsFired++;

    }
}