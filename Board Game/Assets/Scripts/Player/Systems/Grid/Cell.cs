using UnityEngine;

/// <summary>
/// Smallest component in a 3 dimensional grid with [height, Length, Width] corresponding to [y, z, x] in world coordinates
/// </summary>
public class Cell
{
    public Vector3 worldPosition;
    public Vector3Int gridPosition;

    public Cell(Vector3 worldPosition, Vector3Int gridPosition)
    {
        this.worldPosition = worldPosition;
        this.gridPosition = gridPosition;
    }
}
