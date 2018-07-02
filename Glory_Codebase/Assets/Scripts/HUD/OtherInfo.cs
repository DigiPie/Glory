using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OtherInfo : MonoBehaviour {

    public GameManager gameManager;
    public TextMeshProUGUI txtWaveNumber;
    public TextMeshProUGUI txtEnemiesLeft;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        txtEnemiesLeft.text = GameObject.FindGameObjectsWithTag("Enemy").Length.ToString();
        txtWaveNumber.text = "Wave: " + gameManager.GetWave();
	}
}
