using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    private Vector2 dirV;

    public float cooldown = 1f;
    public float damage = 10;
    public float criticalDamage = 15; // Every 3rd strike in a combo is a critical strike
    public float lifespan = 0.5f;
    public float speed = 0.1f;

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
