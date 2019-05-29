using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    public TextMeshProUGUI scoreText;               // Increments when a mirrored frame is selected

    public TextMeshProUGUI timeText;                // Updates every second

    private int seconds = 0;
    private float counterTime = 0;                  // Used for the Seconds UI display

    public int Score { get; private set; }

    private void Start()
    {
        Score = 0;
    }

    private void Update()
    {
        counterTime += Time.deltaTime;

        if (counterTime > 1f)
        {
            timeText.text = "Seconds: " + ++seconds;
            counterTime = 0;
        }
    }

    public void AddScore(int add = 1)
    {
        Score += add;
        scoreText.text = "Score: " + Score;
    }
}