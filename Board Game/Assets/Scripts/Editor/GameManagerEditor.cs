using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager manager = (GameManager)target;
        DrawDefaultInspector();

        if(GUILayout.Button("LevelStart"))
        {
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
