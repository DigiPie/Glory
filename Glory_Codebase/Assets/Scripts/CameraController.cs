using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 originalPosition;
    float shakeAmount = 0;
    public Camera mainCamera;
    public Transform camTarget;
    private bool isShaking = false;

    void Update()
    {
        mainCamera.transform.position = new Vector3(camTarget.position.x, camTarget.position.y, -10);
    }

    public void Shake(float shakeAmount)
    {
        if (isShaking)
        {
            return;
        }

        this.shakeAmount = shakeAmount;
        isShaking = true;
        InvokeRepeating("StartShake", 0, .01f);
        Invoke("EndShake", 0.3f);
    }

    void StartShake()
    {
        if (shakeAmount > 0)
        {
            float quakeAmt = Random.value * shakeAmount * 2 - shakeAmount;
            Vector3 pp = mainCamera.transform.position;
            pp.y += quakeAmt; // can also add to x and/or z
            mainCamera.transform.position = pp;
        }
    }

    void EndShake()
    {
        CancelInvoke("StartShake");
        mainCamera.transform.position = new Vector3(camTarget.position.x, camTarget.position.y, -10);
        isShaking = false;
    }
}