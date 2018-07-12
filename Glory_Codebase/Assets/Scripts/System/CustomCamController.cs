using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCamController : MonoBehaviour
{
    public Transform camTarget;
    public float shakeDuration = 0f;
    public float shakeAmount;
    public float maxShakeAmount = 0.4f;

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = new Vector3(
                camTarget.position.x + Random.insideUnitSphere.x * shakeAmount, 
                camTarget.position.y + Random.insideUnitSphere.y * shakeAmount, 
                -10);

            shakeDuration -= Time.deltaTime;

            // Shaking is reduced over time.
            shakeAmount -= Time.deltaTime * 0.2f;

            if (shakeAmount < 0)
            {
                shakeDuration = 0;
            }
        }
        else
        {
            transform.localPosition = new Vector3(camTarget.position.x, camTarget.position.y, -10);
        }
    }

    // Shaking is stackable; if already shaking, and this function is called, shake even more.
    public void Shake(float addShakeAmount, float shakeDuration)
    {
        if (shakeAmount > 0)
        {
            shakeAmount += addShakeAmount * 0.5f;
        }
        else
        {
            shakeAmount += addShakeAmount;
        }

        if (shakeAmount > maxShakeAmount)
        {
            shakeAmount = maxShakeAmount;
        }

        this.shakeDuration = shakeDuration;
    }
}