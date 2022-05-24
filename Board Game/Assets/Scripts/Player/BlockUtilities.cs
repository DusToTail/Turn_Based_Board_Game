using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A Utility class that is designed to manipulate position and to pool blocks
/// </summary>
public class BlockUtilities
{
    public static List<GameObject> blockPool  = new List<GameObject>();

    public static void PlaceBlockAtCell(GameObject block, LevelPlane plane, Cell cell)
    {
        if(plane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x] != null) { return; }

        block.GetComponent<Block>().SnapToCell(cell);
        plane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x] = block;
    }

    public static void MoveBlockAtCellToPool(LevelPlane plane, Cell cell, Transform pool)
    {
        GameObject block = plane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x];
        if(block == null) { return; }

        block.transform.parent = pool;
        block.transform.localPosition = Vector3.zero;
        block.SetActive(false);
        blockPool.Add(block);
        plane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x] = null;
    }

}
