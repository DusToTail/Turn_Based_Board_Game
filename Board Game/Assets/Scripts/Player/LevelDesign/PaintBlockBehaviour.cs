using UnityEngine;

[ExecuteInEditMode]
public class PaintBlockBehaviour : MonoBehaviour
{
    public int gridDirectionInt;
    public void PaintBlockAtCursor(GameObject prefab, GridController gridController, TerrainPlane plane, LayerMask mask)
    {
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);
        PlaceBlockAtCell(prefab, plane, cell);
    }
    public void PaintBlockAtCursor(GameObject prefab, GridController gridController, TerrainPlane levelPlane, CharacterPlane characterPlane, LayerMask mask)
    {
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);
        PlaceBlockAtCell(prefab, levelPlane, characterPlane, cell);
    }
    public void PaintBlockAtCursor(GameObject prefab, GridController gridController, TerrainPlane levelPlane, ObjectPlane objectPlane, LayerMask mask)
    {
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);
        PlaceBlockAtCell(prefab, levelPlane, objectPlane, cell);
    }
    public void PlaceBlockAtCell(GameObject prefab, TerrainPlane plane, Cell cell)
    {
        if (plane.GetCellAndBlockFromCell(cell).block != null) { return; }
        if (prefab == null) { return; }
        GameObject block = Instantiate(prefab, plane.transform, true);
        block.name = $"Block {cell.gridPosition}";
        block.GetComponent<Block>().Initialize(cell, gridDirectionInt);
        BlockUtilities.PlaceTerrainBlockAtCell(block, plane, cell);
        //Debug.Log($"Created Block {cell.gridPosition} at worldPosition {cell.worldPosition}");
    }
    public void PlaceBlockAtCell(GameObject prefab, TerrainPlane terrainPlane, CharacterPlane characterPlane, Cell cell)
    {
        if (characterPlane.GetCellAndBlockFromCell(cell).block != null) { return; }
        if (terrainPlane.GetCellAndBlockFromCell(cell).block != null) { return; }

        if (prefab == null) { return; }
        GameObject block = Instantiate(prefab, characterPlane.transform, true);
        block.name = $"{prefab.name} {cell.gridPosition}";
        block.GetComponent<Block>().Initialize(cell, gridDirectionInt);
        BlockUtilities.PlaceCharacterBlockAtCell(block, characterPlane, cell);
        //Debug.Log($"Created {prefab.name} {cell.gridPosition} at worldPosition {cell.worldPosition}");
    }
    public void PlaceBlockAtCell(GameObject prefab, TerrainPlane terrainPlane, ObjectPlane objectPlane, Cell cell)
    {
        if(objectPlane.grid == null) { Debug.Log("Grid of object plane is null"); }
        if (objectPlane.GetCellAndBlockFromCell(cell).block != null) { return; }
        if (terrainPlane.GetCellAndBlockFromCell(cell).block != null) { return; }
        if (prefab == null) { return; }
        GameObject block = Instantiate(prefab, objectPlane.transform, true);
        block.name = $"{prefab.name} {cell.gridPosition}";
        block.GetComponent<Block>().Initialize(cell, gridDirectionInt);
        BlockUtilities.PlaceObjectBlockAtCell(block, objectPlane, cell);
        //Debug.Log($"Created {prefab.name} {cell.gridPosition} at worldPosition {cell.worldPosition}");
    }
}
