using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class DamageCounter : MonoBehaviour {
    private readonly float fadeSpeed = 0.6f;
    private TextMeshPro textMeshPro;
    private float transparency = 1.0f;
    
	// Use this for initialization
	void Start () {
        textMeshPro = GetComponent<TextMeshPro>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Translate(0, fadeSpeed * Time.deltaTime, 0);
        transparency -= Time.deltaTime;
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, transparency);

        if (transparency < 0.1)
            Destroy(gameObject);
    }
}
