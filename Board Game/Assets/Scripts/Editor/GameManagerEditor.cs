using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public int levelIndex;
    public override void OnInspectorGUI()
    {
        GameManager manager = (GameManager)target;
        DrawDefaultInspector();

        levelIndex = EditorGUILayout.IntField(levelIndex);


        if(GUILayout.Button("LoadLevel"))
        {
            manager.CallLevelLoadingStarted(levelIndex);
            manager.CallLevelStarted();
        }

        if (GUILayout.Button("PlayerTurnStart"))
        {
            manager.CallPlayerTurnStarted();
        }

        if (GUILayout.Button("LevelFinish"))
        {
            manager.CallLevelFinished();
        }


    }
}
