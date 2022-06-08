using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerController playerController = (PlayerController)target;
        DrawDefaultInspector();
        GUILayout.Label("Can Control: " + playerController.CanControl.ToString());
        GUILayout.Label("ControlMode: " + playerController.Mode.ToString());
    }
}
