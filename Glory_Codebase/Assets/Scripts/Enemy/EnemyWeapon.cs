using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {
    private Vector2 dirV; // Direction of melee projectile

    public float cooldown = 1f; // Attack cooldown
    public float damage = 10;
    public float lifespan = 0.5f; // Lifespan of melee projectile
    public float speed = 0.1f; // Speed of melee projectile

    // Use this for initialization
    void Start()
    {
        Object.Destroy(this.gameObject, lifespan);
    }

    public void Setup(Vector2 dir)
    {
        dirV = speed * dir;
    }

    void FixedUpdate()
    {
        transform.Translate(dirV.x, dirV.y, 0);
    }
}
