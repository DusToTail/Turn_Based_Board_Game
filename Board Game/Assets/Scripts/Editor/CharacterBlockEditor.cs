using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterBlock))]
public class CharacterBlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterBlock characterBlock = (CharacterBlock)target;

        DrawDefaultInspector();
        GUILayout.Label("***Current Stats***");
        GUILayout.Label($"Health: {characterBlock.CurHealth}");
        GUILayout.Label($"Moves Left: {characterBlock.CurMovesLeft}");
        GUILayout.Label($"****************");

    }
}
