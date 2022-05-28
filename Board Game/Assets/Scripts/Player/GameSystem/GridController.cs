using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// English: Controller of the main grid used in the scene
/// </summary>
[ExecuteAlways]
public class GridController : MonoBehaviour
{
    public Vector3 cellSize; // x is width, y is height (elevation), z is length
    public Vector3Int gridSize; // x is width, y is height (elevation), z is length

    public Cell[,,] grid { get; private set; }

    public delegate void GridInitialized(GridController controller);
    public static event GridInitialized OnGridInitialized;
    [SerializeField]
    private bool displayGizmos;
    [SerializeField]
    private int untilHeightIndex;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public Cell GetCellFromCellWithDirection(Cell cell, GridDirection direction)
    {
        Vector3Int newGridPosition = cell.gridPosition + direction;
        if(IsWithinGrid(newGridPosition))
            return grid[newGridPosition.y, newGridPosition.z, newGridPosition.x];

        return cell;
    }

    /// <summary>
    /// English: Only use 2 dimensional grid with odd number of columns. 0 means ignore the cell, 1 means return the cell
    /// Only use for cells on the same height level
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="direction"></param>
    /// <param name="checkGrid"></param>
    /// <returns></returns>
    public Cell[] GetCellsFromCellWithDirectionAnd2DGrid(Cell fromCell, GridDirection direction, int[,] checkGrid)
    {
        List<Cell> cells = new List<Cell>();
        if (fromCell == null) { return cells.ToArray(); }
        if (direction == null) { return cells.ToArray(); }
        if (checkGrid == null) { return cells.ToArray(); }
        Vector2Int directionV2Int = new Vector2Int(direction.direction.x, direction.direction.z);
        if (checkGrid.GetLength(1) % 2 == 0)
        {
            // Width or number of rows is even
            // Not implemented as of now
        }
        else if (checkGrid.GetLength(1) % 2 == 1)
        {
            // Width or number of rows is odd
            int midColumnIndex = checkGrid.GetLength(1) / 2;
            int bottomRowIndex = 0;
            for (int l = 0; l < checkGrid.GetLength(0); l++)
            {
                for (int w = 0; w < checkGrid.GetLength(1); w++)
                {
                    if (l == bottomRowIndex && w == midColumnIndex) { continue; }
                    if(checkGrid[l,w] != 1) { continue; }
                    Vector3Int relativeGridPosition = new Vector3Int(w - midColumnIndex, 0, l - bottomRowIndex);

                    if (direction == GridDirection.Backward)
                    {
                        relativeGridPosition = new Vector3Int(-relativeGridPosition.x, 0, -relativeGridPosition.z);
                        Vector3Int realGridPosition = fromCell.gridPosition + relativeGridPosition;
                        if (IsWithinGrid(realGridPosition))
                        {
                            cells.Add(grid[realGridPosition.y, realGridPosition.z, realGridPosition.x]);
                        }
                    }
                    else if(direction == GridDirection.Forward)
                    {
                        relativeGridPosition = new Vector3Int(relativeGridPosition.x, 0, relativeGridPosition.z);
                        Vector3Int realGridPosition = fromCell.gridPosition + relativeGridPosition;
                        if (IsWithinGrid(realGridPosition))
                        {
                            cells.Add(grid[realGridPosition.y, realGridPosition.z, realGridPosition.x]);
                        }
                    }
                    else if(direction == GridDirection.Left)
                    {
                        relativeGridPosition = new Vector3Int(-relativeGridPosition.z, 0,relativeGridPosition.x);
                        Vector3Int realGridPosition = fromCell.gridPosition + relativeGridPosition;
                        if (IsWithinGrid(realGridPosition))
                        {
                            cells.Add(grid[realGridPosition.y, realGridPosition.z, realGridPosition.x]);
                        }
                    }
                    else if(direction == GridDirection.Right)
                    {
                        relativeGridPosition = new Vector3Int(relativeGridPosition.z, 0, -relativeGridPosition.x);
                        Vector3Int realGridPosition = fromCell.gridPosition + relativeGridPosition;
                        if (IsWithinGrid(realGridPosition))
                        {
                            cells.Add(grid[realGridPosition.y, realGridPosition.z, realGridPosition.x]);
                        }
                    }
                    else
                    {
                        Debug.Log("Direction is not on a horizontal 2D plane");
                    }
                }
            }
        }

        return cells.ToArray();
    }

    public bool IsWithinGrid(Vector3Int gridPosition)
    {
        bool xWithinGrid = gridPosition.x >= 0 && gridPosition.x < gridSize.x;
        bool yWithinGrid = gridPosition.y >= 0 && gridPosition.y < gridSize.y;
        bool zWithinGrid = gridPosition.z >= 0 && gridPosition.z < gridSize.z;

        return xWithinGrid && yWithinGrid && zWithinGrid;
    }

    /// <summary>
    /// English: Initialize the default grid
    /// </summary>
    public void InitializeGrid()
    {
        grid = new Cell[gridSize.y, gridSize.z, gridSize.x];
        for(int h = 0; h < gridSize.y; h++)
        {
            for(int l = 0; l < gridSize.z; l++)
            {
                for(int w = 0; w < gridSize.x; w++)
                {
                    Vector3Int gridPosition = new Vector3Int(w, h, l);
                    Vector3 worldPosition = transform.position + new Vector3(w * cellSize.x, h * cellSize.y, l * cellSize.z) + 0.5f * cellSize;
                    Cell cell = new Cell(worldPosition, gridPosition);
                    grid[h,l,w] = cell;
                    Debug.Log($"Created Cell {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }

        if(OnGridInitialized != null)
        {
            OnGridInitialized(this);
        }
    }

    /// <summary>
    /// English: Initialize the 3 dimensional grid with [height, Length, Width] corresponding to [y, z, x] in world coordinates
    /// </summary>
    public void InitializeGrid(Vector3Int gridSize)
    {
        this.gridSize = gridSize;
        grid = new Cell[gridSize.y, gridSize.z, gridSize.x];
        for (int h = 0; h < gridSize.y; h++)
        {
            for (int l = 0; l < gridSize.z; l++)
            {
                for (int w = 0; w < gridSize.x; w++)
                {
                    Vector3Int gridPosition = new Vector3Int(w, h, l);
                    Vector3 worldPosition = transform.position + new Vector3(w * cellSize.x, h * cellSize.y, l * cellSize.z) + 0.5f * cellSize;
                    Cell cell = new Cell(worldPosition, gridPosition);
                    grid[h, l, w] = cell;
                    Debug.Log($"Created Cell {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }

        if (OnGridInitialized != null)
        {
            OnGridInitialized(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!displayGizmos) { return; }
        if(grid == null)
            Gizmos.color = Color.yellow;
        else
            Gizmos.color = Color.green;

        for (int h = 0; h < gridSize.y; h++)
        {
            if(h > untilHeightIndex) { return; }
            for (int l = 0; l < gridSize.z; l++)
            {
                for (int w = 0; w < gridSize.x; w++)
                {
                    Vector3 worldPosition = transform.position + new Vector3(w * cellSize.x, h * cellSize.y, l * cellSize.z) + 0.5f * cellSize;
                    Gizmos.DrawWireCube(worldPosition, cellSize);
                }
            }
        }

    }

}
