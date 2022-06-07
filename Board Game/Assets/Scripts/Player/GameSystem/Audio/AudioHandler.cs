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
        AudioSource source = GetSourceFromName(audioName);
        if (source == null) { return; }
        Debug.Log($"Play audio called {audioName}"); 
        source.Play();
    }

    public void Stop(string audioName)
    {
        AudioSource source = GetSourceFromName(audioName);
        if (source == null) { return; }
        Debug.Log($"Stop audio called {audioName}"); 
        source.Stop();
    }

    public void Pause(string audioName)
    {
        AudioSource source = GetSourceFromName(audioName);
        if (source == null) { return; }
        Debug.Log($"Pause audio called {audioName}"); 
        source.Pause();
    }

    public void UnPause(string audioName)
    {
        AudioSource source = GetSourceFromName(audioName);
        if(source == null) { return; }
        Debug.Log($"UnPause audio called {audioName}");
        source.UnPause();
    }

    public void TransitionFromToAudioSource(string from, string to, float transitionTimeInSeconds)
    {
        Debug.Log($"Start transition from audio {from} to {to} in {transitionTimeInSeconds}");
        StartCoroutine(TransitionFromToAudioSourceCoroutine(from, to, transitionTimeInSeconds));
    }

    private IEnumerator TransitionFromToAudioSourceCoroutine(string from, string to, float transitionTimeInSeconds)
    {
        AudioSource fromSource = GetSourceFromName(from);
        float differenceFromSourceVolumn = 0 - fromSource.volume;
        AudioSource toSource = GetSourceFromName(to);
        float differenceToSourceVolumn = GetDataFromName(to).volume - toSource.volume;
        float t = 0;
        while(t <= transitionTimeInSeconds)
        {
            yield return null;
            if(differenceFromSourceVolumn < 0)
                fromSource.volume += (differenceFromSourceVolumn / transitionTimeInSeconds) * Time.deltaTime;
            if(differenceToSourceVolumn > 0)
                toSource.volume += (differenceToSourceVolumn / transitionTimeInSeconds) * Time.deltaTime;
            t += Time.deltaTime;
        }
    }

    public void ResetAudioSourceSetting(string audioName)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].audioName == audioName && datas[i].clip != null)
            {
                Debug.Log($"Reset audio called {audioName}");
                sources[i].clip = datas[i].clip;
                sources[i].volume = datas[i].volume;
                sources[i].pitch = datas[i].pitch;
                sources[i].loop = datas[i].isLoop;
            }
        }
        Debug.Log($"There is no audio called {audioName}");
    }

    private AudioData GetDataFromName(string audioName)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].audioName == audioName && datas[i].clip != null)
            {
                return datas[i];
            }
        }
        Debug.Log($"There is no audio called {audioName}");
        return null;
    }

    private AudioSource GetSourceFromName(string audioName)
    {
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].audioName == audioName && datas[i].clip != null)
            {
                return sources[i];
            }
        }
        Debug.Log($"There is no audio called {audioName}");
        return null;
    }


}
