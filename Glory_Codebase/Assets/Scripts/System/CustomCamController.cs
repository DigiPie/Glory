using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCamController : MonoBehaviour
{
    public Transform camTarget;
    public float maxShakeAmount = 1.2f;
    private float shakeDuration = 0f;
    private float shakeAmount;

    private Vector3 targetPos;
    private float chaseSpeed = 0.2f;
    private float chaseDeadzone = 0.05f; // Only start chasing target is chaseDeadzone distance away
    private bool startChase = false;

    void FixedUpdate()
    {
        if (shakeDuration > 0)
        {
            transform.position = new Vector3(
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
            targetPos = new Vector3(camTarget.position.x, camTarget.position.y, -10);

            // If player is chaseDeadzone distance away from camera, start chase
            if (Vector3.Distance(transform.position, targetPos) > chaseDeadzone)
            {
                startChase = true;
            }

            if (startChase)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, chaseSpeed); // Chase

                // If player is minimal distance from camera, stop chase
                if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                {
                    startChase = false;
                }
            }
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

    public void StopShake()
    {
        shakeAmount = 0;
        shakeDuration = 0;
    }
}