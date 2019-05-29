using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    private GameManager gameManager;

    public ParticleSystem correctParticles;         // Particle system to run when a match is correct
    public ParticleSystem wrongParticles;            // Particle system to run when a match is wrong

    private ParticleSystem sP;
    private ParticleSystem fP;

    // Start is called before the first frame update
    void Start()
    {
        spawnParticle();
        gameManager = GetComponent<GameManager>();
        gameManager.useCorrectParticle += playSuccessParticle;
        gameManager.useWrongParticle += playFailParticle;
    }

    // To instantiate both particles
    void spawnParticle()
    {
        sP = Instantiate(correctParticles);
        fP = Instantiate(wrongParticles);
        sP.Stop();
        fP.Stop();
    }

    // Play the correct particle
    void playSuccessParticle()
    {
        sP.transform.localScale = new Vector3(10, 10, 1);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        sP.transform.position = mousePos;
        sP.Play();
    }

    // Play the wrong particle
    void playFailParticle()
    {
        fP.transform.localScale = new Vector3(10, 10, 1);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        fP.transform.position = mousePos;
        fP.Play();
    }
}
