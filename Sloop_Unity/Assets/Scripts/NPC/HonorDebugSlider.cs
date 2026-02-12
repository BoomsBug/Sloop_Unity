using UnityEngine;
using UnityEngine.UI;
using Sloop.Player;
using TMPro;





namespace Sloop.UI
{
    public class HonorDebugSlider : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Slider honorSlider;
        [SerializeField] private TMP_Text honorValueText; // optional

        [Header("Behavior")]
        [SerializeField] private bool snapToInt = true;

        private bool suppressCallback;

        private void Awake()
        {
            if (honorSlider == null)
                honorSlider = GetComponentInChildren<Slider>();

            if (honorSlider == null)
            {
                Debug.LogWarning("HonorDebugSlider: No Slider assigned/found.");
                enabled = false;
                return;
            }

            // Configure slider range for honor
            honorSlider.minValue = 0;
            honorSlider.maxValue = 100;
            honorSlider.wholeNumbers = snapToInt;

            honorSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        private void OnEnable()
        {
            // Sync UI from current honor
            SyncFromState();

            // Listen for changes made elsewhere (NPC events etc.)
            if (PlayerStateManager.Instance != null)
                PlayerStateManager.Instance.OnHonorChanged += OnHonorChanged;
        }

        private void OnDisable()
        {
            if (PlayerStateManager.Instance != null)
                PlayerStateManager.Instance.OnHonorChanged -= OnHonorChanged;
        }

        private void OnDestroy()
        {
            if (honorSlider != null)
                honorSlider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        private void OnSliderChanged(float value)
        {
            if (suppressCallback) return;

            var ps = PlayerStateManager.Instance;
            if (ps == null) return;

            int honor = snapToInt ? Mathf.RoundToInt(value) : Mathf.Clamp((int)value, 0, 100);
            ps.SetHonor(honor);

            UpdateLabel(honor);
        }

        private void OnHonorChanged(int newHonor)
        {
            // Keep slider in sync if honor changes from gameplay
            suppressCallback = true;
            honorSlider.value = newHonor;
            suppressCallback = false;

            UpdateLabel(newHonor);
        }

        private void SyncFromState()
        {
            var ps = PlayerStateManager.Instance;
            int current = ps != null ? ps.Honor : 50;

            suppressCallback = true;
            honorSlider.value = current;
            suppressCallback = false;

            UpdateLabel(current);
        }

        private void UpdateLabel(int honor)
        {
            if (honorValueText != null)
                honorValueText.text = $"Honor: {honor}";
        }
    }
}
