using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateBlockBehaviour : MonoBehaviour
{
    public Block currentBlock;

    public void RotateSelectedBlock(Block block, Block.Rotations rotation)
    {
        if(currentBlock == null) { return; }
        block.RotateHorizontally(rotation);
    }
}
