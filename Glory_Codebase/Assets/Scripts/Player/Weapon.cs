using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    private Vector2 dirV; // Direction of melee projectile

    public float cooldown = 1f; // Attack cooldown
    public float damage = 10;
    public float criticalDamage = 15; // Every 3rd strike in a combo is a critical strike
    public float lifespan = 0.5f; // Lifespan of melee projectile
    public float speed = 0.1f; // Speed of melee projectile
    public float stunDuration = 0.0f; // Stun duration on enemy
    public float blinkDuration = 0.0f; // Blink duration on enemy

    // Use this for initialization
    void Start () {
        Object.Destroy(this.gameObject, lifespan);
    }

    public void Setup(bool isCriticalStrike, Vector2 dir)
    {
        if (isCriticalStrike)
        {
            damage = criticalDamage;
        }

        dirV = speed * dir;
    }

    void FixedUpdate()
    {
        transform.Translate(dirV.x, dirV.y, 0);
    }
}
