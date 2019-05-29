using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

public class FrameManager : Singleton<FrameManager>
{
    public bool framesIsAlwaysMirrored;

    public GameObject framePrefab;

    // When a frame reaches leftExtent it will be destroyed
    private float leftExtent;

    // The width of a frame of sprites
    private float frameWidth;

    // Active frames are stored in this list.
    private List<Frame> frames;

    // When a new frame is created, it must be placed behind the rightmost, or end, frame. endFrame
    // is the rightmost frame. This is updated every time a new frame is created and placed at the end.
    private Frame endFrame;

    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;

        Application.targetFrameRate = 300;

        frames = new List<Frame>();
        endFrame = null;

        float gameHeight = camera.orthographicSize * 1.25f;
        float gameWidth = camera.aspect * gameHeight;

        Frame.LeftBorderPosition = -gameWidth * 1.25f;

        leftExtent = -gameWidth * 2f;
        float rightExtent = gameWidth * 2f;

        float currX = leftExtent;
        while (currX < rightExtent)
        {
            Frame frame = FrameFromObjectPool();

            frame.transform.position = new Vector3(currX, 0f, 0f);

            frame.LeftNeighbour = endFrame;
            endFrame = frame;

            frames.Add(frame);

            frameWidth = frame.GetComponent<BoxCollider2D>().bounds.size.x;

            float gap = frameWidth * 0.1f;
            currX += frameWidth + gap;
        }
        endFrame = frames[frames.Count - 1];
    }

    /// <summary>
    /// Create a new frame and place it at the end of the current existing frames.
    /// </summary>
    /// <returns></returns>
    private Frame CreateFrameAtEnd()
    {
        Frame newFrame = FrameFromObjectPool();

        if (framesIsAlwaysMirrored)
        {
            newFrame.GenerateAnimalsInFrame(true);
        }
        else
        {
            // Make a chance to generate a mirrored frame.
            bool mirrored = Random.Range(0f, 1f) >= 0.5f;
            newFrame.GenerateAnimalsInFrame(mirrored);
        }

        // Set the new frame position.
        float newX = endFrame.transform.position.x + frameWidth;
        float gap = frameWidth * 0.1f;
        newFrame.transform.position = new Vector3(newX + gap, 0f, 0f);

        // Add the new frame to the list of frames.
        newFrame.LeftNeighbour = endFrame;
        endFrame = newFrame;
        frames.Add(newFrame);

        return newFrame;
    }

    /// <summary>
    /// Fetch a frame gameObject from the object pool. If one doesnt exists, it creates one.
    /// </summary>
    /// <returns></returns>
    private Frame FrameFromObjectPool()
    {
        bool existsInPool = ObjectPoolManager.Instance.FrameObjectPool.TryFetchObjectFromPool(out GameObject frameGameObject);

        if (!existsInPool)
        {
            frameGameObject = Instantiate(framePrefab);
            ObjectPoolManager.Instance.FrameObjectPool.AddObjectToPool(frameGameObject);
        }
        else
        {
            frameGameObject.SetActive(true);
        }
        return frameGameObject.GetComponent<Frame>();
    }

    // Update is called once per frame
    void Update()
    {
        SetFrameInActive();

        // Check if the user clicked on a Frame.
        if (UserClickedFrame(out Frame hitFrame))
        {
            WhenUserClickOnFrame(hitFrame);
        }
    }

    /// <summary>
    /// Remove frames that are considered out of bound (Too far left on the screen)
    /// </summary>
    private void SetFrameInActive()
    {
        foreach (var frame in frames.ToArray())
        {
            // Check if the frame's X position is less then leftExtent.
            // If yes, it will be removed and a new frame will be generated at the end.
            if (frame.transform.position.x < leftExtent)
            {
                CreateFrameAtEnd();

                frames.Remove(frame);
                frame.gameObject.SetActive(false);
                frame.LeftNeighbour = null;
                // The frame that is now considered at the left-end has no neighbours
                frames[0].LeftNeighbour = null;
            }
        }
    }

    private bool UserClickedFrame(out Frame hitFrame)
    {
        GameObject frameObj = null;

        // Find the gameobject that the user has clicked on.
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 raypos = GetUserMousePosition();

            RaycastHit2D hit = Physics2D.Raycast(raypos, Vector2.zero);
            if (hit.collider != null)
            {
                frameObj = hit.collider.gameObject;
            }
        }

        // If the user clicked on an object, and the object has a frame component
        // then the user has clicked on a frame.
        if (frameObj != null)
        {
            hitFrame = frameObj.GetComponent<Frame>();
            if (hitFrame != null)
            {
                return true;
            }
        }

        hitFrame = null;
        return false;
    }

    /// <summary>
    /// Checks if the user clicked on a frame, and handles it accordingly
    /// </summary>
    private void WhenUserClickOnFrame(Frame clickedFrame)
    {
        bool matched = clickedFrame.MirroredFrameSections();

        if (matched == true)
        {
            IfFrameMatched(clickedFrame);
        }
        else
        {
            // Play failed particle.
            Vector3 clickPos = GetUserMousePosition();
            ParticleSystemManager.Instance.PlayFailedParticle(clickPos);

            AudioManager.Instance.PlayFailureAudio();
        }
    }

    private void IfFrameMatched(Frame matchedFrame)
    {

        CreateFrameAtEnd();

        if (TryGetNeighbouringFrame(matchedFrame, out Frame leftNeighbour, out Frame rightNeighbour))
        {
            rightNeighbour.LeftNeighbour = leftNeighbour;
        }
        else if (matchedFrame == frames[0])
        {
            frames[1].LeftNeighbour = null;
        }

        // Remove the matched frame.
        matchedFrame.LeftNeighbour = null;
        frames.Remove(matchedFrame);
        matchedFrame.gameObject.SetActive(false);

        // Play a success particle at the user's position
        Vector3 clickPos = GetUserMousePosition();
        ParticleSystemManager.Instance.PlaySuccessParticle(clickPos);

        AudioManager.Instance.PlaySuccessAudio();

        // Increment score, since the player has scored a point
        GameManager.Instance.AddScore();
    }

    private bool TryGetNeighbouringFrame(Frame frame, out Frame leftFrame, out Frame rightFrame)
    {
        try
        {
            int i = frames.IndexOf(frame);

            leftFrame = frames[i - 1];
            rightFrame = frames[i + 1];
            return true;
        }
        catch (System.IndexOutOfRangeException)
        {
            leftFrame = rightFrame = null;
            return false;
        }
        catch (System.ArgumentOutOfRangeException)
        {
            leftFrame = rightFrame = null;
            return false;
        }
    }


    private Vector3 GetUserMousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
