using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame : MonoBehaviour
{

    [System.Serializable]
    public class AnimalAndImage
    {
        public Sprite sprites;

        public AnimalType animalType;
    }

    /// <summary>
    /// Where the position X of the left side of the camera's border is at.
    /// Used to ensure that the left-most frame speeds up when it is not the left-side of the camera border.
    /// </summary>
    public static float LeftBorderPosition;

    public GameObject frameTop;

    public GameObject frameBottom;

    public AnimalAndImage[] animalImagePairs;

    public float frameSpeed;

    public float speedMultiplier;

    public float catchupDistance;

    /// <summary>
    /// The neighbouring frame on the left of this frame.
    /// </summary>
    public Frame LeftNeighbour { get; set; }

    private BoxCollider2D frameCollider;

    private Animal[] bottomFrameAnimals;
    private Animal[] topFrameAnimals;

    private void Awake()
    {
        bottomFrameAnimals = frameTop.GetComponentsInChildren<Animal>();
        topFrameAnimals = frameBottom.GetComponentsInChildren<Animal>();
    }

    private void Start()
    {
        frameCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        UpdateMovement(Time.deltaTime);
    }

    private void UpdateMovement(float deltaTime)
    {
        float finalSpeed = frameSpeed;

        if (LeftNeighbourIsAssignedAndActive())
        {
            SetFinalSpeedBasedOnDistanceToLeftNeighbour(ref finalSpeed);
        }
        // Left-most frame is not at the left side of the camera yet; Speedup is needed.
        else if (transform.position.x > LeftBorderPosition)
        {
            finalSpeed *= speedMultiplier;
        }
        transform.position += Vector3.left * finalSpeed * deltaTime;
    }

    void SetFinalSpeedBasedOnDistanceToLeftNeighbour(ref float finalSpeed)
    {
        float distanceToNeighbour = GetShortestDistanceToLeftNeighbour();

        // Speedup if there is a big gap between this frame and the neighbouring frame.
        if (distanceToNeighbour >= catchupDistance)
        {
            finalSpeed *= speedMultiplier;
        }
    }

    float GetShortestDistanceToLeftNeighbour()
    {
        // Get the left border of the frame.
        float leftPoint = transform.position.x;
        leftPoint -= frameCollider.size.x / 2f;

        // Get the right-most x point of neighbouring frame.
        float neighbourRightPoint = LeftNeighbour.transform.position.x;
        neighbourRightPoint += LeftNeighbour.frameCollider.size.x / 2f;

        return leftPoint - neighbourRightPoint;
    }

    bool LeftNeighbourIsAssignedAndActive()
    {
        if (LeftNeighbour != null)
        {
            if (LeftNeighbour.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Generate animals randomly in the frame.
    /// </summary>
    /// <param name="mirror">True to make sure the top and bottom section of the frame mirrors each other.</param>
    public void GenerateAnimalsInFrame(bool mirror)
    {
        if (mirror)
        {
            PopulateFrameSectionsWithMirror();
        }
        else
        {
            RandomlyPopulateFrameWithAnimals();
        }
    }

    private void RandomlyPopulateFrameWithAnimals()
    {
        AnimalAndImage[] topanimalLayout = CreateLayout();
        FillTopFrame(topanimalLayout);

        AnimalAndImage[] bottomanimalLayout = CreateLayout();
        FillBottomFrame(bottomanimalLayout);
    }

    private void PopulateFrameSectionsWithMirror()
    {
        AnimalAndImage[] animalLayout = CreateLayout();

        FillTopFrame(animalLayout);
        FillBottomFrame(animalLayout);
    }

    AnimalAndImage[] CreateLayout()
    {
        AnimalAndImage[] layout = new AnimalAndImage[topFrameAnimals.Length];
        // Fill the layout with generated animals.
        for (int i = 0; i < layout.Length; ++i)
        {
            AnimalAndImage generatedAnimal = animalImagePairs[Random.Range(0, animalImagePairs.Length)];

            layout[i] = new AnimalAndImage
            {
                sprites = generatedAnimal.sprites,
                animalType = generatedAnimal.animalType
            };
        }
        return layout;
    }

    void FillTopFrame(AnimalAndImage[] layout)
    {
        for (int i = 0; i < topFrameAnimals.Length; ++i)
        {
            topFrameAnimals[i].AnimalType = layout[i].animalType;
            topFrameAnimals[i].ChangeSprite(layout[i].sprites);
        }
    }

    void FillBottomFrame(AnimalAndImage[] layout)
    {
        for (int i = 0; i < bottomFrameAnimals.Length; ++i)
        {
            bottomFrameAnimals[i].AnimalType = layout[i].animalType;
            bottomFrameAnimals[i].ChangeSprite(layout[i].sprites);
        }
    }

    /// <summary>
    /// Determines if the top and bottom section of this frame mirrors each other.
    /// </summary>
    /// <returns>True if the top/bottom of the frame mirrors</returns>
    public bool MirroredFrameSections()
    {
        // Loop through both sections and check if each of the animal type matches.
        for (int i = 0; i < topFrameAnimals.Length; ++i)
        {
            if (topFrameAnimals[i].AnimalType != bottomFrameAnimals[i].AnimalType)
            {
                return false;
            }
        }
        return true;
    }
}
