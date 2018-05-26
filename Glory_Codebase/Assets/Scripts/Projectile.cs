using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public Vector2 dir;

    public float damage = 10;
    public float lifespan = 1.0f;
    public float speed = 10;

	// Use this for initialization
	void Start () {
        Object.Destroy(this.gameObject, lifespan);
    }

    void Update()
    {
        this.transform.Translate(dir);
    }
}
