using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PaintBlockBehaviour : MonoBehaviour
{
    public void PaintBlockAtCursor(GameObject prefab, GridController gridController, LevelPlane plane)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor();
        if (block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor();
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);

        // Place block at cell
        PlaceBlockAtCell(prefab, plane, cell);
    }

    public void DisplayPredictedBlockAtCursor(GridController gridController, LevelPlane plane)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor();
        if(block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor();
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);
        if(cell == null) { return; }

        // Draw a wireframe at the cell in Editor mode
        BlockUtilities.DrawWireframeAtCell(cell);
    }

    public void PlaceBlockAtCell(GameObject prefab, LevelPlane plane, Cell cell)
    {
        if(plane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x] != null) { return; }
        if(prefab == null) { return;}
        GameObject block = Instantiate(prefab, plane.transform, true);
        block.name = $"Block {cell.gridPosition}";
        block.GetComponent<Block>().Initialize(cell, GridDirection.Up, Vector3Int.one);
        BlockUtilities.PlaceBlockAtCell(block, plane, cell);
        Debug.Log($"Created Block {cell.gridPosition} at worldPosition {cell.worldPosition}");

    }

}
