using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData
{
    public string audioName;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
    [Range(0f, 1f)] public float pitch;
    public bool isLoop;
}
