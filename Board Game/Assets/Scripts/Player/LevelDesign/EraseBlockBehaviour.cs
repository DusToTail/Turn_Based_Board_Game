using UnityEngine;

[ExecuteInEditMode]
public class EraseBlockBehaviour : MonoBehaviour
{
    public void EraseBlockAtCursor(TerrainPlane plane, Transform poolTransform, LayerMask mask)
    {
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        BlockUtilities.MoveTerrainBlockAtCellToPool(plane, block.GetComponent<Block>().cell, poolTransform);
    }
    public void EraseBlockAtCursor(CharacterPlane plane, Transform poolTransform, LayerMask mask)
    {
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        BlockUtilities.MoveCharacterBlockAtCellToPool(plane, block.GetComponent<Block>().cell, poolTransform);
    }
    public void EraseBlockAtCursor(ObjectPlane plane, Transform poolTransform, LayerMask mask)
    {
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        BlockUtilities.MoveObjectBlockAtCellToPool(plane, block.GetComponent<Block>().cell, poolTransform);
    }

}
