using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sloop.Player;

public class ShipEncounterController : MonoBehaviour
{
    [Header("References")]
    public Transform playerShip;
    public BoatMovement boatMovement;           
    public GameObject enemyShipPrefab;          
    public EncounterUI ui;                      // UI panel script with 2 buttons (either sacrifice crew or engage with enemy)

    [Header("Approach")]
    public float spawnRadius = 14f;
    public float approachDistance = 12f;
    public float approachSpeed = 6f;

    [Header("Consequences")]
    public int crewCostIfPay = 1;
    public int honorDeltaIfPay = -10;

    public int powerCostIfEngage = 10;
    public int goldLootIfEngage = 25;
    public int honorDeltaIfEngage = +3;

    GameObject enemy;
    PlayerHonor honor;
    PlayerResources resources;
    bool running;

    Vector2 savedVel;
    Rigidbody2D playerRb;

    public GameObject floatingTextPrefab;
    public Vector3 popupOffset = new Vector3(0f, 1.5f, 0f);

    public Canvas encounterCanvas;


    void Awake()
    {
        // Find existing systems in the scene
        honor = FindObjectOfType<PlayerHonor>();
        resources = FindObjectOfType<PlayerResources>();
        if (!boatMovement && playerShip) boatMovement = playerShip.GetComponent<BoatMovement>();
    }

    public void StartEncounter()
    {
        if (running || !playerShip || !enemyShipPrefab || !ui) return;
        running = true;

        // Spawn enemy around player
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = playerShip.position + (Vector3)(dir * spawnRadius);
        enemy = Instantiate(enemyShipPrefab, spawnPos, Quaternion.identity);

        //Popup($"Power: {resources.power}\nGold: {resources.gold}\nCrew: {resources.crewMates}");
        if (resources != null && honor != null)
            Popup($"Honor: {honor.Honor}\nPower: {resources.power}\nGold: {resources.gold}\nCrew: {resources.crewMates}");
        else
            Debug.LogWarning("Missing PlayerHonor or PlayerResources in scene.");



        StartCoroutine(RunEncounter());
    }

    IEnumerator RunEncounter()
    {
        // Disable sailing controls during encounter
        //if (boatMovement) boatMovement.enabled = false;
        playerRb = playerShip.GetComponent<Rigidbody2D>();
        if (playerRb)
        {
            savedVel = playerRb.velocity;
            playerRb.velocity = Vector2.zero;
            playerRb.constraints = RigidbodyConstraints2D.FreezeAll; // stops wind drift too
        }

        // Approach
        Debug.Log("Encounter: approaching...");
        while (enemy && Vector2.Distance(enemy.transform.position, playerShip.position) > approachDistance)
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                playerShip.position,
                approachSpeed * Time.deltaTime
            );
            yield return null;
        }
        Debug.Log("Encounter: reached distance, showing UI now.");

        // Show choice
        ui.Show(
            onPay: PayAndLeave,
            onEngage: EngageAndLoot
        );
    }

    void PayAndLeave()
    {
        ui.Hide();

        if (resources) resources.AddCrew(-crewCostIfPay);
        if (honor) honor.AddHonor(honorDeltaIfPay);

        //Popup($"-1 Crew\nHonor {honorDeltaIfPay}");
        //Popup($"Crew -{crewCostIfPay} (now {resources.crewMates})\nHonor {honorDeltaIfPay} (now {honor.Honor})");
        Popup($"{StatLine("Crew", -crewCostIfPay, resources.crewMates, false)}\n" + $"{StatLine("Honor", honorDeltaIfPay, honor.Honor)}");


        StartCoroutine(EndEncounter(enemyLeaves: true));
    }

    void EngageAndLoot()
    {
        ui.Hide();

        if (resources)
        {
            //resources.AddPower(-powerCostIfEngage);
            resources.AddGold(goldLootIfEngage);
        }
        if (honor) honor.AddHonor(honorDeltaIfEngage);

        //Popup($"Gold +{goldLootIfEngage}\nPower -{powerCostIfEngage}\nHonor +{honorDeltaIfEngage}");
        Popup($"{StatLine("Gold", goldLootIfEngage, resources.gold)}\n" +
              $"{StatLine("Power", -powerCostIfEngage, resources.power, false)}\n" +
              $"{StatLine("Honor", honorDeltaIfEngage, honor.Honor)}");


        StartCoroutine(EndEncounter(enemyLeaves: false));
    }

    IEnumerator EndEncounter(bool enemyLeaves)
    {
        // simple exit animation
        if (enemy)
        {
            if (enemyLeaves)
            {
                Vector3 away = (enemy.transform.position - playerShip.position).normalized;
                float t = 0f;
                while (t < 1.0f && enemy)
                {
                    enemy.transform.position += away * 6f * Time.deltaTime;
                    t += Time.deltaTime;
                    yield return null;
                }
            }

            if (enemy) Destroy(enemy);
        }

        // Re-enable sailing controls
        //if (boatMovement) boatMovement.enabled = true;

        if (playerRb)
        {
            playerRb.constraints = RigidbodyConstraints2D.None;
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation; 
            playerRb.velocity = savedVel;
        }


        running = false;
    }

    /*
    void Popup(string msg)
    {
        if (!floatingTextPrefab || !playerShip) return;

        var go = Instantiate(floatingTextPrefab, playerShip.position + popupOffset, Quaternion.identity);
        var ft = go.GetComponent<FloatingTextPopup>();
        if (ft) ft.Show(msg);
    }
    */
    /*
    void Popup(string msg)
    {
        if (!floatingTextPrefab || !playerShip)
        {
            Debug.LogWarning("Popup skipped: missing prefab or playerShip.");
            return;
        }

        Vector3 pos = playerShip.position + popupOffset;
        var go = Instantiate(floatingTextPrefab, pos, Quaternion.identity);
        Debug.Log("Popup spawned at: " + pos);

        var ft = go.GetComponent<FloatingTextPopup>();
        if (ft) ft.Show(msg);
        else Debug.LogWarning("No FloatingTextPopup on prefab!");
    }
    */

    /*
    void Popup(string msg)
    {
        if (!floatingTextPrefab || !playerShip || !encounterCanvas) return;

        // create under canvas
        var go = Instantiate(floatingTextPrefab, encounterCanvas.transform);

        // set text
        var ft = go.GetComponent<FloatingTextPopup>();
        if (ft) ft.Show(msg);

        // position above ship on screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerShip.position + popupOffset);
        go.GetComponent<RectTransform>().position = screenPos;
    }
    */

    void Popup(string msg)
    {
        Debug.Log($"Popup() called. msg='{msg}'");

        if (!floatingTextPrefab)
        {
            Debug.LogError("Popup aborted: floatingTextPrefab is NOT assigned.");
            return;
        }
        if (!playerShip)
        {
            Debug.LogError("Popup aborted: playerShip is NOT assigned.");
            return;
        }

        // If you're doing UI popups:
        if (!encounterCanvas)
        {
            Debug.LogError("Popup aborted: encounterCanvas is NOT assigned.");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("Popup aborted: Camera.main is NULL (is your camera tagged MainCamera?).");
            return;
        }

        var go = Instantiate(floatingTextPrefab, encounterCanvas.transform);
        go.name = "PopupInstance";

        Debug.Log("Popup instantiated: " + go.name);

        var rt = go.GetComponent<RectTransform>();
        if (rt == null) Debug.LogWarning("PopupInstance has no RectTransform (is this really a UI prefab?).");

        var ft = go.GetComponent<FloatingTextPopup>();
        if (ft == null) Debug.LogError("PopupInstance missing FloatingTextPopup component!");
        else ft.Show(msg);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerShip.position + popupOffset);
        if (rt != null) rt.position = screenPos;
    }

    string StatLine(string name, int delta, int nowValue, bool showPlusForPositive = true)
    {
        string sign = (delta > 0 && showPlusForPositive) ? "+" : "";
        return $"{name} {sign}{delta} (now {nowValue})";
    }




}
