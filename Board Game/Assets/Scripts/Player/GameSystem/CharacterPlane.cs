using UnityEngine;
using System.Linq;

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

    public void UpdateCharacterPosition(CharacterBlock block, Cell toCell)
    {
        GameObject b = block.gameObject;
        Cell fromCell = b.GetComponent<Block>().cell;
        grid[fromCell.gridPosition.y, fromCell.gridPosition.z, fromCell.gridPosition.x].block = null;
        grid[toCell.gridPosition.y, toCell.gridPosition.z, toCell.gridPosition.x].block = b;
        string[] oldNameArray = b.name.Split(' ');
        b.name = oldNameArray[0] + " " + toCell.gridPosition.ToString();

        idGrid[fromCell.gridPosition.y, fromCell.gridPosition.z, fromCell.gridPosition.x] = 0;
        idGrid[fromCell.gridPosition.y, fromCell.gridPosition.z, fromCell.gridPosition.x] = b.GetComponent<Block>().id;

    }

    public bool CheckIfCellIsOccupied(Cell cell)
    {
        if (grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block != null) { return true; }
        return false;
    }

    public CharacterBlock GetPlayerBlock()
    {
        for (int h = 0; h < grid.GetLength(0); h++)
        {
            for (int l = 0; l < grid.GetLength(1); l++)
            {
                for (int w = 0; w < grid.GetLength(2); w++)
                {
                    if(grid[h, l, w].block != null)
                    {
                        if (grid[h, l, w].block.GetComponent<CharacterBlock>() != null)
                        {
                            if (grid[h, l, w].block.GetComponent<CharacterBlock>().id == 1)
                                return grid[h, l, w].block.GetComponent<CharacterBlock>();
                        }
                    }
                }
            }
        }
        return null;
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
                    block.name = $"{block.name} {cell.gridPosition}";
                    block.GetComponent<Block>().Initialize(cell, GridDirection.Forward);
                    BlockUtilities.PlaceCharacterBlockAtCell(block, this, cell);

                    Debug.Log($"Created {block.name} {idGrid[h, l, w]} at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }
        Debug.Log($"Character grid initialized");
        if (OnCharacterPlaneInitialized != null)
            OnCharacterPlaneInitialized(this);
    }
}
