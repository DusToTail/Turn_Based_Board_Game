using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all blocks of a level
/// </summary>
public abstract class Block : MonoBehaviour
{
    public int id;
    public Cell cell;
    public GridDirection forwardDirection;
    public Vector3Int cellBasedSize;

    public enum Rotations
    {
        Left,
        Right,
    }

    public virtual void RotateHorizontally(Rotations rotation)
    {
        if(rotation != Rotations.Left && rotation != Rotations.Right) { return; }

        switch(rotation)
        {
            case Rotations.Left:
                SnapGridDirection(GridDirection.RotateLeft(forwardDirection));
                break;
            case Rotations.Right:
                SnapGridDirection(GridDirection.RotateRight(forwardDirection));
                break;
            default:
                break;
        }
    }

    public virtual void SnapToCell(Cell cell)
    {
        if (cell == null) { return; }
        this.cell = cell;
        transform.position = cell.worldPosition;
    }

    public virtual void SnapGridDirection(GridDirection direction)
    {
        if (cell == null) { return; }
        forwardDirection = direction;
        transform.rotation = Quaternion.LookRotation((Vector3)forwardDirection);
    }

    public void Initialize(Cell cell, int gridDirectionInt)
    {
        this.cell = cell;
        SnapToCell(cell);
        SnapGridDirection(GridDirection.GetDirectionFromInt(gridDirectionInt));
    }
}
