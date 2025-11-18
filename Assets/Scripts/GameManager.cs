using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Maze")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float probabilityAdjacentConnectionPerGridPair;
    [SerializeField] private MazeStyle mazeStyle;
    [Header("Item")]
    [SerializeField] private int itemCount;
    [Header("Camera")]
    [SerializeField] private Vector3 cameraDisplacement;
    [Header("Rule")]
    [SerializeField] private float timeLimit; public float readOnlyTimeLimit => timeLimit;

    private Camera mainCam;
    private Camera minimapCam;

    private bool isStageOngoing;
    public float stageElapsedTime { get; private set; }

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (null != Instance && this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (isStageOngoing) stageElapsedTime += Time.deltaTime;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Stage":
                OnStageStart();
                break;
        }
    }

    private void OnStageStart()
    {
        MazeBuilder.Build(mazeStyle, width, height, probabilityAdjacentConnectionPerGridPair);
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (var cam in cams)
        {
            if (cam.name == "Main Camera") mainCam = cam;
            if (cam.name == "Mini Map Camera") minimapCam = cam;
        }
        isStageOngoing = true;
        stageElapsedTime = 0;
    }

    public void StartStage()
    {
        SceneManager.LoadScene("Stage");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetCamerasPosition(Vector2 horizontalPlayerPosition)
    {
        mainCam.transform.position = new Vector3(horizontalPlayerPosition.x, 0, horizontalPlayerPosition.y) + cameraDisplacement;
        mainCam.transform.rotation = Quaternion.LookRotation(-cameraDisplacement);
    }

    public void GetCameraVector(out Vector3 cameraForward, out Vector3 cameraRight)
    {
        cameraForward = mainCam.transform.forward;
        cameraRight = mainCam.transform.right;
    }
}
