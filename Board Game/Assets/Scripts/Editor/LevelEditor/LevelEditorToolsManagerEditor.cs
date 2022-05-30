using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(LevelEditorToolsManager))]
public class LevelEditorToolsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelEditorToolsManager manager = (LevelEditorToolsManager)target;
        GUILayout.Label("***Tools***");
        GUILayout.Label("--Paint--");
        GUILayout.Label("Paint: Ctrl + LMC");
        GUILayout.Label("--Erase--");
        GUILayout.Label("Erase: Ctrl + LMC");
        GUILayout.Label("--Rotate--");
        GUILayout.Label("Select/Rotate: Ctrl +LMC");
        GUILayout.Label("***********");


        DrawDefaultInspector();
        manager.objectIndex = EditorGUILayout.IntSlider("Block Index", manager.objectIndex, 0, manager.objectList.Length - 1);
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

        if (manager.toolType == LevelEditorToolsManager.ToolTypes.Rotate)
        {
            if(Event.current.type == EventType.MouseDown)
            {
                if (Event.current.control)
                {
                    if (Event.current.button == 0)
                    {
                        manager.rotateTool.currentBlock = BlockUtilities.GetBlockInLevelFromCursor(manager.editingLayers).GetComponent<Block>();
                        manager.rotateTool.RotateSelectedBlock(manager.rotateTool.currentBlock, Block.Rotations.Left);
                    }
                }
            }
        }
        

        if (manager.toolType == LevelEditorToolsManager.ToolTypes.Paint)
        {
            manager.paintTool.DisplayPredictedBlockAtCursor(manager.gridController, manager.editingLayers);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                // May need to reimplement to use pooling instead of instantiation
                if(Event.current.control)
                {
                    if (manager.editingPlane == LevelEditorToolsManager.Planes.Character)
                    {
                        manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.levelPlane, manager.characterPlane, manager.editingLayers);
                    }
                    else if (manager.editingPlane == LevelEditorToolsManager.Planes.Terrain)
                    {
                        manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.levelPlane, manager.editingLayers);
                    }
                    else if (manager.editingPlane == LevelEditorToolsManager.Planes.Object)
                    {
                        manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.levelPlane, manager.objectPlane, manager.editingLayers);
                    }

                }

            }
        }

        if (manager.toolType == LevelEditorToolsManager.ToolTypes.Erase)
        {
            manager.paintTool.DisplayPredictedBlockAtCursor(manager.gridController, manager.editingLayers);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if( Event.current.control)
                {
                    if (manager.editingPlane == LevelEditorToolsManager.Planes.Character)
                    {
                        manager.eraseTool.EraseBlockAtCursor(manager.characterPlane, manager.pool, manager.editingLayers);
                    }
                    else if (manager.editingPlane == LevelEditorToolsManager.Planes.Terrain)
                    {
                        manager.eraseTool.EraseBlockAtCursor(manager.levelPlane, manager.pool, manager.editingLayers);
                    }
                    else if (manager.editingPlane == LevelEditorToolsManager.Planes.Object)
                    {
                        manager.eraseTool.EraseBlockAtCursor(manager.objectPlane, manager.pool, manager.editingLayers);
                    }

                }
            }
        }

    }

}
