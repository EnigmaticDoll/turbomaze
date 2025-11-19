using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pool
{
    private GameObject pooler;
    private GameObject poolee;

    private HashSet<GameObject> trackedObjs = new HashSet<GameObject>();
    private Stack<GameObject> inPool = new Stack<GameObject>();

    public Pool(GameObject poolee, int initialSize)
    {
        pooler = new GameObject("Pooler of: " + poolee.name);
        pooler.SetActive(false);
        this.poolee = poolee;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(poolee, pooler.transform);
            trackedObjs.Add(obj);
            obj.SetActive(false);
            inPool.Push(obj);
        }
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
            trackedObjs.Add(obj);
        }
        obj.SetActive(false);
        obj.transform.SetParent(null);
        return obj;
    }

    public void ReturnOrDestroyGameObject(GameObject obj)
    {
        if (null == obj) return;
        if (!trackedObjs.Contains(obj))
        {
            GameObject.Destroy(obj);
            return;
        }
        obj.SetActive(false);
        obj.transform.SetParent(pooler.transform);
        inPool.Push(obj);
    }

    public bool IsTrackedByPool(GameObject obj)
    {
        return trackedObjs.Contains(obj);
    }

    public bool TryGetPrefabOfGameObject(GameObject obj, out GameObject prefab)
    {
        bool result = IsTrackedByPool(obj);
        prefab = result ? poolee : null;
        return result;
    }

    public void MoveToScene(Scene scene)
    {
        RetrievePooledGameObjects();
        SceneManager.MoveGameObjectToScene(pooler, scene);
    }

    private void RetrievePooledGameObjects()
    {
        foreach (GameObject obj in trackedObjs)
        {
            if (null != obj.transform.parent && obj.transform.parent.gameObject == pooler) continue;
            ReturnOrDestroyGameObject(obj);
        }
    }
}
