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
    public static GridDirection operator -(GridDirection dir)
    {
        return GetDirectionFromVector3Int(-dir.direction);
    }
    public static GridDirection operator -(GridDirection left, GridDirection right)
    {
        return GetDirectionFromVector3Int(left.direction - right.direction);
    }
    public static GridDirection operator +(GridDirection left, GridDirection right)
    {
        return GetDirectionFromVector3Int(left.direction - right.direction);
    }


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
    public static GridDirection GetDirectionFromInt(int mode)
    {
        GridDirection result = None;
        switch (mode)
        {
            case 1:
                result = Forward;
                break;
            case -1:
                result = Backward;
                break;
            case 2:
                result = Left;
                break;
            case -2:
                result = Right;
                break;
            case 3:
                result = Up;
                break;
            case -3:
                result = Down;
                break;
            case 0:
                result = None;
                break;
            default:
                break;
        }
        return result;
    }

    public static int GetIntFromDirection(GridDirection direction)
    {
        if (direction == Forward) { return 1; }
        if (direction == Backward) { return -1; }
        if (direction == Left) { return 2; }
        if (direction == Right) { return -2; }
        if (direction == Up) { return 3; }
        if (direction == Down) { return -3; }

        return 0;
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
