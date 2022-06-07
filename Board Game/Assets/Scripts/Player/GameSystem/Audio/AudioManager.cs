using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float masterVolumn;
    [Range(0f, 1f)]
    public float masterPitch;

    public void TransitionFromToAudioSource(AudioSource from, AudioSource to, float t)
    {

    }


}
