using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A plane made out of terrain blocks, used to define the terrains on the board game
/// </summary>
[ExecuteAlways]
public class LevelPlane : MonoBehaviour
{
    public delegate void LevelPlaneInitialized(LevelPlane levelPlane);
    public static event LevelPlaneInitialized OnLevelPlaneInitialized;

    public GameObject[,,] grid { get; private set; }
    public int[,,] idGrid { get; set; }
    [SerializeField]
    private BlockIDContainer blockIDs;

    public bool CheckIfCellIsOccupied(Cell cell)
    {
        if(grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x] != null) { return true; }
        return false;
    }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGrid;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGrid;
    }

    /// <summary>
    /// English: Initialize the default grid of blocks
    /// </summary>
    /// <param name="controller"></param>
    private void InitializeGrid(GridController controller)
    {
        // Clear past childs in the grid
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            Debug.Log($"Destroying {transform.GetChild(i).gameObject.name}");
            if (Application.isPlaying) { Destroy(transform.GetChild(i).gameObject); }
            else { DestroyImmediate(transform.GetChild(i).gameObject); }
        }

        if(idGrid == null) { idGrid = new int[controller.gridSize.y, controller.gridSize.z, controller.gridSize.x]; }
        grid = new GameObject[controller.gridSize.y, controller.gridSize.z, controller.gridSize.x];

        int count = 0;
        for (int h = 0; h < controller.gridSize.y; h++)
        {
            for (int l = 0; l < controller.gridSize.z; l++)
            {
                for (int w = 0; w < controller.gridSize.x; w++)
                {
                    Cell cell = controller.grid[h, l, w];
                    if(idGrid[h, l, w] == 0) { continue; }
                    GameObject block = blockIDs.GetCopyFromID(idGrid[h, l, w]);
                    block.transform.parent = transform;
                    block.name = $"Block {cell.gridPosition}";
                    block.GetComponent<Block>().Initialize(cell, GridDirection.Forward, Vector3Int.one);
                    BlockUtilities.PlaceTerrainBlockAtCell(block, this, cell);
                    grid[h, l, w] = block;
                    count++;
                    Debug.Log($"Created Block id {idGrid[h, l, w]} at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }
        Debug.Log($"In total, grid intialized with {count} blocks");
        if(OnLevelPlaneInitialized != null)
            OnLevelPlaneInitialized(this);
    }



}
