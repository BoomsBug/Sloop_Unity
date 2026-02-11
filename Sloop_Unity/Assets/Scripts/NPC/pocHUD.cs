using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;
using TMPro;
using Sloop.Economy;
using Sloop.Player;

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
            HookPlayerStateManager();

            RefreshAll();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            UnhookResourceManager();
            UnhookPlayerStateManager();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Re-hook in case managers were created/destroyed due to scene order.
            UnhookResourceManager();
            UnhookPlayerStateManager();

            HookResourceManager();
            HookPlayerStateManager();

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

        private void HookPlayerStateManager()
        {
            if (PlayerStateManager.Instance != null)
                PlayerStateManager.Instance.OnHonorChanged += OnHonorChanged;
        }

        private void UnhookPlayerStateManager()
        {
            if (PlayerStateManager.Instance != null)
                PlayerStateManager.Instance.OnHonorChanged -= OnHonorChanged;
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
            }
        }

        private void OnHonorChanged(int newHonor)
        {
            SetText(honorText, $"Honor: {newHonor}");
        }

        // -----------------------------
        // Refresh
        // -----------------------------
        private void RefreshAll()
        {
            RefreshResources();
            RefreshHonor();
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
        }

        private void RefreshHonor()
        {
            var ps = PlayerStateManager.Instance;
            if (ps == null)
            {
                SetText(honorText, "Honor: (no PS)");
                return;
            }

            SetText(honorText, $"Honor: {ps.Honor}");
        }

        private static void SetText(TMP_Text t, string value)
        {
            if (t != null) t.text = value;
        }
    }
}
