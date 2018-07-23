using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class DamageCounter : MonoBehaviour {
    private TextMeshPro textMeshPro;
    private float transparency = 1.0f;

	// Use this for initialization
	void Start () {
        textMeshPro = GetComponent<TextMeshPro>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Translate(0, 0.8f * Time.deltaTime, 0);
        transparency -= Time.deltaTime;
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, transparency);

        if (transparency < 0)
            Destroy(gameObject);
    }
}
