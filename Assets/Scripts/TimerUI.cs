using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    string timeLimitString;

    void Start()
    {
        timeLimitString = TimeToString(GameManager.Instance.readOnlyTimeLimit);
    }

    // Update is called once per frame
    void Update()
    {
        string ellapsed = TimeToString(GameManager.Instance.stageElapsedTime);
        timerText.text = ellapsed + " / " + timeLimitString;
    }

    private string TimeToString(float time)
    {
        int minutes = (int)(time / 60);
        int second = (int)(time % 60);
        int millisecond = (int)((time * 1000f) % 1000f);

        return string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, second, millisecond);
    }
}
