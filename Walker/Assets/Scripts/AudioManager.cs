using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource bgmAudioSource = null;

    [SerializeField]
    private AudioSource seAudioSource = null;

    [SerializeField]
    private AudioClip[] bgmAudioClips = null;

    [SerializeField]
    private AudioClip[] seAudioClips = null;

    public enum eBGMAudioClip
    {
        BURNING_CAVERN = 0,
    }

    public enum eSEAudioClip
    {
        WHISTLE = 0,
    }

    public void PlayBGM(eBGMAudioClip _audioClip)
    {
        bgmAudioSource.clip = bgmAudioClips[(int)_audioClip];
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }

    public void PlayOneShotSE(eSEAudioClip _audioClip)
    {
        seAudioSource.PlayOneShot(seAudioClips[(int)_audioClip]);
    }


    public void StopSE()
    {
        seAudioSource.Stop();
    }
}
