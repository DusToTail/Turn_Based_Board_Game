using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that normalizes the directions on a 3D grid
/// </summary>
public class GridDirection
{
    public Vector3Int direction;

    public enum Directions
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down,
        None
    }

    private GridDirection(Vector3Int vector)
    {
        direction = vector;
    }

    public static implicit operator Vector3Int(GridDirection dir) => dir.direction;
    public static implicit operator Vector3(GridDirection dir) => dir.direction;
    public static implicit operator GridDirection(Vector3Int vector) => new GridDirection(vector);

    public static readonly GridDirection None = new GridDirection(Vector3Int.zero);
    public static readonly GridDirection Forward = new GridDirection(Vector3Int.forward);
    public static readonly GridDirection Backward = new GridDirection(Vector3Int.back);
    public static readonly GridDirection Left = new GridDirection(Vector3Int.left);
    public static readonly GridDirection Right = new GridDirection(Vector3Int.right);
    public static readonly GridDirection Up = new GridDirection(Vector3Int.up);
    public static readonly GridDirection Down = new GridDirection(Vector3Int.down);

    public static readonly List<GridDirection> AllDirections = new List<GridDirection>()
    {
        None,
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    };

    public static GridDirection GetDirectionFromVector3Int(Vector3Int vector)
    {
        return AllDirections.Where(x => x.direction == vector).DefaultIfEmpty(None).First();
    }

    public static GridDirection RotateLeft(GridDirection direction)
    {
        if(direction == null) { return None; }

        if(direction == Forward) { return Left; }
        if(direction == Backward) { return Right; }
        if(direction == Left) { return Backward; }
        if(direction == Right) { return Forward; }
        if(direction == Up) { return Up; }
        if(direction == Down) { return Down; }

        return None;
    }

    public static GridDirection RotateRight(GridDirection direction)
    {
        if (direction == null) { return None; }

        if (direction == Forward) { return Right; }
        if (direction == Backward) { return Left; }
        if (direction == Left) { return Forward; }
        if (direction == Right) { return Backward; }
        if (direction == Up) { return Up; }
        if (direction == Down) { return Down; }

        return None;
    }

}
