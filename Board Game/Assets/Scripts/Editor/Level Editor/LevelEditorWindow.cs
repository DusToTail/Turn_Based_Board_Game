using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


public partial class LevelEditorWindow : EditorWindow
{
    [SerializeField]
    VisualTreeAsset uxmlAsset;
    //DragAndDropManipulator manipulator;
    private static List<GameObject> objectList = new List<GameObject>();

    private static GUIContent[] toolbarGUIArray = new GUIContent[4];
    private static GUIContent[] objectGUIArray = new GUIContent[9];
    private int objectInt = 0;
    private int toolbarInt = 0;

    [MenuItem("Window/LevelEditor")]
    public static void ShowWindow()
    {
        // Create the window.
        var window = GetWindow<LevelEditorWindow>("Level Editor");
    }

    public static void AddObjectToList(GameObject droppedObject)
    {
        if (objectList.Count > 9) { return; }
        objectList.Add(droppedObject);
        Debug.Log($"Added {droppedObject.name}");
    }

    public static void RemoveObjectFromList(GameObject removedObject)
    {
        objectList.Remove(removedObject);
        Debug.Log($"Removed {removedObject.name}");
    }

    private void OnEnable()
    {
        if (uxmlAsset != null)
            uxmlAsset.CloneTree(rootVisualElement);

        //manipulator = new(rootVisualElement);
        InitializeToolBarGUI();
        InitializeObjectGUI();
    }

    private void OnDisable()
    {
        //manipulator.target.RemoveManipulator(manipulator);
    }

    /// <summary>
    /// English: Initialize each tool's GUIContent
    /// </summary>
    private void InitializeToolBarGUI()
    {
        PaintToolScriptableObject paintToolAsset = Resources.Load<PaintToolScriptableObject>("LevelEditor/PaintToolData");
        EraseToolScriptableObject eraseToolAsset = Resources.Load<EraseToolScriptableObject>("LevelEditor/EraseToolData");
        MoveToolScriptableObject moveToolAsset = Resources.Load<MoveToolScriptableObject>("LevelEditor/MoveToolData");
        ClearToolScriptableObject clearToolAsset = Resources.Load<ClearToolScriptableObject>("LevelEditor/ClearToolData");

        GUIContent paintGUI = new GUIContent(paintToolAsset.toolName, paintToolAsset.icon, paintToolAsset.tooltip);
        GUIContent eraseGUI = new GUIContent(eraseToolAsset.toolName, eraseToolAsset.icon, eraseToolAsset.tooltip);
        GUIContent clearGUI = new GUIContent(clearToolAsset.toolName, clearToolAsset.icon, clearToolAsset.tooltip);
        GUIContent moveGUI = new GUIContent(moveToolAsset.toolName, moveToolAsset.icon, moveToolAsset.tooltip);

        toolbarGUIArray[0] = moveGUI;
        toolbarGUIArray[1] = paintGUI;
        toolbarGUIArray[2] = eraseGUI;
        toolbarGUIArray[3] = clearGUI;
    }

    /// <summary>
    /// English: Initialize each (empty)object's GUIContent
    /// </summary>
    private void InitializeObjectGUI()
    {
        for (int i = 0; i < 9; i++)
        {
            GUIContent defaultGUI = new GUIContent("Null", Texture2D.whiteTexture, "No tip");
            objectGUIArray[i] = defaultGUI;
        }
    }

    private void OnGUI()
    {
        DrawToolBar();
        DrawDropArea();
    }

    /// <summary>
    /// English: Draw a toolbar on to the screen
    /// </summary>
    private void DrawToolBar()
    {
        GUILayout.BeginArea(rootVisualElement.Q<VisualElement>(className: "toolbar_area").layout);
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarGUIArray, GUILayout.MinHeight(21),GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        GUILayout.EndArea();
    }

    /// <summary>
    /// English: Draw the drop area on the screen
    /// </summary>
    private void DrawDropArea()
    {
        GUILayout.BeginArea(rootVisualElement.Q<VisualElement>(className: "drop_area").layout);
        objectInt = GUILayout.SelectionGrid(objectInt, objectGUIArray, 3, GUILayout.MinHeight(200), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUILayout.EndArea();
    }

}
