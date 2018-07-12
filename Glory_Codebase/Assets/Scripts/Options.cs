using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour {

    public static bool inOptionsMenu;
    public GameObject optionsUI;
    public GameObject hudUI;

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && inOptionsMenu)
        {
            exitOptions();
        }
    }

    public void enterOptions()
    {
        optionsUI.SetActive(true);
        inOptionsMenu = true;
    }

    public void exitOptions()
    {
        optionsUI.SetActive(false);
        inOptionsMenu = false;
    }

    public bool getState()
    {
        return inOptionsMenu;
    }
}
