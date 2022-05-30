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

    

    public void PlaceBlockAtCell(GameObject prefab, LevelPlane plane, Cell cell)
    {
        if(plane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x] != null) { return; }
        if(prefab == null) { return;}
        GameObject block = Instantiate(prefab, plane.transform, true);
        block.name = $"Block {cell.gridPosition}";
        block.GetComponent<Block>().Initialize(cell, GridDirection.Forward);
        BlockUtilities.PlaceTerrainBlockAtCell(block, plane, cell);
        Debug.Log($"Created Block {cell.gridPosition} at worldPosition {cell.worldPosition}");
    }



    public void PaintBlockAtCursor(GameObject prefab, GridController gridController, LevelPlane levelPlane, CharacterPlane characterPlane, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);

        // Place block at cell
        PlaceBlockAtCell(prefab, levelPlane, characterPlane, cell);
    }

    

    public void PlaceBlockAtCell(GameObject prefab, LevelPlane levelPlane, CharacterPlane characterPlane, Cell cell)
    {
        if (characterPlane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block != null) { return; }
        if (levelPlane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x] != null) { return; }

        if (prefab == null) { return; }
        GameObject block = Instantiate(prefab, characterPlane.transform, true);
        block.name = $"{prefab.name} {cell.gridPosition}";
        block.GetComponent<Block>().Initialize(cell, GridDirection.Forward);
        BlockUtilities.PlaceCharacterBlockAtCell(block, characterPlane, cell);

        Debug.Log($"Created {prefab.name} {cell.gridPosition} at worldPosition {cell.worldPosition}");
    }

    public void PaintBlockAtCursor(GameObject prefab, GridController gridController, LevelPlane levelPlane, ObjectPlane objectPlane, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);

        // Place block at cell
        PlaceBlockAtCell(prefab, levelPlane, objectPlane, cell);
    }



    public void PlaceBlockAtCell(GameObject prefab, LevelPlane levelPlane, ObjectPlane objectPlane, Cell cell)
    {
        if(objectPlane.grid == null) { Debug.Log("Grid of object plane is null"); }
        if (objectPlane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block != null) { return; }
        if (levelPlane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x] != null) { return; }

        if (prefab == null) { return; }
        GameObject block = Instantiate(prefab, objectPlane.transform, true);
        block.name = $"{prefab.name} {cell.gridPosition}";
        block.GetComponent<Block>().Initialize(cell, GridDirection.Forward);
        BlockUtilities.PlaceObjectBlockAtCell(block, objectPlane, cell);

        Debug.Log($"Created {prefab.name} {cell.gridPosition} at worldPosition {cell.worldPosition}");
    }



    public void DisplayPredictedBlockAtCursor(GridController gridController, LayerMask mask)
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



}
