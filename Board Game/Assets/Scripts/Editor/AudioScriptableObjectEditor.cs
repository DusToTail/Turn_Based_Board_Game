using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(AudioScriptableObject))]
public class AudioScriptableObjectEditor : Editor
{
    public int testIndex;
    public override void OnInspectorGUI()
    {
        AudioScriptableObject data = (AudioScriptableObject)target;

        DrawDefaultInspector();
        if(data.sources == null) { return; }
        testIndex = EditorGUILayout.IntSlider(testIndex, 0, data.sources.Length - 1);
        if (GUILayout.Button("PlayTest"))
        {
            if(data.sources[testIndex].clip != null)
                PlayClip(data.sources[testIndex].clip);
        }
        if(GUILayout.Button("StopAll"))
        {
            StopAllClips();
        }
    }

    public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayPreviewClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(AudioClip), typeof(int), typeof(bool) }, 
            null);
        method.Invoke(null, new object[] {clip, startSample, loop});
    }
    public static void StopAllClips()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllPreviewClips",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { },
            null);
        method.Invoke(null, new object[] {});
    }
}
