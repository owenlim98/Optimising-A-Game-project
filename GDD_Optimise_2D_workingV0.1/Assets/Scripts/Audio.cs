using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    private GameManager gameManager;

    public AudioClip correct, wrong;        // Audios to run when player click in the game

    private AudioSource audiosource;        // To allow audio to play

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        audiosource = GetComponent<AudioSource>();      // Get the Audio Source in the component and attach to audiosource
        gameManager.playCorrectAudio += playCorrect;    // To let GameManager script to know to use playCorrect
        gameManager.playWrongAudio += playWrong;        // To let GameManager script to know to use playWrong
    }

    // Setting audiosource to play the correct sound
    void playCorrect()
    {
        audiosource.clip = correct;
        audiosource.Play();
    }

    // Setting audiosource to play the wrong sound
    void playWrong()
    {
        audiosource.clip = wrong;
        audiosource.Play();
    }
}
