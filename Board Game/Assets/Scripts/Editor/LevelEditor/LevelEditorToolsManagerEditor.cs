using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[ExecuteAlways]
[CustomEditor(typeof(LevelEditorToolsManager))]
public class LevelEditorToolsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelEditorToolsManager manager = (LevelEditorToolsManager)target;
        DrawDefaultInspector();
        manager.objectIndex = EditorGUILayout.IntSlider("Block Index", manager.objectIndex, 0, manager.objectList.Length);
    }

    private void OnSceneGUI()
    {
        LevelEditorToolsManager manager = (LevelEditorToolsManager)target;

        if(manager.toolType == LevelEditorToolsManager.ToolTypes.Paint)
        {
            manager.paintTool.DisplayPredictedBlockAtCursor(manager.gridController, manager.levelPlane, manager.editingLayers);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                // May need to reimplement to use pooling instead of instantiation
                if(Event.current.control)
                    manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.levelPlane, manager.editingLayers);
            }
        }

        if (manager.toolType == LevelEditorToolsManager.ToolTypes.Erase)
        {
            manager.paintTool.DisplayPredictedBlockAtCursor(manager.gridController, manager.levelPlane, manager.editingLayers);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if( Event.current.control)
                    manager.eraseTool.EraseBlockAtCursor(manager.levelPlane, manager.pool, manager.editingLayers);
            }
        }


    }

}
