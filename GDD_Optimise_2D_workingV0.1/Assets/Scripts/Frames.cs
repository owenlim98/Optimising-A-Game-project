using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frames : MonoBehaviour
{
    private Object[] prefabs;
    private float leftExtent;
    private List<GameObject> frames;

    // Start is called before the first frame update
    void Start()
    {
        frames = ObjectPooling.instance.frames;
        leftExtent = ObjectPooling.instance.leftExtent;
    }

    private void OnEnable()
    {
        // Load and store all the sprite prefabs in an array. The Resources.LoadAll() function
        // return an array of Objects.
        //
        prefabs = Resources.LoadAll("Prefabs/Sprites/animals", typeof(SpriteRenderer));
        CreateFrame();

    }

    // Update is called once per frame
    void Update()
    {
        CheckLeft();
    }

    // Creates a new frame. This is done when all the initial frames are created before the
    // game starts, and also when a frame is destroyed after it moves past leftExtent and a
    // new frame is created to take its place.
    //
    private void CreateFrame()
    {
        // Each frame has a set of top and bottom sprites. All the top and bottom sprites must 
        // match to score a point. The top and bottom sprites are children of a top or a
        // bottom empty parent gameobject, which in turn are children of the frame gameobject.
        // The structure is:
        // 
        // frame
        //     top
        //         s0 s1 s2 s3 s4 s5
        //     bottom
        //         s0 s1 s2 s3 s4 s5

        // Get a reference to the top parent
        //
        GameObject top = gameObject.transform.GetChild(0).gameObject;

        // Get the number of children of top. This is the number of sprites for the top part
        // of the frame. 
        //
        int numChildren = top.transform.childCount;

        // Loop across all the top children in the new frame
        //
        for (int i = 0; i < numChildren; i++)
        {
            // Each sprite gameobject in the new frame must be replaced with a new sprite gameobject.
            // Choose a random sprite from the prefabs array.
            //
            int randomIndex = Random.Range(0, prefabs.Length);
            SpriteRenderer sprites = prefabs[randomIndex] as SpriteRenderer;

            // Get a reference to the current sprite's transform. This is so that the newly created
            // sprite can be put in the same position before the existing sprite is destroyed.
            //
            GameObject t = top.transform.GetChild(i).gameObject;
            SpriteRenderer childSprite = t.GetComponent<SpriteRenderer>();

            childSprite.sprite = sprites.sprite;

            if (childSprite.sprite.name == "chicken")
            {
                t.transform.localScale = new Vector3(0.225f, 0.225f, 0f);
            }

            else if (childSprite.sprite.name == "puppy")
            {
                t.transform.localScale = new Vector3(0.225f, 0.225f, 0f);
            }

            else
            {
                t.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
            }
        }

        // Now we replace the default bottom sprites with new sprites
        //
        GameObject bottom = gameObject.transform.GetChild(1).gameObject;
        numChildren = bottom.transform.childCount;

        // Get a random number between 1 and 10. For integers, the Random.Range function is not
        // inclusive of the max value argument, which is why it is 11.
        //
        int rand = Random.Range(1, 11);

        // Check the value of the random number. If it is >=5, then set this new frame to be
        // mirrored (top and bottom sprites are the same). If rand is <=4, then set the frame
        // to be unmirrored. If the frame is mirrored, then the each bottom sprite will be a
        // copy of its corresponding top sprite.
        //
        bool mirror = rand > 4 ? true : false;

        if (mirror)
        {
            // Loop over all the bottom sprites
            //
            for (int i = 0; i < numChildren; i++)
            {
                GameObject b = bottom.transform.GetChild(i).gameObject;

                Sprite mirrorSprite = top.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite;

                SpriteRenderer mirrorToBotSprite = bottom.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();

                mirrorToBotSprite.sprite = mirrorSprite;

                if (mirrorToBotSprite.sprite.name == "chicken")
                {
                    b.transform.localScale = new Vector3(0.225f, 0.225f, 0f);
                }

                else if (mirrorToBotSprite.sprite.name == "puppy")
                {
                    b.transform.localScale = new Vector3(0.225f, 0.225f, 0f);
                }

                else
                {
                    b.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                }
            }
        }
        else
        {
            for (int i = 0; i < numChildren; i++)
            {
                // Each sprite gameobject in the new frame must be replaced with a new sprite gameobject.
                // Choose a random sprite from the prefabs array.
                //
                int randomIndex = Random.Range(0, prefabs.Length);
                SpriteRenderer sprites = prefabs[randomIndex] as SpriteRenderer;

                // Get a reference to the current sprite's transform. This is so that the newly created
                // sprite can be put in the same position before the existing sprite is destroyed.
                //
                GameObject b = bottom.transform.GetChild(i).gameObject;
                SpriteRenderer childSprite = b.GetComponent<SpriteRenderer>();

                childSprite.sprite = sprites.sprite;

                if (childSprite.sprite.name == "chicken")
                {
                    b.transform.localScale = new Vector3(0.225f, 0.225f, 0f);
                }

                else if (childSprite.sprite.name == "puppy")
                {
                    b.transform.localScale = new Vector3(0.225f, 0.225f, 0f);
                }

                else
                {
                    b.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
                }
            }
        }
    }

    // Check if the frame's X position is less then leftExtent. If yes, then it
    // set active false
    void CheckLeft()
    {
        if(transform.position.x < leftExtent)
        {
            gameObject.SetActive(false);
        }
    }
}