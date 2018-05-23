using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour {
    private HealthSystem healthSystem;

	// Use this for initialization
	void Start () {
		healthSystem = GetComponent<HealthSystem>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If colliding with player
        if (collision.gameObject.layer == 11)
        {
            healthSystem.DeductHealth(10);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {

    }
}
