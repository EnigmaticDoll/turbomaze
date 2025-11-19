using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    [SerializeField] private GameObject obj;        public GameObject readOnlyObj => obj;
    [SerializeField] private GameObject vfx;        public GameObject readOnlyVfx => vfx;
    [SerializeField] private float vfxTime;         public float readOnlyVfxTime => vfxTime;
    [SerializeField] private string description;    public string readOnlyDescription => description;
}
