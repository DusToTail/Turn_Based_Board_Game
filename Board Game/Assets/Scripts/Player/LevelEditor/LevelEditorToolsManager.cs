using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class LevelEditorToolsManager : MonoBehaviour
{
    public string saveFileName;
    public LayerMask editingLayers;
    public PaintBlockBehaviour paintTool;
    public EraseBlockBehaviour eraseTool;
    public Transform pool;

    public GridController gridController;
    public LevelPlane levelPlane;
    public CharacterPlane characterPlane;

    public LevelDesign editingDesign;

    public enum ToolTypes
    {
        Paint,
        Erase
    }

    public ToolTypes toolType;
    [HideInInspector]
    public int objectIndex;
    public GameObject[] objectList;


    public void SaveDesign()
    {
        if(gridController == null || levelPlane == null || characterPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }

        editingDesign = new LevelDesign();
        // Save base grid size
        editingDesign.gridHeight = gridController.gridSize.y;
        editingDesign.gridLength = gridController.gridSize.z;
        editingDesign.gridWidth = gridController.gridSize.x;
        editingDesign.terrainGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];
        editingDesign.characterGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];

        int count = 0;
        for (int h = 0; h < gridController.gridSize.y; h++)
        {
            for (int l = 0; l < gridController.gridSize.z; l++)
            {
                for (int w = 0; w < gridController.gridSize.x; w++)
                {
                    // Save terrain grid (Level Plane)
                    if (levelPlane.grid[h, l, w] == null)
                    {
                        editingDesign.terrainGrid[count] = 0;
                    }
                    else
                    {
                        editingDesign.terrainGrid[count] = levelPlane.grid[h, l, w].GetComponent<Block>().id;
                    }

                    // Save character grid (Character Plane)
                    if (characterPlane.grid[h, l, w].block == null)
                    {
                        editingDesign.characterGrid[count] = 0;
                    }
                    else
                    {
                        editingDesign.characterGrid[count] = characterPlane.grid[h, l, w].block.GetComponent<Block>().id;
                    }
                    count++;
                }
            }
        }
        
        SaveSystem.SaveLevelDesign(saveFileName, editingDesign);

    }

    public void LoadDesign()
    {
        if (gridController == null || levelPlane == null || characterPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }

        SaveSystem.LoadLevelDesign(saveFileName, editingDesign);

        levelPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];
        characterPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];

        int count = 0;
        for (int h = 0; h < editingDesign.gridHeight; h++)
        {
            for (int l = 0; l < editingDesign.gridLength; l++)
            {
                for (int w = 0; w < editingDesign.gridWidth; w++)
                {
                    levelPlane.idGrid[h, l, w] =  editingDesign.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = editingDesign.characterGrid[count];
                    count++;
                }
            }
        }

        Vector3Int gridSize = new Vector3Int(editingDesign.gridWidth, editingDesign.gridHeight, editingDesign.gridLength);
        gridController.InitializeGrid(gridSize);
    }

    public void LoadDefaultDesign()
    {
        if (gridController == null || levelPlane == null || characterPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }
        editingDesign = new LevelDesign();
        editingDesign.gridHeight = 5;
        editingDesign.gridLength = 10;
        editingDesign.gridWidth = 10;
        editingDesign.terrainGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];
        System.Array.Fill(editingDesign.terrainGrid, 100);
        editingDesign.characterGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];
        System.Array.Fill(editingDesign.characterGrid, 0);

        levelPlane.idGrid = new int[editingDesign.gridHeight,  editingDesign.gridLength, editingDesign.gridWidth];
        characterPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];

        int count = 0;
        for (int h = 0; h < editingDesign.gridHeight; h++)
        {
            for (int l = 0; l < editingDesign.gridLength; l++)
            {
                for (int w = 0; w < editingDesign.gridWidth; w++)
                {
                    Debug.Log(count);
                    Debug.Log(levelPlane.idGrid[h, l, w]);
                    Debug.Log(editingDesign.terrainGrid[count]);
                    levelPlane.idGrid[h, l, w] = editingDesign.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = editingDesign.characterGrid[count];
                    count++;
                }
            }
        }

        Vector3Int gridSize = new Vector3Int(editingDesign.gridWidth, editingDesign.gridHeight, editingDesign.gridLength);
        gridController.InitializeGrid(gridSize);
    }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized += InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized += InitializeCharacterPlane;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializeCharacterPlane;
    }

    private void InitializeGridController(GridController gridController)
    {
        this.gridController = gridController;
    }

    private void InitializeLevelPlane(LevelPlane levelPlane)
    {
        this.levelPlane = levelPlane;
    }

    private void InitializeCharacterPlane(CharacterPlane characterPlane)
    {
        this.characterPlane = characterPlane;
    }
}
