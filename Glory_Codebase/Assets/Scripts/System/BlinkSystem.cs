using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSystem : MonoBehaviour {
    private float miniBlinkEndTime;
    private float miniBlinkDuration = 0.15f;
    private float blinkEndTime;
    public float blinkDuration = 1.0f;
    private bool isBlinking = false;
    private bool isVisible = true;
	
	// Update is called once per frame
	void Update () {
        if (isBlinking)
        {
            if (Time.timeSinceLevelLoad > blinkEndTime)
            {
                isBlinking = false;
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                return;
            }
            
            if (Time.timeSinceLevelLoad > miniBlinkEndTime)
            {
                isVisible = !isVisible;
                this.gameObject.GetComponent<SpriteRenderer>().enabled = isVisible;
                miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
            }
        }
    }

    public void StartBlink() {
        isBlinking = true;
        blinkEndTime = Time.timeSinceLevelLoad + blinkDuration;

        isVisible = false;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = isVisible;
        miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
    }

    public void StartBlink(float blinkDuration)
    {
        this.blinkDuration = blinkDuration;

        isBlinking = true;
        blinkEndTime = Time.timeSinceLevelLoad + blinkDuration;

        isVisible = false;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = isVisible;
        miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
    }
}
