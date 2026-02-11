using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sloop.Economy
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Header("Persist across scenes")]
        [SerializeField] private bool dontDestroyOnLoad = true;

        [Header("Starting amounts")]
        [SerializeField] private ResourceAmount[] startingResources;

        // Store amounts for each resource type
        private readonly Dictionary<Resource, int> amounts = new();

        /// <summary>
        /// Fires whenever a resource changes: (resourceType, newAmount).
        /// Hook UI into this.
        /// </summary>
        public event Action<Resource, int> OnResourceChanged;

        private void Awake()
        {
            // Singleton setup
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            // Initialize all resources to 0
            foreach (Resource r in Enum.GetValues(typeof(Resource)))
                amounts[r] = 0;

            // Apply starting resources (if any)
            if (startingResources != null)
                Add(startingResources);
        }

        // -----------------------------
        // Read
        // -----------------------------
        public int GetAmount(Resource type)
        {
            return amounts.TryGetValue(type, out int value) ? value : 0;
        }

        // -----------------------------
        // Add (single)
        // -----------------------------
        public void Add(Resource type, int delta)
        {
            if (delta <= 0) return;

            int newValue = GetAmount(type) + delta;
            amounts[type] = newValue;

            OnResourceChanged?.Invoke(type, newValue);
        }

        // -----------------------------
        // Add (multi)
        // -----------------------------
        public void Add(IEnumerable<ResourceAmount> deltas)
        {
            if (deltas == null) return;

            foreach (var d in deltas)
            {
                if (d.amount <= 0) continue;
                Add(d.type, d.amount);
            }
        }

        // -----------------------------
        // CanAfford (single)
        // -----------------------------
        public bool CanAfford(Resource type, int cost)
        {
            if (cost <= 0) return true;
            return GetAmount(type) >= cost;
        }

        // -----------------------------
        // CanAfford (multi)
        // -----------------------------
        public bool CanAfford(IEnumerable<ResourceAmount> costs)
        {
            if (costs == null) return true;

            foreach (var c in costs)
            {
                if (c.amount <= 0) continue;

                if (GetAmount(c.type) < c.amount)
                    return false;
            }
            return true;
        }

        // -----------------------------
        // TrySpend (single)
        // -----------------------------
        public bool TrySpend(Resource type, int cost)
        {
            if (cost <= 0) return true;

            int current = GetAmount(type);
            if (current < cost) return false;

            int newValue = current - cost;
            amounts[type] = newValue;

            OnResourceChanged?.Invoke(type, newValue);
            return true;
        }

        // -----------------------------
        // TrySpend (multi)
        // -----------------------------
        public bool TrySpend(IEnumerable<ResourceAmount> costs)
        {
            if (!CanAfford(costs))
                return false;

            if (costs == null)
                return true;

            // Spend after validation
            foreach (var c in costs)
            {
                if (c.amount <= 0) continue;

                int newValue = GetAmount(c.type) - c.amount;
                amounts[c.type] = newValue;

                OnResourceChanged?.Invoke(c.type, newValue);
            }

            return true;
        }

        // -----------------------------
        // Set (useful for save/load)
        // -----------------------------
        public void Set(Resource type, int value)
        {
            int newValue = Mathf.Max(0, value);
            amounts[type] = newValue;

            OnResourceChanged?.Invoke(type, newValue);
        }

        // -----------------------------
        // Set many (save/load)
        // -----------------------------
        public void Set(IEnumerable<ResourceAmount> values)
        {
            if (values == null) return;

            foreach (var v in values)
            {
                Set(v.type, v.amount);
            }
        }
    }
}
