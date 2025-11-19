using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    [SerializeField] private GameObject obj;        public GameObject readOnlyObj => obj;
    [SerializeField] private string description;    public string readOnlyDescription => description;
}
