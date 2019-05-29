using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip successAudio;

    public AudioClip failureAudio;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySuccessAudio()
    {
        audioSource.PlayOneShot(successAudio);
    }

    public void PlayFailureAudio()
    {
        audioSource.PlayOneShot(failureAudio);
    }
}
