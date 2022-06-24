using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Simple tester (debugger) for grid pathfinding
/// </summary>
public class GridPathfindingDebugger : MonoBehaviour
{
    public static GridPathfinding.PathfindingCell[,,] grid;

    public GridController gridController;
    public TerrainPlane levelPlane;
    public CharacterPlane characterPlane;
    public ObjectPlane objectPlane;

    public Vector3Int fromPosition;
    public Vector3Int toPosition;
    [SerializeField] private int displayHeight;
    [SerializeField] private bool displayGrid;

    private void Update()
    {
        if (!displayGrid) { return; }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gridController.grid == null) { return; }
            Debug.Log("***********************    START   ************************");
            Cell fromCell = gridController.grid[fromPosition.y, fromPosition.z, fromPosition.x];
            Cell toCell = gridController.grid[toPosition.y, toPosition.z, toPosition.x];
            List<GridPathfinding.PathfindingCell> backtrackList = GridPathfinding.GetBacktrackPath(fromCell, toCell, gridController, levelPlane, characterPlane, objectPlane);
            foreach (GridPathfinding.PathfindingCell backtrack in backtrackList)
            {
                Debug.DrawLine(backtrack.cell.worldPosition, backtrack.cell.worldPosition + Vector3.up, Color.red, 10);
            }
            Debug.Log("***********************    END   ************************");
        }
        else if(Input.GetKeyDown(KeyCode.O))
        {
            grid = null;
        }

        
    }

    #if UNITY_EDITOR
    private void OnGUI()
    {
        if (!displayGrid) { return; }
        if (grid == null) { return; }
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            for(int j = 0; j < grid.GetLength(2); j++)
            {
                if(displayHeight > grid.GetLength(0)) { displayHeight = grid.GetLength(0) - 1; }
                if(displayHeight < 0) { displayHeight = 0; }
                GridPathfinding.PathfindingCell cell = grid[displayHeight, i, j];
                Handles.Label(cell.cell.worldPosition, cell.gCost.ToString(), style);

            }
        }
    }
    #endif

    public static void SetGrid(GridPathfinding.PathfindingCell[,,] checkGrid)
    {
        grid = checkGrid;
    }

}
