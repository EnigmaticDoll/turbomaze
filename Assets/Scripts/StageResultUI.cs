using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageResultUI : MonoBehaviour
{
    [SerializeField] GameObject canvasResult;
    [SerializeField] TextMeshProUGUI finalScoreText;

    private void OnEnable()
    {
        GameManager.Instance.SubscribeStageEndEvent(OnStageEnd);
    }

    private void OnDisable()
    {
        GameManager.Instance.UnsubscribeStageEndEvent(OnStageEnd);
    }

    private void OnStageEnd()
    {
        canvasResult.SetActive(true);
        finalScoreText.text = $"Final Score: {TimeToString(GameManager.Instance.stageElapsedTime)}";
    }

    private string TimeToString(float time)
    {
        int minutes = (int)(time / 60);
        int second = (int)(time % 60);
        int millisecond = (int)((time * 1000f) % 1000f);

        return string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, second, millisecond);
    }
}
