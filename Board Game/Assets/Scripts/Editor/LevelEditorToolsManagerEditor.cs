using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(LevelEditorToolsManager))]
public class LevelEditorToolsManagerEditor : Editor
{
    public int gridDirectionInteger;
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
            DisplayPredictedBlockAtCursor(manager.gridController, manager.editingLayers, gridDirectionInteger);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Debug.Log("Left Mouse Clicked");
                if (Event.current.control)
                {
                    Debug.Log("Rotate");
                    manager.rotateTool.currentGridDirectionInt++;
                    if(manager.rotateTool.currentGridDirectionInt > 3)
                        manager.rotateTool.currentGridDirectionInt = -3;
                    manager.paintTool.gridDirectionInt = manager.rotateTool.currentGridDirectionInt;
                    gridDirectionInteger = manager.rotateTool.currentGridDirectionInt;

                }
            }
        }
        

        if (manager.toolType == LevelEditorToolsManager.ToolTypes.Paint)
        {
            DisplayPredictedBlockAtCursor(manager.gridController, manager.editingLayers, gridDirectionInteger);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Debug.Log("Left Mouse Clicked");

                // May need to reimplement to use pooling instead of instantiation
                if (Event.current.control)
                {
                    if (manager.editingPlane == LevelEditorToolsManager.Planes.Character)
                    {
                        manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.terrainPlane, manager.characterPlane, manager.editingLayers);
                    }
                    else if (manager.editingPlane == LevelEditorToolsManager.Planes.Terrain)
                    {
                        manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.terrainPlane, manager.editingLayers);
                    }
                    else if (manager.editingPlane == LevelEditorToolsManager.Planes.Object)
                    {
                        manager.paintTool.PaintBlockAtCursor(manager.objectList[manager.objectIndex], manager.gridController, manager.terrainPlane, manager.objectPlane, manager.editingLayers);
                    }

                }

            }
        }

        if (manager.toolType == LevelEditorToolsManager.ToolTypes.Erase)
        {
            DisplayPredictedBlockAtCursor(manager.gridController, manager.editingLayers, gridDirectionInteger);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Debug.Log("Left Mouse Clicked");

                if ( Event.current.control)
                {
                    if (manager.editingPlane == LevelEditorToolsManager.Planes.Character)
                    {
                        manager.eraseTool.EraseBlockAtCursor(manager.characterPlane, manager.pool, manager.editingLayers);
                    }
                    else if (manager.editingPlane == LevelEditorToolsManager.Planes.Terrain)
                    {
                        manager.eraseTool.EraseBlockAtCursor(manager.terrainPlane, manager.pool, manager.editingLayers);
                    }
                    else if (manager.editingPlane == LevelEditorToolsManager.Planes.Object)
                    {
                        manager.eraseTool.EraseBlockAtCursor(manager.objectPlane, manager.pool, manager.editingLayers);
                    }

                }
            }
        }

    }




    public void DisplayPredictedBlockAtCursor(GridController gridController, LayerMask mask, int gridDirectionInt)
    {
        // Get block at cursor to ensure continuity of blocks
        GameObject block = BlockUtilities.GetBlockInLevelFromCursor(mask);
        if (block == null) { return; }

        // Get direction to get the neighboring cell
        GridDirection direction = BlockUtilities.GetGridDirectionFromBlockInLevelFromCursor(mask);
        Cell cell = gridController.GetCellFromCellWithDirection(block.GetComponent<Block>().cell, direction);
        if (cell == null) { return; }

        // Draw a wireframe at the cell in Editor mode
        BlockUtilities.DrawWireframeAtCell(cell, gridDirectionInt);
    }

}
