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

    public float FireSpeed = 17f;
    public float FireCooldown = 0.75f;
    private float NextFireTime = 0f;

    Vector3 mousePos;

    public int MaxShots = 10;
    private int ShotsLeft;
    private int TotalScore = 0;
    private int CurrentScore = 0;

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
            StartCoroutine(WaitForEndOfCannonShot());
            
        }
    }

    // Wait for 1.5 seconds so that last cannonball doesn't affect spawned in targets
    IEnumerator WaitForEndOfCannonShot()
    {
        yield return new WaitForSeconds(1.5f);
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

            ResetButton.SetActive(true);

            if (TotalScore > 100)
            {
                ReturnButton.SetActive(true);
            }

            if (TotalScore < 100)
            {
                WinLossText.text = "Lets Try That Again";
            }

            else if (TotalScore >= 100 && TotalScore < 200)
            {
                WinLossText.text = "You're A Deadshot!";
            }

            else if (TotalScore > 200)
            {
                WinLossText.text = "You're The Best Pirate There Ever Was!";
            }

            WinLossTextObj.SetActive(true);



            return;
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get mouse pos
        mousePos.z = 0f; // 2D so z must be 0

        Aim();  // Rotate cannon to face mouse position at every update (constantly)

        if (Input.GetMouseButtonDown(0) && Time.time >= NextFireTime)
            // If click left mouse and fire cooldown is over
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
    }
}