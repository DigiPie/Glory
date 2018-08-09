using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour {

    public Overlay overlay;
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Exit"))
        {
            overlay.HideCredits();
        }
    }
}
