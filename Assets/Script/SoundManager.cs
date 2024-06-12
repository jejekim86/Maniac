using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect
{
    fireBullet,
    explosion
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource effectAudioSource;

    [SerializeField] private AudioClip[] audioClips;

    void Start()
    {
        if(Instance == null)
            Instance = this;
    }
    
    public void PlaySoundEffect(SoundEffect soundEffect)
    {
        effectAudioSource.PlayOneShot(audioClips[(int)soundEffect]);
    }
}
