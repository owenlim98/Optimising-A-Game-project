using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject backgroundSprites;

    float backgroundTime = 0;               // Used for the background sprites oscillation

    private Vector3 positionA, positionB;   // Used to set the first and second position of the sprites

    bool showBackgroundCharacters = false;  // The background sprites are shown intermittently, i.e. on/off

    // Start is called before the first frame update
    void Start()
    {
        // Set the left and right X axis values for the background sprite oscillations.
        // This is set to be 5 units to the left and right of each sprite's default position.
        //
        positionA = new Vector3(0 - 5, 0, 2);
        positionB = new Vector3(0 + 5, 0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the positions of the oscillating background sprites. This is done using
        // the PingPong function
        //
        float time = Mathf.PingPong(Time.time * 1f, 1);

        backgroundSprites.transform.localPosition = Vector3.Lerp(positionA, positionB, time);
        ShowBackgroundCharacters();
    }

    // Displays or hides the background sprites at regular intervals, in this case 
    // every 3 seconds
    //
    private void ShowBackgroundCharacters()
    {
        backgroundTime += Time.deltaTime;

        if (backgroundTime > 3f)
        {
            backgroundTime = 0;
            showBackgroundCharacters = !showBackgroundCharacters;

            backgroundSprites.SetActive(showBackgroundCharacters);
        }
    }
}
