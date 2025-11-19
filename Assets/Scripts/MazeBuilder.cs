using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeBuilder
{
    public static float CalculateCellToCellDistance(MazeStyle style)
    {
        return 4 * style.readOnlyBlockDotUnit + 2 * style.readOnlyBlockOffset;
    }

    public static void Build(MazeStyle style, int width, int height, float probabilityAdjacentConnectionPerGridPair = 0.5f)
    {
        Maze maze = new Maze(width, height, probabilityAdjacentConnectionPerGridPair);

        float cellToCellDistance = CalculateCellToCellDistance(style);
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

                if (!isThisToLowerConnected)
                {
                    GameObject wall = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Bar1);
                    wall.transform.position = cellPosition + new Vector3(0f, 0f, -cellToWallDistance);
                    wall.transform.rotation = Quaternion.identity;
                    wall.SetActive(true);
                }

                if (!isThisToRightConnected)
                {
                    GameObject wall = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Bar1);
                    wall.transform.position = cellPosition + new Vector3(cellToWallDistance, 0f, 0f);
                    wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    wall.SetActive(true);
                }

                uint cornerTpyeBitMask =
                    (isThisToRightConnected         ? 0b1000u : 0b0000u)
                    | (isRightToLowerRightConnected ? 0b0100u : 0b0000u)
                    | (isLowerToLowerRightConnected ? 0b0010u : 0b0000u)
                    | (isThisToLowerConnected       ? 0b0001u : 0b0000u);

                GameObject corner;
                Vector3 cornerPosition = cellPosition + new Vector3(cellToWallDistance, 0f, -cellToWallDistance);
                switch (cornerTpyeBitMask)
                {
                    case 0b0000u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Cross);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.identity;
                        corner.SetActive(true);
                        break;


                    case 0b1000u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_T);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.identity;
                        corner.SetActive(true);
                        break;

                    case 0b0100u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_T);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        corner.SetActive(true);
                        break;

                    case 0b0010u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_T);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        corner.SetActive(true);
                        break;

                    case 0b0001u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_T);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
                        corner.SetActive(true);
                        break;


                    case 0b0011u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_L);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.identity;
                        corner.SetActive(true);
                        break;

                    case 0b1001u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_L);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        corner.SetActive(true);
                        break;

                    case 0b1100u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_L);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 180, 0f);
                        corner.SetActive(true);
                        break;

                    case 0b0110u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_L);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 270, 0f);
                        corner.SetActive(true);
                        break;


                    case 0b1010u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Bar3);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.identity;
                        corner.SetActive(true);
                        break;

                    case 0b0101u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Bar3);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        corner.SetActive(true);
                        break;


                    case 0b1110u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Bar2);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.identity;
                        corner.SetActive(true);
                        break;

                    case 0b0111u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Bar2);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        corner.SetActive(true);
                        break;

                    case 0b1011u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Bar2);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 180, 0f);
                        corner.SetActive(true);
                        break;

                    case 0b1101u:
                        corner = GameManager.Instance.GetOrCreateDisabledGameObject(style.readOnlyWall_Bar2);
                        corner.transform.position = cornerPosition;
                        corner.transform.rotation = Quaternion.Euler(0f, 270, 0f);
                        corner.SetActive(true);
                        break;


                    case 0b1111u:   throw new InvalidOperationException("At least one adjacent grid should be disconnected.");
                    default:        throw new InvalidOperationException("Unknown bit mask for corner type.");
                }
            }
        }
    }
}
