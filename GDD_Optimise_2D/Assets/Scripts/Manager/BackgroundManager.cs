using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles toggling the visibilty of the oscillating backgrounds on certain intervals.
/// </summary>
public class BackgroundManager : Singleton<BackgroundManager>
{

    [System.Serializable]
    public class Background
    {
        public GameObject backgroundObj;
        [HideInInspector]
        public Vector2 positionA, positionB;
    }

    public float backgroundToggleIntervalTime = 3f;

    public Background[] oscillatingBG;

    public float oscillationDistance = 5f;

    private float backgroundIntervalTimer;

    private bool backgroundIsShowing;

    void Start()
    {
        // Set the distance where the background should oscillate between.
        foreach (var background in oscillatingBG)
        {
            background.positionA = new Vector2(background.backgroundObj.transform.position.x - oscillationDistance, background.backgroundObj.transform.position.y);
            background.positionB = new Vector2(background.backgroundObj.transform.position.x + oscillationDistance, background.backgroundObj.transform.position.y);
        }
        ShowBackground(false);
        backgroundIntervalTimer = 0f;
        backgroundIsShowing = false;
    }

    void Update()
    {
        backgroundIntervalTimer += Time.deltaTime;

        BackgroundTimer();

        if (backgroundIsShowing)
        {
            OscillateBG();
        }
    }

    private void OscillateBG()
    {
        foreach (var background in oscillatingBG)
        {
            float time = Mathf.PingPong(Time.time * 1f, 1);
            background.backgroundObj.transform.position = Vector3.Lerp(background.positionA, background.positionB, time);
        }
    }

    private void BackgroundTimer()
    {
        if (backgroundIntervalTimer > backgroundToggleIntervalTime)
        {
            backgroundIntervalTimer = 0f;
            backgroundIsShowing = !backgroundIsShowing;

            ShowBackground(backgroundIsShowing);
        }
    }

    private void ShowBackground(bool show)
    {
        foreach (var background in oscillatingBG)
        {
            background.backgroundObj.SetActive(show);
        }
    }
}
