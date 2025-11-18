using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pool
{
    private GameObject pooler;
    private GameObject poolee;
    private uint initialSize;

    private HashSet<GameObject> tracedObjs = new HashSet<GameObject>();
    private Stack<GameObject> inPool = new Stack<GameObject>();

    public Pool(GameObject obj, uint initialSize)
    {
        pooler = new GameObject("Pooler of: " + obj.name);
        pooler.SetActive(false);
        GameObject.DontDestroyOnLoad(pooler);
        poolee = obj;
        this.initialSize = initialSize;
    }

    ~Pool()
    {
        GameObject.Destroy(pooler);
    }

    public GameObject GetOrCreateDisabledGameObject()
    {
        if (!inPool.TryPop(out GameObject obj))
        {
            obj = GameObject.Instantiate(poolee, pooler.transform);
            tracedObjs.Add(obj);
        }
        obj.SetActive(false);
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
        return obj;
    }

    public void ReturnOrDestroyGameObject(GameObject obj)
    {
        if (null == obj) return;
        if (!tracedObjs.Contains(obj))
        {
            GameObject.Destroy(obj);
            return;
        }
        obj.SetActive(false);
        obj.transform.SetParent(pooler.transform);
        inPool.Push(obj);
    }
}
