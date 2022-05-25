using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(LevelEditorToolsManager))]
public class LevelEditorToolsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelEditorToolsManager manager = (LevelEditorToolsManager)target;
        DrawDefaultInspector();
        manager.objectIndex = EditorGUILayout.IntSlider("Block Index", manager.objectIndex, 0, manager.objectList.Length);
        if(GUILayout.Button("Save"))
        {
            manager.SaveDesign();
        }

        if(GUILayout.Button("Load"))
        {
            manager.LoadDesign();
        }

        if (GUILayout.Button("Load Default"))
        {
            manager.LoadDefaultDesign();
        }
    }

    private void OnSceneGUI()
    {
        LevelEditorToolsManager manager = (LevelEditorToolsManager)target;

        

        if (manager.toolType == LevelEditorToolsManager.ToolTypes.Paint)
        {
            if (((1 << LayerMask.NameToLayer(Tags.TERRAIN_LAYER)) & manager.editingLayers) != 0)
            {
                manager.paintTool.DisplayPredictedBlockAtCursor(manager.gridController, manager.levelPlane, manager.editingLayers);
            }
            else if (((1 << LayerMask.NameToLayer(Tags.CHARACTER_LAYER)) & manager.editingLayers) != 0)
            {
                manager.paintTool.DisplayPredictedBlockAtCursor(manager.gridController, manager.characterPlane, manager.editingLayers);
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                // May need to reimplement to use pooling instead of instantiation
                if(Event.current.control)
                {
                    if (((1 << LayerMask.NameToLayer(Tags.TERRAIN_LAYER)) & manager.editingLayers) != 0)
                    {
                        manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.levelPlane, manager.editingLayers);
                    }
                    else if (((1 << LayerMask.NameToLayer(Tags.CHARACTER_LAYER)) & manager.editingLayers) != 0)
                    {
                        manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.characterPlane, manager.editingLayers);
                    }
                }

            }
        }

        if (manager.toolType == LevelEditorToolsManager.ToolTypes.Erase)
        {
            manager.paintTool.DisplayPredictedBlockAtCursor(manager.gridController, manager.levelPlane, manager.editingLayers);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if( Event.current.control)
                {
                    if (((1 << LayerMask.NameToLayer(Tags.TERRAIN_LAYER)) & manager.editingLayers) != 0)
                    {
                        manager.eraseTool.EraseBlockAtCursor(manager.levelPlane, manager.pool, manager.editingLayers);
                    }
                    else if (((1 << LayerMask.NameToLayer(Tags.CHARACTER_LAYER)) & manager.editingLayers) != 0)
                    {
                        manager.eraseTool.EraseBlockAtCursor(manager.characterPlane, manager.pool, manager.editingLayers);
                    }
                }
            }
        }


    }

}
