using UnityEngine;

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
        Debug.Log(cell.gridPosition);
        Debug.Log(direction);

        Vector3Int newGridPosition = cell.gridPosition + direction;
        if(IsWithinGrid(newGridPosition))
            return grid[newGridPosition.y, newGridPosition.z, newGridPosition.x];

        return cell;
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
