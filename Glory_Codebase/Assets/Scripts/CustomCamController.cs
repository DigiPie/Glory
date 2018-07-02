using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCamController : MonoBehaviour
{
    Vector3 originalPosition;
    float shakeAmount = 0;
    public Transform camTarget;
    private bool isShaking = false;

    void Update()
    {
<<<<<<< HEAD:Glory_Codebase/Assets/Scripts/CameraController.cs
        //mainCamera.transform.position = new Vector3(camTarget.position.x, camTarget.position.y, -10);
=======
        transform.position = new Vector3(camTarget.position.x, camTarget.position.y, -10);
>>>>>>> e408c9b15858925685a85c0ffe3c82eebc763e1c:Glory_Codebase/Assets/Scripts/CustomCamController.cs
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
            Vector3 pp = transform.position;
            pp.y += quakeAmt; // can also add to x and/or z
            transform.position = pp;
        }
    }

    void EndShake()
    {
        CancelInvoke("StartShake");
        transform.position = new Vector3(camTarget.position.x, camTarget.position.y, -10);
        isShaking = false;
    }
}