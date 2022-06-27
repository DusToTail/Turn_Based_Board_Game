using UnityEngine;
using UnityEditor;

/// <summary>
/// A plane made out of character blocks, used to keep track of all character in the level
/// </summary>
[ExecuteAlways]
public class CharacterPlane : MonoBehaviour
{
    public delegate void CharacterPlaneInitialized(CharacterPlane characterPlane);
    public static event CharacterPlaneInitialized OnCharacterPlaneInitialized;

    public int activeCharacterCount = 0;

    public CellAndBlock[,,] grid { get; private set; }
    public int[,,] idGrid { get; set; }

    [SerializeField] private BlockIDContainer blockIDs;
    [SerializeField] private bool displayGrid;
    [SerializeField] private int displayHeight;


    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGrid;
        CharacterBlock.OnCharacterAdded += IncrementActiveCount;
        CharacterBlock.OnCharacterRemoved += DecrementActiveCount;
        CharacterBlock.OnPositionUpdated += UpdateCharacterPosition;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGrid;
        CharacterBlock.OnCharacterAdded -= IncrementActiveCount;
        CharacterBlock.OnCharacterRemoved -= DecrementActiveCount;
        CharacterBlock.OnPositionUpdated -= UpdateCharacterPosition;
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
                if (GetBlockFromCell(cell) != null)
                    Handles.Label(cell.worldPosition, GetBlockFromCell(cell).id.ToString(), style);
                else
                    Handles.Label(cell.worldPosition, "NULL", style);
            }
        }
    }
    #endif

    private void UpdateCharacterPosition(CharacterBlock block, Cell toCell)
    {
        GameObject character = block.gameObject;
        Cell fromCell = character.GetComponent<Block>().cell;
        GetCellAndBlockFromCell(fromCell).block = null;
        GetCellAndBlockFromCell(toCell).block = character;
        string[] oldNameArray = character.name.Split(' ');
        character.name = oldNameArray[0] + " " + toCell.gridPosition.ToString();

        idGrid[fromCell.gridPosition.y, fromCell.gridPosition.z, fromCell.gridPosition.x] = 0;
        idGrid[toCell.gridPosition.y, toCell.gridPosition.z, toCell.gridPosition.x] = character.GetComponent<Block>().id;
        character.GetComponent<Block>().cell = toCell;
    }
    public bool CheckIfCellIsOccupied(Cell cell)
    {
        if (GetBlockFromCell(cell) != null) { return true; }
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
                    if (grid[h, l, w].block != null)
                    {
                        var checkCharacter = grid[h, l, w].block.GetComponent<CharacterBlock>();
                        if (checkCharacter.id == 1)
                            return checkCharacter;
                    }
                }
            }
        }
        return null;
    }
    public CharacterBlock GetBlockFromCell(Cell cell)
    {
        GameObject block = grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block;
        if(block != null)
            return block.GetComponent<CharacterBlock>();
        return null;
    }
    public CellAndBlock GetCellAndBlockFromCell(Cell cell)
    {
        CellAndBlock result = null;
        result = grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x];
        return result;
    }

    private void IncrementActiveCount() { activeCharacterCount++; }
    private void DecrementActiveCount() { activeCharacterCount--; }

    private void InitializeGrid(GridController controller, LevelDesign levelDesign)
    {
        Debug.Log($"Character grid initializing");
        ClearChildren();
        CreateGrid(controller.gridSize);
        FillGrid(controller, levelDesign);
        Debug.Log($"Character grid initialized");
        if (OnCharacterPlaneInitialized != null)
            OnCharacterPlaneInitialized(this);
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
                        //Debug.Log($"Character Plane: Null at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                        count++;
                        continue; 
                    }
                    GameObject block = blockIDs.GetCopyFromID(idGrid[h, l, w]);
                    block.transform.parent = transform;
                    block.name = $"{block.name} {cell.gridPosition}";
                    block.GetComponent<Block>().Initialize(cell, levelDesign.rotations[count]);
                    BlockUtilities.PlaceCharacterBlockAtCell(block, this, cell);
                    count++;
                    //Debug.Log($"Character Plane: Created {block.name} {idGrid[h, l, w]} at gridPosition {cell.gridPosition} at worldPosition [{cell.worldPosition}]");
                }
            }
        }
    }

}
