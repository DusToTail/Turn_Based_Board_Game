using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A plane made out of blocks, used to define the terrains on the board game
/// </summary>
[ExecuteAlways]
public class LevelPlane : MonoBehaviour
{
    public delegate void LevelPlaneInitialized(LevelPlane levelPlane);
    public static event LevelPlaneInitialized OnLevelPlaneInitialized;

    public GameObject[,,] grid { get; private set; }
    [SerializeField]
    private GameObject baseBlock;

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGrid;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGrid;
    }


    /// <summary>
    /// English: Initialize the grid of base blocks defined in the Inspector and the injected grid from GridController class
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

        grid = new GameObject[controller.gridSize.y, controller.gridSize.z, controller.gridSize.x];
        int count = 0;
        // Only height 1 as a base for level design
        for (int h = 0; h < 1; h++)
        {
            for (int l = 0; l < controller.gridSize.z; l++)
            {
                for (int w = 0; w < controller.gridSize.x; w++)
                {
                    Cell cell = controller.grid[h, l, w];
                    GameObject block = Instantiate(baseBlock, transform, true);
                    block.name = $"Block {cell.gridPosition}";
                    block.GetComponent<Block>().Initialize(cell, GridDirection.Up, Vector3Int.one);
                    BlockUtilities.PlaceBlockAtCell(block, this, cell);
                    count++;
                    Debug.Log($"Created Block {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }
        Debug.Log($"In total, grid intialized with {count} blocks");
        if(OnLevelPlaneInitialized != null)
            OnLevelPlaneInitialized(this);
    }

}
