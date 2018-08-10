using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCamController : MonoBehaviour
{
    public Transform childCamera; // The actual camera which is a child of this object, a camera container
    public Transform cameraTarget;

    // Chase handled by this object
    public float maxChaseSpeed = 0.2f;
    public float chaseAcceleration = 0.15f;
    public float startChaseFactor = 0.03f; // Only start chasing target when this far from target
    public float stopChaseFactor = 0.01f; // Only stop chasing target when this close to target
    private Vector3 targetPos;
    private float chaseSpeed = 0f;
    private bool isChasing = false;

    // Shake handled by the camera, a child of this object
    public float maxShakeAmount = 1.0f;
    public float shakeReductionSpeed = 0.2f;
    private float shakeAmount = 0f;
    private bool isShaking = false;

    private void Start()
    {
        // Get target position, the player's position but -10 on Z-axis
        targetPos = new Vector3(cameraTarget.position.x, 1, -10);
        transform.position = targetPos;
    }

    void FixedUpdate()
    {
        HandleCameraChase(); // This object, the cameraContainer will chase the target.
        HandleCameraShake(); // The camera, a child of this object, will execute the camera shake.
    }

    void HandleCameraChase()
    {
        // Get target position, the player's position but -10 on Z-axis
        targetPos = new Vector3(cameraTarget.position.x, 1, -10);

        // Start chase
        if (isChasing)
        {
            // Accelerate chase speed
            if (chaseSpeed < maxChaseSpeed)
            {
                chaseSpeed += Time.fixedDeltaTime * chaseAcceleration;

                if (chaseSpeed > maxChaseSpeed)
                {
                    chaseSpeed = maxChaseSpeed;
                }
            }

            // Move towards target position
            transform.position = Vector3.Lerp(transform.position, targetPos, chaseSpeed);

            // If this object is less than stopChaseFactor distance from target
            if (Vector3.Distance(transform.position, targetPos) < stopChaseFactor)
            {
                // Stop chasing
                isChasing = false;
                chaseSpeed = 0;
            }
        }
        // Else if not chasing and if this object is more than startChaseFactor distance from target
        else if(Vector3.Distance(transform.position, targetPos) > startChaseFactor)
        {
            // Start chasing
            isChasing = true;
        }
    }

    void HandleCameraShake()
    {
        // Update shaking status
        isShaking = shakeAmount > 0;
        if (isShaking)
        {
            shakeAmount -= Time.deltaTime;
        }

        childCamera.position = transform.position;

        if (isShaking) {
            childCamera.Translate(Random.insideUnitSphere * shakeAmount);
        }
    }

    // Shaking is stackable; if already shaking, and this function is called, shake even more.
    public void Shake(float addShakeAmount)
    {
        shakeAmount += addShakeAmount;

        if (shakeAmount > maxShakeAmount)
        {
            shakeAmount = maxShakeAmount;
        }
    }

    public void StopShake()
    {
        shakeAmount = 0;
    }
}