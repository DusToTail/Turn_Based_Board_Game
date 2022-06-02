using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Simple tester (debugger) for grid pathfinding
/// </summary>
public class GridPathfindingDebugger : MonoBehaviour
{
    public GridController gridController;
    public LevelPlane levelPlane;
    public CharacterPlane characterPlane;
    public ObjectPlane objectPlane;

    public Vector3Int fromPosition;
    public Vector3Int toPosition;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
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

        
    }

}
