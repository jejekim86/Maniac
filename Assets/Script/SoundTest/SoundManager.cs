using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect
{
    Cash,
    fireBullet,
    explosion,
    siren
}

public enum BGM
{
    menuBGM,
    inGameDefault,
    inGameChase,
    victory
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
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlayBGM(BGM bgm)
    {
        bgmAudioSource.clip = bgms[(int)bgm];
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void PlaySoundEffect(SoundEffect soundEffect, AudioSource audioSource)
    {
        audioSource.clip = effectClips[(int)soundEffect];
        audioSource.Play();
        audioSource.PlayOneShot(effectClips[(int)soundEffect]);
    }
}
