using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void OnScoreUpdate();
    public OnScoreUpdate onScoreUpdate;

    public delegate void AudioPlay();
    public AudioPlay playCorrectAudio;
    public AudioPlay playWrongAudio;

    public delegate void SpawnParticle();
    public SpawnParticle useCorrectParticle;
    public SpawnParticle useWrongParticle;

    public float speed = -5f;


    private List<GameObject> frames;
    private GameObject endFrame;
    private float frameWidth;
    private float gameWidth;
    private float leftExtent;

    private void Start()
    {
        Application.targetFrameRate = 300;

        getDataFromObjectPoolingScript();
    }

    void getDataFromObjectPoolingScript()
    {
        endFrame = ObjectPooling.endFrame;
        frameWidth = ObjectPooling.frameWidth;
        frames = ObjectPooling.instance.frames;
        gameWidth = ObjectPooling.instance.gameWidth;
        leftExtent = ObjectPooling.instance.leftExtent;
    }

    // Check if the user has selected a frame (mousedown or touch). This is done by
    // casting a ray into the game view from the input position (after converting
    // from screen to world coordinates).
    //
    // Returns the frame that was selected, if any.
    //
    private GameObject CheckHitFrame()
    {
        GameObject frame = null;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 raypos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(raypos, Vector3.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider != null)
                {
                    frame = hit.collider.gameObject;
                }
            }
        }
        return frame;
    }

    private void Update()
    {
        // Animate the frames to the left
        //
        ObjectPooling.instance.MoveFrames();

        // Check if the player has selected any of the frames
        //
        GameObject hitFrame = CheckHitFrame();

        // If a frame has been selected, check if it is mirrored or not (check if all the bottom
        // sprites match their corresponding top sprites). Process the frames accordingly.
        //
        if (hitFrame)
        {
            bool matched = true;
            int numChildren = hitFrame.transform.GetChild(0).transform.childCount;
            for (int i = 0; i < numChildren; i++)
            {
                // Get each top sprite (s1) and its corresponding bottom sprite (s2)
                //
                GameObject s1 = hitFrame.transform.GetChild(0).transform.GetChild(i).gameObject;
                GameObject s2 = hitFrame.transform.GetChild(1).transform.GetChild(i).gameObject;

                // Get the name of each sprite
                //
                string s1Name = s1.GetComponent<SpriteRenderer>().sprite.name;
                string s2Name = s2.GetComponent<SpriteRenderer>().sprite.name;

                // If the names are not the same, then the frame is not mirrored
                //
                if (s1Name != s2Name)
                {
                    matched = false;
                }
            }

            // If all the sprites match, loop across the frames list to find the frame to be 
            // deleted. This should be the same as the frame that has just been checked, i.e.
            // hitFrame.
            //
            if (matched == true)
            {
                hitFrame.SetActive(false);
                ObjectPooling.instance.CheckRespawnFrames();

                // Play a particle system for success or failure. The particle system to be
                // instantiated is a pulic property set in the Inspector.
                //
                useCorrectParticle();

                // Play a success sound
                //
                playCorrectAudio();
                onScoreUpdate();
            }
            else
            {
                // Play the fail particle system
                useWrongParticle();

                // Play a fail sound
                //
                playWrongAudio();
            }
        }
    }
}