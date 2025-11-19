using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttractor : MonoBehaviour
{
    [SerializeField] [Tooltip("speed = max(coefficient * distance ^ -factor, min_speed)")] float attractionCoefficient;
    [SerializeField] [Tooltip("speed = max(coefficient * distance ^ -factor, min_speed)")] float attractionFactor;
    [SerializeField] [Tooltip("speed = max(coefficient * distance ^ -factor, min_speed)")] float minimumSpeed;
    int itemLayer;
    int mapObjectLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        itemLayer = LayerMask.NameToLayer("Item");
        mapObjectLayerMask = LayerMask.GetMask("MapObject");
    }

    private void OnTriggerStay(Collider other)
    {
        if (itemLayer == other.gameObject.layer)
        {
            Vector3 rayOrigin = new Vector3(transform.parent.position.x, 0.5f, transform.parent.position.z);
            Vector3 rayTarget = new Vector3(other.transform.parent.position.x, 0.5f, other.transform.parent.position.z);
            Vector3 displacement = rayTarget - rayOrigin;

            if (!Physics.Raycast(rayOrigin, displacement.normalized, displacement.magnitude, mapObjectLayerMask))
            {
                float attractionSpeed = Mathf.Max(minimumSpeed, attractionCoefficient * Mathf.Pow(displacement.magnitude, -attractionFactor));
                other.transform.parent.Translate(-displacement.normalized * attractionSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
