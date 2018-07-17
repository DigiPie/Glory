using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSystem : MonoBehaviour {
    public SpriteRenderer sprite;
    public float blinkDuration = 1.0f;

    private readonly Color halfVisible = new Color(1, 1, 1, 0.5f);
    private float miniBlinkEndTime;
    private float miniBlinkDuration = 0.15f;
    private float blinkEndTime;
    private bool isBlinking = false;
    private bool isLighter = true;

	// Update is called once per frame
	void Update () {
        if (isBlinking)
        {
            if (Time.timeSinceLevelLoad > blinkEndTime)
            {
                isBlinking = false;
                sprite.color = Color.white;
                return;
            }
            
            if (Time.timeSinceLevelLoad > miniBlinkEndTime)
            {
                isLighter = !isLighter;
                sprite.color = (isLighter) ? halfVisible : Color.white;
                miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
            }
        }
    }

    public void StartBlink() {
        isBlinking = true;
        blinkEndTime = Time.timeSinceLevelLoad + blinkDuration;

        isLighter = false;
        sprite.color = (isLighter) ? halfVisible : Color.white;
        miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
    }

    public void StartBlink(float blinkDuration)
    {
        this.blinkDuration = blinkDuration;

        isBlinking = true;
        blinkEndTime = Time.timeSinceLevelLoad + blinkDuration;

        isLighter = false;
        sprite.color = (isLighter) ? halfVisible : Color.white;
        miniBlinkEndTime = Time.timeSinceLevelLoad + miniBlinkDuration;
    }
}
