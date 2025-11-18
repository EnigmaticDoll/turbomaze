using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //    [SerializeField] private GameObject noneObj;
    //    [SerializeField] private GameObject downObj;
    //    [SerializeField] private GameObject rightObj;
    //    [SerializeField] private GameObject anyObj;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float probabilityAdjacentConnectionPerGridPair;
    [SerializeField] private MazeStyle mazeStyle;

    // Start is called before the first frame update
    void Start()
    {
        //Maze.GridConnectionFlag[] data = Maze.GenerateMazeData(width, height, probabilityAdjacentConnectionPerGridPair);
        //if (null == data) return;

        //Instantiate(anyObj, new Vector3(-5.2f * -1, 0, -5.2f * -1), Quaternion.identity);

        //for (int x = 0; x < width; x++)
        //{
        //    Instantiate(rightObj, new Vector3(-5.2f * -1, 0, -5.2f * x), Quaternion.identity);
        //}
        //for (int y = 0; y < height; y++)
        //{
        //    Instantiate(downObj, new Vector3(-5.2f * y, 0, -5.2f * -1), Quaternion.identity);

        //    for (int x = 0; x < width; x++)
        //    {
        //        int idx = y * width + x;
        //        bool isDownBlocked = Maze.GridConnectionFlag.None == (data[idx] & Maze.GridConnectionFlag.Down);
        //        bool isRightBlocked = Maze.GridConnectionFlag.None == (data[idx] & Maze.GridConnectionFlag.Right);
        //        switch (data[idx])
        //        {
        //            case Maze.GridConnectionFlag.None:
        //                Instantiate(noneObj, new Vector3(-5.2f * y, 0, -5.2f * x), Quaternion.identity);
        //                break;
        //            case Maze.GridConnectionFlag.Down:
        //                Instantiate(downObj, new Vector3(-5.2f * y, 0, -5.2f * x), Quaternion.identity);
        //                break;
        //            case Maze.GridConnectionFlag.Right:
        //                Instantiate(rightObj, new Vector3(-5.2f * y, 0, -5.2f * x), Quaternion.identity);
        //                break;
        //            case Maze.GridConnectionFlag.AnyMask:
        //                Instantiate(anyObj, new Vector3(-5.2f * y, 0, -5.2f * x), Quaternion.identity);
        //                break;
        //        };
        //    }
        //}
        MazeBuilder.Build(mazeStyle, width, height, probabilityAdjacentConnectionPerGridPair);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
