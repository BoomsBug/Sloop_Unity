using Sloop.Time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NightFilterController : MonoBehaviour
{
    [SerializeField] private Image overlay;

    [Header("Colors")]
    [SerializeField] private Color dayColor = new Color(0f, 0f, 0f, 0f); // fully transparent
    [SerializeField] private Color sunsetColor = new Color(1f, 0.3f, 0f, 0.4f); // orange/red
    [SerializeField] private Color nightColor = new Color(0f, 0f, 0.15f, 0.55f); // dark blue

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 1.5f;

    private Color targetColor;

    void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayNightChanged += HandleDayNight;
            HandleDayNight(GameTimeManager.Instance.IsNight());
        }
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
            GameTimeManager.Instance.OnDayNightChanged -= HandleDayNight;
    }

    void HandleDayNight(bool isNight)
    {
        // Transition through sunset if it's evening
        if (!isNight && GameTimeManager.Instance.Hour >= 18) // 6pm = sunset start
            targetColor = sunsetColor;
        else
            targetColor = isNight ? nightColor : dayColor;
    }

    void Update()
    {
        if (overlay == null) return;

        overlay.color = Color.Lerp(overlay.color, targetColor, Time.deltaTime * fadeSpeed);
    }
}