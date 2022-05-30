using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class LevelEditorToolsManager : MonoBehaviour
{
    public string saveFileName;
    public LayerMask editingLayers;
    public Planes editingPlane;
    public PaintBlockBehaviour paintTool;
    public EraseBlockBehaviour eraseTool;
    public RotateBlockBehaviour rotateTool;
    public Transform pool;

    public GridController gridController;
    public LevelPlane levelPlane;
    public CharacterPlane characterPlane;
    public ObjectPlane objectPlane;

    public LevelDesign editingDesign;

    public enum ToolTypes
    {
        Paint,
        Rotate,
        Erase,
        None
    }

    public enum Planes
    {
        Terrain,
        Character,
        Object
    }

    public ToolTypes toolType;
    [HideInInspector]
    public int objectIndex;
    public GameObject[] objectList;


    public void SaveDesign()
    {
        if(gridController == null || levelPlane == null || characterPlane == null || objectPlane == null)
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
        editingDesign.objectGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];

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

                    // Save object grid (Object Plane)
                    if (objectPlane.grid[h, l, w].block == null)
                    {
                        editingDesign.objectGrid[count] = 0;
                    }
                    else
                    {
                        editingDesign.objectGrid[count] = objectPlane.grid[h, l, w].block.GetComponent<Block>().id;
                    }
                    count++;
                }
            }
        }
        
        SaveSystem.SaveLevelDesign(saveFileName, editingDesign);

    }

    public void LoadDesign()
    {
        if (gridController == null || levelPlane == null || characterPlane == null || objectPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }
        editingDesign = new LevelDesign();
        LevelDesign saved = SaveSystem.LoadLevelDesign(saveFileName);
        editingDesign.gridHeight = saved.gridHeight;
        editingDesign.gridLength = saved.gridLength;
        editingDesign.gridWidth = saved.gridWidth;
        editingDesign.terrainGrid = saved.terrainGrid;
        editingDesign.characterGrid = saved.characterGrid;
        editingDesign.objectGrid = saved.objectGrid;

        levelPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];
        characterPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];
        objectPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];

        int count = 0;
        for (int h = 0; h < editingDesign.gridHeight; h++)
        {
            for (int l = 0; l < editingDesign.gridLength; l++)
            {
                for (int w = 0; w < editingDesign.gridWidth; w++)
                {
                    levelPlane.idGrid[h, l, w] =  editingDesign.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = editingDesign.characterGrid[count];
                    objectPlane.idGrid[h, l, w] = editingDesign.objectGrid[count];
                    count++;
                }
            }
        }

        Vector3Int gridSize = new Vector3Int(editingDesign.gridWidth, editingDesign.gridHeight, editingDesign.gridLength);
        gridController.InitializeGrid(gridSize);
    }

    public void LoadDefaultDesign()
    {
        if (gridController == null || levelPlane == null || characterPlane == null || objectPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }
        editingDesign = new LevelDesign();
        editingDesign.gridHeight = 5;
        editingDesign.gridLength = 10;
        editingDesign.gridWidth = 10;
        editingDesign.terrainGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];
        editingDesign.characterGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];
        editingDesign.objectGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];

        int count = 0;
        for (int h = 0; h < 1; h++)
        {
            for (int l = 0; l < editingDesign.gridLength; l++)
            {
                for (int w = 0; w < editingDesign.gridWidth; w++)
                {
                    editingDesign.terrainGrid[count] = 100;
                    count++;
                }
            }
        }


        levelPlane.idGrid = new int[editingDesign.gridHeight,  editingDesign.gridLength, editingDesign.gridWidth];
        characterPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];
        objectPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];

        count = 0;
        for (int h = 0; h < editingDesign.gridHeight; h++)
        {
            for (int l = 0; l < editingDesign.gridLength; l++)
            {
                for (int w = 0; w < editingDesign.gridWidth; w++)
                {
                    levelPlane.idGrid[h, l, w] = editingDesign.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = editingDesign.characterGrid[count];
                    objectPlane.idGrid[h, l, w] = editingDesign.objectGrid[count];
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
        ObjectPlane.OnObjectPlaneInitialized += InitializeObjectPlane;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializeCharacterPlane;
        ObjectPlane.OnObjectPlaneInitialized -= InitializeObjectPlane;
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

    private void InitializeObjectPlane(ObjectPlane objectPlane)
    {
        this.objectPlane = objectPlane;
    }
}
