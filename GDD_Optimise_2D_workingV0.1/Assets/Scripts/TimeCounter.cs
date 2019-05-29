using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeCounter : MonoBehaviour
{
    public Text timeText;                   // Updates every second

    int seconds = 0;
    float counterTime = 0;                  // Used for the Seconds UI display

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
    }

    // Update the Seconds text in the UI
    //
    private void UpdateTime()
    {
        counterTime += Time.deltaTime;

        if (counterTime > 1f)
        {
            timeText.text = "Seconds: " + ++seconds;
            counterTime = 0;
        }
    }
}
