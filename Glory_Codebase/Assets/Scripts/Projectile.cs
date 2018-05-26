using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private Vector2 dirV;

    public float cooldown = 1f;
    public float damage = 10;
    public float lifespan = 0.5f;
    public float speed = 0.1f;

	// Use this for initialization
	void Start () {
        Object.Destroy(this.gameObject, lifespan);
    }

    public void SetDir(Vector2 dir)
    {
        dirV = speed * dir;
    }

    void FixedUpdate()
    {
        transform.Translate(dirV.x, dirV.y, 0);
    }
}
