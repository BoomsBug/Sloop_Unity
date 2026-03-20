using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;
using TMPro;
using Sloop.Economy;
using Sloop.Player;
using Sloop.Time;

namespace Sloop.UI
{
    public class pocHUD : MonoBehaviour
    {
        public static pocHUD Instance { get; private set; }

        [Header("UI Text References")]
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text woodText;
        [SerializeField] private TMP_Text foodText;
        [SerializeField] private TMP_Text powerText;
        [SerializeField] private TMP_Text honorText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text dateText;

        [Header("Persist across scenes")]
        [SerializeField] private bool dontDestroyOnLoad = true;

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            HookResourceManager();
            //HookPlayerStateManager();
            HookTimeManager();

            RefreshAll();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            UnhookResourceManager();
            //UnhookPlayerStateManager();
            UnhookTimeManager();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Re-hook in case managers were created/destroyed due to scene order.
            UnhookResourceManager();
            //UnhookPlayerStateManager();
            UnhookTimeManager();

            HookResourceManager();
            //HookPlayerStateManager();
            HookTimeManager();

            RefreshAll();
        }

        // -----------------------------
        // Hooking
        // -----------------------------
        private void HookResourceManager()
        {
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
        }

        private void UnhookResourceManager()
        {
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
        }

        // private void HookPlayerStateManager()
        // {
        //     if (PlayerStateManager.Instance != null)
        //         PlayerStateManager.Instance.OnHonorChanged += OnHonorChanged;
        // }

        // private void UnhookPlayerStateManager()
        // {
        //     if (PlayerStateManager.Instance != null)
        //         PlayerStateManager.Instance.OnHonorChanged -= OnHonorChanged;
        // }

        private void HookTimeManager()
        {
            if (GameTimeManager.Instance != null)
                GameTimeManager.Instance.OnTimeChanged += OnTimeChanged;
        }

        private void UnhookTimeManager()
        {
            if (GameTimeManager.Instance != null)
                GameTimeManager.Instance.OnTimeChanged -= OnTimeChanged;
        }

        // -----------------------------
        // Events
        // -----------------------------
        private void OnResourceChanged(Resource type, int newAmount)
        {
            switch (type)
            {
                case Resource.Gold:  SetText(goldText,  $"Gold: {newAmount}"); break;
                case Resource.Wood:  SetText(woodText,  $"Wood: {newAmount}"); break;
                case Resource.Food:  SetText(foodText,  $"Food: {newAmount}"); break;
                case Resource.Power: SetText(powerText, $"Power: {newAmount}"); break;
                case Resource.Honour: SetText(powerText, $"Honour: {newAmount}"); break;
            }
        }

        // private void OnHonorChanged(int newHonor)
        // {
        //     SetText(honorText, $"Honor: {newHonor}");
        // }

        private void OnTimeChanged(int day, int hour, int minute)
        {
            string period = hour >= 12 ? "pm" : "am";

            int displayHour = hour % 12;
            if (displayHour == 0)
                displayHour = 12;

            SetText(timeText, $"{displayHour}{period}");

            var tm = GameTimeManager.Instance;
            SetText(dateText, $"{tm.Month:00}/{tm.Day:00}/{tm.Year}");
        }
        // -----------------------------
        // Refresh
        // -----------------------------
        private void RefreshAll()
        {
            RefreshResources();
            //RefreshHonor();
            RefreshTime();
        }

        private void RefreshResources()
        {
            var rm = ResourceManager.Instance;
            if (rm == null)
            {
                SetText(goldText,  "Gold: (no RM)");
                SetText(woodText,  "Wood: (no RM)");
                SetText(foodText,  "Food: (no RM)");
                SetText(powerText, "Power: (no RM)");
                return;
            }

            SetText(goldText,  $"Gold: {rm.GetAmount(Resource.Gold)}");
            SetText(woodText,  $"Wood: {rm.GetAmount(Resource.Wood)}");
            SetText(foodText,  $"Food: {rm.GetAmount(Resource.Food)}");
            SetText(powerText, $"Power: {rm.GetAmount(Resource.Power)}");
            SetText(powerText, $"Honour: {rm.GetAmount(Resource.Honour)}");
        }

        // private void RefreshHonor()
        // {
        //     var ps = PlayerStateManager.Instance;
        //     if (ps == null)
        //     {
        //         SetText(honorText, "Honor: (no PS)");
        //         return;
        //     }

        //     SetText(honorText, $"Honor: {ps.Honor}");
        // }

        private void RefreshTime()
        {
            var tm = GameTimeManager.Instance;

            if (tm == null)
            {
                SetText(timeText, "(no time)");
                SetText(dateText, "(no date)");
                return;
            }

            string period = tm.Hour >= 12 ? "pm" : "am";

            int displayHour = tm.Hour % 12;
            if (displayHour == 0)
                displayHour = 12;

            SetText(timeText, $"{displayHour}{period}");
            SetText(dateText, $"{tm.Month:00}/{tm.Day:00}/{tm.Year}");
        }

        private static void SetText(TMP_Text t, string value)
        {
            if (t != null) t.text = value;
        }
    }
}
