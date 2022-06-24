using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A plane made out of object blocks, used to keep track of all objects in the level
/// </summary>
[ExecuteAlways]
public class ObjectPlane : MonoBehaviour
{
    public delegate void ObjectPlaneInitialized(ObjectPlane objectPlane);
    public static event ObjectPlaneInitialized OnObjectPlaneInitialized;

    public CellAndBlock[,,] grid { get; private set; }
    public int[,,] idGrid { get; set; }

    [SerializeField] private BlockIDContainer blockIDs;
    [SerializeField] private bool displayGrid;
    [SerializeField] private int displayHeight;

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGrid;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGrid;
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
            for (int j = 0; j < grid.GetLength(2); j++)
            {
                if (displayHeight > grid.GetLength(0)) { displayHeight = grid.GetLength(0) - 1; }
                if (displayHeight < 0) { displayHeight = 0; }
                Cell cell = grid[displayHeight, i, j].cell;
                if(GetBlockFromCell(cell) != null)
                    Handles.Label(cell.worldPosition, GetBlockFromCell(cell).id.ToString(), style);
                else
                    Handles.Label(cell.worldPosition, "NULL", style);
            }
        }
    }
    #endif

    public bool CheckIfCellIsOccupied(Cell cell)
    {
        if (GetBlockFromCell(cell) != null) { return true; }
        return false;
    }
    public ObjectBlock GetBlockFromCell(Cell cell)
    {
        ObjectBlock result = null;
        result = grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block?.GetComponent<ObjectBlock>();
        return result;
    }
    public CellAndBlock GetCellAndBlockFromCell(Cell cell)
    {
        CellAndBlock result = null;
        result = grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x];
        return result;
    }

    private void InitializeGrid(GridController controller, LevelDesign levelDesign)
    {
        Debug.Log($"Object grid initializing");
        ClearChildren();
        CreateGrid(controller.gridSize);
        FillGrid(controller, levelDesign);

        Debug.Log($"Object grid initialized");
        if (OnObjectPlaneInitialized != null)
            OnObjectPlaneInitialized(this);
    }
    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            //Debug.Log($"Destroying {transform.GetChild(i).gameObject.name}");
            if (Application.isPlaying) { Destroy(transform.GetChild(i).gameObject); }
            else { DestroyImmediate(transform.GetChild(i).gameObject); }
        }
    }
    private void CreateGrid(Vector3Int gridSize)
    {
        if (idGrid == null) { idGrid = new int[gridSize.y, gridSize.z, gridSize.x]; }
        grid = new CellAndBlock[gridSize.y, gridSize.z, gridSize.x];
    }
    private void FillGrid(GridController controller, LevelDesign levelDesign)
    {
        int count = 0;
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
                        //Debug.Log($"Terrain Plane: Null at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                        count++;
                        continue;
                    }
                    GameObject block = blockIDs.GetCopyFromID(idGrid[h, l, w]);
                    block.transform.parent = transform;
                    block.name = $"{block.name} {cell.gridPosition}";
                    block.GetComponent<Block>().Initialize(cell, levelDesign.rotations[count]);
                    BlockUtilities.PlaceObjectBlockAtCell(block, this, cell);
                    count++;
                    //Debug.Log($"Terrain Plane: Created {block.name} {idGrid[h, l, w]} at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }
    }


}
