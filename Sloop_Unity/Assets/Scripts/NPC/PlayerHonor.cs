using UnityEngine;

namespace Sloop.Player
{
    public class PlayerHonor : MonoBehaviour
    {
        [Header("Debug / Tuning")]
        [Range(0, 100)]
        [SerializeField] private int honor = 50;

        public int Honor => honor;

        // Optional helpers for later gameplay hooks
        public void SetHonor(int value)
        {
            honor = Mathf.Clamp(value, 0, 100);
        }

        public void AddHonor(int delta)
        {
            honor = Mathf.Clamp(honor + delta, 0, 100);
        }
    }
}
