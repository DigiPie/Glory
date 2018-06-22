using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Object.Destroy(this.gameObject, 0.5f);
    }
}
