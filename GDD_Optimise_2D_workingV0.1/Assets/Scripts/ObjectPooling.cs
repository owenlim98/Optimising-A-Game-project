using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling instance = null;

    private void Awake()
    {
        instance = this;
    }

    public List<GameObject> frames;                // Created frames are stored in this list
    public GameObject framePrefab;                  // Used to create a new frame of sprites
    public static float frameWidth;                       // The width of a frame of sprites
    public List<GameObject> pooledObjects;
    public int amountToPool;

    // When a frame moves left, once it reaches leftExtent it will be destroyed
    //
    public float leftExtent;

    // When frames are created at the start of the game, they are placed from left
    // to right. When the frames reach rightExtent on the X axis, no more frames
    // are created, and the game can start.
    //
    public float rightExtent;

    // When a new frame is created, it must be placed behind the rightmost, or end, frame. endFrame
    // is the rightmost frame. This is updated every time a new frame is created and placed at the end.
    public static GameObject endFrame;

    public float gameWidth { get; private set; }    // Width of the game view
    public float gameHeight { get; private set; }   // Height of the game view

    // Start is called before the first frame update
    void Start()
    {
        CreateStartingFrames();
    }

    public GameObject Pooling()
    {
        foreach(GameObject frame in frames)
        {
            if(frame.activeInHierarchy == false)
            {
                return frame;
            }
        }
        return null;
    }

    void CreateStartingFrames()
    {
        // Find the height and width of the game view in world units
        //
        gameHeight = Camera.main.orthographicSize * 2f;
        gameWidth = Camera.main.aspect * gameHeight;

        // Calculate the X axis values for frame removal and the positioning of new frames
        //
        leftExtent = -gameWidth * 1f;
        rightExtent = gameWidth * 1f;

        frames = new List<GameObject>();

        int n = 0;
        float currX = leftExtent;
        while (currX < rightExtent)
        {
            Vector3 currPos = new Vector3(currX, 0f, 0f);
            GameObject frame = Instantiate(framePrefab);
            frame.name = "Frame_" + n++;

            frame.transform.position = currPos;

            // The box collider has been sized to fit the boundaries of the sprite in the Frame prefab.
            //
            frameWidth = frame.GetComponent<BoxCollider>().bounds.size.x;

            // The gap between one frame and the next: |     |<-gap->|     |
            float gap = frameWidth * 0.1f;

            currX += frameWidth + gap;
            frames.Add(frame);
        }
        endFrame = frames[frames.Count - 1];  // endFrame is the last frame added to the list
    }

    // Animate the frames from right to left. Each frame has a RigidBody component, which is
    // used to move it across the screen. RigidBody is used instead of RigidBody2D because 
    // 2D physics sometimes has bugs. Each frame also has a BoxCollider on it. The BoxCollider
    // is used to check for player interaction.
    //
    public void MoveFrames()
    {
        //Debug.Log(frames.Count);
        // Loop over all the frames in the frames list
        //
        foreach (GameObject frame in frames)
        {
            // We need to have a reference to the GameManager object so that we can access
            // the animation speed
            //
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            //Debug.Log(frame.name);

            // If a mirrored frame is selected, it will be destroyed. This leaves a gap in the 
            // row of frames. All the frames to the right should speed up to close the gap. So
            // for each frame, find the distance to the frame on its left. If the distance is
            // greater than half a framewidth, increase the frame's animation speed.
            //
            float distance = GetDistanceToNeighbour(frame);

            if (distance > frameWidth * 0.5f)
            {
                frame.GetComponent<Rigidbody>().velocity = new Vector3(gm.speed * 10f, 0f, 0f);
            }
            else
            {
                frame.GetComponent<Rigidbody>().velocity = new Vector3(gm.speed, 0f, 0f);
            }
        }

        // Now check if any of the frames have gone past leftExtent on the X axis. If so, they
        // must be destroyed and a new frame created at the end of the row of frames
        //
        CheckRespawnFrames();
    }

    // Returns the distance from a frame to its left neighbouring frame. This is done by casting a
    // ray from the frame's position towards the left. The distance between the frames' x position
    // and the x position of the hit object is calculated.
    //
    float GetDistanceToNeighbour(GameObject frame)
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Get the width of the frame
        //
        float frameWidth = frame.gameObject.GetComponent<BoxCollider>().bounds.size.x;

        float distance = 0f;

        // Set up the ray parameters
        //
        Vector3 pos = frame.gameObject.transform.position;
        Vector3 raypos = new Vector3(pos.x - frameWidth, 0f, pos.z);
        RaycastHit hit;

        // Cast the ray. Check for a collision, then check if the collided object's name 
        // starts with "Frame". If so, them set distance to the hit distance.
        //
        if (Physics.Raycast(raypos, Vector3.left, out hit, gameWidth * 2f))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name.StartsWith("Frame"))
                {
                    distance = hit.distance;
                }
            }
        }
        else
        {
            // There is no hit. This means that the frame has no left neighbour. In this
            // case, check if it is the leftmost frame. If it is, and if there is a gap
            // between the frame and the left side of the game view, then the frame must 
            // be speeded up to close the gap. So return a large value for distance, in
            // this case twice the width of the game view.
            //
            // HINT: Why is this function doing two different things?
            //
            bool leftFrame = true;
            foreach (GameObject f in frames)
            {
                if (frame.gameObject.transform.position.x > f.gameObject.transform.position.x)
                {
                    leftFrame = false;
                }
            }
            if (leftFrame == true && frame.gameObject.transform.position.x > -gameWidth / 2f)
            {
                distance = gameWidth * 2f; ;
            }
        }
        return distance;
    }

    // Check if a frame needs to be destroyed and a new one spawned to take its place. 
    // Loop through all the frames in the frames list. If any frame's X position is 
    // less than leftExtent it must be destroyed. We must loop backwards over the list,
    // since C# doesn't allow a list element to be removed when iterating over it from
    // front to back (this is because all the remaining list elements will be shuffled
    // up in the list, which messes up the iteration). If iterating from back to front,
    // the iteration will not be affected.
    //
    public void CheckRespawnFrames()
    {
        GameObject checkFrame = null;

        // Set the X position for the new frame that wil be created
        //
        float newX = endFrame.transform.position.x + frameWidth;

        float gap = frameWidth * 0.1f;

        // Set the new frame's position so that it is at the end of the row of 
        // frames, with a gap between the new frame and the current end frame
        // (the new frame will become the current end frame in the next line)
        //
        Vector3 spawnPos = new Vector3(newX + gap, 0f, 0f);

        checkFrame = Pooling();

        if(checkFrame != null)
        {
            // Update endFrame to be the new frame
            //
            checkFrame.transform.position = spawnPos;
            checkFrame.SetActive(true);
            endFrame = checkFrame;

        }
    }
}