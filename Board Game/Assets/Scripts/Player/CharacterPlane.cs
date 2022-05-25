using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A plane made out of character blocks, used to keep track of all character in the level
/// </summary>
[ExecuteAlways]
public class CharacterPlane : MonoBehaviour
{
    public delegate void CharacterPlaneInitialized(CharacterPlane characterPlane);
    public static event CharacterPlaneInitialized OnCharacterPlaneInitialized;

    public int activeCharacterCount = 0;

    public CellAndBlock[,,] grid { get; private set; }
    public int[,,] idGrid { get; set; }

    [SerializeField]
    private BlockIDContainer blockIDs;


    public bool CheckIfCellIsOccupied(Cell cell)
    {
        if (grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block != null) { return true; }
        return false;
    }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGrid;
        CharacterBlock.OnCharacterAdded += IncrementActiveCount;
        CharacterBlock.OnCharacterRemoved += DecrementActiveCount;

    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGrid;
        CharacterBlock.OnCharacterAdded -= IncrementActiveCount;
        CharacterBlock.OnCharacterRemoved -= DecrementActiveCount;
    }

    private void IncrementActiveCount() { activeCharacterCount++; }
    private void DecrementActiveCount() { activeCharacterCount--; }


    private void InitializeGrid(GridController controller)
    {
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

                    if (idGrid[h, l, w] == 0) { continue; }

                    GameObject block = blockIDs.GetCopyFromID(idGrid[h, l, w]);
                    block.transform.parent = transform;
                    block.name = $"Block {cell.gridPosition}";
                    block.GetComponent<Block>().Initialize(cell, GridDirection.Forward, Vector3Int.one);
                    BlockUtilities.PlaceCharacterBlockAtCell(block, this, cell);

                    Debug.Log($"Created Block id {idGrid[h, l, w]} at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }
        Debug.Log($"Character grid initialized");
        if (OnCharacterPlaneInitialized != null)
            OnCharacterPlaneInitialized(this);
    }
}
