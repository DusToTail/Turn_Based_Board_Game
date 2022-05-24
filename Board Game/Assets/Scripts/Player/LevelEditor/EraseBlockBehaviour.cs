using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EraseBlockBehaviour : MonoBehaviour
{

    public void EraseBlockAtCursor(LevelPlane plane, Transform poolTransform)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor();

        // Move the block the the bool and disable it
        BlockUtilities.MoveBlockAtCellToPool(plane, block.GetComponent<Block>().cell, poolTransform);
    }

}
