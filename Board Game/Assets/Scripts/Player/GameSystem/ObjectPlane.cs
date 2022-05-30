using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A plane made out of object blocks, used to keep track of all objects in the level. Object is not passable like terrain
/// </summary>
[ExecuteAlways]
public class ObjectPlane : MonoBehaviour
{
    public delegate void ObjectPlaneInitialized(ObjectPlane objectPlane);
    public static event ObjectPlaneInitialized OnObjectPlaneInitialized;

    public CellAndBlock[,,] grid { get; private set; }
    public int[,,] idGrid { get; set; }

    [SerializeField]
    private BlockIDContainer blockIDs;

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGrid;

    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGrid;
    }

    private void InitializeGrid(GridController controller)
    {
        Debug.Log($"Object grid initializing");

        // Clear past childs in the grid
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Debug.Log($"Destroying {transform.GetChild(i).gameObject.name}");
            if (Application.isPlaying) { Destroy(transform.GetChild(i).gameObject); }
            else { DestroyImmediate(transform.GetChild(i).gameObject); }
        }

        if (idGrid == null) { idGrid = new int[controller.gridSize.y, controller.gridSize.z, controller.gridSize.x]; }
        grid = new CellAndBlock[controller.gridSize.y, controller.gridSize.z, controller.gridSize.x];

        for (int h = 0; h < controller.gridSize.y; h++)
        {
            for (int l = 0; l < controller.gridSize.z; l++)
            {
                for (int w = 0; w < controller.gridSize.x; w++)
                {
                    Cell cell = controller.grid[h, l, w];
                    CellAndBlock cellAndBlock = new CellAndBlock(cell, null);
                    grid[h, l, w] = cellAndBlock;

                    if (idGrid[h, l, w] == 0) 
                    {
                        Debug.Log($"Object Plane: Null at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                        continue; 
                    }

                    GameObject block = blockIDs.GetCopyFromID(idGrid[h, l, w]);
                    block.transform.parent = transform;
                    block.name = $"{block.name} {cell.gridPosition}";
                    block.GetComponent<Block>().Initialize(cell, GridDirection.Forward);
                    BlockUtilities.PlaceObjectBlockAtCell(block, this, cell);

                    Debug.Log($"Object Plane: Created {block.name} {idGrid[h, l, w]} at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }
        Debug.Log($"Object grid initialized");
        if (OnObjectPlaneInitialized != null)
            OnObjectPlaneInitialized(this);
    }



}
