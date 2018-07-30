using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuff : MonoBehaviour {
    private SpriteRenderer rend;
    public float lifespan = 4f; // Lifespan of buff
    public float invulDuration = 4f;
    public float fasterSpeedDuration = 4f;
    public float speedMultiplier = 1.5f;

    private bool isFadingIn = true;
    private bool isFadingOut = false;
    private float opacity = 0f;
    private float fadeInSpeed = 5.0f;
    private float fadeOutSpeed = 3.0f;

    public void Setup()
    {
        rend = GetComponent<SpriteRenderer>();
        Invoke("StartDestroy", lifespan);
    }

    private void Update()
    {
        if (isFadingOut)
        {
            if (opacity > 0.1f)
            {
                opacity -= fadeOutSpeed * Time.fixedDeltaTime;
                rend.color = new Color(1.0f, 1.0f, 1.0f, opacity);
            }
            else
            {
                Destroy(gameObject);
            }

            return;
        }

        if (isFadingIn)
        {
            if (opacity < 1.0f)
            {
                opacity += fadeInSpeed * Time.fixedDeltaTime;
            }
            else
            {
                opacity = 1.0f;
                isFadingIn = false;
            }

            rend.color = new Color(1.0f, 1.0f, 1.0f, opacity);
        }
    }

    private void StartDestroy()
    {
        if (rend == null)
        {
            Destroy(gameObject);
            return;
        }

        opacity = 1.0f;

        isFadingOut = true;
    }

    public void SetToPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
