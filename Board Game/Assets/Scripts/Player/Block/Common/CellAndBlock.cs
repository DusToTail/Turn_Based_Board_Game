using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper of a cell and a game object (block), used in Character and Object Plane's grid
/// </summary>
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
