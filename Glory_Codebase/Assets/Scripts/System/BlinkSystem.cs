using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSystem : MonoBehaviour {
    private readonly Color halfVisible = new Color(1, 1, 1, 0.5f);

    private float miniBlinkEndTime;
    private float miniBlinkDuration = 0.15f;
    private float blinkEndTime;
    public float blinkDuration = 1.0f;
    private bool isBlinking = false;
    private bool isLighter = true;

	// Update is called once per frame
	void Update () {
        if (isBlinking)
        {
            if (Time.timeSinceLevelLoad > blinkEndTime)
            {
                isBlinking = false;
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                return;
            }
            
            if (Time.timeSinceLevelLoad > miniBlinkEndTime)
            {
                isLighter = !isLighter;
                this.gameObject.GetComponent<SpriteRenderer>().color = (isLighter) ? halfVisible : Color.white;
                miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
            }
        }
    }

    public void StartBlink() {
        isBlinking = true;
        blinkEndTime = Time.timeSinceLevelLoad + blinkDuration;

        isLighter = false;
        this.gameObject.GetComponent<SpriteRenderer>().color = (isLighter) ? halfVisible : Color.white;
        miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
    }

    public void StartBlink(float blinkDuration)
    {
        this.blinkDuration = blinkDuration;

        isBlinking = true;
        blinkEndTime = Time.timeSinceLevelLoad + blinkDuration;

        isLighter = false;
        this.gameObject.GetComponent<SpriteRenderer>().color = (isLighter) ? halfVisible : Color.white;
        miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
    }
}
