using UnityEngine;

[ExecuteInEditMode]
public class EraseBlockBehaviour : MonoBehaviour
{

    public void EraseBlockAtCursor(TerrainPlane plane, Transform poolTransform, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);

        // Move the block the the bool and disable it
        BlockUtilities.MoveTerrainBlockAtCellToPool(plane, block.GetComponent<Block>().cell, poolTransform);
    }

    public void EraseBlockAtCursor(CharacterPlane plane, Transform poolTransform, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);

        // Move the block the the bool and disable it
        BlockUtilities.MoveCharacterBlockAtCellToPool(plane, block.GetComponent<Block>().cell, poolTransform);
    }

    public void EraseBlockAtCursor(ObjectPlane plane, Transform poolTransform, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);

        // Move the block the the bool and disable it
        BlockUtilities.MoveObjectBlockAtCellToPool(plane, block.GetComponent<Block>().cell, poolTransform);
    }

}
