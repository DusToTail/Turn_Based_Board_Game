using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Data")]
public class AudioScriptableObject : ScriptableObject
{
    public AudioData[] sources;
}
