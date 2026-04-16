using UnityEngine;
using UnityEngine.SceneManagement;
using Sloop.UI;
using Sloop.Economy;

public class FishingGameManager : MonoBehaviour
{
    //public PlayerResources resources;

    [Header("Rewards")]
    public int foodPerFish = 2;
    public int goldPerSack = 15;

    [Header("Goal")]
    public int targetFish = 10;
    public float timeLimit = 30f;

    [Header("UI")]
    [SerializeField] public FishingHUDPanel hud;
    [SerializeField] public FishingResultPanel resultPanel;      // optional

    [Header("Audio")]
    public AudioClip catchFishSound;

    [Range(0f, 1f)]
    public float catchVolume = 1f;

    [Range(2.2f, 3f)]
    public float catchPitchMin = 2.3f;

    [Range(2.2f, 3)]
    public float catchPitchMax = 2.9f;

    private AudioSource audioSource;


    [Header("End")]
    public string returnSceneName = "PRODUCTION"; // or use GameManager later

    [Header("Popups")]
    public RectTransform uiRoot;         
    public GameObject floatingTextPrefab; 
    public Vector3 popupWorldOffset = new Vector3(0f, 0.6f, 0f);

    int fishCaught = 0;
    float timeLeft;
    bool ended = false;

    void Awake()
    {
        //if (!resources) resources = FindObjectOfType<PlayerResources>();
        audioSource = GetComponent<AudioSource>();
    }
    void PlayCatchSound()
    {
        if (catchFishSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(catchPitchMin, catchPitchMax);
            audioSource.PlayOneShot(catchFishSound, catchVolume);
        }
    }

    void Start()
    {
        timeLeft = timeLimit;
        UpdateGoalUI();
        UpdateTimerUI();
        UIManager.Instance.OpenPanel(resultPanel);
    }

    void Update()
    {
        if (ended) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        UpdateTimerUI();
        resultPanel.Hide();

        if (timeLeft <= 0f)
        {
            // Time's up => win if met target
            if (fishCaught >= targetFish) Win();
            else Lose();
        }
    }

    void UpdateGoalUI()
    {
        if (hud != null) hud.SetGoal(fishCaught,targetFish);
    }

    void UpdateTimerUI()
    {
        if (hud!=null) hud.SetTime(timeLeft);
    }

    public void CatchFish(GameObject fish)
    {
        if (ended) return;

        PlayCatchSound();

        Destroy(fish);
        Popup("Caught!", fish.transform.position + popupWorldOffset);
        Debug.Log("Popup");

        fishCaught++;
        UpdateGoalUI();

        int food = foodPerFish;
        //if (resources) resources.AddFood(food);
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(Resource.Food, food);

        // Instant win if you reach target before time ends
        if (fishCaught >= targetFish)
            Win();
    }

    public void CatchGold(GameObject loot)
    {
        if (ended) return;

        Destroy(loot);
        Popup("Gold!", loot.transform.position + popupWorldOffset);
        //if (resources) resources.AddGold(goldPerSack);
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.Add(Resource.Gold, goldPerSack);
    }

    public void Miss()
    {
        // optional: nothing
    }

    void Win()
    {
        ended = true;
        resultPanel.Show();
        ShowResult($"You win!\nCaught {fishCaught}/{targetFish} fish.");
        Invoke(nameof(ReturnToSailing), 1.5f);
    }

    void Lose()
    {
        ended = true;
        resultPanel.Show();
        ShowResult($"You lose!\nCaught {fishCaught}/{targetFish} fish.");
        Invoke(nameof(ReturnToSailing), 1.5f);
    }

    void ShowResult(string msg)
    {
        Debug.Log(msg);
        if (resultPanel!=null) resultPanel.SetResult(msg);
        UIManager.Instance.OpenPanel(resultPanel);
    }

    void ReturnToSailing()
    {
        SceneManager.LoadScene(returnSceneName);
    }

    void Popup(string msg, Vector3 worldPos)
    {
        if (!floatingTextPrefab || !uiRoot || Camera.main == null) return;

        var go = Instantiate(floatingTextPrefab, uiRoot);
        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
            rt.position = Camera.main.WorldToScreenPoint(worldPos);

        var ft = go.GetComponent<FloatingTextPopup>();
        if (ft != null) ft.Show(msg);
    }
}
