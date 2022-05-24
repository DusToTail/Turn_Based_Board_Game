using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Base class for all building blocks of a level
/// </summary>
public abstract class Block : MonoBehaviour
{
    public Cell cell { get; private set; }
    public GridDirection forwardDirection { get; private set; }
    public Vector3Int cellBasedSize { get; private set; }

    public enum Rotations
    {
        Left,
        Right,
    }

    /// <summary>
    /// English: Rotate the block horizontally
    /// </summary>
    /// <param name="rotation"></param>
    public void RotateHorizontally(Rotations rotation)
    {
        if(rotation != Rotations.Left && rotation != Rotations.Right) { return; }

        switch(rotation)
        {
            case Rotations.Left:
                forwardDirection = GridDirection.RotateLeft(forwardDirection);
                break;
            case Rotations.Right:
                forwardDirection = GridDirection.RotateRight(forwardDirection);
                break;
            default:
                break;
        }
        transform.rotation = Quaternion.LookRotation((Vector3)forwardDirection);
    }

    /// <summary>
    /// English: Snap the block to the position of the cell
    /// </summary>
    /// <param name="cell"></param>
    public void SnapToCell(Cell cell)
    {
        if (cell == null) { return; }
        this.cell = cell;
        transform.position = cell.worldPosition;
    }

    public void Initialize(Cell cell, GridDirection forwardDirection, Vector3Int cellBasedSize)
    {
        this.cell = cell;
        this.forwardDirection = forwardDirection;
        this.cellBasedSize = cellBasedSize;
    }
}
