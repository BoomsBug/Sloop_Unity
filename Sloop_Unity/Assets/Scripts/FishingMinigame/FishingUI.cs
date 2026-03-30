using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Sloop.UI
{
    public class FishingUI : UIPanel
    {
        [SerializeField] private TMP_Text fishCounterText;
        [SerializeField] private RectTransform uiRoot;
        [SerializeField] private GameObject floatingTextPrefab;

        private Camera cam;

        protected override void Awake()
        {
            base.Awake();
            cam = Camera.main;
        }
        public void SetFishCount(int count)
        {
            if (fishCounterText != null) fishCounterText.text = $"{count}";
        }
        public void ShowPopup(string msg, Vector3 worldPos)
        {
            if (!floatingTextPrefab || !uiRoot || cam ==null) return;
            var go = Instantiate(floatingTextPrefab, uiRoot);

            var rt = go.GetComponent<RectTransform>();
            if (rt!=null) rt.position = cam.WorldToScreenPoint(worldPos);

            var ft = go.GetComponent<FloatingTextPopup>();
            if (ft != null) ft.Show(msg);
        }
    }
}

