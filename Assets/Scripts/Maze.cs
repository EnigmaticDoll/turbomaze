using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class describing a maze
/// </summary>
public class Maze
{
    /// <summary>
    /// A grid discriptor, indicating whether this grid is connected to the next grid in the right or down directions.
    /// Connections toward left and up directions are determined by the next grids themselves.
    /// </summary>
    [Flags]
    public enum GridConnectionFlag : byte
    {
        // Do not use HasFlag() as it causes boxing.
        None = 0,
        Right = 1 << 0,
        Down = 1 << 1,
        AnyMask = Right | Down,
    }

    private GridConnectionFlag[] data;
    public int width { get; private set; }
    public int height { get; private set; }

    public bool TryGetFlag(int x, int y, out GridConnectionFlag flag)
    {
        if (x < 0 || width <= x
            || y < 0 || height <= y)
        {
            flag = GridConnectionFlag.None;
            return false;
        }

        flag = data[y * width + x];
        return true;
    }

    public Maze(int width, int height, float probabilityAdjacentConnectionPerGridPair = 0.5f)
    {
        data = GenerateMazeData(width, height, probabilityAdjacentConnectionPerGridPair);
        this.width = width;
        this.height = height;
    }

    /// <summary>
    /// A maze data generator based on Eller's method.
    /// Think sets of grids as 'colors'. Each uncolored grid gets its own color.
    /// </summary>
    /// <param name="width">maze width</param>
    /// <param name="height">maze height</param>
    /// <param name="probabilityAdjacentConnectionPerGridPair">probability of adjacent grid connection per grid-grid pair</param>
    /// <returns></returns>
    public static GridConnectionFlag[] GenerateMazeData(int width, int height, float probabilityAdjacentConnectionPerGridPair)
    {

        if (width < 3 || height < 3) return null;

        GridConnectionFlag[] result = new GridConnectionFlag[width * height];
        int[] currentLineColors = new int[width];
        Stack<int> unusedColorPool = new Stack<int>();
        HashSet<int> colorCacheToNextLine = new HashSet<int>();

        for (int i = 0; i < width; i++) currentLineColors[i] = i + 1;
        int maxColorID = width;

        for (int i = 0; i < height - 1; i++)
        {
            AppendLine(result.AsSpan(i * width, width), currentLineColors, colorCacheToNextLine, unusedColorPool, ref maxColorID, probabilityAdjacentConnectionPerGridPair);
        }

        Validate(result.AsSpan((height - 1) * width, width), currentLineColors, unusedColorPool, ref maxColorID);

        return result;
    }

    /// <summary>
    /// Merge two groups of a line.
    /// </summary>
    /// <param name="color0"></param>
    /// <param name="color1"></param>
    /// <param name="currentLineColors"></param>
    /// <param name="unusedColorPool"></param>
    /// <param name="maxColorID"></param>
    private static void MergeColors(
        int color0,
        int color1,
        int[] currentLineColors,
        Stack<int> unusedColorPool,
        ref int maxColorID)
    {
        if (color0 == color1) return;

        int width = currentLineColors.Length;

        int colorMin = Math.Min(color0, color1);
        int colorMax = Math.Max(color0, color1);
        for (int i = 0; i < width; i++) if (currentLineColors[i] == colorMax) currentLineColors[i] = colorMin;
        unusedColorPool.Push(colorMax);
    }

    /// <summary>
    /// Append a line of a maze, using Eller's method.
    /// </summary>
    /// <param name="gridDataSpan"></param>
    /// <param name="currentLineColors"></param>
    /// <param name="colorCacheToNextLineReused"></param>
    /// <param name="unusedColorPool"></param>
    /// <param name="maxColorID"></param>
    /// <param name="probabilityAdjacentConnectionPerGrid"></param>
    private static void AppendLine(
        Span<GridConnectionFlag> gridDataSpan,
        int[] currentLineColors,
        HashSet<int> colorCacheToNextLineReused,
        Stack<int> unusedColorPool,
        ref int maxColorID,
        float probabilityAdjacentConnectionPerGrid)
    {
        int width = gridDataSpan.Length;
        colorCacheToNextLineReused.Clear();

        // randomly connect to the grid on the right if it is not in the same group
        for (int i = 0; i < width - 1; i++)
        {
            if (currentLineColors[i] != currentLineColors[i + 1]
                && UnityEngine.Random.value < probabilityAdjacentConnectionPerGrid)
            {
                gridDataSpan[i] |= GridConnectionFlag.Right;

                MergeColors(currentLineColors[i], currentLineColors[i + 1], currentLineColors, unusedColorPool, ref maxColorID);
            }
        }

        // memorize a list of visible colors, and flip colors to negative values to indicate whether it is connected vertically (+) or not (-)
        for (int i = 0; i < width; i++)
        {
            colorCacheToNextLineReused.Add(currentLineColors[i]);
            currentLineColors[i] = -currentLineColors[i];
        }

        // randomly connect to the lower grid, reflipping its color from negative to positive
        for (int i = 0; i < width; i++)
        {
            if (UnityEngine.Random.value < probabilityAdjacentConnectionPerGrid)
            {
                gridDataSpan[i] |= GridConnectionFlag.Down;
                currentLineColors[i] = -currentLineColors[i];
                colorCacheToNextLineReused.Remove(currentLineColors[i]);
            }
        }

        // mandate at least one grid per color to be connected to the next line
        foreach (int color in colorCacheToNextLineReused)
        {
            for (int i = 0; i < width; i++)
            {
                if (color == -currentLineColors[i])
                {
                    gridDataSpan[i] |= GridConnectionFlag.Down;
                    currentLineColors[i] = color;
                    break;
                }
            }
        }

        // move to the next line, negative color means not connected and should be replaced with new color
        for (int i = 0; i < width; i++)
        {
            if (currentLineColors[i] < 0)
            {
                if (!unusedColorPool.TryPop(out int color)) color = ++maxColorID;
                currentLineColors[i] = color;
            }
        }
    }

    /// <summary>
    /// Make a maze valid, using Eller's method.
    /// </summary>
    /// <param name="gridDataSpan"></param>
    /// <param name="currentLineColors"></param>
    /// <param name="unusedColorPool"></param>
    /// <param name="maxColorID"></param>
    private static void Validate(
        Span<GridConnectionFlag> gridDataSpan,
        int[] currentLineColors,
        Stack<int> unusedColorPool,
        ref int maxColorID)
    {
        int width = gridDataSpan.Length;

        for (int i = 0; i < width - 1; i++)
        {
            if (currentLineColors[i] != currentLineColors[i + 1])
            {
                gridDataSpan[i] |= GridConnectionFlag.Right;

                MergeColors(currentLineColors[i], currentLineColors[i + 1], currentLineColors, unusedColorPool, ref maxColorID);
            }
        }
    }
}
