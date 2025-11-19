using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollectBehaviour : MonoBehaviour
{
    private int itemLayer;

    private void Start()
    {
        itemLayer = LayerMask.NameToLayer("Item");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemLayer == other.gameObject.layer)
        {
            GameManager.Instance.OnItemCollected();

            GameObject prefab = GameManager.Instance.FindPrefabOfPooledGameObject(other.transform.parent.gameObject); // item collider is not at prefab root!
            if (null != prefab && GameManager.Instance.itemDataMap.TryGetValue(prefab, out ItemData itemData) && null != itemData.readOnlyVfx)
            {
                GameObject vfx = Instantiate(itemData.readOnlyVfx, transform);
                if (null != vfx)
                {
                    ParticleSystem particleSystem = vfx.GetComponent<ParticleSystem>();
                    if (null != particleSystem)
                    {
                        var main = particleSystem.main;
                        main.startColor = Color.Lerp(Color.cyan, Color.red, GameManager.Instance.leftItemCount / (float)GameManager.Instance.readOnlyItemSpawnCount);
                    }
                    Destroy(vfx, itemData.readOnlyVfxTime);
                }
            }
            GameManager.Instance.ReturnOrDestroyGameObject(other.transform.parent.gameObject);
        }
    }
}
