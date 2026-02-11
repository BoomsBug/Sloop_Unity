using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EncounterUI : MonoBehaviour
{
    public GameObject panel;
    Action pay;
    Action engage;

    public void Show(Action onPay, Action onEngage)
    {
        pay = onPay;
        engage = onEngage;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void ClickPay() => pay?.Invoke();
    public void ClickEngage() => engage?.Invoke();
}
