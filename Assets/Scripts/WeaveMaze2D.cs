//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;

///// <summary>
///// </summary>
//public static class WeavedMaze2D
//{
//    /// <summary>
//    /// A grid discriptor, indicating whether this grid is connected to the next grid in the right or down directions,
//    /// respectively for the weave-base and weave-above.
//    /// 
//    /// Connections toward left and up directions are determined by the next grids themselves.
//    /// </summary>
//    [Flags]
//    public enum GridConnectionFlag: byte
//    {
//        // Do not use HasFlag() as it causes boxing.
//        None = 0,
//        BaseRight           = 1 << 0,
//        BaseDown            = 1 << 1,
//        BaseMask            = BaseRight | BaseDown,
//        AboveHorizontalKeep = 1 << 2,
//        AboveVerticalKeep   = 1 << 3,
//        AboveToRightStart   = 1 << 4,
//        AboveToDownStart    = 1 << 5,
//        AboveFromLeftEnd    = 1 << 6,
//        AboveFromUpEnd      = 1 << 7,
//        AboveKeepMask       = AboveHorizontalKeep | AboveVerticalKeep,
//        AboveMask           = AboveKeepMask | AboveToRightStart | AboveToDownStart | AboveFromLeftEnd | AboveFromUpEnd,
//    }

//    /// <summary>
//    /// A weave maze generator based on Eller's algorithm, but iterating in both vertical and horizontal axes,
//    /// with a probability based on the numbers of reamining lines.
//    /// </summary>
//    /// <param name="x">maze width</param>
//    /// <param name="y">maze height</param>
//    /// <param name="probabilityAdjacentConnectionPerGrid">probability of adjacent grid connection</param>
//    /// <param name="probabilityWeaveConnectionPerDisconnectedGridPair">probability of weave connection between a pair of disconnected grids</param>
//    /// <returns>maze grid discriptor</returns>
//    /// <exception cref="InvalidOperationException">for debug, thrown when maze failed to be validated</exception>
//    public static GridConnectionFlag[] Build(int x, int y, float probabilityAdjacentConnectionPerGrid, float probabilityWeaveConnectionPerDisconnectedGridPair)
//    {
//        // Think sets of grids as colors. Each uncolored grid gets its own color.
//        // Then it is randomly connected to other grids on the same line, and then repainted.

//        if (x < 3 || y < 3) return null;

//        GridConnectionFlag[] result = new GridConnectionFlag[x * y];
//        int[] gridColors = new int[x * y];
//        Array.Fill(gridColors, -1);

//        int row = -1;
//        int column = -1;

//        Stack<int> unusedColorPool = new Stack<int>();
//        int maxColorID = -1;

//        HashSet<int> hashSetBuffer = new HashSet<int>();

//        while (column + 1 < x && row + 1 < y)
//        {
//            int horizontalLeft = x - 1 - row;
//            int verticalLeft = y - 1 - column;
//            float totalLeft = horizontalLeft + verticalLeft;
//            float probabilityHorizontal = horizontalLeft / totalLeft;
//            if(UnityEngine.Random.value < probabilityHorizontal)
//            {
//                FillRow(result, gridColors, ++row, column, x, y, unusedColorPool, ref maxColorID, hashSetBuffer, probabilityAdjacentConnectionPerGrid, probabilityWeaveConnectionPerDisconnectedGridPair);
//            }
//            else
//            {
//                FillColumn(result, gridColors, row, ++column, x, y, unusedColorPool, ref maxColorID, hashSetBuffer, probabilityAdjacentConnectionPerGrid, probabilityWeaveConnectionPerDisconnectedGridPair);
//            }
//            PrintDebug(gridColors, x, y);
//        }

//        MakeValid(result, gridColors, x, y, unusedColorPool);

//        if (!IsValid(gridColors)) throw new InvalidOperationException("Maze was not valid");

//        return result;
//    }

//    private static void PrintDebug(int[] gridColors, int x, int y)
//    {
//        StringBuilder sb = new StringBuilder();
//        for (int i = 0; i < y; i++)
//        {
//            for (int j = 0; j < x; j++)
//            {
//                sb.Append($"{(int)gridColors[i * x + j]} ");
//            }
//            sb.Append('\n');
//        }
//        Debug.Log(sb.ToString());
//    }

//    private static void JoinGridColor(int[] gridColors, int color0, int color1, Stack<int> unusedColorPool)
//    {
//        if (-1 ==  color0 || -1 == color1) throw new InvalidOperationException("Cannot join nil grid color.");
//        int colorMin = Math.Min(color0, color1);
//        int colorMax = Math.Max(color0, color1);
//        for (int i = 0; i < gridColors.Length; i++) if (gridColors[i] == colorMax) gridColors[i] = colorMin;
//        unusedColorPool.Push(colorMax);
//    }

//    private static void FillRow(
//        GridConnectionFlag[] result,
//        int[] gridColors,
//        int targetRow,
//        int currentColumn,
//        int gridWidth,
//        int gridHeight,
//        Stack<int> unusedColorPool,
//        ref int maxColorID,
//        HashSet<int> hashSetBuffer,
//        float probabilityAdjacentConnectionPerGrid,
//        float probabilityWeaveConnectionPerDeadEnd)
//    {
//        hashSetBuffer.Clear();

//        // fill empty grid
//        for (int i = currentColumn + 1; i < gridWidth; i++)
//        {
//            int idx = targetRow * gridWidth + i;
//            if (-1 == gridColors[idx])
//            {
//                if (!unusedColorPool.TryPop(out int color)) color = ++maxColorID;
//                gridColors[idx] = color;
//            }
//        }

//        // randomly connect adjacent grid
//        for (int i = currentColumn + 1; i < gridWidth - 1; i++)
//        {
//            int idx = targetRow * gridWidth + i;
//            if (UnityEngine.Random.value < probabilityAdjacentConnectionPerGrid)
//            {
//                result[idx] |= GridConnectionFlag.BaseRight;
//                JoinGridColor(gridColors, gridColors[idx], gridColors[idx + 1], unusedColorPool);
//            }

//        }

//        // build next row stage if it is not the final row
//        if (gridHeight - 1 != targetRow)
//        {
//            // cache seen color
//            for (int i = currentColumn + 1; i < gridWidth; i++)
//            {
//                int idx = targetRow * gridWidth + i;
//                hashSetBuffer.Add(gridColors[idx]);
//            }

//            // random extension to the next row, remove color from color cache
//            for (int i = currentColumn + 1; i < gridWidth; i++)
//            {
//                int idx = targetRow * gridWidth + i;
//                if (UnityEngine.Random.value < probabilityAdjacentConnectionPerGrid)
//                {
//                    result[idx] |= GridConnectionFlag.BaseDown;
//                    gridColors[idx + gridWidth] = gridColors[idx];
//                    hashSetBuffer.Remove(gridColors[idx]);
//                }
//            }

//            // remove color which current column has, from color cache
//            if (0 <= currentColumn)
//            {
//                for (int j = targetRow; j < gridHeight; j++)
//                {
//                    int idx = j * gridWidth + currentColumn;
//                    hashSetBuffer.Remove(gridColors[idx]);
//                }
//            }

//            // mandate extension to the next row if color is in color cache
//            for (int i = currentColumn + 1; i < gridWidth; i++)
//            {
//                int idx = targetRow * gridWidth + i;
//                if (hashSetBuffer.Contains(gridColors[idx]))
//                {
//                    result[idx] |= GridConnectionFlag.BaseDown;
//                    gridColors[idx + gridWidth] = gridColors[idx];
//                    hashSetBuffer.Remove(gridColors[idx]);
//                }
//            }
//        }

//        if (0 == targetRow || gridHeight - 1 == targetRow) return;

//        // weave connect stage
//        for (int i = 0; i < gridWidth; i++)
//        {
//            int idx = targetRow * gridWidth + i;
//            if (GridConnectionFlag.None == (result[idx] & GridConnectionFlag.BaseRight))
//            {
//                int nextIdx = -1;
//                for (int j = i + 1; j < gridWidth - 1; j++)
//                {
//                    int searchIdx = targetRow * gridWidth + j;
//                    if (GridConnectionFlag.None != (result[searchIdx] & GridConnectionFlag.AboveKeepMask)) break;
//                    if (GridConnectionFlag.None == (result[searchIdx] & GridConnectionFlag.BaseRight))
//                    {
//                        nextIdx = searchIdx + 1;
//                        break;
//                    }
//                }

//                if (-1 != nextIdx && UnityEngine.Random.value < probabilityWeaveConnectionPerDeadEnd)
//                {
//                    result[idx] |= GridConnectionFlag.AboveToRightStart;
//                    for (int j = idx + 1; j < nextIdx; j++)
//                    {
//                        result[j] |= GridConnectionFlag.AboveHorizontalKeep;
//                    }
//                    result[nextIdx] |= GridConnectionFlag.AboveFromLeftEnd;
//                    JoinGridColor(gridColors, gridColors[idx], gridColors[nextIdx], unusedColorPool);
//                    i += nextIdx - idx + 1;
//                }
//            }
//        }
//    }

//    private static void FillColumn(
//        GridConnectionFlag[] result,
//        int[] gridColors,
//        int currentRow,
//        int targetColumn,
//        int gridWidth,
//        int gridHeight,
//        Stack<int> unusedColorPool,
//        ref int maxColorID,
//        HashSet<int> hashSetBuffer,
//        float probabilityAdjacentConnectionPerGrid,
//        float probabilityWeaveConnectionPerDeadEnd)
//    {
//        hashSetBuffer.Clear();

//        // fill empty grid
//        for (int i = currentRow + 1; i < gridHeight; i++)
//        {
//            int idx = i * gridWidth + targetColumn;
//            if (-1 == gridColors[idx])
//            {
//                if (!unusedColorPool.TryPop(out int color)) color = ++maxColorID;
//                gridColors[idx] = color;
//            }
//        }

//        // randomly connect adjacent grid
//        for (int i = currentRow + 1; i < gridHeight - 1; i++)
//        {
//            int idx = i * gridWidth + targetColumn;
//            if (UnityEngine.Random.value < probabilityAdjacentConnectionPerGrid)
//            {
//                result[idx] |= GridConnectionFlag.BaseDown;
//                JoinGridColor(gridColors, gridColors[idx], gridColors[idx + gridWidth], unusedColorPool);
//            }

//        }

//        // build next row stage if it is not the final row
//        if (gridWidth - 1 != targetColumn)
//        {
//            // cache seen color
//            for (int i = currentRow + 1; i < gridHeight; i++)
//            {
//                int idx = i * gridWidth + targetColumn;
//                hashSetBuffer.Add(gridColors[idx]);
//            }

//            // random extension to the next row, remove color from color cache
//            for (int i = currentRow + 1; i < gridHeight; i++)
//            {
//                int idx = i * gridWidth + targetColumn;
//                if (UnityEngine.Random.value < probabilityAdjacentConnectionPerGrid)
//                {
//                    result[idx] |= GridConnectionFlag.BaseRight;
//                    gridColors[idx + 1] = gridColors[idx];
//                    hashSetBuffer.Remove(gridColors[idx]);
//                }
//            }

//            // remove color which current row has, from color cache
//            if (0 <= currentRow)
//            {
//                for (int j = targetColumn; j < gridWidth; j++)
//                {
//                    int idx = currentRow * gridWidth + j;
//                    hashSetBuffer.Remove(gridColors[idx]);
//                }
//            }

//            // mandate extension to the next row if color is in color cache
//            for (int i = currentRow + 1; i < gridHeight; i++)
//            {
//                int idx = i * gridWidth + targetColumn;
//                if (hashSetBuffer.Contains(gridColors[idx]))
//                {
//                    result[idx] |= GridConnectionFlag.BaseDown;
//                    gridColors[idx + 1] = gridColors[idx];
//                    hashSetBuffer.Remove(gridColors[idx]);
//                }
//            }
//        }

//        if (0 == targetColumn || gridWidth - 1 == targetColumn) return;

//        // weave connect stage
//        for (int i = 0; i < gridHeight; i++)
//        {
//            int idx = i * gridWidth + targetColumn;
//            if (GridConnectionFlag.None == (result[idx] & GridConnectionFlag.BaseDown))
//            {
//                int nextIdx = -1;
//                for (int j = i + 1; j < gridHeight - 1; j++)
//                {
//                    int searchIdx = j * gridWidth + targetColumn;
//                    if (GridConnectionFlag.None != (result[searchIdx] & GridConnectionFlag.AboveKeepMask)) break;
//                    if (GridConnectionFlag.None == (result[searchIdx] & GridConnectionFlag.BaseDown))
//                    {
//                        nextIdx = searchIdx + gridWidth;
//                        break;
//                    }
//                }
//                if (-1 != nextIdx && UnityEngine.Random.value < probabilityWeaveConnectionPerDeadEnd)
//                {
//                    result[idx] |= GridConnectionFlag.AboveToDownStart;
//                    for (int j = idx + gridWidth; j < nextIdx; j += gridWidth)
//                    {
//                        result[j] |= GridConnectionFlag.AboveVerticalKeep;
//                    }
//                    result[nextIdx] |= GridConnectionFlag.AboveFromUpEnd;
//                    JoinGridColor(gridColors, gridColors[idx], gridColors[nextIdx], unusedColorPool);
//                    i += (nextIdx - idx) / gridWidth + 1;
//                }
//            }
//        }
//    }

//    private static void MakeValid(
//        GridConnectionFlag[] result,
//        int[] gridColors,
//        int gridWidth,
//        int gridHeight,
//        Stack<int> unusedColorPool)
//    {
//        for (int idx = gridWidth * (gridHeight - 1); idx < gridWidth * gridHeight - 1; idx++)
//        {
//            if (gridColors[idx] != gridColors[idx + 1])
//            {
//                result[idx] |= GridConnectionFlag.BaseRight;
//                JoinGridColor(gridColors, gridColors[idx], gridColors[idx + 1], unusedColorPool);
//            }
//            PrintDebug(gridColors, gridWidth, gridHeight);
//        }
//    }

//    private static bool IsValid(int[] gridColors)
//    {
//        foreach (int i in gridColors) if (0 != i) return false;
//        return true;
//    }
//}