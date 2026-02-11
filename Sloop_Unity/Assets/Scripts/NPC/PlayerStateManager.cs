using System;
using UnityEngine;

namespace Sloop.Player
{
    public class PlayerStateManager : MonoBehaviour
    {
        public static PlayerStateManager Instance { get; private set; }

        [Header("Persist across scenes")]
        [SerializeField] private bool dontDestroyOnLoad = true;

        [Header("Honor")]
        [Range(0, 100)]
        [SerializeField] private int honor = 50;

        public int Honor => honor;

        /// <summary> Fires when honor changes: (newHonor). </summary>
        public event Action<int> OnHonorChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        public void SetHonor(int value)
        {
            int newValue = Mathf.Clamp(value, 0, 100);
            if (newValue == honor) return;

            honor = newValue;
            OnHonorChanged?.Invoke(honor);
        }

        public void AddHonor(int delta)
        {
            SetHonor(honor + delta);
        }
    }
}
