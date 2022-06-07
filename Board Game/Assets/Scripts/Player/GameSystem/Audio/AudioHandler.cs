using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public AudioData[] datas;
    public AudioSource[] sources;

    public void InitializeAudioSources(AudioData[] dataArray)
    {
        datas = new AudioData[dataArray.Length];
        sources = new AudioSource[dataArray.Length];

        for(int i = 0; i < dataArray.Length; i++)
        {
            datas[i] = dataArray[i];
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = dataArray[i].volume;
            source.clip = dataArray[i].clip;
            source.pitch = dataArray[i].pitch;
            source.loop = dataArray[i].isLoop;
            sources[i] = source;
        }
    }

    public void Play(string audioName)
    {
        for(int i = 0; i < datas.Length; i++)
        {
            if(datas[i].audioName == audioName && datas[i].clip != null)
            {
                Debug.Log($"Play audio called {audioName}");
                sources[i].Play();
            }
        }
        Debug.Log($"There is no audio called {audioName}");
    }

    public void Stop(string audioName)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].audioName == audioName && datas[i].clip != null)
            {
                Debug.Log($"Play audio called {audioName}");
                sources[i].Stop();
            }
        }
        Debug.Log($"There is no audio called {audioName}");
    }

    public void Pause(string audioName)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].audioName == audioName && datas[i].clip != null)
            {
                Debug.Log($"Play audio called {audioName}");
                sources[i].Pause();
            }
        }
        Debug.Log($"There is no audio called {audioName}");
    }

    public void UnPause(string audioName)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].audioName == audioName && datas[i].clip != null)
            {
                Debug.Log($"Play audio called {audioName}");
                sources[i].UnPause();
            }
        }
        Debug.Log($"There is no audio called {audioName}");
    }

    public void TransitionFromToAudioSource(AudioSource from, AudioSource to, float t)
    {

    }


}
