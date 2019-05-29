using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public AnimalType animalType;

    public float rotationSpeed = 2.5f;

    public AnimalType AnimalType 
    {
        get => animalType;
        set => animalType = value;
    }


    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, rotationSpeed));
    }

    public void ChangeSprite(Sprite sprite)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        spriteRenderer.sprite = sprite;
    }
}
