using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttractor : MonoBehaviour
{
    [SerializeField] float attractSpeed;
    int itemLayer;
    int mapObjectLayer;

    // Start is called before the first frame update
    void Start()
    {
        itemLayer = LayerMask.NameToLayer("Item");
        mapObjectLayer = LayerMask.NameToLayer("MapObject");
    }

    private void OnTriggerStay(Collider other)
    {
        if (itemLayer == other.gameObject.layer)
        {
            Vector3 rayOrigin = new Vector3(transform.parent.position.x, 0.5f, transform.parent.position.z);
            Vector3 rayTarget = new Vector3(other.transform.parent.position.x, 0.5f, other.transform.parent.position.z);
            Vector3 rayDirectionNotNormalized = rayTarget - rayOrigin;

            if (!Physics.Raycast(rayOrigin, rayDirectionNotNormalized.normalized, rayDirectionNotNormalized.magnitude, /*mapObjectLayer*/LayerMask.GetMask("MapObject")))
            {
                other.transform.parent.Translate(-rayDirectionNotNormalized.normalized * attractSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
