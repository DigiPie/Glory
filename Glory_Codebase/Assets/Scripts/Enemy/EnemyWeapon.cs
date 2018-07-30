using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {
    private SpriteRenderer rend;
    private Vector2 dirV; // Direction of melee projectile

    public float cooldown = 1f; // Attack cooldown
    public float damage = 10;
    public float lifespan = 0.5f; // Lifespan of melee projectile
    public float speed = 0.1f; // Speed of melee projectile

    private bool isFadingIn = true;
    private bool isFadingOut = false;
    private float opacity = 0f;
    private float fadeInSpeed = 6.0f;
    private float fadeOutSpeed = 6.0f;

    public void Setup(Vector2 dir)
    {
        rend = GetComponent<SpriteRenderer>();
        rend.color = new Color(1.0f, 1.0f, 1.0f, 0f);

        Invoke("StartDestroy", lifespan);
        dirV = speed * dir;

        rend = GetComponent<SpriteRenderer>();
        if (rend != null && dir.x < 0)
            rend.flipX = !rend.flipX;
    }

    public void StartDestroy()
    {
        GetComponent<Collider2D>().enabled = false;

        if (rend == null)
        {
            Destroy(gameObject);
            return;
        }

        opacity = 1.0f;

        isFadingOut = true;
    }

    void FixedUpdate()
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
        }
        else if (isFadingIn)
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

        transform.Translate(dirV.x, dirV.y, 0);
    }
}
