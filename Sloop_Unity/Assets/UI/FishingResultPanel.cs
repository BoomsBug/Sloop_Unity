using TMPro;
using UnityEngine;

namespace Sloop.UI
{
    public class FishingResultPanel : UIPanel
    {
        [SerializeField] private TMP_Text resultText;

        public void SetResult(string msg)
        {
            resultText.text = msg;
        }
    }
}

