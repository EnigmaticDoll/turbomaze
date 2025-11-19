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
    [SerializeField] private int itemSpawnCount; public int readOnlyItemSpawnCount => itemSpawnCount;
    [SerializeField] private ItemData[] itemDataArray;

    [Header("Camera")]
    [SerializeField] private Vector3 cameraDisplacement;

    [Header("Rule")]
    [SerializeField] private float timeLimit; public float readOnlyTimeLimit => timeLimit;

    [Header("Misc")]
    [SerializeField] private GameObject eventSystem;

    public Dictionary<GameObject, ItemData> itemDataMap {  get; private set; }
    public Dictionary<GameObject, Pool> pools {  get; private set; }

    private Camera mainCam;
    private Camera minimapCam;

    private bool isStageOngoing;
    private bool isStageEnded;
    private bool isStageEndedTriggered;
    public bool isStageFailed { get; private set; }
    public float stageElapsedTime { get; private set; }
    public int leftItemCount { get; private set; }

    private Scene prevScene;
    private bool isAnySceneEverLoaded;

    private event Action onStageEnd;


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
        DontDestroyOnLoad(eventSystem);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        pools = new Dictionary<GameObject, Pool>();
        pools.Add(mazeStyle.readOnlyWall_Cross, new Pool(mazeStyle.readOnlyWall_Cross,  100));
        pools.Add(mazeStyle.readOnlyWall_T,     new Pool(mazeStyle.readOnlyWall_T,      100));
        pools.Add(mazeStyle.readOnlyWall_L,     new Pool(mazeStyle.readOnlyWall_L,      100));
        pools.Add(mazeStyle.readOnlyWall_Bar3,  new Pool(mazeStyle.readOnlyWall_Bar3,   100));
        pools.Add(mazeStyle.readOnlyWall_Bar2,  new Pool(mazeStyle.readOnlyWall_Bar2,   100));
        pools.Add(mazeStyle.readOnlyWall_Bar1,  new Pool(mazeStyle.readOnlyWall_Bar1,   100));

        foreach (ItemData i in itemDataArray)
        {
            pools.Add(i.readOnlyObj, new Pool(i.readOnlyObj, itemSpawnCount));
        }

        itemDataMap = new Dictionary<GameObject, ItemData>();
        foreach (ItemData i in itemDataArray)
        {
            itemDataMap.Add(i.readOnlyObj, i);
        }
        isAnySceneEverLoaded = false;
    }

    void OnDisable()
    {
        itemDataMap.Clear();
        pools.Clear();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (isStageOngoing && !isStageEnded) stageElapsedTime += Time.deltaTime;
        if (timeLimit <= stageElapsedTime)
        {
            isStageEnded = true;
            isStageFailed = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);

        foreach (Pool p in pools.Values) p.MoveToScene(scene);

        switch (scene.name)
        {
            case "Menu":
                break;
            case "Stage":
                OnStageStart(scene);
                break;
        }

        if (isAnySceneEverLoaded) SceneManager.UnloadSceneAsync(prevScene);
        prevScene = scene;
        isAnySceneEverLoaded = true;
    }

    private void OnStageStart(Scene scene)
    {
        MazeBuilder.Build(mazeStyle, width, height, probabilityAdjacentConnectionPerGridPair);
        ItemSpawner.Spawn(mazeStyle, itemDataArray, width, height, itemSpawnCount);

        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (var cam in cams)
        {
            if (scene != cam.gameObject.scene) continue;
            if (cam.name == "Main Camera") mainCam = cam;
            if (cam.name == "Minimap Camera") minimapCam = cam;
        }

        isStageOngoing = true;
        isStageEnded = false;
        isStageEndedTriggered = false;
        isStageFailed = false;
        stageElapsedTime = 0;
        leftItemCount = itemSpawnCount;
    }

    public void SetCamerasPosition(Vector2 horizontalPlayerPosition)
    {
        mainCam.transform.position = new Vector3(horizontalPlayerPosition.x, 0, horizontalPlayerPosition.y) + cameraDisplacement;
        mainCam.transform.rotation = Quaternion.LookRotation(-cameraDisplacement);
        minimapCam.transform.position = new Vector3(horizontalPlayerPosition.x, 10, horizontalPlayerPosition.y);
    }

    public void GetMainCameraVector(out Vector3 cameraForward, out Vector3 cameraRight)
    {
        cameraForward = mainCam.transform.forward;
        cameraRight = mainCam.transform.right;
    }

    public GameObject GetOrCreateDisabledGameObject(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out Pool pool))
        {
            return pool.GetOrCreateDisabledGameObject();
        }
        return GameObject.Instantiate(prefab);
    }

    public void ReturnOrDestroyGameObject(GameObject obj)
    {
        foreach (Pool p in pools.Values)
        {
            if (p.IsTrackedByPool(obj))
            {
                p.ReturnOrDestroyGameObject(obj);
                return;
            }
        }
        Destroy(obj);
    }

    public GameObject FindPrefabOfPooledGameObject(GameObject obj)
    {
        foreach (Pool p in pools.Values)
        {
            if (!p.TryGetPrefabOfGameObject(obj, out GameObject prefab)) continue;
            return prefab;
        }
        return null;
    }

    public void OnItemCollected()
    {
        leftItemCount--;
        if (0 == leftItemCount) isStageEnded = true;
    }

    private void LateUpdate()
    {
        if (isStageEnded && !isStageEndedTriggered)
        {
            isStageEndedTriggered = true;
            onStageEnd.Invoke();
        }
    }

    public void SubscribeStageEndEvent(Action action)
    {
        onStageEnd += action;
    }

    public void UnsubscribeStageEndEvent(Action action)
    {
        onStageEnd -= action;
    }
}
