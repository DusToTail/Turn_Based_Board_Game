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
    public TerrainPlane terrainPlane;
    public CharacterPlane characterPlane;
    public ObjectPlane objectPlane;
    public StairsManager stairsManager;
    public RemoteTriggerManager remoteTriggerManager;
    public RemoteDoorManager remoteDoorManager;

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
        if(gridController == null || terrainPlane == null || characterPlane == null || objectPlane == null)
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
        editingDesign.rotations = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];

        int count = 0;
        for (int h = 0; h < gridController.gridSize.y; h++)
        {
            for (int l = 0; l < gridController.gridSize.z; l++)
            {
                for (int w = 0; w < gridController.gridSize.x; w++)
                {
                    Cell cell = gridController.grid[h, l, w];
                    // Save rotations
                    editingDesign.rotations[count] = 0;

                    // Save terrain grid (Terrain Plane)
                    Block terrainBlock = terrainPlane.GetBlockFromCell(cell);
                    if (terrainBlock == null)
                    {
                        editingDesign.terrainGrid[count] = 0;
                    }
                    else
                    {
                        editingDesign.terrainGrid[count] = terrainBlock.id;
                        editingDesign.rotations[count] = GridDirection.GetIntFromDirection(terrainBlock.forwardDirection);
                    }

                    // Save character grid (Character Plane)
                    Block characterBlock = characterPlane.GetBlockFromCell(cell);
                    if (characterBlock == null)
                    {
                        editingDesign.characterGrid[count] = 0;
                    }
                    else
                    {
                        editingDesign.characterGrid[count] = characterBlock.id;
                        editingDesign.rotations[count] = GridDirection.GetIntFromDirection(characterBlock.forwardDirection);
                    }

                    // Save object grid (Object Plane)
                    Block objectBlock = objectPlane.GetBlockFromCell(cell);
                    if (objectBlock == null)
                    {
                        editingDesign.objectGrid[count] = 0;
                    }
                    else
                    {
                        editingDesign.objectGrid[count] = objectBlock.id;
                        editingDesign.rotations[count] = GridDirection.GetIntFromDirection(objectBlock.forwardDirection);
                    }
                    count++;
                }
            }
        }

        
        if (remoteTriggerManager != null)
        {
            editingDesign.remoteTriggersData = new int[remoteTriggerManager.remoteTriggers.Count * 3];
            for (int i = 0; i < remoteTriggerManager.remoteTriggers.Count; i++)
            {
                editingDesign.remoteTriggersData[3 * i] = remoteTriggerManager.remoteTriggers[i].toBeTriggeredBlock.cell.gridPosition.x;
                editingDesign.remoteTriggersData[3 * i + 1] = remoteTriggerManager.remoteTriggers[i].toBeTriggeredBlock.cell.gridPosition.y;
                editingDesign.remoteTriggersData[3 * i + 2] = remoteTriggerManager.remoteTriggers[i].toBeTriggeredBlock.cell.gridPosition.z;
            }
        }

        if (remoteDoorManager != null)
        {
            editingDesign.remoteDoorsData = new int[remoteDoorManager.remoteDoors.Count];
            for (int i = 0; i < remoteDoorManager.remoteDoors.Count; i++)
            {
                editingDesign.remoteDoorsData[i] = remoteDoorManager.remoteDoors[i].isOpen? 1 : 0;
            }
        }

        SaveSystem.SaveLevelDesign(saveFileName, editingDesign);

    }

    public void LoadDesign()
    {
        if (gridController == null || terrainPlane == null || characterPlane == null || objectPlane == null)
        {
            Debug.Log("Grids are not initialized during loading");
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
        editingDesign.rotations = saved.rotations;
        editingDesign.remoteTriggersData = saved.remoteTriggersData;
        editingDesign.remoteDoorsData = saved.remoteDoorsData;

        terrainPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];
        characterPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];
        objectPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];

        int count = 0;
        for (int h = 0; h < editingDesign.gridHeight; h++)
        {
            for (int l = 0; l < editingDesign.gridLength; l++)
            {
                for (int w = 0; w < editingDesign.gridWidth; w++)
                {
                    terrainPlane.idGrid[h, l, w] =  editingDesign.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = editingDesign.characterGrid[count];
                    objectPlane.idGrid[h, l, w] = editingDesign.objectGrid[count];
                    count++;
                }
            }
        }

        remoteTriggerManager?.InitializeRemoteTriggersData(editingDesign.remoteTriggersData);
        remoteDoorManager?.InitializeRemoteDoorsData(editingDesign.remoteDoorsData);

        gridController.InitializeGrid(editingDesign);
    }

    public void LoadDefaultDesign()
    {
        if (gridController == null || terrainPlane == null || characterPlane == null || objectPlane == null)
        {
            Debug.Log("Grids are not initialized during loading");
            return;
        }
        editingDesign = new LevelDesign();
        editingDesign.gridHeight = gridController.gridSize.y;
        editingDesign.gridLength = gridController.gridSize.z;
        editingDesign.gridWidth = gridController.gridSize.x;
        editingDesign.terrainGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];
        editingDesign.characterGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];
        editingDesign.objectGrid = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];
        editingDesign.rotations = new int[editingDesign.gridHeight * editingDesign.gridLength * editingDesign.gridWidth];

        int count = 0;
        for (int h = 0; h < 1; h++)
        {
            for (int l = 0; l < editingDesign.gridLength; l++)
            {
                for (int w = 0; w < editingDesign.gridWidth; w++)
                {
                    editingDesign.terrainGrid[count] = 100;
                    editingDesign.rotations[count] = 1;
                    count++;
                }
            }
        }


        terrainPlane.idGrid = new int[editingDesign.gridHeight,  editingDesign.gridLength, editingDesign.gridWidth];
        characterPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];
        objectPlane.idGrid = new int[editingDesign.gridHeight, editingDesign.gridLength, editingDesign.gridWidth];

        count = 0;
        for (int h = 0; h < editingDesign.gridHeight; h++)
        {
            for (int l = 0; l < editingDesign.gridLength; l++)
            {
                for (int w = 0; w < editingDesign.gridWidth; w++)
                {
                    terrainPlane.idGrid[h, l, w] = editingDesign.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = editingDesign.characterGrid[count];
                    objectPlane.idGrid[h, l, w] = editingDesign.objectGrid[count];
                    count++;
                }
            }
        }

        gridController.InitializeGrid(editingDesign);
    }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGridController;
        TerrainPlane.OnLevelPlaneInitialized += InitializeTerrainPlane;
        CharacterPlane.OnCharacterPlaneInitialized += InitializeCharacterPlane;
        ObjectPlane.OnObjectPlaneInitialized += InitializeObjectPlane;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        TerrainPlane.OnLevelPlaneInitialized -= InitializeTerrainPlane;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializeCharacterPlane;
        ObjectPlane.OnObjectPlaneInitialized -= InitializeObjectPlane;
    }

    private void InitializeGridController(GridController gridController, LevelDesign levelDesign)
    {
        this.gridController = gridController;
    }

    private void InitializeTerrainPlane(TerrainPlane terrainPlane)
    {
        this.terrainPlane = terrainPlane;
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
