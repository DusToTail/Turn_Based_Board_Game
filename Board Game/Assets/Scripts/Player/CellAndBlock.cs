using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAndBlock
{
    public Cell cell;
    public GameObject block;

    public CellAndBlock(Cell cell, GameObject block)
    {
        this.cell = cell;
        this.block = block;
    }
    public bool ContentIsEmpty() { return cell == null; }
    public GameObject GetContent() { return block; }
}
