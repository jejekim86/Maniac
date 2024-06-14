using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect
{
    fireBullet,
    explosion,
    siren
}

public enum BGM
{
    menuBGM,
    inGameDefault,
    inGameChase
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmAudioSource;

    [SerializeField] private AudioClip[] effectClips;
    [SerializeField] private AudioClip[] bgms;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void PlayBGM(BGM bgm)
    {
        bgmAudioSource.clip = bgms[(int)bgm];
        bgmAudioSource.Play();
    }

    public void PlaySoundEffect(SoundEffect soundEffect, AudioSource audioSource)
    {
        audioSource.PlayOneShot(effectClips[(int)soundEffect]);
    }
}
