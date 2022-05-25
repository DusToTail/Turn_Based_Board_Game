using UnityEngine;

[ExecuteInEditMode]
public class PaintBlockBehaviour : MonoBehaviour
{
    public void PaintBlockAtCursor(GameObject prefab, GridController gridController, LevelPlane plane, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);

        // Place block at cell
        PlaceBlockAtCell(prefab, plane, cell);
    }

    public void DisplayPredictedBlockAtCursor(GridController gridController, LevelPlane plane, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if(block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
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
        BlockUtilities.PlaceTerrainBlockAtCell(block, plane, cell);
        Debug.Log($"Created Block {cell.gridPosition} at worldPosition {cell.worldPosition}");
    }



    public void PaintBlockAtCursor(GameObject prefab, GridController gridController, CharacterPlane plane, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);

        // Place block at cell
        PlaceBlockAtCell(prefab, plane, cell);
    }

    public void DisplayPredictedBlockAtCursor(GridController gridController, CharacterPlane plane, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);
        if (cell == null) { return; }

        // Draw a wireframe at the cell in Editor mode
        BlockUtilities.DrawWireframeAtCell(cell);
    }

    public void PlaceBlockAtCell(GameObject prefab, CharacterPlane plane, Cell cell)
    {
        if (plane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block != null) { return; }
        if (prefab == null) { return; }
        GameObject block = Instantiate(prefab, plane.transform, true);
        block.name = $"Block {cell.gridPosition}";
        block.GetComponent<Block>().Initialize(cell, GridDirection.Up, Vector3Int.one);
        BlockUtilities.PlaceCharacterBlockAtCell(block, plane, cell);
        Debug.Log($"Created Block {cell.gridPosition} at worldPosition {cell.worldPosition}");
    }

}
