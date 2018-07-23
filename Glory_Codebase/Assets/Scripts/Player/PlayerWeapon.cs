using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {
    private Vector2 dirV; // Direction of melee projectile
    private float damage = 10;
    private float stunDuration = 0.0f; // Stun duration on enemy
    private float blinkDuration = 1.0f; // Blink duration on enemy
    public float lifespan = 0.2f; // Lifespan of melee projectile
    public float speed = 0.05f; // Speed of melee projectile

    public void Setup(Vector2 dir)
    {
        dirV = speed * dir;
        Destroy(gameObject, lifespan);
    }

    public void Setup(Vector2 dir, float damage)
    {
        dirV = speed * dir;
        Destroy(gameObject, lifespan);
        this.damage = damage;
    }

    public void Setup(Vector2 dir, float damage, float stunDuration)
    {
        dirV = speed * dir;
        Destroy(gameObject, lifespan);
        this.damage = damage;
        this.stunDuration = stunDuration;
    }

    public void Setup(Vector2 dir, float damage, float stunDuration, float blinkDuration)
    {
        dirV = speed * dir;
        Destroy(gameObject, lifespan);
        this.damage = damage;
        this.stunDuration = stunDuration;
        this.blinkDuration = blinkDuration;
    }

    void FixedUpdate()
    {
        transform.Translate(dirV.x, dirV.y, 0);
    }

    public float getDamage()
    {
        return damage;
    }

    public float getStunDuration()
    {
        return stunDuration;
    }

    public float getBlinkDuration()
    {
        return blinkDuration;
    }
}
