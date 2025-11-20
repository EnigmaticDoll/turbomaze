using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemCollectCounterUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemLeftCounter;
    private float prevLeftItemCount;

    // Update is called once per frame
    void Update()
    {
        if (prevLeftItemCount != GameManager.Instance.leftItemCount)
        {
            prevLeftItemCount = GameManager.Instance.leftItemCount;
            itemLeftCounter.text = $"{GameManager.Instance.leftItemCount} out of {GameManager.Instance.itemSpawnCount} left.";
        }
    }
}
