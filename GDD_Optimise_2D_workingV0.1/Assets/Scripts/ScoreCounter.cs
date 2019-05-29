using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreCounter : MonoBehaviour
{
    private GameManager gameManager;

    public Text scoreText;                  // Increments when a mirrored frame is selected

    public int score = 0;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        gameManager.onScoreUpdate += UpdateScore;
    }

    // Update the Score text in the UI
    //
    public void UpdateScore()
    {
        Debug.Log("update score");
        score++;
        scoreText.text = "Score: " + score;
    }
}
