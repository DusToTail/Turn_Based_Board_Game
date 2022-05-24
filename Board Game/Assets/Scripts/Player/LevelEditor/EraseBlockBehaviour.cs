using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EraseBlockBehaviour : MonoBehaviour
{

    public void EraseBlockAtCursor(LevelPlane plane, Transform poolTransform, LayerMask mask)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);

        // Move the block the the bool and disable it
        BlockUtilities.MoveBlockAtCellToPool(plane, block.GetComponent<Block>().cell, poolTransform);
    }

}
