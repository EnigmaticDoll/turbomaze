using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public static class ItemSpawner
{
    public static void Spawn(MazeStyle style, ItemData[] itemDataArray, int width, int height, int spawnCount)
    {
        float cellToCellDistance = MazeBuilder.CalculateCellToCellDistance(style);

        int length = width * height;
        if (length <= spawnCount) throw new InvalidOperationException("Tried to spawn items more than maze grids.");

        // Fisher Yates shuffle
        int[] slots = Enumerable.Range(0, length).ToArray();
        for (int i = slots.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int swap = slots[i];
            slots[i] = slots[j];
            slots[j] = swap;
        }

        for (int i = 0, currentlySpawnedCount = 0; currentlySpawnedCount < spawnCount; i++)
        {
            int x = slots[i] % width;
            int y = slots[i] / width;
            if (0 == x && 0 == y) continue;

            Vector3 pos = new Vector3(x, 0, -y) * cellToCellDistance;

            GameObject prefab = itemDataArray[UnityEngine.Random.Range(0, itemDataArray.Length)].readOnlyObj;
            GameObject item = GameManager.Instance.GetOrCreateDisabledGameObject(prefab);
            item.transform.position = pos;
            item.SetActive(true);

            currentlySpawnedCount++;
        }
    }
}
