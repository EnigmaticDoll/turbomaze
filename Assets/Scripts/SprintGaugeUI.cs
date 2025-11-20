using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SprintGaugeUI : MonoBehaviour
{
    [SerializeField] private GameObject sprintGaugeInner;
    [SerializeField] private TextMeshProUGUI sprintGaugeText;

    private RectTransform sprintGaugeInnerRectTransform;

    private void OnEnable()
    {
        sprintGaugeInnerRectTransform = sprintGaugeInner.GetComponent<RectTransform>();
    }

    public void SetSprintGauge(float timeLeft, float timeMax, bool isWaitingFullRecharge)
    {
        sprintGaugeInnerRectTransform.offsetMax = new Vector2(timeLeft / timeMax * 200 - 202, 2);
        sprintGaugeText.text = string.Format("{0:F2} s\n{1}", timeLeft, isWaitingFullRecharge ? "recharging" : "");
    }
}
