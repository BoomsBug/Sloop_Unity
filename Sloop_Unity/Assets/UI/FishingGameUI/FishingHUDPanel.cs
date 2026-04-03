using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sloop.UI
{
    public class FishingHUDPanel : UIPanel
    {
        [SerializeField] private TMP_Text goalText;
        //[SerializeField] private TMP_Text timerText;
        [SerializeField] private Slider timerSlider;
        private float maxTime;

        public void Init(float timeLimit)
        {
            maxTime = timeLimit;

            if (timerSlider != null)
            {
                timerSlider.minValue=0f;
                timerSlider.maxValue=maxTime;
                timerSlider.value = maxTime;
                timerSlider.interactable=false;
            }
        }

        public void SetGoal(int current, int target)
        {
            goalText.text = $"{current} / {target}";
        }

        public void SetTime(float time)
        {
            //timerText.text = $"Time: {Mathf.CeilToInt(time)}";
            if (timerSlider!=null) timerSlider.value=time;
        }
    }
}