using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeBuilder
{
    public static void Build(MazeStyle style, int width, int height, float probabilityAdjacentConnectionPerGridPair = 0.5f)
    {
        Maze maze = new Maze(width, height, probabilityAdjacentConnectionPerGridPair);

        float cellToCellDistance = 3 * style.readOnlyBlockDotUnit + 1 * style.readOnlyBlockOffset;
        float cellToWallDistance = 0.5f * cellToCellDistance;

        for (int y = -1; y < height; y++)
        {
            for (int x = -1; x < width; x++)
            {
                string cellRowColumnString = $"({x},{y})";

                bool isOutOfMazeThisGrid        = !maze.TryGetFlag(x,       y,      out Maze.GridConnectionFlag flagThisGrid);
                bool isOutOfMazeLowerGrid       = !maze.TryGetFlag(x,       y + 1,  out Maze.GridConnectionFlag flagLowerGrid);
                bool isOutOfMazeRightGrid       = !maze.TryGetFlag(x + 1,   y,      out Maze.GridConnectionFlag flagRightGrid);
                bool isOutOfMazeLowerRightGrid  = !maze.TryGetFlag(x + 1,   y + 1,  out Maze.GridConnectionFlag flagLowerRightGrid);

                bool isThisToLowerConnected =
                    (isOutOfMazeThisGrid && isOutOfMazeLowerGrid)
                    || (!isOutOfMazeThisGrid && (Maze.GridConnectionFlag.None != (flagThisGrid & Maze.GridConnectionFlag.Down)));

                bool isThisToRightConnected =
                    (isOutOfMazeThisGrid && isOutOfMazeRightGrid)
                    || (!isOutOfMazeThisGrid && (Maze.GridConnectionFlag.None != (flagThisGrid & Maze.GridConnectionFlag.Right)));

                bool isLowerToLowerRightConnected =
                    (isOutOfMazeLowerGrid && isOutOfMazeLowerRightGrid)
                    || (!isOutOfMazeLowerGrid && (Maze.GridConnectionFlag.None != (flagLowerGrid & Maze.GridConnectionFlag.Right)));

                bool isRightToLowerRightConnected =
                    (isOutOfMazeRightGrid && isOutOfMazeLowerRightGrid)
                    || (!isOutOfMazeRightGrid && (Maze.GridConnectionFlag.None != (flagRightGrid & Maze.GridConnectionFlag.Down)));

                Vector3 cellPosition = new Vector3(cellToCellDistance * x, 0f, -cellToCellDistance * y);

                //if (!isThisToLowerConnected) GameObject.Instantiate(style.readOnlyWall_Bar1, cellPosition + new Vector3(0f, 0f, -cellToWallDistance), Quaternion.identity).name = cellRowColumnString;
                //if (!isThisToRightConnected) GameObject.Instantiate(style.readOnlyWall_Bar1, cellPosition + new Vector3(cellToWallDistance, 0f, 0f), Quaternion.Euler(0f, 90f, 0f)).name = cellRowColumnString;

                uint cornerTpyeBitMask =
                    (isThisToRightConnected         ? 0b1000u : 0b0000u)
                    | (isRightToLowerRightConnected ? 0b0100u : 0b0000u)
                    | (isLowerToLowerRightConnected ? 0b0010u : 0b0000u)
                    | (isThisToLowerConnected       ? 0b0001u : 0b0000u);

                switch (cornerTpyeBitMask)
                {
                    case 0b0000u: GameObject.Instantiate(style.readOnlyWall_Cross, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.identity).name = cellRowColumnString; break;

                    case 0b1000u: GameObject.Instantiate(style.readOnlyWall_T, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.identity).name = cellRowColumnString; break;
                    case 0b0100u: GameObject.Instantiate(style.readOnlyWall_T, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 90f, 0f)).name = cellRowColumnString; break;
                    case 0b0010u: GameObject.Instantiate(style.readOnlyWall_T, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 180f, 0f)).name = cellRowColumnString; break;
                    case 0b0001u: GameObject.Instantiate(style.readOnlyWall_T, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 270, 0f)).name = cellRowColumnString; break;

                    case 0b0011u: GameObject.Instantiate(style.readOnlyWall_L, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.identity).name = cellRowColumnString; break;
                    case 0b1001u: GameObject.Instantiate(style.readOnlyWall_L, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 90f, 0f)).name = cellRowColumnString; break;
                    case 0b1100u: GameObject.Instantiate(style.readOnlyWall_L, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 180, 0f)).name = cellRowColumnString; break;
                    case 0b0110u: GameObject.Instantiate(style.readOnlyWall_L, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 270, 0f)).name = cellRowColumnString; break;

                    case 0b1010u: GameObject.Instantiate(style.readOnlyWall_Bar3, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.identity).name = cellRowColumnString; break;
                    case 0b0101u: GameObject.Instantiate(style.readOnlyWall_Bar3, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 90f, 0f)).name = cellRowColumnString; break;

                    case 0b1110u: GameObject.Instantiate(style.readOnlyWall_Bar2, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.identity).name = cellRowColumnString; break;
                    case 0b0111u: GameObject.Instantiate(style.readOnlyWall_Bar2, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 90f, 0f)).name = cellRowColumnString; break;
                    case 0b1011u: GameObject.Instantiate(style.readOnlyWall_Bar2, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 180f, 0f)).name = cellRowColumnString; break;
                    case 0b1101u: GameObject.Instantiate(style.readOnlyWall_Bar2, cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance), Quaternion.Euler(0f, 270, 0f)).name = cellRowColumnString; break;

                    case 0b1111u:   throw new InvalidOperationException("At least one adjacent grid should be disconnected.");
                    default:        throw new InvalidOperationException("Unknown bit mask for corner type.");
                }
            }
        }
    }
}
