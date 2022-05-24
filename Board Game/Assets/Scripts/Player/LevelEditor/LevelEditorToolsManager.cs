using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;



public class LevelEditorToolsManager : Editor
{
    [SerializeField] private PaintBlockBehaviour paintTool;
    [SerializeField] private EraseBlockBehaviour eraseTool;

    private GridController gridController;
    private LevelPlane levelPlane;

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

    private void OnSceneGUI()
    {
        ToolScriptableObject.ToolType toolType = Resources.Load<ToolScriptableObject>("LevelEditor/Tool Data Asset").toolType;

        if(Event.current.type == EventType.MouseDown)
        {
            Debug.Log("MouseDown");
            if(toolType == ToolScriptableObject.ToolType.Paint)
            {
                paintTool.PaintBlockAtCursor(gridController, levelPlane);
            }
        }


        if(Event.current.type == EventType.MouseUp)
        {
            Debug.Log("MouseUp");
        }
    }

    private void InitializeGridController(GridController gridController)
    {
        this.gridController = gridController;
    }

    private void InitializeLevelPlane(LevelPlane levelPlane)
    {
        this.levelPlane = levelPlane;
    }


}