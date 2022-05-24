using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LevelEditorToolsManager : MonoBehaviour
{
    public LayerMask editingLayers;
    public PaintBlockBehaviour paintTool;
    public EraseBlockBehaviour eraseTool;
    public Transform pool;

    public GridController gridController;
    public LevelPlane levelPlane;

    public enum ToolTypes
    {
        Paint,
        Erase
    }

    public ToolTypes toolType;
    [HideInInspector]
    public int objectIndex;
    public GameObject[] objectList;
    

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized += InitializeLevelPlane;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
    }

    private void InitializeGridController(GridController gridController)
    {
        this.gridController = gridController;
        Debug.Log("Listened and intialized grid control");
    }

    private void InitializeLevelPlane(LevelPlane levelPlane)
    {
        this.levelPlane = levelPlane;
        Debug.Log("Listened and intialized block grid");
    }
}
