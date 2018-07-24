using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Vector2 dirV; // Direction of melee projectile

    public float cooldown = 1f; // Attack cooldown
    public float damage = 10;
    public float lifespan = 0.5f; // Lifespan of melee projectile
    public float speed = 0.1f; // Speed of melee projectile

    public void Setup(Vector2 dir)
    {
        Destroy(gameObject, lifespan);
        dirV = speed * dir;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && dir.x < 0)
            spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    void FixedUpdate()
    {
        transform.Translate(dirV.x, dirV.y, 0);
    }
}
