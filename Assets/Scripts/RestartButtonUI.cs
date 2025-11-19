using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButtonUI : MonoBehaviour
{
    public void OnRestartButtonPressed()
    {
        SceneManager.LoadScene("Stage", LoadSceneMode.Additive);
    }
}
