using System;
using UnityEngine;

public class AudioMananger : MonoBehaviour
{
    public static AudioMananger instance { get; private set; }
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClipData[] audioClipData;
    [SerializeField] private AudioClipData[] musicClipData;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusicClip("Menu");
    }

    public void PlayAudioClip(string clipName)
    {
        foreach (var clip in audioClipData)
        {
            if (clip.clip.name == clipName)
            {
                audioSource.clip = clip.clip;
                audioSource.volume = clip.volume;
                audioSource.pitch = clip.pitch;
                audioSource.loop = clip.loop;
                audioSource.Play();
                return;
            }
        }
        Debug.LogError($"Audio clip {clipName} not found");
    }
    
    public void StopAudioClip()
    {
        audioSource.Stop();
    }
    
    public void PlayMusicClip(string clipName)
    {
        foreach (var clip in musicClipData)
        {
            if (clip.clip.name != clipName) continue;
            musicSource.Stop();
            musicSource.clip = clip.clip;
            musicSource.volume = clip.volume;
            musicSource.pitch = clip.pitch;
            musicSource.loop = clip.loop;
            musicSource.Play();
            return;
        }
        Debug.LogError($"Music clip {clipName} not found");
    }
}

[Serializable]
public struct AudioClipData
{
    public AudioClip clip;
    public float volume;
    public float pitch;
    public bool loop;
}
