using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SprintGaugeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sprintGaugeText;
    [SerializeField] GameObject sprintGaugeInner;

    // Update is called once per frame
    void Update()
    {
        // todo world space canvas
        //sprintGaugeText.text = GameManager.Instance.spr
    }
}
