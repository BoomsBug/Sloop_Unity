using System.Collections;
using UnityEngine;
using Sloop.Player;


/*
Most of the code in this script is AI-generated

Code functionality:
This script is responsible for simulating a chase encounter while sailing.
At random, the enemy ship starts chasing the player ship until either 
the player gets away for escapeDistance for escapeHoldTime or the chase time has exceeded
the maxChaseTime. If the player gets caught, they lose goldLostOnCaught
and honorDeltaOnCaught. If they escape, they get honorDeltaOnEscape.
*/
public class ShipChaseController : MonoBehaviour
{
    [Header("Refs")]
    public Transform playerShip;
    public GameObject enemyShipPrefab;
    public BoatMovement boatMovement; 

    [Header("Spawn / Movement")]
    public float spawnRadius = 18f; //radius which within enemy spawns
    public float enemySpeed = 6f; //how fast enemy moves towards you

    [Header("Win/Lose")]
    public float catchDistance = 4f; //how close enemy ship needs to be to catch you
    public float escapeDistance = 14f; //how far away you need to be to start escaping
    public float escapeHoldTime = 4f; //how long you need to stay beyond escapeDistance
    public float maxChaseTime = 25f;

    [Header("Consequences")]
    public int honorDeltaOnEscape = +2;
    public int honorDeltaOnCaught = -5;
    public int goldLostOnCaught = 10;

    [Header("Popups (optional)")]
    public GameObject floatingTextPrefab;
    public Canvas encounterCanvas;
    public Vector3 popupOffset = new Vector3(0f, 1.5f, 0f);

    GameObject enemy;
    bool running;
    public bool IsRunning => running;

    PlayerHonor honor;
    PlayerResources resources;

    void Awake()
    {
        honor = FindObjectOfType<PlayerHonor>();
        resources = FindObjectOfType<PlayerResources>();
    }

    public void StartChase()
    {
        if (running || !playerShip || !enemyShipPrefab) return;
        running = true;

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = playerShip.position + (Vector3)(dir * spawnRadius); //randomly spawn enemy within spawnRadius
        enemy = Instantiate(enemyShipPrefab, spawnPos, Quaternion.identity); //instantiate enemy prefab

        Popup("Chase started!");
        StartCoroutine(RunChase());
    }

    IEnumerator RunChase()
    {
        float farTimer = 0f;
        float chaseTimer = 0f;

        while (enemy && chaseTimer < maxChaseTime)
        {
            chaseTimer += Time.deltaTime;

            // simple pursuit
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                playerShip.position,
                enemySpeed * Time.deltaTime
            );

            float d = Vector2.Distance(enemy.transform.position, playerShip.position);

            // caught
            if (d <= catchDistance)
            {
                ApplyCaught();
                yield break;
            }

            // escape (must stay far away for some time)
            if (d >= escapeDistance) farTimer += Time.deltaTime;
            else farTimer = 0f;

            if (farTimer >= escapeHoldTime)
            {
                ApplyEscape();
                yield break;
            }

            yield return null;
        }

        ApplyEscape();
    }

    void ApplyEscape()
    {
        if (honor) honor.AddHonor(honorDeltaOnEscape); //add honor
        Popup($"Escaped!\nHonor +{honorDeltaOnEscape} (now {honor?.Honor})"); //display popup message
        EndChase();
    }

    void ApplyCaught()
    {
        if (honor) honor.AddHonor(honorDeltaOnCaught); //subtract honor
        if (resources) resources.SpendGold(goldLostOnCaught); //subtract gold

        Popup($"Caught!\nGold -{goldLostOnCaught} (now {resources?.gold})\nHonor {honorDeltaOnCaught} (now {honor?.Honor})"); //display popup message
        EndChase();
    }

    void EndChase()
    {
        if (enemy) Destroy(enemy); //destroy enemy prefab
        enemy = null;
        running = false;
    }

    // Shows popup messages
    void Popup(string msg)
    {
        if (!floatingTextPrefab || !playerShip || !encounterCanvas || Camera.main == null) return;

        var go = Instantiate(floatingTextPrefab, encounterCanvas.transform);
        var ft = go.GetComponent<FloatingTextPopup>();
        if (ft) ft.Show(msg);

        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
            rt.position = Camera.main.WorldToScreenPoint(playerShip.position + popupOffset);
    }
}