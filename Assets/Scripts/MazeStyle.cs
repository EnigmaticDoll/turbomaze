using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MazeStyle : ScriptableObject
{
    [SerializeField] private GameObject wall_Cross; public GameObject   readOnlyWall_Cross      => wall_Cross;
    [SerializeField] private GameObject wall_T;     public GameObject   readOnlyWall_T          => wall_T;
    [SerializeField] private GameObject wall_L;     public GameObject   readOnlyWall_L          => wall_L;
    [SerializeField] private GameObject wall_Bar1;  public GameObject   readOnlyWall_Bar1       => wall_Bar1;
    [SerializeField] private GameObject wall_Bar2;  public GameObject   readOnlyWall_Bar2       => wall_Bar2;
    [SerializeField] private GameObject wall_Bar3;  public GameObject   readOnlyWall_Bar3       => wall_Bar3;
    [SerializeField] float blockDotUnit;            public float        readOnlyBlockDotUnit    => blockDotUnit;
    [SerializeField] float blockOffset;             public float        readOnlyBlockOffset     => blockOffset;
}
